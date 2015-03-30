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
    public class Maokai : Champion
    {
        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.E, SpellSlot.W, SpellSlot.Q };
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = new ChampionSpell(SpellSlot.Q, 600, ChampionSpell.CastType.LINEAR);
            var W = new ChampionSpell(SpellSlot.W, 525, ChampionSpell.CastType.TARGET);
            var E = new ChampionSpell(SpellSlot.E, 1100, ChampionSpell.CastType.CIRCLE);

            result.Add(Q);
            result.Add(W);
            result.Add(E); 
            
            return result; 
        }
    }
}
