using LeagueSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO
{
    /// <summary>
    ///     Log utility
    /// </summary>
    public static class Logger
    {
        /// <summary>
        ///     LogType, used to determine color print 
        /// </summary>
        public sealed class LogType
        {
            public static readonly LogType Danger = new LogType(HtmlColor.Red);
            public static readonly LogType Debug = new LogType(HtmlColor.Gray);
            public static readonly LogType Warning = new LogType(HtmlColor.Yellow);
            public static readonly LogType Success = new LogType(HtmlColor.Green);
            public static readonly LogType Normal = new LogType(HtmlColor.White); 

            public string Color { get; set; }

            public LogType(string Color)
            {
                this.Color = Color;
            }
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
    }
}
