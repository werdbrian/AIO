using AIO.Wrapper;
using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Champions
{
    public class Skarner : Champion
    {
        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.E, SpellSlot.W, SpellSlot.Q, SpellSlot.R };
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>(); 

            var Q = new ChampionSpell(SpellSlot.Q, 350, ChampionSpell.CastType.SELF);
            var W = new ChampionSpell(SpellSlot.W, 350, ChampionSpell.CastType.SELF);
            var E = ChampionSpell.FromLibrary("Skarner", SpellSlot.E, ChampionSpell.CastType.LINEAR);
            var R = new ChampionSpell(SpellSlot.R, 350, ChampionSpell.CastType.TARGET);

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R); 

            return result;
        }
    }
}
