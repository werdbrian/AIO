// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DXLogPane.cs" company="LeagueSharp">
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
//   The dx log pane.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common.DX
{
    using System.Collections.Generic;

    using AIO.Helpers;

    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     The dx log pane.
    /// </summary>
    public class DXLogPane : DXItem
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
        ///     The entries.
        /// </summary>
        private Dictionary<Render.Text, uint> Entries = new Dictionary<Render.Text, uint>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DXLogPane" /> class.
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
        public DXLogPane(int X, int Y, int Width, int Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;

            this.ContentRectangle = new Render.Rectangle(X, Y, Width, Height, this.DefaultColor);
            this.Position = new Vector2(X, Y);
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

            foreach (var item in this.Entries)
            {
                item.Key.Add();
            }
        }

        /// <summary>
        ///     The insert.
        /// </summary>
        /// <param name="Text">
        ///     The text.
        /// </param>
        public void Insert(Render.Text Text)
        {
            Text.VisibleCondition = (a) => { return this.ContentRectangle.Contains(new Vector2(Text.X, Text.Y)); };
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        public override void Remove()
        {
            this.ContentRectangle.Remove();

            foreach (var item in this.Entries)
            {
                item.Key.Remove();
            }
        }

        /// <summary>
        ///     The update.
        /// </summary>
        public void Update()
        {
            var offset = 0;
            foreach (var item in this.Entries)
            {
                if (item.Value > Utils.TickCount)
                {
                    this.Entries.Remove(item.Key);
                    break;
                }

                item.Key.X = this.X + 15;
                item.Key.Y = (this.Y - this.ContentRectangle.Height) + offset;

                offset += 15;
            }
        }

        #endregion
    }
}