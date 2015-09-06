// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlertSystem.cs" company="LeagueSharp">
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
//   The alert system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using AIO.Wrapper;

    using LeagueSharp;

    /// <summary>
    ///     The alert system.
    /// </summary>
    [Obsolete("Use LeagueSharp.Common")]
    public class AlertSystem : IStepable
    {
        #region Fields

        /// <summary>
        ///     The alerts.
        /// </summary>
        private List<Alert> Alerts = new List<Alert>();

        /// <summary>
        ///     The duration.
        /// </summary>
        private int Duration;

        /// <summary>
        ///     The max alerts.
        /// </summary>
        private int MaxAlerts;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AlertSystem" /> class.
        /// </summary>
        /// <param name="MaxAlerts">
        ///     The max alerts.
        /// </param>
        /// <param name="Duration">
        ///     The duration.
        /// </param>
        public AlertSystem(int MaxAlerts = 5, int Duration = 10000)
        {
            this.MaxAlerts = MaxAlerts;
            this.Duration = Duration;
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
                return true;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="Text">
        ///     The text.
        /// </param>
        public void Add(string Text)
        {
            var Alert = new Alert(Text, this.Duration);

            if (this.Alerts.Count > 4 && this.Alerts.Count < 9)
            {
                Alert.Start = Environment.TickCount + this.Duration; // Increase duration x 2 
            }
            else if (this.Alerts.Count > 9 && this.Alerts.Count < 14)
            {
                Alert.Start = Environment.TickCount + (this.Duration * 2);
            }

            this.Alerts.Add(Alert);
        }

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="Text">
        ///     The text.
        /// </param>
        /// <param name="formatted">
        ///     The formatted.
        /// </param>
        public void Add(string Text, params object[] formatted)
        {
            this.Add(string.Format(Text, formatted));
        }

        /// <summary>
        ///     The step.
        /// </summary>
        public void Step()
        {
            if (this.Alerts.Count == 0)
            {
                return;
            }

            var StartX = Drawing.Width * 0.89f;
            var StartY = Drawing.Height * 0.69f;

            for (var i = 0; i < this.MaxAlerts; i++)
            {
                var Alert = this.Alerts.ElementAt(i);
                var completed = (int)Math.Round((double)(100 * (Environment.TickCount - Alert.Start)) / this.Duration);
                var alpha = i == 0
                                ? 27
                                : this.Alerts.Count > this.MaxAlerts
                                      ? (255 / this.MaxAlerts) * i
                                      : (255 / this.Alerts.Count) * i;

                // Drawing.DrawText(StartX, StartY - (300 + (i * 15)), Color.FromArgb(i == 0 ? 27 : 51 * i, Color.White), "" + completed);
                switch (Alert.Stage)
                {
                    case Alert.AnimationStage.SlideIn:
                        {
                            if (Alert.InternalTimer > 5)
                            {
                                Alert.Stage = Alert.AnimationStage.Display;
                            }
                            else
                            {
                                Drawing.DrawText(
                                    (StartX - 50) + (Alert.InternalTimer * 10), 
                                    StartY - (i * 15), 
                                    Color.FromArgb(alpha, Color.White), 
                                    Alert.Text);
                                Alert.InternalTimer++;
                            }
                        }

                        break;
                    case Alert.AnimationStage.SlideOut:
                        {
                            if (Alert.InternalTimer > 5)
                            {
                                this.Alerts.Remove(Alert);
                                continue;
                            }
                            else
                            {
                                Drawing.DrawText(
                                    StartX + (Alert.InternalTimer * 10), 
                                    StartY - (i * 15), 
                                    Color.FromArgb(alpha, Color.White), 
                                    Alert.Text);
                                Alert.InternalTimer++;
                            }
                        }

                        break;
                    case Alert.AnimationStage.Display:
                        {
                            Drawing.DrawText(StartX, StartY - (i * 15), Color.FromArgb(alpha, Color.White), Alert.Text);

                            if (completed > 80)
                            {
                                Alert.Stage = Alert.AnimationStage.SlideOut;
                                Alert.InternalTimer = 0;
                            }
                        }

                        break;
                }
            }
        }

        #endregion

        /// <summary>
        ///     The alert.
        /// </summary>
        public class Alert
        {
            #region Fields

            /// <summary>
            ///     The display text.
            /// </summary>
            public string DisplayText;

            /// <summary>
            ///     The end.
            /// </summary>
            public int End;

            /// <summary>
            ///     The internal timer.
            /// </summary>
            public int InternalTimer = 0;

            /// <summary>
            ///     The stage.
            /// </summary>
            public AnimationStage Stage = AnimationStage.SlideIn;

            /// <summary>
            ///     The start.
            /// </summary>
            public int Start;

            /// <summary>
            ///     The text update.
            /// </summary>
            public TextUpdateDelegate TextUpdate;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Alert" /> class.
            /// </summary>
            /// <param name="DisplayText">
            ///     The display text.
            /// </param>
            /// <param name="Duration">
            ///     The duration.
            /// </param>
            /// <param name="TextUpdate">
            ///     The text update.
            /// </param>
            public Alert(string DisplayText, int Duration, TextUpdateDelegate TextUpdate = null)
            {
                this.DisplayText = DisplayText;
                this.TextUpdate = TextUpdate;

                this.Start = Environment.TickCount;
                this.End = this.Start + Duration;
            }

            #endregion

            #region Delegates

            /// <summary>
            ///     The text update delegate.
            /// </summary>
            public delegate string TextUpdateDelegate();

            #endregion

            #region Enums

            /// <summary>
            ///     The animation stage.
            /// </summary>
            public enum AnimationStage
            {
                /// <summary>
                ///     The slide in.
                /// </summary>
                SlideIn, 

                /// <summary>
                ///     The slide out.
                /// </summary>
                SlideOut, 

                /// <summary>
                ///     The display.
                /// </summary>
                Display
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the text.
            /// </summary>
            public string Text
            {
                get
                {
                    return this.TextUpdate != null ? this.TextUpdate() : this.DisplayText;
                }
            }

            #endregion
        }
    }
}