// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Selector.cs" company="LeagueSharp">
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
//   Target Selector
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     Target Selector
    /// </summary>
    public static class Selector
    {
        #region Static Fields

        /// <summary>
        ///     Primary damage type for the TargetSelector
        /// </summary>
        public static TargetSelector.DamageType DamageType = TargetSelector.DamageType.Magical;

        /// <summary>
        ///     The _ target selector.
        /// </summary>
        private static TargetSelector _TargetSelector;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Selector" /> class.
        /// </summary>
        static Selector()
        {
            TargetSelector.AddToMenu(Configuration.Main.AddSubMenu(new Menu("Target Selector", "TS")));

            // SpeechRecongition.OnRecongized += Instance_OnRecongized;            
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Ensures a sane TargetSelector instance
        /// </summary>
        public static TargetSelector TargetSelector
        {
            get
            {
                return _TargetSelector ?? (_TargetSelector = new TargetSelector());
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns either the selected target (if valid) or the most priority target in range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Obj_AI_Hero GetTarget(float range)
        {
            return TargetSelector.GetSelectedTarget() != null
                       ? TargetSelector.GetSelectedTarget()
                       : TargetSelector.GetTarget(range, DamageType);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The instance_ on recongized.
        /// </summary>
        /// <param name="output">
        ///     The output.
        /// </param>
        private static void Instance_OnRecongized(string output)
        {
            foreach (var champion in ObjectManager.Get<Obj_AI_Hero>().Where(unit => unit.IsVisible && unit.IsEnemy))
            {
                if (champion.ChampionName.ToLower() == output.ToLower())
                {
                    TargetSelector.SetTarget(champion);
                }
            }
        }

        #endregion
    }
}