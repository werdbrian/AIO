// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="LeagueSharp">
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
//   Log utility
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common
{
    using System;

    using LeagueSharp;

    /// <summary>
    ///     Log utility
    /// </summary>
    public static class Logger
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Outputs a debug message only if the Configuration debug is enabled
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            if (Configuration.Miscellaneous.Item("Debug").GetValue<bool>())
            {
                Print(message, LogType.Debug);
            }

            Console.WriteLine(message);
        }

        /// <summary>
        ///     Outputs a formatted message determined by log type
        /// </summary>
        /// <param name="message"></param>
        /// <param name="LogType"></param>
        public static void Print(string message, LogType LogType)
        {
            Game.PrintChat("<font color='{0}'>{1}</font>", LogType.Color, message);
        }

        /// <summary>
        ///     Output normally formatted message
        /// </summary>
        /// <param name="message"></param>
        public static void Print(string message)
        {
            Print(message, LogType.Normal);
        }

        #endregion

        /// <summary>
        ///     LogType, used to determine color print
        /// </summary>
        public sealed class LogType
        {
            #region Static Fields

            /// <summary>
            ///     The danger.
            /// </summary>
            public static readonly LogType Danger = new LogType(HtmlColor.Red);

            /// <summary>
            ///     The debug.
            /// </summary>
            public static readonly LogType Debug = new LogType(HtmlColor.Gray);

            /// <summary>
            ///     The normal.
            /// </summary>
            public static readonly LogType Normal = new LogType(HtmlColor.White);

            /// <summary>
            ///     The success.
            /// </summary>
            public static readonly LogType Success = new LogType(HtmlColor.Green);

            /// <summary>
            ///     The warning.
            /// </summary>
            public static readonly LogType Warning = new LogType(HtmlColor.Yellow);

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="LogType" /> class.
            /// </summary>
            /// <param name="Color">
            ///     The color.
            /// </param>
            public LogType(string Color)
            {
                this.Color = Color;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the color.
            /// </summary>
            public string Color { get; set; }

            #endregion
        }
    }
}