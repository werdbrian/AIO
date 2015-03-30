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
    public class Rengar : Champion
    {
        public override SpellSlot[] GetOrder()
        {
            return new SpellSlot[] { SpellSlot.E, SpellSlot.W, SpellSlot.Q }; 
        }

        public override List<ChampionSpell> GetSpells()
        {
            var result = new List<ChampionSpell>();

            var Q = new ChampionSpell(SpellSlot.Q, 325, ChampionSpell.CastType.AA_ATTACK);
            var W = new ChampionSpell(SpellSlot.W, 350, ChampionSpell.CastType.SELF);
            var E = ChampionSpell.FromLibrary("Rengar", SpellSlot.E, ChampionSpell.CastType.LINEAR_COLLISION);

            Q.CastCondition = (Obj_AI_Base unit) =>
            {
                var mana = ObjectManager.Player.Mana; 
                return mana < 5 || (mana == 5 && !(E.IsReady(unit) && E.Instance.WillHit(unit.ServerPosition, ObjectManager.Player.ServerPosition)) && ObjectManager.Player.HealthPercentage() > 75); 
            };

            W.CastCondition = (Obj_AI_Base unit) =>
            {
                var mana = ObjectManager.Player.Mana;
                return mana < 5 || (mana == 5 && !(E.IsReady(unit) && E.Instance.WillHit(unit.ServerPosition, ObjectManager.Player.ServerPosition)) && ObjectManager.Player.HealthPercentage() < 75);
            };

            E.CastCondition = (Obj_AI_Base unit) =>
            {
                var mana = ObjectManager.Player.Mana;
                return mana < 5 || (mana == 5 && (E.IsReady(unit) && E.Instance.WillHit(unit.ServerPosition, ObjectManager.Player.ServerPosition))); 
            };

            result.Add(Q);
            result.Add(W);
            result.Add(E); 
            
            return result;
        }
    }
}
