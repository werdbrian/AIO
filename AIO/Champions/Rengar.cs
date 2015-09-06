// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Rengar.cs" company="LeagueSharp">
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
//   The rengar.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Champions
{
    using System.Collections.Generic;

    using AIO.Wrapper;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The rengar.
    /// </summary>
    public class Rengar : Champion
    {
        #region Methods

        /// <summary>
        ///     The get order.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override SpellSlot[] GetOrder()
        {
            return new[] { SpellSlot.E, SpellSlot.W, SpellSlot.Q };
        }

        /// <summary>
        ///     The get spells.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = new ChampionSpell(SpellSlot.Q, 325, ChampionSpell.CastType.AaAttack);
            var W = new ChampionSpell(SpellSlot.W, 350, ChampionSpell.CastType.Self);
            var E = ChampionSpell.FromLibrary("Rengar", SpellSlot.E, ChampionSpell.CastType.LinearCollision);

            Q.CastCondition = (Obj_AI_Base unit) =>
                {
                    var mana = ObjectManager.Player.Mana;
                    return mana < 5
                           || (mana == 5
                               && !(E.IsReady(unit)
                                    && E.Instance.WillHit(unit.ServerPosition, ObjectManager.Player.ServerPosition))
                               && ObjectManager.Player.HealthPercentage() > 75);
                };

            W.CastCondition = (Obj_AI_Base unit) =>
                {
                    var mana = ObjectManager.Player.Mana;
                    return mana < 5
                           || (mana == 5
                               && !(E.IsReady(unit)
                                    && E.Instance.WillHit(unit.ServerPosition, ObjectManager.Player.ServerPosition))
                               && ObjectManager.Player.HealthPercentage() < 75);
                };

            E.CastCondition = (Obj_AI_Base unit) =>
                {
                    var mana = ObjectManager.Player.Mana;
                    return mana < 5
                           || (mana == 5
                               && (E.IsReady(unit)
                                   && E.Instance.WillHit(unit.ServerPosition, ObjectManager.Player.ServerPosition)));
                };

            result.Add(Q);
            result.Add(W);
            result.Add(E);

            return result;
        }

        #endregion
    }
}