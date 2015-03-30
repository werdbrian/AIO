using AIO.Wrapper;
using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Champions
{
    public class Talon : Champion
    {
        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.E, SpellSlot.W, SpellSlot.Q, SpellSlot.R };
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = new ChampionSpell(SpellSlot.Q, 125, ChampionSpell.CastType.AA_ATTACK);
            var W = ChampionSpell.FromLibrary("Talon", SpellSlot.W, ChampionSpell.CastType.LINEAR);
            var E = new ChampionSpell(SpellSlot.E, 700, ChampionSpell.CastType.TARGET);
            var R = new ChampionSpell(SpellSlot.R, 500, ChampionSpell.CastType.SELF);

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R);

            return result; 
        }
    }
}
