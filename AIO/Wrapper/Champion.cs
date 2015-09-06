// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Champion.cs" company="LeagueSharp">
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
//   Champion Instance, abstracted class for override instances
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Wrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AIO.Common;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     Champion Instance, abstracted class for override instances
    /// </summary>
    public abstract class Champion
    {
        #region Fields

        /// <summary>
        ///     The _ longest range.
        /// </summary>
        private int? longestRange = null;

        /// <summary>
        ///     The _ order.
        /// </summary>
        private List<SpellSlot> order = null;

        /// <summary>
        ///     The _ shortest range.
        /// </summary>
        private int? shortestRange = null;

        /// <summary>
        ///     The _ spells.
        /// </summary>
        private List<ChampionSpell> spells = null;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the longest range of the spell list
        /// </summary>
        public int? LongestRange
        {
            get
            {
                return this.longestRange
                       ?? (this.longestRange = (int)this.Spells.OrderByDescending(spell => spell.Range).First().Range);
            }
        }

        /// <summary>
        ///     Gets a one-time-set order list used to maintain sanity of the one-time call GetOrder
        /// </summary>
        public List<SpellSlot> Order
        {
            get
            {
                return this.order ?? (this.order = this.GetOrder().ToList());
            }
        }

        /// <summary>
        ///     Gets the shortest range of the spell list
        /// </summary>
        public int? ShortestRange
        {
            get
            {
                return this.shortestRange
                       ?? (this.shortestRange = (int)this.Spells.OrderBy(spell => spell.Range).First().Range);
            }
        }

        /// <summary>
        ///     Gets a one-time-set spell list used to maintain the sanity of the one-time call GetSpells
        /// </summary>
        public List<ChampionSpell> Spells
        {
            get
            {
                return this.spells ?? (this.spells = this.GetSpells());
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Insert a new spell into the spell list
        /// </summary>
        /// <param name="spell"></param>
        public void Add(ChampionSpell spell)
        {
            this.Spells.Add(spell);
        }

        /// <summary>
        ///     Returns a sane list of enabled lane clear and ready spells
        /// </summary>
        /// <returns></returns>
        public List<ChampionSpell> AsLaneClear()
        {
            return this.Spells.Where(spell => spell.IsEnabled_LaneClear && spell.Instance.IsReady()).ToList();
        }

        /// <summary>
        ///     Returns a sane list of enabled last hit and ready spells
        /// </summary>
        /// <returns></returns>
        public List<ChampionSpell> AsLastHit()
        {
            return this.Spells.Where(spell => spell.IsEnabled_LastHit && spell.Instance.IsReady()).ToList();
        }

        /// <summary>
        ///     Ordered combo sorted by Order
        /// </summary>
        /// <returns></returns>
        public List<ChampionSpell> AsOrderedCombo()
        {
            return
                this.Spells.Where(s => s.Instance.IsReady() && s.IsEnabled)
                    .OrderBy(s => this.Order.IndexOf(s.Slot))
                    .ToList();
        }

        /// <summary>
        ///     Returns the sane spell list
        /// </summary>
        /// <returns></returns>
        public List<ChampionSpell> GetList()
        {
            return this.Spells;
        }

        /// <summary>
        ///     Virtual OnLoad for customized loading actions
        /// </summary>
        public virtual Action OnLoad()
        {
            return PerformOnce.A(() => { Logger.Debug("Champion#OnLoad not implemented."); });
        }

        /// <summary>
        ///     The on tick.
        /// </summary>
        /// <returns>
        ///     The <see cref="Action" />.
        /// </returns>
        public virtual Action OnTick()
        {
            return null;
        }

        /// <summary>
        ///     Remove the specified spell from the spell list
        /// </summary>
        /// <param name="spell"></param>
        public void Remove(ChampionSpell spell)
        {
            this.Spells.Remove(spell);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Priority order for casting in a combo
        /// </summary>
        /// <returns></returns>
        protected abstract SpellSlot[] GetOrder();

        /// <summary>
        ///     Retrieves a new instance of the champion spells, should only be called once or risk overriding important methods.
        /// </summary>
        /// <returns></returns>
        protected abstract List<ChampionSpell> GetSpells();

        #endregion
    }
}