using AIO.Wrapper;
using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Champions
{
    public class Shyvana : Champion
    {

        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.R, SpellSlot.E, SpellSlot.W, SpellSlot.Q };           
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = new ChampionSpell(SpellSlot.Q, 325, ChampionSpell.CastType.AA_ATTACK);
            var W = new ChampionSpell(SpellSlot.W, 325, ChampionSpell.CastType.SELF); 
            var E = ChampionSpell.FromLibrary("Shyvana", SpellSlot.E, ChampionSpell.CastType.LINEAR);
            var R = ChampionSpell.FromLibrary("Shyvana", SpellSlot.R, ChampionSpell.CastType.LINEAR_COLLISION);

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R); 

            return result;
        }
    }
}
