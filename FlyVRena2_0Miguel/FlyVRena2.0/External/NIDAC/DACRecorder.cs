using System;
using Sardine.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace FlyVRena2._0.External.NIDAC
{
    public class DACRecorder<T> : Module<T> where T : DACData
    {
        private StreamWriter fileStream;
        private string path;
        public DACRecorder(string path)
        {
            this.path = path;
            fileStream = new StreamWriter(path);
            fileStream.Flush();
        }

        protected override void Process(T data)
        {
            int channelCount = data.data.GetLength(0);
            int dataCount = data.data.GetLength(1);
            for (int i = 0; i < dataCount; i++)
            {
                for (int j = 0; j < channelCount; j++)
                {
                    double dataValue = (double)data.data.GetValue(j,i);
                    fileStream.Write(dataValue.ToString("e6"));
                    fileStream.Write("\t");
                }
                fileStream.WriteLine();
            }
            fileStream.Flush();
        }

        public void OnExit()
        {
            Thread.Sleep(5);
            fileStream.Close();
            fileStream.Dispose();
            this.Abort();
        }

        public void DisposeModule()
        {
            this.Dispose();
        }
    }
}
