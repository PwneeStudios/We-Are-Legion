using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Input = FragSharpHelper.Input;

using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Newtonsoft.Json;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// On Exit callback from JavaScript from Awesomium
        /// </summary>
        public void OnExit(object sender, JavascriptMethodEventArgs e)
        {
            Exit();
        }

        private void DrawWebView()
        {
            if (SteamWrapper.SteamHtml.Texture != null)
            {
                Render.StartText();
                Render.MySpriteBatch.Draw(SteamWrapper.SteamHtml.Texture, GraphicsDevice.Viewport.Bounds, Color.White);
                Render.EndText();
            }
        }

        public bool MouseDownOverUi = false;
        public void CalculateMouseDownOverUi()
        {
            //fixme
            /*
            if (!GameInputEnabled)
            {
                SteamWrapper.SteamHtml.AllowMouseEvents = true;
                MouseOverHud = true;
                MouseDownOverUi = true;
                return;
            }

            if (World != null && World.BoxSelecting)
            {
                SteamWrapper.SteamHtml.AllowMouseEvents = false;
            }
            else
            {
                SteamWrapper.SteamHtml.AllowMouseEvents = true;
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
                    MouseDownOverUi = awesomium.WebViewTexture.GetData(Input.CurMousePos).A > 20;
                }
                catch
                {
                    MouseDownOverUi = false;
                }
            }*/
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

            //fixme
        }
    }
}
