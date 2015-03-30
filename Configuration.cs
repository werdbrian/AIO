using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO
{
    public static class Configuration
    {
        private static Menu _Main = null;
        private static Menu _Spell = null;
        private static Menu _Extra = null;
        private static Menu _Draw = null;
        private static Menu _Miscellaneous = null;

        public static Menu Main
        {
            get
            {
                return (_Main ?? (_Main = new Menu("AIO", "AIO", true)));
            }
        }

        public static Menu Spell
        {
            get
            {
                return (_Spell ?? (_Spell = Main.AddSubMenu(new Menu("Spells", "Spells"))));
            }
        }

        public static Menu Extra
        {
            get
            {
                return (_Extra ?? (_Extra = Main.AddSubMenu(new Menu("Extra", "Extra"))));
            }
        }

        public static Menu Draw
        {
            get
            {
                return (_Draw ?? (_Draw = Main.AddSubMenu(new Menu("Drawing", "Drawing"))));
            }
        }

        public static Menu Miscellaneous
        {
            get
            {
                return (_Miscellaneous ?? (_Miscellaneous = Main.AddSubMenu(new Menu("Miscellaneous", "Miscellaneous"))));
            }
        }

        public static void Rebuild()
        {
            _Main = null;
            _Spell = null;
            _Extra = null;
            _Draw = null;
            _Miscellaneous = null; 
        }

     }
}
