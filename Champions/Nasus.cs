using AIO.Wrapper;
using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Champions
{
    public class Nasus : Champion
    {
        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.W, SpellSlot.E, SpellSlot.Q, SpellSlot.R };
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = new ChampionSpell(SpellSlot.Q, 325, ChampionSpell.CastType.AA_ATTACK);
            var W = new ChampionSpell(SpellSlot.W, 600, ChampionSpell.CastType.TARGET);
            var E = new ChampionSpell(SpellSlot.E, 650, ChampionSpell.CastType.CIRCLE);
            var R = new ChampionSpell(SpellSlot.R, 325, ChampionSpell.CastType.SELF); 

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R);

            return result;
        }
    }
}
