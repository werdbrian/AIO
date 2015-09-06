// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DXButton.cs" company="LeagueSharp">
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
//   The dx button.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common.DX
{
    using System;
    using System.Drawing;

    using AIO.Helpers;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     The dx button.
    /// </summary>
    public class DXButton : DXItem
    {
        #region Fields

        /// <summary>
        ///     Content rectangle parameters
        /// </summary>
        public int Height;

        /// <summary>
        ///     Text retrieval delegate for live text
        /// </summary>
        public TextUpdateH TextUpdate;

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
        ///     Internal enable variable
        /// </summary>
        private bool Enabled = false;

        /// <summary>
        ///     The render text.
        /// </summary>
        private Render.Text RenderText;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DXButton" /> class.
        ///     A DirectX rendered button
        /// </summary>
        /// <param name="X">
        /// </param>
        /// <param name="Y">
        /// </param>
        /// <param name="Width">
        /// </param>
        /// <param name="Height">
        /// </param>
        /// <param name="TextUpdate">
        /// </param>
        /// <param name="CanToggle">
        /// </param>
        /// <param name="IsToggled">
        /// </param>
        public DXButton(
            int X, 
            int Y, 
            int Width, 
            int Height, 
            TextUpdateH TextUpdate, 
            bool CanToggle = false, 
            bool IsToggled = false)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            this.TextUpdate = TextUpdate;
            this.CanToggle = CanToggle;
            this.IsToggled = IsToggled;

            this.ContentRectangle = new Render.Rectangle(X, Y, Width, Height, this.DefaultColor);
            this.Position = new Vector2(X, Y);

            var size = this.ContentSize;
            this.RenderText = new Render.Text(string.Empty, new Vector2(0, 0), 0, new ColorBGRA(0));
            this.RenderText.TextUpdate = () => { return TextUpdate(); };
            this.RenderText.PositionUpdate =
                () =>
                    {
                        return new Vector2(
                            (this.ContentRectangle.Width - size.Width) / 2, 
                            (this.ContentRectangle.Height - size.Height) / 2);
                    };

            this.OnClick += (Vector2 v) =>
                {
                    if (CanToggle)
                    {
                        IsToggled = !IsToggled;
                        this.ContentRectangle.Color = IsToggled ? this.ToggleColor : this.DefaultColor;
                    }
                };

            Game.OnWndProc += this.Game_OnWndProc;
            Game.OnUpdate += this.Game_OnUpdate;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="DXButton" /> class.
        ///     Deconstructor
        /// </summary>
        ~DXButton()
        {
            this.ContentRectangle.Remove();
            this.RenderText.Remove();
            Game.OnWndProc -= this.Game_OnWndProc;
            Game.OnUpdate -= this.Game_OnUpdate;
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     The on click h.
        /// </summary>
        /// <param name="v">
        ///     The v.
        /// </param>
        public delegate void OnClickH(Vector2 v);

        /// <summary>
        ///     The on hover h.
        /// </summary>
        /// <param name="v">
        ///     The v.
        /// </param>
        public delegate void OnHoverH(Vector2 v);

        /// <summary>
        ///     The text update h.
        /// </summary>
        public delegate string TextUpdateH();

        #endregion

        #region Public Events

        /// <summary>
        ///     On click event
        /// </summary>
        public event OnClickH OnClick;

        /// <summary>
        ///     On hover event
        /// </summary>
        public event OnHoverH OnHover;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Allows the button to become a toggle button
        /// </summary>
        public bool CanToggle { get; set; }

        /// <summary>
        ///     Drawing#Size of text
        /// </summary>
        public Size ContentSize
        {
            get
            {
                return Drawing.GetTextExtent(this.TextUpdate());
            }
        }

        /// <summary>
        ///     Default button color
        /// </summary>
        public ColorBGRA DefaultColor { get; set; }

        /// <summary>
        ///     Simple get/set for an enabled bool, on set attaches DirectX Rendering or removes it
        /// </summary>
        public override bool IsEnabled
        {
            get
            {
                return this.Enabled;
            }

            set
            {
                if (value)
                {
                    this.Add();
                }
                else
                {
                    this.Remove();
                }

                this.Enabled = value;
            }
        }

        /// <summary>
        ///     Stores the variable if the button is toggled
        /// </summary>
        public bool IsToggled { get; set; }

        /// <summary>
        ///     Validation of the DXItem, always true
        /// </summary>
        public override bool IsValid
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Content rectangle position
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

        /// <summary>
        ///     Color for Toggled button
        /// </summary>
        public ColorBGRA ToggleColor { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        public override void Add()
        {
            this.ContentRectangle.Add();
            this.RenderText.Add();
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        public override void Remove()
        {
            this.ContentRectangle.Remove();
            this.RenderText.Remove();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The game_ on update.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private void Game_OnUpdate(EventArgs args)
        {
            if (this.ContentRectangle.Contains(Utils.GetCursorPos()))
            {
                if (this.OnHover != null)
                {
                    this.OnHover(Utils.GetCursorPos());
                }
            }
        }

        /// <summary>
        ///     The game_ on wnd proc.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDBLCLCK)
            {
                return;
            }

            if (this.ContentRectangle.Contains(Utils.GetCursorPos()))
            {
                if (this.OnClick != null)
                {
                    this.OnClick(Utils.GetCursorPos());
                }
            }
        }

        #endregion
    }
}