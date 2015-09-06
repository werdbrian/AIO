// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MinimapNotification.cs" company="LeagueSharp">
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
//   The minimap notification.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common.Rendering
{
    using System.Collections.Generic;
    using System.Drawing;

    using AIO.Wrapper;

    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    ///     The minimap notification.
    /// </summary>
    public class MinimapNotification : IStepable
    {
        #region Fields

        /// <summary>
        ///     The notifications.
        /// </summary>
        private List<Notification> Notifications = new List<Notification>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether valid.
        /// </summary>
        public bool Valid
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the height.
        /// </summary>
        private static int Height
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        private static int Width
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        ///     Gets the x.
        /// </summary>
        private static int X
        {
            get
            {
                return 0; // Drawing.Minimap
            }
        }

        /// <summary>
        ///     Gets the y.
        /// </summary>
        private static int Y
        {
            get
            {
                return 0; // Drawing.Minimap 
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The remove all.
        /// </summary>
        public void RemoveAll()
        {
            this.Notifications.ForEach(p => p.Remove());
            this.Notifications.Clear();
        }

        /// <summary>
        ///     The step.
        /// </summary>
        public void Step()
        {
            this.Notifications.ForEach(p => p.Step());
        }

        #endregion

        /// <summary>
        ///     The notification.
        /// </summary>
        public class Notification : IStepable
        {
            #region Fields

            /// <summary>
            ///     The preset color.
            /// </summary>
            private Color PresetColor = Color.White;

            /// <summary>
            ///     The sprite.
            /// </summary>
            private Render.Sprite Sprite;

            /// <summary>
            ///     The text.
            /// </summary>
            private Render.Text Text;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Notification" /> class.
            /// </summary>
            /// <param name="Text">
            ///     The text.
            /// </param>
            /// <param name="Bit">
            ///     The bit.
            /// </param>
            public Notification(string Text, Bitmap Bit)
            {
                this.Sprite = new Render.Sprite(Bit, new Vector2(X, Y));
                this.Text = new Render.Text(
                    Text, 
                    new Vector2(X, Y), 
                    12, 
                    new ColorBGRA(this.PresetColor.R, this.PresetColor.G, this.PresetColor.B, this.PresetColor.A));
                this.Text.Centered = true;

                this.Text.Add();
                this.Sprite.Add();
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets a value indicating whether valid.
            /// </summary>
            public bool Valid
            {
                get
                {
                    return this.Sprite.X <= X + Width;
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The remove.
            /// </summary>
            public void Remove()
            {
                this.Text.Remove();
                this.Sprite.Remove();
            }

            /// <summary>
            ///     The set color.
            /// </summary>
            /// <param name="PresetColor">
            ///     The preset color.
            /// </param>
            public void SetColor(Color PresetColor)
            {
                this.PresetColor = PresetColor;

                this.Text.Color = ColorBGRA.FromRgba(PresetColor.ToArgb());
            }

            /// <summary>
            ///     The step.
            /// </summary>
            public void Step()
            {
                if (this.Valid)
                {
                    this.Text.X += 1;
                    this.Sprite.X += 1;
                }
                else
                {
                    this.Remove();
                }
            }

            #endregion
        }
    }
}