// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicInitializer.cs" company="LeagueSharp">
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
//   The dynamic initializer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common
{
    using System;
    using System.Reflection.Emit;

    /// <summary>
    ///     The dynamic initializer.
    /// </summary>
    public class DynamicInitializer
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The create instance.
        /// </summary>
        /// <typeparam name="TV">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public static TV CreateInstance<TV>() where TV : class
        {
            return ObjectGenerator(typeof(TV)) as TV;
        }

        /// <summary>
        ///     The create instance.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        /// </returns>
        public static object CreateInstance(Type type)
        {
            return ObjectGenerator(type);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The object generator.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        /// </returns>
        private static object ObjectGenerator(Type type)
        {
            var target = type.GetConstructor(Type.EmptyTypes);
            var dynamic = new DynamicMethod(string.Empty, type, new Type[0], target.DeclaringType);
            var il = dynamic.GetILGenerator();
            il.DeclareLocal(target.DeclaringType);
            il.Emit(OpCodes.Newobj, target);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            var method = (Func<object>)dynamic.CreateDelegate(typeof(Func<object>));
            return method();
        }

        #endregion
    }
}