﻿using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FlyVRena2._0.VirtualWorld;
using System.Diagnostics;
using System.Threading;

namespace FlyVRena2._0.Display
{
    sealed class MainWindow : GameWindow
    {
        private VirtualWorld.VirtualWorld virtualWorld;
        public MainWindow() : base(800, 800, GraphicsMode.Default, "FlyVRena2.0", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible) //720,720
        {
            VSync = VSyncMode.Off;
            GraphicsContext.ShareContexts = false;
            this.Location = new System.Drawing.Point(1920+250, 50);
        }

        public string protName;
        public string protFolder;
        public MainWindow(string protocolName, string protFolder) : base(800, 800, GraphicsMode.Default, "FlyVRena2.0", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible) //720,720
        {
            this.protName = protocolName;
            this.protFolder = protFolder;
            VSync = VSyncMode.Off;
            GraphicsContext.ShareContexts = false;
            this.Location = new System.Drawing.Point(1920 + 250, 50);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (protName == null)
                virtualWorld = new VirtualWorld.VirtualWorld(Width);
            else if (protFolder == null)
                virtualWorld = new VirtualWorld.VirtualWorld(Width, protName);
            else
                virtualWorld = new VirtualWorld.VirtualWorld(Width, protName, protFolder);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Key.Escape) || virtualWorld.finish)
            {
                virtualWorld.OnExiting();
                virtualWorld.Dispose();
                Exit();
            }

            virtualWorld.Update(e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            this.MakeCurrent();
            GL.Flush();
            Title = $"FlyVRena2.0: (Vsync: {VSync}) RenderFPS: {1f / e.Time:0}";
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            virtualWorld.Draw(e.Time);
            SwapBuffers();
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            virtualWorld.OnExiting();
            Exit();
        }
    }
}
