using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Size = System.Drawing.Size;
using Timer = System.Windows.Forms.Timer;

namespace FlyVRena2._0.Visualizers
{
    public class ImageVisualizer
    {
        const int TargetInterval = 200;
        Panel imagePanel;
        VisualizerCanvas visualizerCanvas;
        IplImageTexture imageTexture;
        IplImage visualizerImage;
        Timer updateTimer;

        public void Show(object value)
        {
            using (var inputImage = (IplImage)value)
            {
                visualizerImage = IplImageHelper.EnsureImageFormat(visualizerImage, inputImage.Size, inputImage.Depth, inputImage.Channels);
                CV.Copy(inputImage, visualizerImage);
            }
            value = null;
        }

        protected virtual void RenderFrame()
        {
            imageTexture.Draw();
        }

        public void Load(IServiceProvider provider, int width, int height)
        {
            visualizerCanvas = new VisualizerCanvas { Dock = DockStyle.Fill };
            visualizerCanvas.RenderFrame += (sender, e) => RenderFrame();
            visualizerCanvas.Load += (sender, e) => imageTexture = new IplImageTexture();
            imagePanel = new Panel { Dock = DockStyle.Fill, Size = new Size(width, height) };
            imagePanel.Controls.Add(visualizerCanvas);

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            if (visualizerService != null)
            {
                updateTimer = new Timer();
                updateTimer.Interval = TargetInterval;
                updateTimer.Tick += updateTimer_Tick;
                visualizerService.AddControl(imagePanel);
                updateTimer.Start();
            }
        }

        public void Load(IServiceProvider provider)
        {
            visualizerCanvas = new VisualizerCanvas { Dock = DockStyle.Fill };
            visualizerCanvas.RenderFrame += (sender, e) => RenderFrame();
            visualizerCanvas.Load += (sender, e) => imageTexture = new IplImageTexture();
            //imagePanel = new Panel { Dock = DockStyle.Fill, Size = new Size(512, 512) };
            imagePanel = new Panel { Dock = DockStyle.Fill, Size = new Size(512, 512) };
            imagePanel.Controls.Add(visualizerCanvas);

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            if (visualizerService != null)
            {
                updateTimer = new Timer();
                updateTimer.Interval = TargetInterval;
                updateTimer.Tick += updateTimer_Tick;
                visualizerService.AddControl(imagePanel);
                updateTimer.Start();
            }
        }

        void updateTimer_Tick(object sender, EventArgs e)
        {
             UpdateCanvas();
        }

        void UpdateCanvas()
        {
            visualizerCanvas.MakeCurrent();
            if (visualizerImage != null)
            {
                imageTexture.Update(visualizerImage);
            }
            visualizerCanvas.Canvas.Invalidate();
        }

        public void Unload()
        {
            updateTimer.Stop();
            updateTimer.Dispose();
            imageTexture.Dispose();
            imagePanel.Dispose();
            updateTimer = null;
            imagePanel = null;
            visualizerCanvas = null;
            imageTexture = null;
            visualizerImage = null;
        }

        public virtual void SequenceCompleted()
        {
        }
    }
}
