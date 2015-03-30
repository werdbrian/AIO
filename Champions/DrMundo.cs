using AIO.Wrapper;
using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Champions
{
    public class DrMundo : Champion 
    {
        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.Q, SpellSlot.E, SpellSlot.W, SpellSlot.R };
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>(); 

            var Q = ChampionSpell.FromLibrary("DrMundo", SpellSlot.Q, ChampionSpell.CastType.LINEAR_COLLISION);
            var W = new ChampionSpell(SpellSlot.W, 300, ChampionSpell.CastType.SELF);
            var E = new ChampionSpell(SpellSlot.E, 300, ChampionSpell.CastType.SELF);
            var R = new ChampionSpell(SpellSlot.R, int.MaxValue, ChampionSpell.CastType.SELF);

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R);

            return result;
        }
    }
}
