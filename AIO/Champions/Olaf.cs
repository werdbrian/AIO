// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Olaf.cs" company="LeagueSharp">
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
//   The olaf.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Champions
{
    using System.Collections.Generic;

    using AIO.Helpers;
    using AIO.Wrapper;

    using LeagueSharp;

    /// <summary>
    ///     The olaf.
    /// </summary>
    public class Olaf : Champion
    {
        #region Static Fields

        /// <summary>
        ///     The cleansabl e_ buffs.
        /// </summary>
        private static readonly BuffType[] CLEANSABLE_BUFFS = new[]
                                                                  {
                                                                      BuffType.Blind, BuffType.Charm, BuffType.Fear, 
                                                                      BuffType.Flee, BuffType.Knockback, BuffType.Knockup, 
                                                                      BuffType.NearSight, BuffType.Poison, 
                                                                      BuffType.Polymorph, BuffType.Silence, BuffType.Sleep, 
                                                                      BuffType.Slow, BuffType.Snare, BuffType.Stun, 
                                                                      BuffType.Suppression, BuffType.Taunt
                                                                  };

        #endregion

        #region Methods

        /// <summary>
        ///     The get order.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override SpellSlot[] GetOrder()
        {
            return new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
        }

        /// <summary>
        ///     The get spells.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = ChampionSpell.FromLibrary("Olaf", SpellSlot.Q, ChampionSpell.CastType.LinearCollision);
            var W = new ChampionSpell(SpellSlot.W, 325, ChampionSpell.CastType.Self);
            var E = new ChampionSpell(SpellSlot.E, 325, ChampionSpell.CastType.Target);
            var R = new ChampionSpell(SpellSlot.R, int.MaxValue, ChampionSpell.CastType.Self);

            R.CastCondition = (unit) => { return ObjectManager.Player.HasCC(CLEANSABLE_BUFFS); };

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R);

            return result;
        }

        #endregion
    }
}