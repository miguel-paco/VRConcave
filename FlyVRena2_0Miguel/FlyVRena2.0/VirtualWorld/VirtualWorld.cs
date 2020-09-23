﻿using FlyVRena2._0.External;
using FlyVRena2._0.External.NIDAC;
using FlyVRena2._0.ImageProcessing;
using FlyVRena2._0.VirtualWorld.ServiceFactories;
using FlyVRena2._0.VirtualWorld.Services;
using FlyVRena2._0.VirtualWorld.Subsystems;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace FlyVRena2._0.VirtualWorld
{
    public class VirtualWorld
    {
        //Acquisition
        private uEyeCamera cam1;
        private uEyeCamera cam2;
        private ReadFTDI treadmill;
        private PulsePal<MovementData> pp;

        // Vars Stimuli
        private WorldObject root;
        public VRProtocol vRProtocol;
        private FastTracking<Frame> fastT;
        private TransformTreadmill<TreadmillData> tt;
        public KalmanFilterTrack<MovementData> kft;

        public RenderSubsystem render;
        public UpdateSubsystem update;

        // Vars Data Storage
        private DataRecorder<FilteredData> dataRecorder;
        private DataRecorderRaw<MovementData> dataRecorderRaw;
        private DACReader dacReader;
        private DACRecorder<DACData> dacRecorder;
        public double _time = 0;

        public bool finish = false;
        public VirtualWorld(int Size)
        {
            Initialize(Size);
        }

        public void Initialize(int Size)
        {
            // Initialize vr components
            render = new RenderSubsystem(this, new StaticCamera(Size), Size);
            update = new UpdateSubsystem(this);
            kft = new KalmanFilterTrack<MovementData>(false);

            // Load virtual world
            GetStimulus();

            // Initialize data acquisition objects           
            if (vRProtocol.usePulsePal)
            {
                pp = new PulsePal<MovementData>(vRProtocol.portPulsePal);
                pp.StartPulsePal();
                //dataRecorder = new DataRecorder<FilteredData>(vRProtocol.recordPathTracking, cam2, this);
                //dataRecorder.Start();
            }
            if (vRProtocol.useCam1)
            {
                cam1 = new uEyeCamera(1, vRProtocol.paramsPathCam1, vRProtocol.trackCam1, vRProtocol.dispCam1, 0, vRProtocol.fpsCam1, null);
                while (!cam1.m_IsLive) { }
                if (cam1.m_IsLive)
                {
                    cam1.Start();
                    if (vRProtocol.trackCam1)
                    {
                        fastT = new FastTracking<Frame>(cam1.firstFrame, 1, 50, 28, 21, true);
                        fastT.Start();
                        kft.Start();
                    }
                }
            }
            if (vRProtocol.useCam2)
            {
                if (vRProtocol.usePulsePal)
                    cam2 = new uEyeCamera(0, vRProtocol.paramsPathCam2, vRProtocol.trackCam2, vRProtocol.dispCam2, 800, vRProtocol.fpsCam2, pp);
                else
                    cam2 = new uEyeCamera(0, vRProtocol.paramsPathCam2, vRProtocol.trackCam2, vRProtocol.dispCam2, 800, vRProtocol.fpsCam2, null);

                if (cam2.m_IsLive)
                {
                    cam2.Start();
                    if (vRProtocol.trackCam2)
                    {
                        fastT = new FastTracking<Frame>(cam2.firstFrame, 10, 5000, 35, 51, false);
                        fastT.Start();
                        kft.Start();
                    }
                }
            }

            if (vRProtocol.useTreadmill)
            {
                treadmill = new ReadFTDI(0);
                if (treadmill.status == FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    treadmill.Start();
                    tt = new TransformTreadmill<TreadmillData>();
                    tt.Start();
                    kft.Start();
                }
            }
            if (vRProtocol.recordTreadmill & vRProtocol.useCam2)
            {
                dataRecorder = new DataRecorder<FilteredData>(vRProtocol.recordPathTreadmill, cam2, this);
                dataRecorder.Start();
            }
            else if (vRProtocol.recordTreadmill & !vRProtocol.useCam2)
            {
                dataRecorder = new DataRecorder<FilteredData>(vRProtocol.recordPathTreadmill, this);
                dataRecorder.Start();
            }

            if (vRProtocol.recordTracking & vRProtocol.useCam2)
            {
                dataRecorder = new DataRecorder<FilteredData>(vRProtocol.recordPathTracking, cam2, true, this);
                dataRecorder.Start();
            }
            else if (vRProtocol.recordTracking & !vRProtocol.useCam2)
            {
                dataRecorder = new DataRecorder<FilteredData>(vRProtocol.recordPathTracking, true, this);
                dataRecorder.Start();
            }

            if (vRProtocol.recordTrackingRaw & vRProtocol.useCam2)
            {
                dataRecorderRaw = new DataRecorderRaw<MovementData>(vRProtocol.recordPathTracking, cam2, true, this);
                dataRecorderRaw.Start();
            }
            else if (vRProtocol.recordTrackingRaw & !vRProtocol.useCam2)
            {
                dataRecorderRaw = new DataRecorderRaw<MovementData>(vRProtocol.recordPathTracking, true, this);
                dataRecorderRaw.Start();
            }

            if (vRProtocol.useDAC)
            {
                dacReader = new DACReader(-10, 10);
                dacReader.Init();
                if (vRProtocol.recordDAC)
                {
                    if (dacReader.dacON)
                    {
                        dacRecorder = new DACRecorder<DACData>(vRProtocol.recordPathDAC);
                        dacRecorder.Start();
                    }
                }
            }
            
        }

        bool firstUp = true;
        bool secondUp = true;
        Stopwatch stopwatch = new Stopwatch();
        public void Update(double time)
        {
            if (firstUp)
            {
                firstUp = false;
                if (vRProtocol.useTreadmill)
                {
                    if (treadmill.status == FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                        treadmill.StartTreadmill();
                }
                if (vRProtocol.useCam1 && vRProtocol.recordCam1)
                {
                    if (cam1.m_IsLive == true)
                        cam1.RecordVideo(vRProtocol.recordPathCam1);
                }
                if (vRProtocol.useCam2 && vRProtocol.recordCam2)
                {
                    if (cam2.m_IsLive == true)
                        cam2.RecordVideo(vRProtocol.recordPathCam2);
                }
                if (vRProtocol.useTreadmill)
                    while (!treadmill.toStart) { }
                stopwatch.Stop();
                stopwatch.Start();
            }
            if (!firstUp && secondUp)
            {
                update.Update(time);
                _time += time;
            }
            if (stopwatch.ElapsedMilliseconds >= 1000 * (vRProtocol.duration + 0.5) && secondUp)
            {
                if (vRProtocol.useTreadmill)
                {
                    treadmill.StopTreadmill();
                }
                if (vRProtocol.useCam1 && vRProtocol.recordCam1)
                {
                    if (cam1.m_IsLive == true)
                        cam1.StopRecording();
                }
                secondUp = false;
            }
            if (stopwatch.ElapsedMilliseconds > 1000 * (vRProtocol.duration + 1))
                finish = true;
        }

        public void Draw(double time)
        {
            if (secondUp != false)
            {
                if (stopwatch.IsRunning)
                    render.Draw(time);
            }
        }

        // Funtion to open a new dialog window to load the virtual world file
        public void GetStimulus()
        {
            string fileName = "";
            Thread t = new Thread((ThreadStart)(() =>
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.InitialDirectory = Assembly.GetExecutingAssembly().Location;
                fileDialog.Title = "Open File";
                fileDialog.Filter = "XML Files (*.xml)|*.xml|" +
                                        "All Files (*.*)|*.*";

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = fileDialog.SafeFileName;
                    root = LoadStimulus(fileDialog.FileName);
                }
            }));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            if (root != null)
            {
                Init(root);
                VRProtocolFactory vrpF = (VRProtocolFactory)root.objectBuilder[0];
                vrpF.Initialize(root, this);
                this.vRProtocol = (VRProtocol)root.GetService(typeof(VRProtocol));
                if (vRProtocol.recordCam1 || vRProtocol.recordCam2 || vRProtocol.recordDAC || vRProtocol.recordTreadmill || vRProtocol.recordTracking || vRProtocol.recordTrackingRaw)
                {
                    CreateSaveDirectory(fileName, this.vRProtocol);
                }
            }
        }

        public void CreateSaveDirectory(string protocolName, VRProtocol protocol)
        {
            int index0 = protocolName.LastIndexOf(".");
            string directory = "";
            protocolName = protocolName.Substring(0, index0);
            Thread t = new Thread((ThreadStart)(() =>
            {
                FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowserDialog1.Description = "Select the directory that you want to save the data.";
                folderBrowserDialog1.ShowNewFolderButton = true;
                folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.Desktop;
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    directory = folderBrowserDialog1.SelectedPath;
                }
            }));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            directory = directory + "\\";
            var subDirectories = Directory.GetDirectories(directory);
            int nFolders = 1;
            foreach (string subDirectory in subDirectories)
            {
                int index1 = subDirectory.LastIndexOf("\\");
                string folderName = subDirectory.Substring(index1 + 1);
                int index2 = folderName.LastIndexOf("__");
                if (index2 != -1)
                {
                    folderName = folderName.Substring(0, index2);
                }
                if (String.Compare(folderName, protocolName) == 0)
                {
                    nFolders = nFolders + 1;
                }
            }
            directory = directory + protocolName + "__" + nFolders.ToString();
            Directory.CreateDirectory(directory);
            protocol.recordPathCam1 = directory + protocol.recordPathCam1.Substring(protocol.recordPathCam1.LastIndexOf("\\"));
            protocol.recordPathCam2 = directory + protocol.recordPathCam2.Substring(protocol.recordPathCam2.LastIndexOf("\\"));
            protocol.recordPathDAC = directory + protocol.recordPathDAC.Substring(protocol.recordPathDAC.LastIndexOf("\\"));
            protocol.recordPathTreadmill = directory + protocol.recordPathTreadmill.Substring(protocol.recordPathTreadmill.LastIndexOf("\\"));
            protocol.recordPathTracking = directory + protocol.recordPathTracking.Substring(protocol.recordPathTracking.LastIndexOf("\\"));
        }



        // Function to initialize the virtual world
        public void Init(WorldObject WObj)
        {
            foreach (WorldObject obj in WObj.WObjects)
            {
                if (obj.GetService(typeof(NameService)) == null)
                {
                    foreach (ServiceFactory s in obj.objectBuilder)
                    {
                        s.Initialize(obj, this);
                    }
                    Init(obj);
                }
            }
        }

        // Auxiliary function to load XML file
        public WorldObject LoadStimulus(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WorldObject));
            WorldObject result;
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                result = (WorldObject)serializer.Deserialize(fileStream);
            }
            return result;
        }

        public void OnExiting()
        {

            if (vRProtocol.useTreadmill)
            {
                if (treadmill.status == FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    this.tt.OnExit();
                    this.treadmill.OnExit();
                }
            }
            if (vRProtocol.useCam1)
            {
                if (cam1.m_IsLive == true)
                {
                    if (vRProtocol.trackCam1)
                        this.fastT.OnExit();
                    this.cam1.OnExit();
                }
            }
            if (vRProtocol.useCam2)
            {
                if (cam2.m_IsLive == true)
                {
                    if (vRProtocol.trackCam2)
                        this.fastT.OnExit();
                    this.cam2.OnExit();
                }
            }
            if (vRProtocol.useDAC)
            {
                if (dacReader.dacON)
                {
                    dacReader.OnExit();
                    if (vRProtocol.recordDAC)
                    {
                        dacRecorder.OnExit();
                    }
                }
            }
            if (vRProtocol.usePulsePal)
            {
                pp.OnExit();
                //dataRecorder.OnExit();
            }
            this.kft.OnExit();
            if (vRProtocol.recordTreadmill)
            {
                this.dataRecorder.OnExit();
            }
            if (vRProtocol.recordTracking)
            {
                this.dataRecorder.OnExit();
            }
            if (vRProtocol.recordTrackingRaw)
            {
                this.dataRecorderRaw.OnExit();
            }
            render.OnExit();
            update.OnExit();
        }

        public void Dispose()
        {
            if (vRProtocol.useTreadmill)
            {
                if (treadmill.status == FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    this.tt.DisposeModule();
                    this.treadmill.DisposeModule();
                }
            }
            if (vRProtocol.useCam1)
            {
                if (cam1.m_IsLive == true)
                {
                    if (vRProtocol.trackCam1)
                        this.fastT.DisposeModule();
                    this.cam1.DisposeModule();
                }
            }
            if (vRProtocol.useCam2)
            {
                if (cam2.m_IsLive == true)
                {
                    if (vRProtocol.trackCam2)
                        this.fastT.DisposeModule();
                    this.cam2.DisposeModule();
                }
            }
            if (vRProtocol.useDAC)
            {
                if (dacReader.dacON)
                {
                    dacReader.DisposeModule();
                    if (vRProtocol.recordDAC)
                    {
                        dacRecorder.DisposeModule();
                    }
                }
            }
            if (vRProtocol.usePulsePal)
            {
                pp.DisposeModule();
                //dataRecorder.DisposeModule();
            }
            this.kft.DisposeModule();
            if (vRProtocol.recordTreadmill)
            {
                this.dataRecorder.DisposeModule();
            }
            if (vRProtocol.recordTracking)
            {
                this.dataRecorder.DisposeModule();
            }
            if (vRProtocol.recordTrackingRaw)
            {
                this.dataRecorderRaw.DisposeModule();
            }
            update.Dispose();
        }
    }
}
