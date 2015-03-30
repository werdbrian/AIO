using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Helpers
{
    /// <summary>
    ///     Extension class, as the name states
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     Determines if the Obj_AI_Base has a buff of the param types
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="buffs"></param>
        /// <returns></returns>
        public static bool HasCC(this Obj_AI_Base unit, params BuffType[] buffs)
        {
            return buffs.Any(b => unit.HasBuffOfType(b));
        }
        
        /// <summary>
        ///     Compiles an array from an Enum 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        ///     Simple Math function for checking if a Rectangle contains a specified Point
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool Contains(this Render.Rectangle rectangle, Vector2 point)
        {
            return rectangle.X < point.X && rectangle.Y < point.Y && rectangle.X + rectangle.Width > point.X && rectangle.Y + rectangle.Height > point.Y;  
        }

        /// <summary>
        ///     Split String by length 
        ///     
        ///     http://stackoverflow.com/questions/3008718/split-string-into-smaller-strings-by-length-variable
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitByLength(this string str, int maxLength)
        {
            for (int index = 0; index < str.Length; index += maxLength)
            {
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
            }
        }
    }
}
