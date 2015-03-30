using AIO.Wrapper;
using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Champions
{
    public class Gragas : Champion
    {
        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.W, SpellSlot.Q, SpellSlot.E, SpellSlot.R };
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = ChampionSpell.FromLibrary("Gragas", SpellSlot.Q, ChampionSpell.CastType.CIRCLE);
            var W = new ChampionSpell(SpellSlot.W, 400, ChampionSpell.CastType.SELF);
            var E = ChampionSpell.FromLibrary("Gragas", SpellSlot.E, ChampionSpell.CastType.LINEAR_COLLISION);
            var R = ChampionSpell.FromLibrary("Gragas", SpellSlot.R, ChampionSpell.CastType.CIRCLE);

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R);

            return result;
        }
    }
}
