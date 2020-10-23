using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace StartFlyVRenaProtocol {
    class StartFlyVRena {
        static void Main() {

            string protocolPath = GetProtocol();
            string[] protocol = System.IO.File.ReadAllLines(protocolPath);
            string[] p1 = protocol[0].Split(' ');
            List<string> fullProtocol = new List<string>();

            foreach (string protName in protocol) {
                string[] subString = protName.Split(' ');
                int nReps = Int32.Parse(subString[1]);
                for (int i = 0; i < nReps; i++) {
                    fullProtocol.Add(subString[0]);
                }
            }

            //randomly shuffle the order of the stimuli presentation
            List<string> shuffledProtocols = fullProtocol.OrderBy(a => Guid.NewGuid()).ToList();

            string initialFolder = GetSaveDirectory();

            foreach (string protName in shuffledProtocols) {
                Console.WriteLine("{0}", protName);
                //string initialFolder = @"C:\Users\ChiappeLab\Desktop\dump";
                ProcessStartInfo startInfo = new ProcessStartInfo();
                //remove the initial console window
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = false;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                startInfo.FileName = "C:\\Users\\ALBINO\\Documents\\VRConcave\\FlyVRena2_0Miguel\\FlyVRena2.0\\bin\\Debug\\FlyVRena2.0.exe";
                startInfo.WorkingDirectory = "C:\\Users\\ALBINO\\Documents\\VRConcave\\FlyVRena2_0Miguel\\FlyVRena2.0\\bin\\Debug";
                startInfo.Arguments = protName + " " + initialFolder;
                var p = Process.Start(startInfo);
                //wait for the protocol to end
                p.WaitForExit();
            }
        }

        //fileopen
        public static string GetProtocol() {
            string fileName = "";
            Thread t = new Thread((ThreadStart)(() => {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.InitialDirectory = Assembly.GetExecutingAssembly().Location;
                fileDialog.Title = "Open File";
                fileDialog.Filter = "XML Files (*.txt)|*.txt|" +
                                        "All Files (*.*)|*.*";
                if (fileDialog.ShowDialog() == DialogResult.OK) {
                    fileName = fileDialog.SafeFileName;
                    fileName = fileDialog.FileName;
                }
            }));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            return fileName;
        }

        public static string GetSaveDirectory() {
            string directory = "";

            Thread t = new Thread((ThreadStart)(() => {
                FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowserDialog1.Description = "Select the directory that you want to save the data.";
                folderBrowserDialog1.ShowNewFolderButton = true;
                folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.Desktop;
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                    directory = folderBrowserDialog1.SelectedPath;
                }
            }));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            directory = directory + "\\";
            return directory;
        }

    }
}
