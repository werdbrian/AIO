using AIO.Wrapper;
using LeagueSharp;
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
    [Obsolete("Use LeagueSharp.Common")]
    public class AlertSystem : IStepable
    {
        public class Alert
        {
            public enum AnimationStage
            {
                SlideIn,
                SlideOut,
                Display
            }

            public string Text
            {
                get
                {
                    return TextUpdate != null ? TextUpdate() : DisplayText; 
                }
            }

            public AnimationStage Stage = AnimationStage.SlideIn; 
            public string DisplayText;
            public int End, Start, InternalTimer = 0;

            public TextUpdateDelegate TextUpdate; 
            public delegate string TextUpdateDelegate();

            public Alert(string DisplayText, int Duration, TextUpdateDelegate TextUpdate = null)
            {
                this.DisplayText = DisplayText;
                this.TextUpdate = TextUpdate; 

                Start = Environment.TickCount;
                End = Start + Duration;
            }
        }

        private int Duration, MaxAlerts;
        private List<Alert> Alerts = new List<Alert>();

        public AlertSystem(int MaxAlerts = 5, int Duration = 10000)
        {
            this.MaxAlerts = MaxAlerts;
            this.Duration = Duration; 
        }

        public bool Valid
        {
            get { return true; }
        }

        public void Step()
        {
            if (Alerts.Count == 0)
                return;

            var StartX = Drawing.Width * 0.89f;
            var StartY = Drawing.Height * 0.69f; 

            for (int i = 0; i < MaxAlerts; i++)
            {
                var Alert = Alerts.ElementAt(i);
                var completed = (int)Math.Round((double)(100 * (Environment.TickCount - Alert.Start)) / Duration);
                var alpha = i == 0 ? 27 : Alerts.Count > MaxAlerts ? (255 / MaxAlerts) * i : (255 / Alerts.Count) * i;

                //Drawing.DrawText(StartX, StartY - (300 + (i * 15)), Color.FromArgb(i == 0 ? 27 : 51 * i, Color.White), "" + completed);

                switch (Alert.Stage)
                {
                    case AlertSystem.Alert.AnimationStage.SlideIn:
                        {
                            if (Alert.InternalTimer > 5)
                            {
                                Alert.Stage = AlertSystem.Alert.AnimationStage.Display;
                            }
                            else
                            {
                                Drawing.DrawText((StartX - 50) + (Alert.InternalTimer * 10), StartY - (i * 15), Color.FromArgb(alpha, Color.White), Alert.Text);
                                Alert.InternalTimer++; 
                            }
                        }
                        break; 
                    case AlertSystem.Alert.AnimationStage.SlideOut:
                        {
                            if (Alert.InternalTimer > 5)
                            {
                                Alerts.Remove(Alert); 
                                continue;
                            }
                            else
                            {
                                Drawing.DrawText(StartX + (Alert.InternalTimer * 10), StartY - (i * 15), Color.FromArgb(alpha, Color.White), Alert.Text);
                                Alert.InternalTimer++; 
                            }                           
                        }
                        break;
                    case AlertSystem.Alert.AnimationStage.Display:
                        {
                            Drawing.DrawText(StartX, StartY - (i * 15), Color.FromArgb(alpha, Color.White), Alert.Text);

                            if (completed > 80)
                            {
                                Alert.Stage = AlertSystem.Alert.AnimationStage.SlideOut;
                                Alert.InternalTimer = 0; 
                            }
                        }
                        break;
                }
            }
        }

        public void Add(string Text)
        {
            var Alert = new AlertSystem.Alert(Text, Duration);

            if (Alerts.Count > 4 && Alerts.Count < 9)
                Alert.Start = Environment.TickCount + Duration; // Increase duration x 2 
            else if (Alerts.Count > 9 && Alerts.Count < 14)
                Alert.Start = Environment.TickCount + (Duration * 2); 

            Alerts.Add(Alert);
        }

        public void Add(string Text, params object[] formatted)
        {
            Add(string.Format(Text, formatted)); 
        }
    }
}
