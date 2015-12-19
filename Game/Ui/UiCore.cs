using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Input = FragSharpHelper.Input;

using Awesomium.Core;
using Awesomium.Core.Data;
using AwesomiumXNA;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Newtonsoft.Json;

namespace Game
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

    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        public JSObject xnaObj;
        public AwesomiumComponent awesomium;

        void AwesomiumInitialize()
        {
            return;

            if (awesomium != null)
            {
                Components.Remove(awesomium);
                awesomium.Dispose();
            }

            awesomium = new AwesomiumComponent(this, new Rectangle(0, 0, Program.Width, Program.Height));

            // Don't forget to add the awesomium component to the game
            Components.Add(awesomium);

            WebCore.QueueWork(() => {
                // Add a data source that will simply act as a pass-through
                awesomium.WebView.WebSession.AddDataSource("root", new MainDataSource());

                // This will trap all console messages
                awesomium.WebView.ConsoleMessage += WebView_ConsoleMessage;

                // A document must be loaded in order for me to make a global JS object, but the presence of
                // the global JS object affects the first page to be loaded, so give it an egg:
                while (!awesomium.WebView.IsLive)
                {
                    Thread.Sleep(1);
                }

                awesomium.WebView.LoadHTML("<html><head><title>Loading...</title></head><body></body></html>");
            });

            WebCore.QueueWork(LoadMainPage);
        }

        void LoadMainPage()
        {
            if (!awesomium.WebView.IsDocumentReady)
            {
                WebCore.QueueWork(LoadMainPage);
                //WebCore.Update();
                return;
            }

            // Trap log commands so that we can differentiate between log statements and JS errors
            JSObject console = awesomium.WebView.CreateGlobalJavascriptObject("console");
            console.Bind("log", WebView_ConsoleLog);
            console.Bind("dir", WebView_ConsoleLog);

            // Create an object that will allow JS inside Awesomium to communicate with XNA
            xnaObj = awesomium.WebView.CreateGlobalJavascriptObject("xna");
            BindMethods();

            awesomium.WebView.Source = @"asset://root/index.html".ToUri();

            //WaitForPage ();
        }

        void WaitForPage()
        {
            if (!awesomium.WebView.IsDocumentReady)
            {
                WebCore.QueueWork(WaitForPage);
                WebCore.Update();
                return;
            }
        }

        JSValue WebView_ConsoleLog(object sender, JavascriptMethodEventArgs javascriptMethodEventArgs)
        {
            //Console.WriteLine(javascriptMethodEventArgs.Arguments[0].ToString());
            return JSValue.Null;
        }

        void WebView_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            // All JS errors will come here
            string msg = string.Format("Awesomium JS Error: {0}, {1} on line {2}", e.Message, e.Source, e.LineNumber);

            Console.WriteLine(msg);
            Program.LogDump(msg);
        }

        /// <summary>
        /// On Exit callback from JavaScript from Awesomium
        /// </summary>
        public JSValue OnExit(object sender, JavascriptMethodEventArgs e)
        {
            Exit();
            return JSValue.Null;
        }

        private void DrawWebView()
        {
            if (awesomium != null && awesomium.WebViewTexture != null)
            {
                Render.StartText();
                Render.MySpriteBatch.Draw(awesomium.WebViewTexture, GraphicsDevice.Viewport.Bounds, Color.White);
                Render.EndText();
            }
        }

        public bool MouseDownOverUi = false;
        public void CalculateMouseDownOverUi()
        {
            if (!GameInputEnabled)
            {
                if (awesomium != null) awesomium.AllowMouseEvents = true;
                MouseOverHud = true;
                MouseDownOverUi = true;
                return;
            }

            if (World != null && World.BoxSelecting)
            {
                if (awesomium != null) awesomium.AllowMouseEvents = false;
            }
            else
            {
                if (awesomium != null) awesomium.AllowMouseEvents = true;
            }

            if (!Input.LeftMouseDown || !MouseOverHud)
            {
                MouseDownOverUi = false;
            }
            else
            {

                try
                {
                    Render.UnsetDevice();
                    if (awesomium != null) MouseDownOverUi = awesomium.WebViewTexture.GetData(Input.CurMousePos).A > 20;
                }
                catch
                {
                    MouseDownOverUi = false;
                }
            }
        }

        JsonSerializer jsonify = new JsonSerializer();
        JsonSerializerSettings settings = new JsonSerializerSettings();
        Dictionary<string, object> obj = new Dictionary<string, object>(100);

        string Jsonify(object obj)
        {
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var json = JsonConvert.SerializeObject(obj, Formatting.None, settings);
            return json;
        }

        public void Send(string function, params object[] args)
        {
            string s = "";
            bool first = true;

            foreach (var arg in args)
            {
                if (!first) { s += ","; }

                s += Jsonify(arg);
                first = false;
            }

            if (awesomium != null)
            {
                WebCore.QueueWork(() =>
                {
                    try
                    {
                        awesomium.WebView.ExecuteJavascript(function + "(" + s + ");");
                        //var result = awesomium.WebView.ExecuteJavascriptWithResult(function + "(" + s + ");");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Could not communicate with Awesomium, {0}({1}): {2}", function, s, e);
                    }
                });
            }
        }
    }
}
