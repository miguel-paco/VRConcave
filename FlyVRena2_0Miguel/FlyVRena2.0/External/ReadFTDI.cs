using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FTD2XX_NET;
using Sardine.Core;

namespace FlyVRena2._0.External
{
    public class ReadFTDI : Module
    {
        /* FTDI variables */
        public UInt32 ftdiDeviceCount = 0;
        private FTDI myFtdiDevice;
        public FTDI.FT_STATUS status;
        private UInt32 numBytesAvailable;
        private UInt32 writenValues;
        private UInt32 numBytesRead;
        public bool toStart = false;
        /* Buffers for data */
        private byte[] buffer = new byte[12];
        private byte[] wBuffer = new byte[10];
        private int[] value = new int[4];
        private int[] sumValues = new int[4];
        string deviceNo;

        public ReadFTDI(int deviceNumber)
        {
            myFtdiDevice = new FTDI();
            // Determine the number of FTDI devices connected to the machine
            status |= myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            if (ftdiDeviceCount != 0)
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                // Get the information of the device
                FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];
                status |= myFtdiDevice.GetDeviceList(ftdiDeviceList);
                // Open device
                //status |= myFtdiDevice.OpenByDescription(ftdiDeviceList[deviceNumber].Description);
                status |= myFtdiDevice.OpenBySerialNumber(ftdiDeviceList[deviceNumber].SerialNumber);
                status |= myFtdiDevice.ResetDevice();
                deviceNo = deviceNumber.ToString();
                // Set communication settings
                status |= myFtdiDevice.SetTimeouts(2000, 2000);
                status |= myFtdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
                status |= myFtdiDevice.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_NONE, 0, 0);
                status |= myFtdiDevice.SetBaudRate(1250000);
                //// Stop acquiring data
                //wBuffer[0] = 254;
                //wBuffer[1] = 0;
                //status |= myFtdiDevice.Write(wBuffer, 2, ref writenValues);
                //status |= myFtdiDevice.Purge(FTDI.FT_PURGE.FT_PURGE_TX | FTDI.FT_PURGE.FT_PURGE_RX);
                //// Start acquiring data
                //wBuffer[0] = 255;
                //wBuffer[1] = 0;
                //status |= myFtdiDevice.Write(wBuffer, 2, ref writenValues);
                toStart = false;
            }
            else
                status = FTDI.FT_STATUS.FT_DEVICE_NOT_FOUND;
        }

        private ulong tickNumber = 0;
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlThread)]
        protected override void Work(CancellationToken cancellationToken)
        {
            myFtdiDevice.ResetDevice();
            while (buffer != null)
            {
                numBytesRead = 0;
                numBytesAvailable = 0;

                if (toStart)
                {
                    /* Wait for all the data */
                    while (numBytesAvailable < 12)
                    {
                        myFtdiDevice.GetRxBytesAvailable(ref numBytesAvailable);
                    }

                    /* Read the data */
                    myFtdiDevice.Read(buffer, numBytesAvailable, ref numBytesRead);

                    if ((int)buffer[0] == 0)
                    {
                        /* Integrate read data */
                        for (int i = 0; i < 4; i++)
                        {
                            value[i] = ((int)buffer[2 + i] - 128);
                           // sumValues[i] += value[i];
                        }
                        tickNumber++;
                    }
                    else
                    {
                        myFtdiDevice.ResetDevice();
                    }
                }
            }
        }

        /* Close the device safely */
        public void OnExit()
        {
            Thread.EndThreadAffinity();
            // Stop acquisition
            myFtdiDevice.Purge(FTDI.FT_PURGE.FT_PURGE_TX | FTDI.FT_PURGE.FT_PURGE_RX);
            wBuffer[0] = 254;
            wBuffer[1] = 0;
            myFtdiDevice.Write(wBuffer, 2, ref writenValues);
            myFtdiDevice.Purge(FTDI.FT_PURGE.FT_PURGE_TX | FTDI.FT_PURGE.FT_PURGE_RX);
            // Close device
            myFtdiDevice.Close();
            this.Abort();
        }

        public void DisposeModule()
        {
            this.Dispose();
        }
    }
}
