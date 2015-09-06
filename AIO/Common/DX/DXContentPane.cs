// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DXContentPane.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by 
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//   
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
//    GNU General Public License for more details.
//   
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The dx content pane.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common.DX
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     The dx content pane.
    /// </summary>
    public class DXContentPane : DXItem
    {
        #region Fields

        /// <summary>
        ///     Content rectangle parameters
        /// </summary>
        public int Height;

        /// <summary>
        ///     Content rectangle parameters
        /// </summary>
        public int Width;

        /// <summary>
        ///     Content rectangle parameters
        /// </summary>
        public int X;

        /// <summary>
        ///     Content rectangle parameters
        /// </summary>
        public int Y;

        /// <summary>
        ///     The _ position.
        /// </summary>
        private Vector2 _Position;

        /// <summary>
        ///     The content rectangle.
        /// </summary>
        private Render.Rectangle ContentRectangle;

        /// <summary>
        ///     The enabled.
        /// </summary>
        private bool Enabled = false;

        /// <summary>
        ///     The items.
        /// </summary>
        private List<DXItem> Items = new List<DXItem>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DXContentPane" /> class.
        /// </summary>
        /// <param name="X">
        ///     The x.
        /// </param>
        /// <param name="Y">
        ///     The y.
        /// </param>
        /// <param name="Width">
        ///     The width.
        /// </param>
        /// <param name="Height">
        ///     The height.
        /// </param>
        public DXContentPane(int X, int Y, int Width, int Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;

            this.Position = new Vector2(X, Y);

            this.ContentRectangle = new Render.Rectangle(X, Y, Width, Height, this.DefaultColor);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the default color.
        /// </summary>
        public ColorBGRA DefaultColor { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is enabled.
        /// </summary>
        public override bool IsEnabled
        {
            get
            {
                return this.Enabled;
            }

            set
            {
                this.Enabled = value;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets or sets the position.
        /// </summary>
        public override Vector2 Position
        {
            get
            {
                return this._Position;
            }

            set
            {
                this._Position = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        public override void Add()
        {
            this.Update();
            this.ContentRectangle.Add();
            this.Items.ForEach(i => i.Add());
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        public override void Remove()
        {
            this.ContentRectangle.Remove();
            this.Items.ForEach(i => i.Remove());
        }

        /// <summary>
        ///     Updates the positioning of each DXItem, used to split everything evenly
        ///     http://stackoverflow.com/questions/6190019/split-a-rectangle-into-equal-sized-rectangles
        /// </summary>
        public void Update()
        {
            var size = this.Items.Count;
            var columns = Math.Ceiling(Math.Sqrt(size));
            var rows = Math.Ceiling(size / columns); // full rows
            var orphan = size % columns; // "remaining" 

            var width = this.ContentRectangle.Width / columns;
            var height = this.ContentRectangle.Height / (orphan == 0 ? rows : rows + 1);

            int row = 0, column = 0;

            foreach (var item in this.Items)
            {
                item.Position = new Vector2(row * (float)width, column * (float)height);

                row++;
                column++;
            }

            if (orphan > 0)
            {
                var owidth = this.ContentRectangle.Width / orphan;
                foreach (var item in this.Items.GetRange(this.Items.Count - (int)orphan, (int)orphan))
                {
                    item.Position = new Vector2(row * (float)owidth, column * (float)owidth);
                }
            }
        }

        #endregion
    }
}