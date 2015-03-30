using AIO.Wrapper;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color; 

namespace AIO.Common.Rendering
{
    public class MinimapNotification : IStepable
    {
        private List<Notification> Notifications = new List<Notification>();

        public bool Valid
        {
            get
            {
                return true; 
            }
        }

        private static int X
        {
            get
            {
                return 0; // Drawing.Minimap
            }
        }

        private static int Y
        {
            get
            {
                return 0; // Drawing.Minimap 
            }
        }

        private static int Width
        {
            get
            {
                return 0;
            }
        }

        private static int Height
        {
            get
            {
                return 0;
            }
        }

        public class Notification : IStepable
        {
            private Color PresetColor = Color.White;
            private Render.Sprite Sprite;
            private Render.Text Text;

            public bool Valid
            {
                get
                {
                    return Sprite.X <= X + Width;
                }
            }

            public Notification(string Text, Bitmap Bit)
            {
                Sprite = new Render.Sprite(Bit, new Vector2(X, Y));
                this.Text = new Render.Text(Text, new Vector2(X, Y), 12, new ColorBGRA(PresetColor.R, PresetColor.G, PresetColor.B, PresetColor.A));
                this.Text.Centered = true;

                this.Text.Add();
                Sprite.Add();
            }

            public void SetColor(Color PresetColor)
            {
                this.PresetColor = PresetColor;

                Text.Color = ColorBGRA.FromRgba(PresetColor.ToArgb());
            }

            public void Step()
            {
                if (Valid)
                {
                    Text.X += 1;
                    Sprite.X += 1;
                }
                else
                    Remove();
            }

            public void Remove()
            {
                Text.Remove();
                Sprite.Remove();
            }
        }

        public void Step()
        {
            Notifications.ForEach(p => p.Step());
        }

        public void RemoveAll()
        {
            Notifications.ForEach(p => p.Remove());
            Notifications.Clear();
        }
    }
}
