// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStepable.cs" company="LeagueSharp">
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
//   The Stepable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Wrapper
{
    /// <summary>
    ///     The Stepable interface.
    /// </summary>
    public interface IStepable
    {
        #region Public Properties

        /// <summary>
        ///     Determines if the current stepable is valid and able to proceed
        /// </summary>
        bool Valid { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     A "step" action
        /// </summary>
        void Step();

        #endregion
    }
}