﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace API.V0
{
    public static class Initial
    {
        static List<Type> lo = new List<Type>();
        public static void AddSlotData<T>() where T : SlotObject
        {
            T a = default;
            lo.Add(a.GetType());
        }
        public static List<SlotObject> GetPluginInstances()
        {
            List<SlotObject> so = new List<SlotObject>();
            foreach (Type t in lo)
            {
                if (t.IsValueType && typeof(SlotObject).IsAssignableFrom(t))
                {
                    so.Add((SlotObject)Activator.CreateInstance(t));
                }
            }
            return so;
        }
    }
    public class SlotObject
    {
        public string Name { get; set; } = "DefaultName";
        public SlotObject()
        {
            Initialize();
        }
        public virtual void Start()
        {
            Initial.AddSlotData<SlotObject>();
        }
        public virtual void Initialize()
        {
            Reload();
        }
        public virtual void Reload()
        {

        }
        public virtual Control GetControl()
        {
            return null;
        }
    }
}
