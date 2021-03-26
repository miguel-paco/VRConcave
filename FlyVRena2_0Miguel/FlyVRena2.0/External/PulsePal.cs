using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sardine.Core;
using System.Threading;
using System.IO.Ports;
using FlyVRena2._0.ImageProcessing;
using OpenCV.Net;

namespace FlyVRena2._0.External
{
    public class PulsePal<T> : Module<T> where T : MovementData
    {

        public ManualResetEventSlim MREvent = new ManualResetEventSlim(false);
        public const int BaudRate = 115200; // Transfer Data rate 
        const int MaxDataBytes = 35; // Max Size of the Command Data

        SerialPort serialPort;
        bool initialized;
        byte[] readBuffer;
        byte[] responseBuffer;
        byte[] commandBuffer;

        // Pulse Pal Option Bytes For Data Transfer
        // 1st byte in command
        const byte OpMenu = 0xD5; // All commands should start with the Op menu byte 213
        // 2nd byte in command
        const byte HandshakeCommand = 0x48; // Option byte 72 ('H') - If Pulse Pal connected, it sends a byte 75 ('K') and firmware byte in response.
        const byte SetVoltageCommand = 0x4F; // Option byte 79 - Set a fixed voltage on an output channel
        const byte Acknowledge = 0x4B; // Option byte 75 (Program custom pulse train 1)

        public PulsePal(string portID)
        {
            // Serial Port Settings  - Pulse Pal v1.X (https://sites.google.com/site/pulsepalwiki/serial-interface)
            serialPort = new SerialPort(portID);
            serialPort.BaudRate = BaudRate; // Transfer Data rate 
            serialPort.DataBits = 8; // Transfer Data bit size
            serialPort.StopBits = StopBits.One; // Number of stop bits
            serialPort.Parity = Parity.None; // Method to detect errors
            serialPort.DtrEnable = false; // Data Terminal Ready
            serialPort.RtsEnable = true; //  Request to Send
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            readBuffer = new byte[serialPort.ReadBufferSize];
            commandBuffer = new byte[MaxDataBytes]; // Max Size of the Command Data
            responseBuffer = new byte[4];
        }

        // Initialize Connection To Pulse Pal
        public void StartPulsePal()
        {
            serialPort.Open();
            serialPort.ReadExisting();
            commandBuffer[0] = OpMenu;
            commandBuffer[1] = HandshakeCommand;
            serialPort.Write(commandBuffer, 0, 2);
            this.Start();
        }

        protected override void Process(T data)
        {
            // Updates the voltage input into Pulsepal according to the tracking result
            Coordinates coord = new Coordinates() { PixelsCurve = new Point2d(data.position[0], data.position[1]) };

            if (data.ID != 0)
            {
                commandBuffer[0] = OpMenu; // Signal to pulsepal it will receive a command
                commandBuffer[1] = SetVoltageCommand; // command pulsepal to set a voltage value
                commandBuffer[2] = 1; // 1st Coordenate (X)
                commandBuffer[3] = (byte)Math.Round(coord.VoltageCurve.X); // Voltage Value
                if (commandBuffer[3] > 255) { commandBuffer[3] = 255; } else if (commandBuffer[3] < 0) { commandBuffer[3] = 0; } // Voltage can't be higher then 255 or lower then 0
                serialPort.Write(commandBuffer, 0, 4); // Send Command
                commandBuffer[2] = 2; // 2nd Coordenate (Y)
                commandBuffer[3] = (byte)Math.Round(coord.VoltageCurve.Y); // Voltage Value
                if (commandBuffer[3] > 255) { commandBuffer[3] = 255; } else if (commandBuffer[3] < 0) { commandBuffer[3] = 0; } // Voltage can't be higher then 255 or lower then 0
                serialPort.Write(commandBuffer, 0, 4); // Send Command
            }
            MREvent.Wait();
            MREvent.Reset();
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var bytesToRead = serialPort.BytesToRead;
            if (serialPort.IsOpen && bytesToRead > 0)
            {
                bytesToRead = serialPort.Read(readBuffer, 0, bytesToRead);
                for (int i = 0; i < bytesToRead; i++)
                {
                    ProcessInput(readBuffer[i]);
                }
            }
        }

        void ProcessInput(byte inputData)
        {
            if (!initialized && inputData != Acknowledge)
            {
                throw new InvalidOperationException("Unexpected return value from PulsePal.");
            }
            switch (inputData)
            {
                case Acknowledge:
                    initialized = true;
                    break;
                default:
                    break;
            }
        }

        public void OnExit()
        {
            this.Stop();
            serialPort.Close();
            serialPort.Dispose();
            MREvent.Dispose();
            this.Abort();
        }

        public void DisposeModule()
        {
            this.Dispose();
        }

    }
}
