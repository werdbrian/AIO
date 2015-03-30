using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIO.Common;

namespace AIO
{
    /// <summary>
    ///     Target Selector
    /// </summary>
    public static class Selector
    {
        /// <summary>
        ///     Primary damage type for the TargetSelector
        /// </summary>
        public static TargetSelector.DamageType DamageType = TargetSelector.DamageType.Magical; 

        private static TargetSelector _TargetSelector;

        /// <summary>
        ///     Ensures a sane TargetSelector instance
        /// </summary>
        public static TargetSelector TargetSelector
        {
            get
            {
                return (_TargetSelector ?? (_TargetSelector = new TargetSelector())); 
            }
        }

        static Selector()
        {
            TargetSelector.AddToMenu(Configuration.Main.AddSubMenu(new Menu("Target Selector", "TS")));
            SpeechRecongition.OnRecongized += Instance_OnRecongized;            
        }

        private static void Instance_OnRecongized(string output)
        {
            foreach (var champion in ObjectManager.Get<Obj_AI_Hero>().Where(unit => unit.IsVisible && unit.IsEnemy))
            {
                if (champion.ChampionName.ToLower() == output.ToLower())
                    TargetSelector.SetTarget(champion);
            }
        }

        /// <summary>
        ///     Returns either the selected target (if valid) or the most priority target in range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Obj_AI_Hero GetTarget(float range)
        {
            return TargetSelector.GetSelectedTarget() != null ? TargetSelector.GetSelectedTarget() : TargetSelector.GetTarget(range, DamageType); 
        }
    }
}
