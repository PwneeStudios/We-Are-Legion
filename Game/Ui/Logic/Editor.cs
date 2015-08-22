using System;
using System.IO;

using Windows = System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

using Awesomium.Core;
using Awesomium.Core.Data;
using Awesomium.Core.Dynamic;
using AwesomiumXNA;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;

using Newtonsoft.Json;

namespace Game
{
    using Dict = Dictionary<string, object>;

    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        void BindMethods_Editor()
        {
            xnaObj.Bind("PlayButtonPressed", PlayButtonPressed);
            xnaObj.Bind("EditorUiClicked", EditorUiClicked);

            xnaObj.Bind("SetUnitPaint", (sender, e) => { World.SetUnitPaint((int)e.Arguments[0]); return JSValue.Null; });
            xnaObj.Bind("SetTilePaint", (sender, e) => { World.SetTilePaint((int)e.Arguments[0]); return JSValue.Null; });
            xnaObj.Bind("SetPlayer", (sender, e) => { World.Editor_SwitchPlayer((int)e.Arguments[0]); return JSValue.Null; });
            xnaObj.Bind("SetPaintChoice", (sender, e) => { World.SetUnitPlaceStyle((int)e.Arguments[0]); return JSValue.Null; });

            xnaObj.Bind("GetMaps", GetMaps);
            xnaObj.Bind("LoadMap", LoadMap);
            xnaObj.Bind("SaveMap", SaveMap);
            xnaObj.Bind("CreateNewMap", CreateNewMap);
        }

        public void UpdateEditorJsData()
        {
            Send("updateEditor",
                new
                {
                    EditorActive = World.MapEditorActive,
                    UnitPlaceStyle = (int)Math.Round(World.UnitPlaceStyle),
                }
            );
        }

        JSValue PlayButtonPressed(object sender, JavascriptMethodEventArgs e)
        {
            World.Editor_ToggleMapEditor();

            return JSValue.Null;
        }

        JSValue EditorUiClicked(object sender, JavascriptMethodEventArgs e)
        {
            if (!World.MapEditorActive) return JSValue.Null;

            World.SetModeToSelect();

            return JSValue.Null;
        }

        object _GetMaps(string path)
        {
            var Maps = new List<object>();

            foreach (string file in Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                Maps.Add(new {
                    name = name,
                    list = _GetMaps(Path.Combine(path, name)),
                });
            }

            foreach (string file in Directory.EnumerateFiles(path, "*.m3n", SearchOption.TopDirectoryOnly))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                Maps.Add(name);
            }

            return Maps;
        }

        JSValue GetMaps(object sender, JavascriptMethodEventArgs e)
        {
            string directory = e.Arguments[0];
            string path = Path.Combine(MapDirectory, directory);

            return Jsonify(_GetMaps(path));
        }

        JSValue LoadMap(object sender, JavascriptMethodEventArgs e)
        {
            string path = e.Arguments[0];
            path.Replace('/', '\\');

            path = Path.Combine(MapDirectory, path);
            path = Path.ChangeExtension(path, "m3n");

            Console.WriteLine("Loading map {0}...", path);
            NewWorldEditor(path);

            return JSValue.Null;
        }

        JSValue SaveMap(object sender, JavascriptMethodEventArgs e)
        {
            string path;
            if (e.Arguments.Length == 0 || e.Arguments[0] == JSValue.Undefined)
            {
                path = World.MapFilePath;
                if (path == null || path.Length == 0) return JSValue.Null;
            }
            else
            {
                path = e.Arguments[0];
                path.Replace('/', '\\');

                if (path == null || path.Length == 0) return JSValue.Null;

                path = Path.Combine(MapDirectory, path);
                path = Path.ChangeExtension(path, "m3n");
            }

            Console.WriteLine("Saving map {0}...", path);

            World.Save(path);

            return JSValue.Null;
        }

        JSValue CreateNewMap(object sender, JavascriptMethodEventArgs e)
        {
            NewWorldEditor();

            return JSValue.Null;
        }
    }
}
