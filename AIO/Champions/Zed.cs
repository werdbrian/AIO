// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Zed.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by 
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//   
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
//    GNU General Public License for more details.
//   
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The zed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Champions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AIO.Wrapper;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Configuration = AIO.Configuration;

    /// <summary>
    ///     The zed.
    /// </summary>
    public sealed class Zed : Champion
    {
        #region Constants

        /// <summary>
        ///     The delay w.
        /// </summary>
        private const int DelayW = 500;

        #endregion

        #region Fields

        /// <summary>
        ///     The e.
        /// </summary>
        private ChampionSpell championSpellE;

        /// <summary>
        ///     The q.
        /// </summary>
        private ChampionSpell championSpellQ;

        /// <summary>
        ///     The r.
        /// </summary>
        private ChampionSpell championSpellR;

        /// <summary>
        ///     The w.
        /// </summary>
        private ChampionSpell championSpellW;

        /// <summary>
        ///     The lastUltimatePosition.
        /// </summary>
        private Vector3 lastUltimatePosition;

        /// <summary>
        ///     The tick w.
        /// </summary>
        private int tickW;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Zed" /> class.
        /// </summary>
        public Zed()
        {
            this.championSpellQ = ChampionSpell.FromLibrary("Zed", SpellSlot.Q, ChampionSpell.CastType.Linear);
            this.championSpellQ.CastCondition = (unit) =>
                {
                    if (this.ShadowW != null && unit.Distance(this.ShadowW.ServerPosition) <= 900
                        && unit.Distance(ObjectManager.Player.ServerPosition) > 450)
                    {
                        this.championSpellQ.From = this.ShadowW.ServerPosition;
                    }
                    else
                    {
                        this.championSpellQ.From = ObjectManager.Player.ServerPosition;
                    }

                    return true;
                };
            this.championSpellW = new ChampionSpell(SpellSlot.W, 550f, ChampionSpell.CastType.Linear)
                                      {
                                          CastCondition = unit => true, CastFunction = (unit) =>
                                              {
                                                  if (DelayW >= (Environment.TickCount - this.tickW)
                                                      || (this.ShadowStage != ShadowCastStage.First)
                                                      || (unit.HasBuff("zedulttargetmark")
                                                          && LastCastedSpell.LastCastPacketSent != null
                                                          && LastCastedSpell.LastCastPacketSent.Slot == SpellSlot.R
                                                          && this.UltimateStage == UltimateCastStage.Cooldown))
                                                  {
                                                      return;
                                                  }

                                                  var position = unit.ServerPosition.Extend(
                                                      ObjectManager.Player.Position, 
                                                      -200);
                                                  this.championSpellW.Instance.Cast(position);
                                                  this.tickW = Environment.TickCount;
                                              }
                                      };
            this.championSpellE = new ChampionSpell(SpellSlot.E, 270f, ChampionSpell.CastType.Self)
                                      {
                                          CastCondition =
                                              (unit) =>
                                              (unit.Distance(ObjectManager.Player.ServerPosition)
                                               <= this.championSpellE.Range)
                                              || ((this.ShadowW != null
                                                   && this.ShadowW.Distance(unit) <= this.championSpellE.Range)
                                                  || (this.ShadowR != null
                                                      && this.ShadowR.Distance(unit) <= this.championSpellE.Range)), 
                                      };
            this.championSpellR = new ChampionSpell(SpellSlot.R, 650f, ChampionSpell.CastType.Target);
        }

        #endregion

        #region Enums

        /// <summary>
        ///     The shadow cast stage.
        /// </summary>
        private enum ShadowCastStage
        {
            /// <summary>
            ///     The first.
            /// </summary>
            First, 

            /// <summary>
            ///     The second.
            /// </summary>
            Second, 

            /// <summary>
            ///     The cooldown.
            /// </summary>
            Cooldown
        }

        /// <summary>
        ///     The ultimate cast stage.
        /// </summary>
        private enum UltimateCastStage
        {
            /// <summary>
            ///     The first.
            /// </summary>
            First, 

            /// <summary>
            ///     The second.
            /// </summary>
            Second, 

            /// <summary>
            ///     The cooldown.
            /// </summary>
            Cooldown
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the r shadow.
        /// </summary>
        private Obj_AI_Minion ShadowR
        {
            get
            {
                return
                    ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(
                            minion =>
                            minion.IsVisible && minion.IsAlly && (minion.ServerPosition == this.lastUltimatePosition)
                            && minion.Name == "Shadow");
            }
        }

        /// <summary>
        ///     Gets the shadow stage.
        /// </summary>
        private ShadowCastStage ShadowStage
        {
            get
            {
                if (!this.championSpellW.IsReady())
                {
                    return ShadowCastStage.Cooldown;
                }

                return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "ZedW"
                           ? ShadowCastStage.First
                           : ShadowCastStage.Second;
            }
        }

        /// <summary>
        ///     Gets the w shadow.
        /// </summary>
        private Obj_AI_Minion ShadowW
        {
            get
            {
                return
                    ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(
                            minion =>
                            minion.IsVisible && minion.IsAlly && (minion.ServerPosition != this.lastUltimatePosition)
                            && minion.Name == "Shadow");
            }
        }

        /// <summary>
        ///     Gets the ultimate stage.
        /// </summary>
        private UltimateCastStage UltimateStage
        {
            get
            {
                if (!this.championSpellR.IsReady())
                {
                    return UltimateCastStage.Cooldown;
                }

                return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "ZedR" 
                           ? UltimateCastStage.First
                           : UltimateCastStage.Second;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The on load.
        /// </summary>
        /// <returns>
        ///     The onLoad
        /// </returns>
        public override Action OnLoad()
        {
            return PerformOnce.A(
                () =>
                    {
                        Configuration.Combo.AddItem(new MenuItem("sep", string.Empty));
                        Configuration.Combo.AddItem(new MenuItem("AlwaysW", "Use W even if the Target is within AA Range")).SetValue<bool>(true);
                        Configuration.Combo.AddItem(new MenuItem("ComboType", "Combo Type"))
                            .SetValue<StringList>(new StringList(new[] { "Default", "Line" }));

                        Configuration.Harass.AddItem(new MenuItem("sep", string.Empty));
                        Configuration.Harass.AddItem(new MenuItem("LongHarass", "Enable Long Harass")).SetValue(false);

                        Configuration.Handler.AddItem(new MenuItem("sep", string.Empty));
                        Configuration.Handler.AddItem(new MenuItem("AutoE", "Enable Auto E")).SetValue(false);
                    });
        }

        /// <summary>
        ///     The on tick.
        /// </summary>
        /// <returns>
        ///     Tick return
        /// </returns>
        public override Action OnTick()
        {
            return () =>
                {
                    if (LastCastedSpell.LastCastPacketSent != null
                        && LastCastedSpell.LastCastPacketSent.Slot == SpellSlot.R)
                    {
                        var shadow =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .FirstOrDefault(minion => minion.IsVisible && minion.IsAlly && minion.Name == "Shadow");

                        this.lastUltimatePosition = shadow != null ? shadow.ServerPosition : Vector3.Zero;
                    }

                    var unit = TargetSelector.GetSelectedTarget()
                               ?? TargetSelector.GetTarget(1900, TargetSelector.DamageType.Physical);

                    if (unit != null && unit.IsValidTarget())
                    {
                        if (this.championSpellE.IsEnabled
                            && Configuration.Handler.Item("AutoE").GetValue<bool>()
                            && ((this.ShadowW != null
                                 && this.ShadowW.ServerPosition.Distance(unit.ServerPosition)
                                 <= this.championSpellE.Range)
                                || (this.ShadowR != null && this.ShadowR.Distance(unit) <= this.championSpellE.Range)))
                        {
                            this.championSpellE.Instance.Cast();
                        }

                        Program.DamageIndicator.Target = unit;
                    }

                    switch (Program.Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.Combo:
                            if (unit == null || !unit.IsValidTarget())
                            {
                                return;
                            }

                            var option = Configuration.Combo.Item("ComboType").GetValue<StringList>();

                            switch (option.SelectedIndex)
                            {
                                case 0:
                                    this.PerformCombo(unit);
                                    break;
                                case 1:
                                    this.PerformLineCombo(unit);
                                    break;
                            }

                            break;
                        case Orbwalking.OrbwalkingMode.Mixed:
                            if (unit == null || !unit.IsValidTarget())
                            {
                                return;
                            }

                            this.PerformHarass(unit);
                            break;
                        case Orbwalking.OrbwalkingMode.LaneClear:
                            this.PerformLaneClear();
                            break;
                        case Orbwalking.OrbwalkingMode.LastHit:
                            this.PerformLastHit();
                            break;
                    }
                };
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Priority order for casting in a combo
        /// </summary>
        /// <returns>The ordered spell</returns>
        protected override SpellSlot[] GetOrder()
        {
            return new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
        }

        /// <summary>
        ///     Retrieves a new instance of the champion spells, should only be called once or risk overriding important methods.
        /// </summary>
        /// <returns>Created Spells</returns>
        protected override List<ChampionSpell> GetSpells()
        {
            return new List<ChampionSpell>()
                       {
                          this.championSpellQ, this.championSpellW, this.championSpellE, this.championSpellR 
                       };
        }

        /// <summary>
        ///     The perform combo.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        private void PerformCombo(Obj_AI_Base unit)
        {
            var damage = this.championSpellQ.GetDamage(unit) + this.championSpellE.GetDamage(unit)
                         + (ObjectManager.Player.GetAutoAttackDamage(unit, true) * 2);

            if (this.championSpellR.IsEnabled && this.UltimateStage == UltimateCastStage.First
                && (damage < unit.Health
                    || (!this.championSpellW.IsReady() && this.championSpellW.Instance.Instance.Cooldown > 2f
                        && this.championSpellQ.GetDamage(unit) < unit.Health
                        && unit.Distance(ObjectManager.Player.ServerPosition) > 400)))
            {
                if (this.championSpellW.IsEnabled
                    && ((unit.Distance(ObjectManager.Player.ServerPosition) < 700
                         && unit.MoveSpeed > ObjectManager.Player.MoveSpeed)
                        || unit.Distance(ObjectManager.Player) > 800))
                {
                    this.championSpellW.Cast(unit, false, true);
                    this.championSpellW.Instance.Cast();
                }

                this.championSpellR.Cast(unit);
            }
            else
            {
                if (this.championSpellW.IsEnabled && this.ShadowStage == ShadowCastStage.First
                    && unit.Distance(ObjectManager.Player.ServerPosition) > (Configuration.Combo.Item("AlwaysW").GetValue<bool>() ? 0 : 400)
                    && unit.Distance(ObjectManager.Player.ServerPosition) < 1300)
                {
                    this.championSpellW.Cast(unit, false, true);
                }

                if (this.championSpellW.IsEnabled && this.ShadowStage == ShadowCastStage.Second
                    && unit.Distance(this.ShadowW.ServerPosition) < unit.Distance(ObjectManager.Player.ServerPosition))
                {
                    this.championSpellW.Instance.Cast();
                }

                if (this.championSpellE.IsEnabled)
                {
                    this.championSpellE.Cast(unit);

                    if (this.ShadowW != null && unit.Distance(this.ShadowW.ServerPosition) < this.championSpellE.Range)
                    {
                        this.championSpellE.Instance.Cast();
                    }
                }

                if (this.championSpellQ.IsEnabled)
                {
                    this.championSpellQ.Cast(unit);
                }
            }
        }

        /// <summary>
        ///     The perform harass.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        private void PerformHarass(Obj_AI_Base unit)
        {
            var position = unit.Distance(ObjectManager.Player.ServerPosition);

            if (this.championSpellW.IsEnabled_Harass && Configuration.Harass.Item("LongHarass").GetValue<bool>()
                && this.championSpellW.IsReady() && this.championSpellQ.IsReady()
                && ObjectManager.Player.Mana
                > this.championSpellQ.Instance.Instance.ManaCost + this.championSpellW.Instance.Instance.ManaCost
                && position > 850 && position < 1400)
            {
                Console.WriteLine("LONG HARASS");
                this.championSpellW.Cast(unit, false, true);
            }

            if (this.championSpellQ.IsEnabled_Harass && this.championSpellQ.IsReady() && (!this.championSpellW.IsEnabled_LastHit || (this.ShadowStage != ShadowCastStage.First
                && (position <= 900 || (this.ShadowW != null && unit.Distance(this.ShadowW.ServerPosition) <= 900)))))
            {
                Console.WriteLine("q HARASS");
                this.championSpellQ.Cast(unit);
            }

            if (this.championSpellW.IsEnabled_Harass && this.championSpellW.IsReady() && this.championSpellQ.IsReady()
                && ObjectManager.Player.Mana
                > this.championSpellQ.Instance.Instance.ManaCost + this.championSpellW.Instance.Instance.ManaCost
                && position < 750)
            {
                Console.WriteLine("w HARASS");
                this.championSpellW.Cast(unit, false, true);
            }

            if (this.championSpellE.IsEnabled_Harass)
            {
                Console.WriteLine("e HARASS");
                this.championSpellE.Cast(unit);

                if (this.ShadowW != null && unit.Distance(this.ShadowW.ServerPosition) < this.championSpellE.Range)
                {
                    Console.WriteLine("e shadow HARASS");
                    this.championSpellE.Instance.Cast();
                }
            }
        }

        /// <summary>
        ///     The perform lane clear.
        /// </summary>
        private void PerformLaneClear()
        {
            var mana = ObjectManager.Player.Mana;

            if (this.championSpellE.IsEnabled_LaneClear && this.championSpellW.IsEnabled_LaneClear
                && mana
                > this.championSpellE.Instance.Instance.ManaCost + this.championSpellW.Instance.Instance.ManaCost
                && this.ShadowStage == ShadowCastStage.First)
            {
                var position =
                    MinionManager.GetBestCircularFarmLocation(
                        MinionManager.GetMinions(this.championSpellE.Range + this.championSpellW.Range)
                            .Select(s => s.ServerPosition.To2D())
                            .ToList(), 
                        this.championSpellE.Width, 
                        this.championSpellW.Range + this.championSpellE.Range);

                if (ObjectManager.Player.Distance(position.Position)
                    < this.championSpellW.Range + this.championSpellE.Range)
                {
                    if (DelayW < (Environment.TickCount - this.tickW))
                    {
                        this.championSpellW.Instance.Cast(position.Position);
                        this.tickW = Environment.TickCount;
                    }

                    this.championSpellE.Instance.Cast();
                }
            }

            if (this.championSpellQ.IsEnabled_LaneClear)
            {
                this.championSpellQ.LaneClear();
            }

            if (this.championSpellE.IsEnabled_LaneClear)
            {
                this.championSpellE.LaneClear();
            }
        }

        /// <summary>
        ///     The perform last hit.
        /// </summary>
        private void PerformLastHit()
        {
            if (this.championSpellQ.IsEnabled_LastHit)
            {
                this.championSpellQ.LastHit();
            }

            if (this.championSpellE.IsEnabled_LastHit)
            {
                this.championSpellE.LastHit();
            }
        }

        /// <summary>
        ///     The perform line combo.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        private void PerformLineCombo(Obj_AI_Base unit)
        {
            if (!this.championSpellR.IsReady() || unit.Distance(ObjectManager.Player.ServerPosition) >= 640)
            {
                return;
            }

            if (this.UltimateStage == UltimateCastStage.First)
            {
                this.championSpellR.Cast(unit);
            }

            if (this.ShadowStage == ShadowCastStage.First && this.UltimateStage == UltimateCastStage.Second
                && LastCastedSpell.LastCastPacketSent != null && LastCastedSpell.LastCastPacketSent.Slot != SpellSlot.W)
            {
                var position = unit.Position.Extend(ObjectManager.Player.ServerPosition, -500);

                this.championSpellW.Instance.Cast(position);
                this.championSpellE.Instance.Cast();
                this.championSpellQ.Cast(unit);
            }

            if (this.ShadowW != null && this.UltimateStage == UltimateCastStage.Second
                && unit.Distance(ObjectManager.Player) > 250
                && unit.Distance(this.ShadowW.ServerPosition) < unit.Distance(ObjectManager.Player))
            {
                this.championSpellW.Instance.Cast();
            }
        }

        #endregion
    }
}
