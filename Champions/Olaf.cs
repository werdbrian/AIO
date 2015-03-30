using AIO.Wrapper;
using AIO.Helpers;
using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Champions
{
    public class Olaf : Champion 
    {
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
            return new SpellSlot[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = ChampionSpell.FromLibrary("Olaf", SpellSlot.Q, ChampionSpell.CastType.LINEAR_COLLISION);
            var W = new ChampionSpell(SpellSlot.W, 325, ChampionSpell.CastType.SELF);
            var E = new ChampionSpell(SpellSlot.E, 325, ChampionSpell.CastType.TARGET);
            var R = new ChampionSpell(SpellSlot.R, int.MaxValue, ChampionSpell.CastType.SELF);

            R.CastCondition = (unit) =>
            {
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
