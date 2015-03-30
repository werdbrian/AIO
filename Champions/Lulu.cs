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
    public class Lulu : Champion
    {
        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.E, SpellSlot.W, SpellSlot.Q, SpellSlot.R }; 
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = ChampionSpell.FromLibrary("Lulu", SpellSlot.Q, ChampionSpell.CastType.LINEAR);
            var W = new ChampionSpell(SpellSlot.W, 750, ChampionSpell.CastType.TARGET);
            var E = new ChampionSpell(SpellSlot.E, 650, ChampionSpell.CastType.TARGET);
            var R = new ChampionSpell(SpellSlot.R, 900, ChampionSpell.CastType.SHIELD);
            
            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R); 

            return result; 
        }
    }
}
