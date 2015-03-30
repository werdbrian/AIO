using AIO.Library;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Common
{
    public class GameNotifier
    {
        public GameNotifier()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsValid)
            {
                return; 
            }

            var spell = sender.Spellbook.Spells.FirstOrDefault(s => s.SData.Hash == args.SData.Hash);

            if (spell != null && spell.Slot == SpellSlot.R)
            {
                var notification = new Notification(string.Format("{0} has just cast {1}, will be back up in {2}", sender.Name, spell.Slot, spell.Cooldown), 5 * 1000);
                notification.Flash();

                Notifications.AddNotification(notification); 
            }
        }
    }
}
