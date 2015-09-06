// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DXItem.cs" company="LeagueSharp">
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
//   Abstract DXItem
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common.DX
{
    using SharpDX;

    /// <summary>
    ///     Abstract DXItem
    /// </summary>
    public abstract class DXItem
    {
        #region Public Properties

        /// <summary>
        ///     Determines if the DXItem is Enabled
        /// </summary>
        public abstract bool IsEnabled { get; set; }

        /// <summary>
        ///     Determines if the DXItem is valid
        /// </summary>
        public abstract bool IsValid { get; }

        /// <summary>
        ///     Gets or sets the position.
        /// </summary>
        public abstract Vector2 Position { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        public abstract void Add();

        /// <summary>
        ///     The remove.
        /// </summary>
        public abstract void Remove();

        #endregion
    }
}