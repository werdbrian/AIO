// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Khazix.cs" company="LeagueSharp">
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
//   The khazix.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Champions
{
    using System.Collections.Generic;

    using AIO.Wrapper;

    using LeagueSharp;

    /// <summary>
    ///     The khazix.
    /// </summary>
    public class Khazix : Champion
    {
        #region Methods

        /// <summary>
        ///     The get order.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override SpellSlot[] GetOrder()
        {
            return new[] { SpellSlot.E, SpellSlot.W, SpellSlot.Q, SpellSlot.R };
        }

        /// <summary>
        ///     The get spells.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = new ChampionSpell(SpellSlot.Q, 325, ChampionSpell.CastType.Target);
            var W = ChampionSpell.FromLibrary("Khazix", SpellSlot.W, ChampionSpell.CastType.LinearCollision);
            var E = ChampionSpell.FromLibrary("Khazix", SpellSlot.E, ChampionSpell.CastType.Circle);
            var R = new ChampionSpell(SpellSlot.R, 400, ChampionSpell.CastType.Self);

            Q.CastCondition = (unit) =>
                {
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "khazixqlong")
                    {
                        Q.Range = 375;
                    }

                    return true; // always return true, this will make the condition checked and passed each call 
                };

            E.CastCondition = (unit) =>
                {
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Name == "khazixelong")
                    {
                        E.Range = 900;
                    }

                    return true; // always return true, this will make the condition checked and passed each call 
                };

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R);

            return result;
        }

        #endregion
    }
}