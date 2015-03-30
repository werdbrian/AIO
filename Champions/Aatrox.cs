using AIO.Wrapper;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Champions
{
    public class Aatrox : Champion 
    {
        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.Q, SpellSlot.R, SpellSlot.E, SpellSlot.W };
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = ChampionSpell.FromLibrary("Aatrox", SpellSlot.Q, ChampionSpell.CastType.CIRCLE);
            var W = new ChampionSpell(SpellSlot.W, 125, ChampionSpell.CastType.SELF); 
            var E = ChampionSpell.FromLibrary("Aatrox", SpellSlot.E, ChampionSpell.CastType.CONE);
            var R = new ChampionSpell(SpellSlot.R, 350, ChampionSpell.CastType.SELF);

            W.CastCondition = (unit) =>
            {
                var healPercent = W.SpellMenu.Item("AbsorbPercent").GetValue<Slider>();
                var damagePercent = W.SpellMenu.Item("DamagePercent").GetValue<Slider>(); 
                var currentPercent = ObjectManager.Player.HealthPercentage(); 
                var name = ObjectManager.Player.GetSpell(SpellSlot.W).Name;

                return !name.Equals("AatroxW") ? healPercent.Value > currentPercent : currentPercent > damagePercent.Value; 
            };

            return result; 
        }

        public override Action OnLoad()
        {
            return PerformOnce.A(() => {
                Configuration.Spell.SubMenu("W").AddItem(new MenuItem("AbsorbPercent", "Absorb Percentage").SetValue<Slider>(new Slider(75, 0, 100)));
                Configuration.Spell.SubMenu("W").AddItem(new MenuItem("DamagePercent", "Damage Percentage").SetValue<Slider>(new Slider(75, 0, 100)));
            });
        }
    }
}
