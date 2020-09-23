using Sardine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.External
{
    public class TreadmillData : Message
    {
        public string source;
        public int[] values;
        public int clock;
        public TreadmillData(ulong dataN, string source, int[] values, int clock)
        {
            this.ID = dataN;
            this.source = "Treadmill" + source;
            this.values = values;
            this.clock = clock;
        }
    }
}
