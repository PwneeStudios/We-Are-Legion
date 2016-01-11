using System;
using System.IO;
using System.Collections.Generic;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        public void SetUnitPaint(int type) { World.SetUnitPaint(type); }
        public void SetTilePaint(int type) { World.SetTilePaint(type); }
        public void SetPlayer(int player) { World.Editor_SwitchPlayer(player); }
        public void SetPaintChoice(int style) { World.SetUnitPlaceStyle(style); }

        public void UpdateEditorJsData()
        {
            if (!World.MapEditor) return;

            Send("updateEditor",
                new
                {
                    EditorActive = World.MapEditorActive,
                    UnitPlaceStyle = (int)Math.Round(World.UnitPlaceStyle),
                    MapName = Path.GetFileNameWithoutExtension(World.MapFilePath),
                }
            );
        }

        public void SendCommand(string command)
        {
            Send("command", command);
        }

        public void StartEditor()
        {
            State = GameState.ToEditor;
        }

        public void PlayButtonPressed()
        {
            World.Editor_ToggleMapEditor();

            if (!World.MapEditorActive)
            {
                World.FinalizeGeodesics();
            }
        }

        public void EditorUiClicked()
        {
            if (!World.MapEditorActive)

            World.SetModeToSelect();
        }

        public void ToggleChat(bool state)
        {
            if (state)
            {
                GameClass.Game.ToggleChatViaFlag(Toggle.On);
            }
            else
            {
                GameClass.Game.ToggleChatViaFlag(Toggle.Off);
            }
        }

        public object _GetMaps(string path)
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

        public string GetMaps(string directory)
        {
            string path = Path.Combine(MapDirectory, directory);

            return Jsonify(_GetMaps(path));
        }

        public void LoadMap(string path)
        {
            path.Replace('/', '\\');

            path = Path.Combine(MapDirectory, path);
            path = Path.ChangeExtension(path, "m3n");

            Console.WriteLine("Loading map {0}...", path);
            NewWorldEditor(path);
        }

        public void SaveMap(string path = null)
        {
            if (path == null)
            {
                path = World.MapFilePath;
                
                if (path == null || path.Length == 0)

                path = Path.GetFileName(path);
                path = Path.Combine("Custom", path);
                path = Path.Combine(MapDirectory, path);
                path = Path.ChangeExtension(path, "m3n");
            }
            else
            {
                path.Replace('/', '\\');

                if (path == null || path.Length == 0)

                path = Path.Combine(MapDirectory, path);
                path = Path.ChangeExtension(path, "m3n");
            }

            Console.WriteLine("Finalizing geodesics...", path);
            World.FinalizeGeodesics();

            Console.WriteLine("Saving map {0}...", path);
            World.Save(path);
        }

        public void CreateNewMap()
        {
            NewWorldEditor();
        }
    }
}
