// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SnowEffect.cs" company="LeagueSharp">
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
//   The snow effect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common.Rendering
{
    using System;
    using System.Collections.Generic;

    using AIO.Wrapper;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    #region SharpDX Snow Effect

    /// <summary>
    ///     The snow effect.
    /// </summary>
    public class SnowEffect : IStepable
    {
        #region Fields

        /// <summary>
        ///     The max offset.
        /// </summary>
        public int MaxOffset;

        /// <summary>
        ///     The max particle.
        /// </summary>
        public int MaxParticle;

        /// <summary>
        ///     The particle dimension.
        /// </summary>
        public int ParticleDimension;

        // Instead of using a primitive array, it is much easier to use a flexible array for insertion / removal of particles
        // @See Generic#List<T> for more information 
        /// <summary>
        ///     The _particles.
        /// </summary>
        private readonly List<SnowParticle> _particles = new List<SnowParticle>();

        /// <summary>
        ///     The _preset color.
        /// </summary>
        private Color _presetColor = Color.White;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SnowEffect" /> class.
        /// </summary>
        /// <param name="MaxOffset">
        ///     The max offset.
        /// </param>
        /// <param name="MaxParticle">
        ///     The max particle.
        /// </param>
        /// <param name="ParticleDimension">
        ///     The particle dimension.
        /// </param>
        public SnowEffect(int MaxOffset = 15, int MaxParticle = 150, int ParticleDimension = 30)
        {
            this.MaxOffset = MaxOffset;
            this.MaxParticle = MaxParticle;
            this.ParticleDimension = ParticleDimension;

            for (var i = 0; i < MaxParticle; i++)
            {
                this._particles.Add(
                    new SnowParticle(
                        new Render.Rectangle(
                            (Drawing.Width / MaxParticle) + ParticleDimension + Program.Random.Next(0, Drawing.Width), 
                            Program.Random.Next(Program.Random.Next(0, Drawing.Height - 1), Drawing.Height), 
                            ParticleDimension, 
                            ParticleDimension, 
                            new ColorBGRA(this._presetColor.R, this._presetColor.G, this._presetColor.B, 90)), 
                        MaxOffset));
            }
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="SnowEffect" /> class.
        ///     Not absolutely necessary, but this will ensure full removal of all particles before disposal of class
        /// </summary>
        ~SnowEffect()
        {
            this._particles.ForEach(p => p.Remove());
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
                return this._particles.Count >= 1;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The remove all.
        /// </summary>
        public void RemoveAll()
        {
            this._particles.ForEach(p => p.Remove());
        }

        /// <summary>
        ///     The set color.
        /// </summary>
        /// <param name="color">
        ///     The color.
        /// </param>
        public void SetColor(Color color)
        {
            this._presetColor = color;

            this._particles.ForEach(
                p => p.Color = new ColorBGRA(this._presetColor.R, this._presetColor.G, this._presetColor.B, 90));
        }

        /// <summary>
        ///     Takes a "step" for each particle (each step is usually considered a Frame)
        /// </summary>
        public void Step()
        {
            foreach (var particle in this._particles)
            {
                if (!particle.Valid)
                {
                    particle.Y = Program.Random.Next(0, Drawing.Height);
                }

                particle.Step();
            }
        }

        #endregion

        /// <summary>
        ///     The snow particle.
        /// </summary>
        public class SnowParticle : IStepable
        {
            #region Fields

            /// <summary>
            ///     The _max offset.
            /// </summary>
            private readonly int _maxOffset;

            /// <summary>
            ///     The _rectangle.
            /// </summary>
            private readonly Render.Rectangle _rectangle;

            /// <summary>
            ///     The _x.
            /// </summary>
            private readonly int _x;

            /// <summary>
            ///     The _modifier.
            /// </summary>
            private int _modifier;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="SnowParticle" /> class.
            /// </summary>
            /// <param name="Rectangle">
            ///     The rectangle.
            /// </param>
            /// <param name="MaxOffset">
            ///     The max offset.
            /// </param>
            public SnowParticle(Render.Rectangle Rectangle, int MaxOffset = 15)
            {
                this._maxOffset = MaxOffset;
                this._rectangle = Rectangle;

                Rectangle.Add();

                this._x = Rectangle.X;
                this._modifier = Program.Random.Next(1, 5);

                if (Program.Random.Next(0, 1000) >= Program.Random.Next(0, 1000))
                {
                    this._modifier = -this._modifier;
                }
            }

            /// <summary>
            ///     Finalizes an instance of the <see cref="SnowParticle" /> class.
            /// </summary>
            ~SnowParticle()
            {
                // Ensure rectangle is no longer inside the Render list
                this._rectangle.Remove();
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the color.
            /// </summary>
            public ColorBGRA Color
            {
                get
                {
                    return this._rectangle.Color;
                }

                set
                {
                    this._rectangle.Color = value;
                }
            }

            /// <summary>
            ///     Gets a value indicating whether valid.
            /// </summary>
            public bool Valid
            {
                get
                {
                    return this.Y < Drawing.Height;
                }
            }

            /// <summary>
            ///     Gets or sets the x.
            /// </summary>
            public int X
            {
                get
                {
                    return this._rectangle.X;
                }

                set
                {
                    this._rectangle.X = value;
                }
            }

            /// <summary>
            ///     Gets or sets the y.
            /// </summary>
            public int Y
            {
                get
                {
                    return this._rectangle.Y;
                }

                set
                {
                    this._rectangle.Y = value;
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The add.
            /// </summary>
            public void Add()
            {
                this._rectangle.Add();
            }

            /// <summary>
            ///     The remove.
            /// </summary>
            public void Remove()
            {
                this._rectangle.Remove();
            }

            /// <summary>
            ///     The step.
            /// </summary>
            public void Step()
            {
                if (Math.Abs(Math.Abs(this.X) - Math.Abs(this._x)) >= this._maxOffset || this.X > Drawing.Width)
                {
                    this._modifier = -this._modifier;
                }

                this.X += this._modifier;
                this.Y += Program.Random.Next(1, 5);
            }

            #endregion
        }
    }

    #endregion
}