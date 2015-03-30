using AIO.Wrapper;
using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Champions
{
    public class Khazix : Champion
    {
        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.E, SpellSlot.W, SpellSlot.Q, SpellSlot.R };
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = new ChampionSpell(SpellSlot.Q, 325, ChampionSpell.CastType.TARGET);
            var W = ChampionSpell.FromLibrary("Khazix", SpellSlot.W, ChampionSpell.CastType.LINEAR_COLLISION);
            var E = ChampionSpell.FromLibrary("Khazix", SpellSlot.E, ChampionSpell.CastType.CIRCLE);
            var R = new ChampionSpell(SpellSlot.R, 400, ChampionSpell.CastType.SELF);

            Q.CastCondition = (unit) =>
            {
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "khazixqlong")
                    Q.Range = 375; 
                return true; // always return true, this will make the condition checked and passed each call 
            };

            E.CastCondition = (unit) =>
            {
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Name == "khazixelong")
                    E.Range = 900;
                return true; // always return true, this will make the condition checked and passed each call 
            };

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R);

            return result;
        }
    }
}
