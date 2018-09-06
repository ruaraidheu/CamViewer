using API.V0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cam_Program.Inbuilt
{
    class MQTT : API.V0.SlotObject
    {
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Start()
        {
            Initial.AddSlotData<MQTT>();
        }
        public override void Reload()
        {
            base.Reload();
        }
    }
}
