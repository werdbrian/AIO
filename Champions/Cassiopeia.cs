using AIO.Wrapper;
using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Champions
{
    public class Cassiopeia : Champion
    {
        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.R, SpellSlot.W, SpellSlot.Q, SpellSlot.E }; 
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = ChampionSpell.FromLibrary("Cassiopeia", SpellSlot.Q, ChampionSpell.CastType.CIRCLE);
            var W = new ChampionSpell(SpellSlot.W, 850, ChampionSpell.CastType.CIRCLE);
            var E = new ChampionSpell(SpellSlot.E, 700, ChampionSpell.CastType.TARGET);
            var R = ChampionSpell.FromLibrary("Cassiopeia", SpellSlot.R, ChampionSpell.CastType.CONE);

            E.CastCondition = (Obj_AI_Base unit) =>
            {
                return unit.HasBuffOfType(BuffType.Poison); 
            };

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R); 

            return result; 
        }
    }
}
