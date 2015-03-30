using AIO.Wrapper;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Common.Rendering
{
    #region SharpDX Snow Effect

    public class SnowEffect : IStepable
    {
        public class SnowParticle : IStepable
        {
            private Render.Rectangle Rectangle;

            private int MaxOffset;
            private int _X, _Modifier;

            public int X
            {
                get
                {
                    return Rectangle.X;
                }
                set
                {
                    Rectangle.X = value;
                }
            }

            public int Y
            {
                get
                {
                    return Rectangle.Y;
                }
                set
                {
                    Rectangle.Y = value;
                }
            }

            public ColorBGRA Color
            {
                get
                {
                    return Rectangle.Color;
                }
                set
                {
                    Rectangle.Color = value;
                }
            }

            public bool Valid
            {
                get
                {
                    return Y < Drawing.Height;
                }
            }

            public SnowParticle(Render.Rectangle Rectangle, int MaxOffset = 15)
            {
                this.MaxOffset = MaxOffset;
                this.Rectangle = Rectangle;

                Rectangle.Add();

                _X = Rectangle.X;
                _Modifier = Program.Random.Next(1, 5);

                if (Program.Random.Next(0, 1000) >= Program.Random.Next(0, 1000))
                    _Modifier = -_Modifier;
            }

            ~SnowParticle()
            {
                // Ensure rectangle is no longer inside the Render list
                Rectangle.Remove();
            }

            public void Add()
            {
                Rectangle.Add();
            }

            public void Step()
            {
                if (Math.Abs(Math.Abs(X) - Math.Abs(_X)) >= MaxOffset || X > Drawing.Width)
                    _Modifier = -_Modifier;

                X += _Modifier;
                Y += Program.Random.Next(1, 5);
            }

            public void Remove()
            {
                Rectangle.Remove();
            }
        }

        private int MaxOffset; // Max "sway" 
        private int MaxParticle; // Max snow particles
        private int ParticleDimension; // Snow width & height 
        private Color PresetColor = Color.White; // What? What other color could snow be? 

        // Instead of using a primitive array, it is much easier to use a flexible array for insertion / removal of particles
        // @See Generic#List<T> for more information 
        private List<SnowParticle> Particles = new List<SnowParticle>();

        public bool Valid
        {
            get
            {
                return Particles.Count >= 1; 
            }
        }

        public SnowEffect(int MaxOffset = 15, int MaxParticle = 150, int ParticleDimension = 30)
        {
            this.MaxOffset = MaxOffset;
            this.MaxParticle = MaxParticle;
            this.ParticleDimension = ParticleDimension;

            for (int i = 0; i < MaxParticle; i++)
            {
                Particles.Add(new SnowParticle(new Render.Rectangle((Drawing.Width / MaxParticle) + ParticleDimension + Program.Random.Next(0, Drawing.Width), Program.Random.Next(Program.Random.Next(0, Drawing.Height - 1), Drawing.Height), ParticleDimension, ParticleDimension, new ColorBGRA(PresetColor.R, PresetColor.G, PresetColor.B, 90)), MaxOffset));
            }
        }

        /// <summary>
        ///     Not absolutely necessary, but this will ensure full removal of all particles before disposal of class
        /// </summary>
        ~SnowEffect()
        {
            Particles.ForEach(p => p.Remove());
        }

        /// <summary>
        ///     Takes a "step" for each particle (each step is usually considered a Frame) 
        /// </summary>
        public void Step()
        {
            foreach (var Particle in Particles)
            {
                if (!Particle.Valid)
                {
                    Particle.Y = Program.Random.Next(0, Drawing.Height);
                }

                Particle.Step();
            }
        }

        public void SetColor(Color color)
        {
            PresetColor = color;

            Particles.ForEach(p => p.Color = new ColorBGRA(PresetColor.R, PresetColor.G, PresetColor.B, 90));
        }

        public void RemoveAll()
        {
            Particles.ForEach(p => p.Remove());
        }
    }

    #endregion 
}
