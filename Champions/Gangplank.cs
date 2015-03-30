using AIO.Wrapper;
using AIO.Helpers;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Champions
{
    public class Gangplank : Champion
    {
        // Gangplank can remove all CC, including hard CC
        private static readonly BuffType[] CLEANSABLE_BUFFS = new BuffType[] {
            BuffType.Blind, 
            BuffType.Charm, 
            BuffType.Fear, 
            BuffType.Flee, 
            BuffType.Knockback, 
            BuffType.Knockup,
            BuffType.NearSight, 
            BuffType.Poison, 
            BuffType.Polymorph, 
            BuffType.Silence, 
            BuffType.Sleep,
            BuffType.Slow, 
            BuffType.Snare, 
            BuffType.Stun, 
            BuffType.Suppression, 
            BuffType.Taunt
        };

        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] {
                SpellSlot.W, // Always set W to first priority, since it is a CC cleanse
                SpellSlot.E, // Check for E (basically cast when up) to maintain a constant DPS buff
                SpellSlot.Q, // Main DPS ability, cast before ultimate 
                SpellSlot.R  // Ultimate, only really useful in combo when used for killing the opponent or AP gangplank 
            };
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = new ChampionSpell(SpellSlot.Q, 625, ChampionSpell.CastType.TARGET);
            var W = new ChampionSpell(SpellSlot.W, int.MaxValue, ChampionSpell.CastType.SELF);
            var E = new ChampionSpell(SpellSlot.E, 1300, ChampionSpell.CastType.SELF);
            var R = new ChampionSpell(SpellSlot.R, int.MaxValue, ChampionSpell.CastType.CIRCLE);

            W.CreateHandler(ChampionSpell.HandlerType.ON_SELF_HEALTH_BELOW); // Create heal handle

            W.CastCondition = (unit) =>
            {
                // Cleanse all CC with W... TODO: Test
                return ObjectManager.Player.HasCC(CLEANSABLE_BUFFS);
            };

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R);

            return result; 
        }
    }
}
