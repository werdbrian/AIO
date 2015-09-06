// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameNotifier.cs" company="LeagueSharp">
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
//   The game notifier.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The game notifier.
    /// </summary>
    public class GameNotifier
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameNotifier" /> class.
        /// </summary>
        public GameNotifier()
        {
            Obj_AI_Base.OnProcessSpellCast += this.Obj_AI_Base_OnProcessSpellCast;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The obj_ a i_ base_ on process spell cast.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsValid)
            {
                return;
            }

            var spell = sender.Spellbook.Spells.FirstOrDefault(s => s.SData.Hash == args.SData.Hash);

            if (spell != null && spell.Slot == SpellSlot.R)
            {
                var notification =
                    new Notification(
                        string.Format(
                            "{0} has just cast {1}, will be back up in {2}", 
                            sender.Name, 
                            spell.Slot, 
                            spell.Cooldown), 
                        5 * 1000);
                notification.Flash();

                Notifications.AddNotification(notification);
            }
        }

        #endregion
    }
}