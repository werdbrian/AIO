using AIO.Wrapper;
using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Champions
{
    public class Kassadin : Champion
    {
        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.R, SpellSlot.E, SpellSlot.W, SpellSlot.Q };
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = new ChampionSpell(SpellSlot.Q, 650, ChampionSpell.CastType.TARGET);
            var W = new ChampionSpell(SpellSlot.W, 125, ChampionSpell.CastType.AA_ATTACK);
            var E = new ChampionSpell(SpellSlot.E, 400, ChampionSpell.CastType.CONE);
            var R = new ChampionSpell(SpellSlot.R, 500, ChampionSpell.CastType.CIRCLE);

            R.CastCondition = (u) =>
            {
                return R.Instance.Instance.ManaCost < ObjectManager.Player.Mana; 
            };

            result.Add(Q);
            result.Add(W);
            result.Add(E);
            result.Add(R); 

            return result;
        }
    }
}
