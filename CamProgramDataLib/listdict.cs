using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamProgramDataLib
{
    public struct listdict
    {
        List<Slot> ld;

        public listdict(List<Slot> ls)
        {
            ld = ls;
        }
        public listdict(string ls)
        {
            ld = new List<Slot>();
        }

        public static implicit operator List<Slot>(listdict d)
        {
            return d.ld;
        }
        public static implicit operator listdict(List<Slot> d)
        {
            return new listdict(d);
        }
        public static implicit operator listdict(string d)
        {
            return new listdict(d);
        }
    }
    [Serializable]
    public abstract class Slot
    {
        string address = "";
        [DefaultValue("")]
        public string Address
        {
            get { return address; }
            set { address = value; }
        }
    }
    public class SlotCam : Slot
    {

    }
    public class SlotMQTT : Slot
    {
        string subs = "#";
        [DefaultValue("#")]
        public string Subs
        {
            get { return subs; }
            set { subs = value; }
        }
    }
}
