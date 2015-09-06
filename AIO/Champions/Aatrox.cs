// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Aatrox.cs" company="LeagueSharp">
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
//   The aatrox.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Champions
{
    using System;
    using System.Collections.Generic;

    using AIO.Wrapper;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The aatrox.
    /// </summary>
    public class Aatrox : Champion
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The on load.
        /// </summary>
        /// <returns>
        /// </returns>
        public override Action OnLoad()
        {
            return PerformOnce.A(
                () =>
                    {
                        Configuration.Main.AddItem(new MenuItem("sep", string.Empty));
                        Configuration.Main.AddItem(
                                new MenuItem("AbsorbPercent", "W Absorb Percentage").SetValue<Slider>(
                                    new Slider(75, 0, 100)));
                        Configuration.Main.AddItem(
                                new MenuItem("DamagePercent", "W Damage Percentage").SetValue<Slider>(
                                    new Slider(75, 0, 100)));
                    });
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The get order.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override SpellSlot[] GetOrder()
        {
            return new[] { SpellSlot.Q, SpellSlot.R, SpellSlot.E, SpellSlot.W };
        }

        /// <summary>
        ///     The get spells.
        /// </summary>
        /// <returns>
        /// </returns>
        protected override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = ChampionSpell.FromLibrary("Aatrox", SpellSlot.Q, ChampionSpell.CastType.Circle);
            var W = new ChampionSpell(SpellSlot.W, 125, ChampionSpell.CastType.Self);
            var E = ChampionSpell.FromLibrary("Aatrox", SpellSlot.E, ChampionSpell.CastType.Cone);
            var R = new ChampionSpell(SpellSlot.R, 350, ChampionSpell.CastType.Self);

            W.CastCondition = (unit) =>
                {
                    var healPercent = Configuration.Main.Item("AbsorbPercent").GetValue<Slider>();
                    var damagePercent = Configuration.Main.Item("DamagePercent").GetValue<Slider>();
                    var currentPercent = ObjectManager.Player.HealthPercentage();
                    var name = ObjectManager.Player.GetSpell(SpellSlot.W).Name;

                    return !name.Equals("AatroxW")
                               ? healPercent.Value > currentPercent
                               : currentPercent > damagePercent.Value;
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