// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="LeagueSharp">
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
//   The configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO
{
    using LeagueSharp.Common;

    /// <summary>
    ///     The configuration.
    /// </summary>
    public static class Configuration
    {
        #region Static Fields

        /// <summary>
        ///     The _ draw.
        /// </summary>
        private static Menu draw = null;

        /// <summary>
        ///     The _ extra.
        /// </summary>
        private static Menu extra = null;

        /// <summary>
        ///     The _ main.
        /// </summary>
        private static Menu main = null;

        /// <summary>
        ///     The _ miscellaneous.
        /// </summary>
        private static Menu miscellaneous = null;

        /// <summary>
        ///     The _ spell.
        /// </summary>
        private static Menu spell = null;

        #endregion

        #region Public Properties

        /// <summary>
        ///     "Draw" Configuration Menu
        /// </summary>
        public static Menu Draw
        {
            get
            {
                return draw ?? (draw = Main.AddSubMenu(new Menu("Drawing", "Drawing")));
            }
        }

        /// <summary>
        ///     "Extra" Configuration Menu
        /// </summary>
        public static Menu Extra
        {
            get
            {
                return extra ?? (extra = Main.AddSubMenu(new Menu("Extra", "Extra")));
            }
        }

        /// <summary>
        ///     "Main" Configuration Menu
        /// </summary>
        public static Menu Main
        {
            get
            {
                return main ?? (main = new Menu("AIO", "AIO", true));
            }
        }

        /// <summary>
        ///     "Miscellaneous" Configuration Menu
        /// </summary>
        public static Menu Miscellaneous
        {
            get
            {
                return miscellaneous ?? (miscellaneous = Main.AddSubMenu(new Menu("Miscellaneous", "Miscellaneous")));
            }
        }

        /// <summary>
        ///     "Spell" Configuration Menu
        ///     Note: If instantiated the sub-menu for each spell will be simply "Q, W" etc.
        /// </summary>
        public static Menu Spell
        {
            get
            {
                return spell ?? (spell = Main.AddSubMenu(new Menu("Spells", "Spells")));
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Re-Instantiate menus
        /// </summary>
        public static void Rebuild()
        {
            main = null;
            spell = null;
            extra = null;
            draw = null;
            miscellaneous = null;
        }

        #endregion
    }
}