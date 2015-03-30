using LeagueSharp;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO
{
    /// <summary>
    ///     Credit to detuks (Again, no credit to myself, this is all credit to original author) 
    ///     https://github.com/detuks/LeagueSharp/blob/master/JayceSharpV2/HpBarIndicator.cs
    /// </summary>
    public class HpDamageIndicator
    {

        public Device DirectXDevice = Drawing.Direct3DDevice;
        public Line DirectXLine;
        
        public Obj_AI_Base unit { get; set; }

        public float width = 104;

        public float hight = 9;

        public HpDamageIndicator()
        {
            DirectXLine = new Line(DirectXDevice) { Width = 9 };
            Drawing.OnPreReset += DrawingOnOnPreReset;
            Drawing.OnPostReset += DrawingOnOnPostReset;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;

        }


        private void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            DirectXLine.Dispose();
        }

        private void DrawingOnOnPostReset(EventArgs args)
        {
            DirectXLine.OnResetDevice();
        }

        private void DrawingOnOnPreReset(EventArgs args)
        {
            DirectXLine.OnLostDevice();
        }

        private Vector2 Offset
        {
            get
            {
                if (unit != null)
                {
                    return unit.IsAlly ? new Vector2(34, 9) : new Vector2(10, 20);
                }

                return new Vector2();
            }
        }

        public Vector2 StartPosition
        {

            get { return new Vector2(unit.HPBarPosition.X + Offset.X, unit.HPBarPosition.Y + Offset.Y); }
        }


        private float CalcDamage(float dmg = 0)
        {
            float health = ((unit.Health - dmg) > 0) ? (unit.Health - dmg) : 0;
            return (health / unit.MaxHealth);
        }

        private Vector2 PositionAfterDamage(float dmg)
        {
            float w = CalcDamage(dmg) * width;
            return new Vector2(StartPosition.X + w, StartPosition.Y);
        }

        public void DrawDamage(Obj_AI_Base unit, float dmg, System.Drawing.Color color)
        {
            this.unit = unit; 

            var hpPosNow = PositionAfterDamage(0);
            var hpPosAfter = PositionAfterDamage(dmg);

            FillBar(hpPosNow, hpPosAfter, color);
            //fillHPBar((int)(hpPosNow.X - startPosition.X), (int)(hpPosAfter.X- startPosition.X), color);
        }

        private void FillBar(int to, int from, System.Drawing.Color color)
        {
            Vector2 sPos = StartPosition;

            for (int i = from; i < to; i++)
            {
                Drawing.DrawLine(sPos.X + i, sPos.Y, sPos.X + i, sPos.Y + 9, 1, color);
            }
        }

        private void FillBar(Vector2 from, Vector2 to, System.Drawing.Color color)
        {
            DirectXLine.Begin();

            DirectXLine.Draw(new[] 
            {
                new Vector2((int)from.X, (int)from.Y + 4f),
                new Vector2( (int)to.X, (int)to.Y + 4f)
            }, new ColorBGRA(color.R, color.G, color.B, 90)); 

            // Vector2 sPos = startPosition;
            //Drawing.DrawLine((int)from.X, (int)from.Y + 9f, (int)to.X, (int)to.Y + 9f, 9f, color);

            DirectXLine.End();
        }

    }
}
