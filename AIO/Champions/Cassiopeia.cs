// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cassiopeia.cs" company="LeagueSharp">
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
//   The cassiopeia.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Champions
{
    using System.Collections.Generic;

    using AIO.Wrapper;

    using LeagueSharp;

    /// <summary>
    ///     The cassiopeia.
    /// </summary>
    public class Cassiopeia : Champion
    {
        #region Methods

        /// <summary>
        ///     The get order.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override SpellSlot[] GetOrder()
        {
            return new[] { SpellSlot.R, SpellSlot.W, SpellSlot.Q, SpellSlot.E };
        }

        /// <summary>
        ///     The get spells.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = ChampionSpell.FromLibrary("Cassiopeia", SpellSlot.Q, ChampionSpell.CastType.Circle);
            var W = new ChampionSpell(SpellSlot.W, 850, ChampionSpell.CastType.Circle);
            var E = new ChampionSpell(SpellSlot.E, 700, ChampionSpell.CastType.Target);
            var R = ChampionSpell.FromLibrary("Cassiopeia", SpellSlot.R, ChampionSpell.CastType.Cone);

            E.CastCondition = (Obj_AI_Base unit) => { return unit.HasBuffOfType(BuffType.Poison); };

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R);

            return result;
        }

        #endregion
    }
}