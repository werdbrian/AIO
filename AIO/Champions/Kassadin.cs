// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Kassadin.cs" company="LeagueSharp">
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
//   The kassadin.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Champions
{
    using System.Collections.Generic;

    using AIO.Wrapper;

    using LeagueSharp;

    /// <summary>
    ///     The kassadin.
    /// </summary>
    public class Kassadin : Champion
    {
        #region Methods

        /// <summary>
        ///     The get order.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override SpellSlot[] GetOrder()
        {
            return new[] { SpellSlot.R, SpellSlot.E, SpellSlot.W, SpellSlot.Q };
        }

        /// <summary>
        ///     The get spells.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = new ChampionSpell(SpellSlot.Q, 650, ChampionSpell.CastType.Target);
            var W = new ChampionSpell(SpellSlot.W, 125, ChampionSpell.CastType.AaAttack);
            var E = new ChampionSpell(SpellSlot.E, 400, ChampionSpell.CastType.Cone);
            var R = new ChampionSpell(SpellSlot.R, 500, ChampionSpell.CastType.Circle);

            R.CastCondition = (u) => R.Instance.Instance.ManaCost < ObjectManager.Player.Mana;

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R);

            return result;
        }

        #endregion
    }
}