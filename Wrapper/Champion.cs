using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Wrapper
{
    /// <summary>
    ///     Champion Instance, abstracted class for override instances
    /// </summary>
    public abstract class Champion
    {
        private List<ChampionSpell> _Spells = null;
        private List<SpellSlot> _Order = null;
        private int? _LongestRange = null;
        private int? _ShortestRange = null;

        /// <summary>
        ///     Priority order for casting in a combo 
        /// </summary>
        /// <returns></returns>
        public abstract SpellSlot[] GetOrder();

        /// <summary>
        ///     Retrieves a new instance of the champion spells, should only be called once or risk overriding important methods. 
        /// </summary>
        /// <returns></returns>
        public abstract List<ChampionSpell> GetSpells();

        /// <summary>
        ///     Returns a one-time-set spell list used to maintain the sanity of the one-time call GetSpells
        /// </summary>
        public List<ChampionSpell> Spells
        {
            get
            {
                return (_Spells ?? (_Spells = GetSpells()));
            }
        }

        /// <summary>
        ///     Returns a one-time-set order list used to maintain sanity of the one-time call GetOrder
        /// </summary>
        public List<SpellSlot> Order
        {
            get
            {
                return (_Order ?? (_Order = GetOrder().ToList()));
            }
        }

        /// <summary>
        ///     Calculates the shortest range of the spell list
        /// </summary>
        public int? ShortestRange
        {
            get
            {
                return (_ShortestRange ?? (_ShortestRange = Spells.OrderBy(spell => spell.Range).FirstOrDefault().Range));
            }
        }
        
        /// <summary>
        ///     Calculates the longest range of the spell list
        /// </summary>
        public int? LongestRange 
        {
            get 
            {
                return (_LongestRange ?? (_LongestRange = Spells.OrderByDescending(spell => spell.Range).FirstOrDefault().Range));
            }
        }

        /// <summary>
        ///     Insert a new spell into the spell list
        /// </summary>
        /// <param name="spell"></param>
        public void Add(ChampionSpell spell)
        {
            Spells.Add(spell);
        }

        /// <summary>
        ///     Remove the specified spell from the spell list
        /// </summary>
        /// <param name="spell"></param>
        public void Remove(ChampionSpell spell)
        {
            Spells.Remove(spell);
        }

        /// <summary>
        ///     Returns the sane spell list
        /// </summary>
        /// <returns></returns>
        public List<ChampionSpell> GetList()
        {
            return Spells;
        }

        /// <summary>
        ///     Ordered combo sorted by Order  
        /// </summary>
        /// <returns></returns>
        public List<ChampionSpell> AsOrderedCombo()
        {
            return Spells.Where(s => s.Instance.IsReady() && s.IsEnabled).OrderBy(s => Order.IndexOf(s.Slot)).ToList(); 
        }

        /// <summary>
        ///     Returns a sane list of enabled lane clear and ready spells
        /// </summary>
        /// <returns></returns>
        public List<ChampionSpell> AsLaneClear()
        {
            return Spells.Where(spell => spell.IsEnabled_LaneClear && spell.Instance.IsReady()).ToList();
        }

        /// <summary>
        ///     Returns a sane list of enabled last hit and ready spells
        /// </summary>
        /// <returns></returns>
        public List<ChampionSpell> AsLastHit()
        {
            return Spells.Where(spell => spell.IsEnabled_LastHit && spell.Instance.IsReady()).ToList(); 
        }
    }
}
