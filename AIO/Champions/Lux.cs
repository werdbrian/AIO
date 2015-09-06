// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lux.cs" company="LeagueSharp">
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
//   The lux.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Champions
{
    using System.Collections.Generic;

    using AIO.Wrapper;

    using LeagueSharp;

    /// <summary>
    ///     The lux.
    /// </summary>
    public class Lux : Champion
    {
        #region Methods

        /// <summary>
        ///     The get order.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override SpellSlot[] GetOrder()
        {
            return new[] { SpellSlot.R, SpellSlot.Q, SpellSlot.E };
        }

        /// <summary>
        ///     The get spells.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            result.Add(ChampionSpell.FromLibrary("Lux", SpellSlot.Q, ChampionSpell.CastType.LinearCollision));
            result.Add(ChampionSpell.FromLibrary("Lux", SpellSlot.E, ChampionSpell.CastType.Circle));
            result.Add(ChampionSpell.FromLibrary("Lux", SpellSlot.R, ChampionSpell.CastType.Linear));

            return result;
        }

        #endregion
    }
}