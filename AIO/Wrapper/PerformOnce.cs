// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerformOnce.cs" company="LeagueSharp">
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
//   http://stackoverflow.com/a/5597840
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Wrapper
{
    using System;

    /// <summary>
    ///     http://stackoverflow.com/a/5597840
    /// </summary>
    public class PerformOnce
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Performs the desired Action once per runtime
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Action A(Action action)
        {
            var context = new Context();

            Action ret = () =>
                {
                    if (!context.Performed)
                    {
                        action();
                        context.Performed = true;
                    }
                };

            return ret;
        }

        /// <summary>
        ///     The f.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public static Func<T> F<T>(Func<T> action)
        {
            var context = new Context();

            Func<T> ret = () =>
                {
                    if (!context.Performed)
                    {
                        context.Performed = true;
                        return action();
                    }

                    return default(T);
                };

            return ret;
        }

        #endregion

        /// <summary>
        ///     The context.
        /// </summary>
        private class Context
        {
            #region Fields

            /// <summary>
            ///     The performed.
            /// </summary>
            internal bool Performed;

            #endregion
        }
    }
}