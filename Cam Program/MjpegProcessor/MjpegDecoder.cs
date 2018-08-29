﻿using System;
using System.Text;
using System.Net;
using System.IO;
using Ruaraidheulib.Winforms;

#if !XNA && !WINRT
using System.Threading;
using System.Windows;
#endif

using System.Drawing;


namespace MjpegProcessor
{
    public class MjpegDecoder
    {
        public Bitmap Bitmap { get; set; }

        // magic 2 byte header for JPEG images
        private readonly byte[] JpegHeader = new byte[] { 0xff, 0xd8 };

        // pull down 1024 bytes at a time
        private const int ChunkSize = 1024;

        // used to cancel reading the stream
        private bool _streamActive;

        // current encoded JPEG image
        public byte[] CurrentFrame { get; private set; }

        private SynchronizationContext _context;

        // event to get the buffer above handed to you
        public event EventHandler<FrameReadyEventArgs> FrameReady;
        public event EventHandler<ErrorEventArgs> Error;

        public MjpegDecoder()
        {
            _context = SynchronizationContext.Current;
        }

        public void ParseStream(Uri uri)
        {
            ParseStream(uri, null, null);
        }

        public void ParseStream(Uri uri, string username, string password)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
                request.Credentials = new NetworkCredential(username, password);

            // asynchronously get a response
            Thread t = new Thread(()=> { GetResponse(request); });
            t.Start();
        }
        public void GetResponse(HttpWebRequest hwr)
        {
            hwr.BeginGetResponse(OnGetResponse, hwr);
            Console.WriteLine(hwr.HaveResponse);
            if (hwr.HaveResponse)
            {
                MessageBox.Show("Could not connect to "+hwr.RequestUri+Environment.NewLine+"Have you made a typo?");
            }
        }

        public void StopStream()
        {
            _streamActive = false;
        }

        private void OnGetResponse(IAsyncResult asyncResult)
        {
            byte[] imageBuffer = new byte[1024 * 1024];

            // get the response
            HttpWebRequest req = (HttpWebRequest)asyncResult.AsyncState;

            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.EndGetResponse(asyncResult);

                // find our magic boundary value
                string contentType = resp.Headers["Content-Type"];
                if (!string.IsNullOrEmpty(contentType) && !contentType.Contains("="))
                    throw new Exception("Invalid content-type header.  The camera is likely not returning a proper MJPEG stream.");
                string boundary = resp.Headers["Content-Type"].Split('=')[1].Replace("\"", "");
                byte[] boundaryBytes = Encoding.UTF8.GetBytes(boundary.StartsWith("--") ? boundary : "--" + boundary);

                Stream s = resp.GetResponseStream();
                BinaryReader br = new BinaryReader(s);

                _streamActive = true;

                byte[] buff = br.ReadBytes(ChunkSize);

                System.Diagnostics.Stopwatch st0 = new System.Diagnostics.Stopwatch();
                System.Diagnostics.Stopwatch st1 = new System.Diagnostics.Stopwatch();
                System.Diagnostics.Stopwatch st2 = new System.Diagnostics.Stopwatch();
                while (_streamActive)
                {
                    st0.Start();
                    // find the JPEG header
                    int imageStart = buff.Find(JpegHeader);

                    if (imageStart != -1)
                    {
                        // copy the start of the JPEG image to the imageBuffer
                        int size = buff.Length - imageStart;
                        Array.Copy(buff, imageStart, imageBuffer, 0, size);

                        while (true)
                        {
                            buff = br.ReadBytes(ChunkSize);

                            // find the boundary text
                            int imageEnd = buff.Find(boundaryBytes);
                            if (imageEnd != -1)
                            {
                                // copy the remainder of the JPEG to the imageBuffer
                                Array.Copy(buff, 0, imageBuffer, size, imageEnd);
                                size += imageEnd;

                                byte[] frame = new byte[size];
                                Array.Copy(imageBuffer, 0, frame, 0, size);

                                ProcessFrame(frame);

                                // copy the leftover data to the start
                                Array.Copy(buff, imageEnd, buff, 0, buff.Length - imageEnd);

                                // fill the remainder of the buffer with new data and start over
                                byte[] temp = br.ReadBytes(imageEnd);

                                Array.Copy(temp, 0, buff, buff.Length - imageEnd, temp.Length);
                                break;
                            }

                            // copy all of the data to the imageBuffer
                            Array.Copy(buff, 0, imageBuffer, size, buff.Length);
                            size += buff.Length;
                        }
                    }
                }
                br.Dispose();
                s.Dispose();
                resp.Close();
            }
            catch (Exception ex)
            {
                if (Error != null)
                    _context.Post(delegate { Error(this, new ErrorEventArgs() { Message = ex.Message }); }, null);

                return;
            }
        }

        private void ProcessFrame(byte[] frame)
        {
            CurrentFrame = frame;
            {
                _context.Post(delegate
                {
                    // create a simple GDI+ happy Bitmap
                    Bitmap = new Bitmap(new MemoryStream(frame));

                    // tell whoever's listening that we have a frame to draw
                    if (FrameReady != null)
                    {
                        FrameReady(this, new FrameReadyEventArgs { FrameBuffer = CurrentFrame, Bitmap = Bitmap });
                    }
                }, null);
            }
        }
    }

    static class Extensions
    {
        public static int Find(this byte[] buff, byte[] search)
        {
            // enumerate the buffer but don't overstep the bounds
            for (int start = 0; start < buff.Length - search.Length; start++)
            {
                // we found the first character
                if (buff[start] == search[0])
                {
                    int next;

                    // traverse the rest of the bytes
                    for (next = 1; next < search.Length; next++)
                    {
                        // if we don't match, bail
                        if (buff[start + next] != search[next])
                            break;
                    }

                    if (next == search.Length)
                        return start;
                }
            }
            // not found
            return -1;
        }
    }

    public class FrameReadyEventArgs : EventArgs
    {
        public byte[] FrameBuffer;
        public Bitmap Bitmap;
    }

    public sealed class ErrorEventArgs : EventArgs
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }
    }
}
