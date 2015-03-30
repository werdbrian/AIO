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
    public class Malzahar : Champion
    {
        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = ChampionSpell.FromLibrary("Malzahar", SpellSlot.Q, ChampionSpell.CastType.CIRCLE);
            var W = new ChampionSpell(SpellSlot.W, 800, ChampionSpell.CastType.CIRCLE);
            var E = new ChampionSpell(SpellSlot.E, 650, ChampionSpell.CastType.TARGET);
            var R = new ChampionSpell(SpellSlot.R, 700, ChampionSpell.CastType.TARGET);

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R);

            return result;
        }
    }
}
