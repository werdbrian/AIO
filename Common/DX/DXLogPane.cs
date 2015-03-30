using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIO.Helpers; 

namespace AIO.Common.DX
{
    public class DXLogPane : DXItem
    {
        public override bool IsValid
        {
            get { return true; }
        }

        public override bool IsEnabled
        {
            get
            {
                return Enabled; 
            }
            set
            {
                Enabled = value;
            }
        }

        public override Vector2 Position
        {
            get
            {
                return _Position; 
            }
            set
            {
                _Position = value; 
            }
        }

        public ColorBGRA DefaultColor
        {
            get;
            set;
        }

        private Dictionary<Render.Text, uint> Entries = new Dictionary<Render.Text, uint>(); 
        private bool Enabled = false;
        private Render.Rectangle ContentRectangle; 
        private Vector2 _Position; 

        /// <summary>
        ///     Content rectangle parameters
        /// </summary>
        public int X, Y, Width, Height;

        public DXLogPane(int X, int Y, int Width, int Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;

            ContentRectangle = new Render.Rectangle(X, Y, Width, Height, DefaultColor); 
            Position = new Vector2(X, Y); 
        }

        public override void Add()
        {
            Update(); 
            ContentRectangle.Add();

            foreach (var item in Entries)
            {
                item.Key.Add(); 
            }
        }

        public override void Remove()
        {
            ContentRectangle.Remove();

            foreach (var item in Entries)
            {
                item.Key.Remove(); 
            }
        }

        public void Update()
        {
            var offset = 0; 
            foreach (var item in Entries)
            {
                if (item.Value > Utils.TickCount)
                {
                    Entries.Remove(item.Key);
                    break; 
                }

                item.Key.X = X + 15;
                item.Key.Y = (Y - ContentRectangle.Height) + offset;

                offset += 15; 
            }
        }

        public void Insert(Render.Text Text)
        {
            Text.VisibleCondition = (a) =>
            {
                return ContentRectangle.Contains(new Vector2(Text.X, Text.Y));
            };
        }
    }
}
