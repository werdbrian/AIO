using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Common.DX
{
    public class DXContentPane : DXItem
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

        public SharpDX.ColorBGRA DefaultColor
        {
            get;
            set;
        }

        public override Vector2 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }

        private bool Enabled = false;
        private List<DXItem> Items = new List<DXItem>();
        private Render.Rectangle ContentRectangle;
        private Vector2 _Position; 

        /// <summary>
        ///     Content rectangle parameters
        /// </summary>
        public int X, Y, Width, Height;

        public DXContentPane(int X, int Y, int Width, int Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;

            Position = new Vector2(X, Y); 

            ContentRectangle = new Render.Rectangle(X, Y, Width, Height, DefaultColor); 
        }

        public override void Add()
        {
            Update(); 
            ContentRectangle.Add(); 
            Items.ForEach(i => i.Add()); 
        }

        public override void Remove()
        {
            ContentRectangle.Remove();
            Items.ForEach(i => i.Remove()); 
        }
        
        /// <summary>
        ///     Updates the positioning of each DXItem, used to split everything evenly 
        /// http://stackoverflow.com/questions/6190019/split-a-rectangle-into-equal-sized-rectangles
        /// </summary>
        public void Update()
        {
            var size = Items.Count;
            var columns = Math.Ceiling(Math.Sqrt(size));
            var rows = Math.Ceiling(size / columns); // full rows
            var orphan = size % columns; // "remaining" 

            var width = ContentRectangle.Width / columns;
            var height = ContentRectangle.Height / (orphan == 0 ? rows : rows + 1);

            int row = 0, column = 0;

            foreach (var item in Items)
            {
                item.Position = new SharpDX.Vector2(row * (float)width, column * (float)height);

                row++;
                column++; 
            }

            if (orphan > 0)
            {
                var owidth = ContentRectangle.Width / orphan;
                foreach (var item in Items.GetRange(Items.Count - (int)orphan, (int)orphan))
                {
                    item.Position = new SharpDX.Vector2(row * (float)owidth, column * (float)owidth); 
                }
            }
        }
    }
}
