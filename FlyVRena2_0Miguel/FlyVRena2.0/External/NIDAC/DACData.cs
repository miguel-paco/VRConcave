using Sardine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyVRena2._0.External.NIDAC
{
    public class DACData : Message
    {
        //public double electrodeValue;
        //public double photodiodeValue;
        //public double treadmillClockValue;
        public double[,] data;
        public DACData(double[,] values, ulong ID)
        {
            this.data = values;
            //electrodeValue = values[0];
            //photodiodeValue = values[1];
            //treadmillClockValue = values[2];
            this.ID = ID;
        }
    }
}
