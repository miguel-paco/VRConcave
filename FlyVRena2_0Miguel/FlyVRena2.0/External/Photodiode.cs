using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;

namespace FlyVRena2._0.External
{
    public class Photodiode
    {

        // NOTES:
        //-------> If the output is "E", then the arduino didn't have time to receive and deliver a value request before the next request was made\


        // Configure Photodiode Port
        public const int BaudRate = 115200; // Transfer Data rate 
        private SerialPort serialPort;
        private string portValue;
        private string[] portList;

        public Photodiode(string portID)    
        {
            // Begin communications with photodiode
            serialPort = new SerialPort(portID, BaudRate, Parity.None, 8, StopBits.One);
        }

        object _lock = new object();

        public virtual string Read
        {
            get =>Update(); 
        }

        public void StartPhotodiode()
        {
            serialPort.Open();
        }

        protected virtual string Update()
        {
            serialPort.Write("REQ\n");
            portValue = serialPort.ReadExisting();
            portValue = portValue.Trim();
            return portValue;
        }

        public void OnExit()
        {
            serialPort.Close();
            serialPort.Dispose();
        }

    }
}
