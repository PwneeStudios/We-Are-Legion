using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Steamworks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FragSharpHelper;

namespace Game
{
    public class JavascriptMethodEventArgs
    {
        public List<object> Arguments = new List<object>();
    }

    public class WebHelper
    {
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

        private MouseState lastMouseState;
        private MouseState currentMouseState;
        private Keys[] lastPressedKeys;
        private Keys[] currentPressedKeys = new Keys[0];
        public Texture2D WebViewTexture { get; private set; }

        public void Update()
        {
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            /*
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
            */
        }
    }
}

