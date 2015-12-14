using Awesomium.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.IO;
using Microsoft.Xna.Framework.Content;

using Awesomium.Core.Data;
using Awesomium.Core.Dynamic;
using AwesomiumXNA;

using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace AwesomiumXNA
{
    public class MainDataSource : DataSource
    {
        protected override void OnRequest(DataSourceRequest request)
        {
            Console.WriteLine("Request for: " + request.Path);

            var response = new DataSourceResponse();

#if DEBUG
            var data = File.ReadAllBytes(Environment.CurrentDirectory + @"/../../../html/" + request.Path);
#else
			var data = File.ReadAllBytes(Environment.CurrentDirectory + @"/html/" + request.Path);
#endif

            IntPtr unmanagedPointer = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, unmanagedPointer, data.Length);

            response.Buffer = unmanagedPointer;
            response.MimeType = "text/html";
            response.Size = (uint)data.Length;
            SendResponse(request, response);

            Marshal.FreeHGlobal(unmanagedPointer);
        }
    }

    public class AwesomiumComponent : DrawableGameComponent
    {
        private delegate Int32 ProcessMessagesDelegate(Int32 code, Int32 wParam, ref Message lParam);

        // I might be able to scrap these two message things once I'm done moving to mac
        private enum WindowsMessage
        {
            KeyDown = 0x0100,
            KeyUp = 0x0101,
            Char = 0x0102,

            MouseMove = 0x0200,
            LeftButtonDown = 0x0201,
            LeftButtonUp = 0x0202,
            LeftButtonDoubleClick = 0x0203,

            RightButtonDown = 0x0204,
            RightButtonUp = 0x0205,
            RightButtonDoubleClick = 0x0206,

            MiddleButtonDown = 0x0207,
            MiddleButtonUp = 0x0208,
            MiddleButtonDoubleClick = 0x0209,

            MouseWheel = 0x020A,
        }

        private struct Message
        {
            internal IntPtr HWnd;
            internal Int32 Msg;
            internal IntPtr WParam;
            internal IntPtr LParam;
            internal IntPtr Result;
        }


        private IntPtr hookHandle;
        private ProcessMessagesDelegate processMessages;


        private IntPtr imagePtr;
        private Byte[] imageBytes;

        private Rectangle area;
        private Rectangle? newArea;
        private Boolean resizing;
        private SpriteBatch spriteBatch;
        private SynchronizationContext awesomiumContext = null;
        private BitmapSurface Surface { get; set; }
        private MouseState lastMouseState;
        private MouseState currentMouseState;
        private Keys[] lastPressedKeys;
        private Keys[] currentPressedKeys = new Keys[0];
        private static ManualResetEvent awesomiumReady = new ManualResetEvent(false);

        public AwesomiumComponent(Game game, Rectangle area)
            : base(game)
        {
            Init(game, area);
        }

        public AwesomiumComponent(Game game)
            : base(game)
        {
            Init(game, Rectangle.Empty);
        }

        private void Init(Game game, Rectangle area)
        {
            this.area = area;

            this.spriteBatch = new SpriteBatch(game.GraphicsDevice);

            /*Thread awesomiumThread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                WebCore.Started += (s, e) => {
                    awesomiumContext = SynchronizationContext.Current;
                    awesomiumReady.Set();
                };

                WebCore.Run();
            }));

            awesomiumThread.Start();
*/

            if (!WebCore.IsInitialized)
            {
                WebCore.Initialize(new WebConfig() { });
            }

            int w = area.Width;
            int h = area.Height;
            Console.WriteLine(area);

            //awesomiumReady.WaitOne();

            //WebCore.QueueWork(() =>
            //{
            WebView = WebCore.CreateWebView(w, h);
            //this.WebView = WebCore.CreateWebView(this.area.Width, this.area.Height);
            // this.WebView = WebCore.CreateWebView(this.area.Width, this.area.Height, WebViewType.Offscreen);

            // WebView doesn't seem to listen when I say this
            //WebView.SelfUpdate = true;
            // So I have to say this:
            WebCore.AutoUpdatePeriod = 10000000;   // TEEENN MIILLLIOON
                                                   //WebCore.AutoUpdatePeriod = 20;   // TEEENN MIILLLIOON

            this.WebView.IsTransparent = true;
            this.WebView.CreateSurface += (s, e) =>
            {
                this.Surface = new BitmapSurface(this.area.Width, this.area.Height);
                e.Surface = this.Surface;
            };
            //});
        }

        public Texture2D WebViewTexture { get; private set; }

        public WebView WebView { get; private set; }

        public Rectangle Area
        {
            get { return area; }
            set
            {
                newArea = value;
            }
        }

        public bool AllowMouseEvents = true;

        static bool shift = false, ctrl = false, alt = false;

        //private void WebView_ResizeComplete(Object sender, SurfaceResizedEventArgs e)
        //{
        //    resizing = false;
        //}

        public void SetResourceInterceptor(IResourceInterceptor interceptor)
        {
            //awesomiumContext.Post(state =>
            //{
            WebCore.ResourceInterceptor = interceptor;
            //}, null);
        }

        public void Execute(string method, params object[] args)
        {
            string script = string.Format("viewModel.{0}({1})", method, string.Join(",", args.Select(x => "\"" + x.ToString() + "\"")));
            this.WebView.ExecuteJavascript(script);
        }

        protected override void LoadContent()
        {
            if (this.area.IsEmpty)
            {
                this.area = this.GraphicsDevice.Viewport.Bounds;
                this.newArea = this.GraphicsDevice.Viewport.Bounds;
            }
            this.WebViewTexture = new Texture2D(this.Game.GraphicsDevice, this.area.Width, this.area.Height, false, SurfaceFormat.Color);

            this.imageBytes = new Byte[this.area.Width * 4 * this.area.Height];
        }

        public override void Update(GameTime gameTime)
        {
            WebCore.Update();

            if (this.newArea.HasValue && !this.resizing && gameTime.TotalGameTime.TotalSeconds > 0.10f)
            {
                this.area = this.newArea.Value;
                if (this.area.IsEmpty)
                    this.area = this.GraphicsDevice.Viewport.Bounds;

                this.WebView.Resize(this.area.Width, this.area.Height);
                this.WebViewTexture = new Texture2D(this.Game.GraphicsDevice, this.area.Width, this.area.Height, false, SurfaceFormat.Color);
                this.imageBytes = new Byte[this.area.Width * 4 * this.area.Height];
                this.resizing = true;

                this.newArea = null;
            }

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            this.WebView.InjectMouseMove(currentMouseState.X - this.area.X, currentMouseState.Y - this.area.Y);

            if (currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
            {
                this.WebView.InjectMouseDown(MouseButton.Left);
            }
            if (currentMouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed)
            {
                this.WebView.InjectMouseUp(MouseButton.Left);
            }
            if (currentMouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released)
            {
                this.WebView.InjectMouseDown(MouseButton.Right);
            }
            if (currentMouseState.RightButton == ButtonState.Released && lastMouseState.RightButton == ButtonState.Pressed)
            {
                this.WebView.InjectMouseUp(MouseButton.Right);
            }
            if (currentMouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Released)
            {
                this.WebView.InjectMouseDown(MouseButton.Middle);
            }
            if (currentMouseState.MiddleButton == ButtonState.Released && lastMouseState.MiddleButton == ButtonState.Pressed)
            {
                this.WebView.InjectMouseUp(MouseButton.Middle);
            }

            if (currentMouseState.ScrollWheelValue != lastMouseState.ScrollWheelValue)
            {
                this.WebView.InjectMouseWheel((currentMouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue), 0);
            }

            lastPressedKeys = currentPressedKeys;
            currentPressedKeys = Keyboard.GetState().GetPressedKeys();

            // Key Down
            foreach (var key in currentPressedKeys)
            {
                if (!lastPressedKeys.Contains(key))
                {
                    this.WebView.InjectKeyboardEvent(new WebKeyboardEvent()
                    {
                        Type = WebKeyboardEventType.KeyDown,
                        VirtualKeyCode = (VirtualKey)(int)key,
                        NativeKeyCode = (int)key
                    });

                    if ((int)key >= 65 && (int)key <= 90)
                    {
                        this.WebView.InjectKeyboardEvent(new WebKeyboardEvent()
                        {
                            Type = WebKeyboardEventType.Char,
                            Text = key.ToString().ToLower()
                        });
                    }
                    else if (key == Keys.Space)
                    {
                        this.WebView.InjectKeyboardEvent(new WebKeyboardEvent()
                        {
                            Type = WebKeyboardEventType.Char,
                            Text = " "
                        });
                    }
                }
            }

            // Key Up
            foreach (var key in lastPressedKeys)
            {
                if (!currentPressedKeys.Contains(key))
                {
                    this.WebView.InjectKeyboardEvent(new WebKeyboardEvent()
                    {
                        Type = WebKeyboardEventType.KeyUp,
                        VirtualKeyCode = (VirtualKey)(int)key,
                        NativeKeyCode = (int)key
                    });
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Surface != null && Surface.IsDirty && !resizing)
            {
                unsafe
                {
                    // This part saves us from double copying everything.
                    fixed (Byte* imagePtr = this.imageBytes)
                    {
                        Surface.CopyTo((IntPtr)imagePtr, Surface.Width * 4, 4, true, false);
                    }
                }
                this.WebViewTexture.SetData(this.imageBytes);
            }

            base.Draw(gameTime);
        }
    }
}