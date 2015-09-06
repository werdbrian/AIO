// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChampionSpell.cs" company="LeagueSharp">
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
//   Represents an in game Champion spell, this wrapper automates cast, last hit, and lane clear behavior.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Wrapper
{
    using System;
    using System.Linq;

    using AIO.Common;
    using AIO.Library;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Configuration = AIO.Configuration;

    /// <summary>
    ///     Represents an in game Champion spell, this wrapper automates cast, last hit, and lane clear behavior.
    /// </summary>
    public class ChampionSpell
    {
        #region Fields

        /// <summary>
        ///     Custom cast conditional, checked on call of Cast
        /// </summary>
        public Func<Obj_AI_Base, bool> CastCondition;

        /// <summary>
        ///     Cast method, can be bypassed through Cast parameter
        /// </summary>
        public Action<Obj_AI_Base> CastFunction;

        /// <summary>
        ///     The casting type.
        /// </summary>
        public CastType CastingType;

        /// <summary>
        ///     The clear.
        /// </summary>
        public ClearDelegate Clear;

        /// <summary>
        ///     The delay.
        /// </summary>
        public float Delay;

        /// <summary>
        ///     The farm.
        /// </summary>
        public FarmDelegate Farm;

        /// <summary>
        ///     The instance.
        /// </summary>
        public Spell Instance;

        /// <summary>
        ///     The range.
        /// </summary>
        public float Range;

        /// <summary>
        ///     The slot.
        /// </summary>
        public readonly SpellSlot Slot;

        /// <summary>
        ///     The speed.
        /// </summary>
        public float Speed;

        /// <summary>
        ///     The width.
        /// </summary>
        public float Width;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChampionSpell" /> class.
        /// </summary>
        /// <param name="Slot">
        ///     The slot.
        /// </param>
        /// <param name="Range">
        ///     The range.
        /// </param>
        /// <param name="CastingType">
        ///     The casting type.
        /// </param>
        /// <param name="Speed">
        ///     The speed.
        /// </param>
        /// <param name="Delay">
        ///     The delay.
        /// </param>
        /// <param name="Width">
        ///     The width.
        /// </param>
        /// <param name="AddToMenu">
        ///     The add to menu.
        /// </param>
        public ChampionSpell(
            SpellSlot Slot, 
            float Range, 
            CastType CastingType, 
            float Speed = float.MaxValue, 
            float Delay = 0.250f, 
            float Width = 50, 
            bool AddToMenu = true)
        {
            this.Slot = Slot;
            this.Range = Range;
            this.CastingType = CastingType;
            this.Speed = Speed;
            this.Delay = Delay;
            this.Width = Width;

            this.CanDamage = true;
            this.Instance = new Spell(Slot, Range);

            if (CastingType == CastType.Linear || CastingType == CastType.LinearCollision
                || CastingType == CastType.Circle || CastingType == CastType.Cone)
            {
                this.Instance.SetSkillshot(
                    Delay, 
                    Width, 
                    Speed, 
                    CastingType == CastType.LinearCollision, 
                    this.SkillShotType);
            }
            else if (CastingType == CastType.Target)
            {
                this.Instance.SetTargetted(Delay, Speed);
            }

            if (AddToMenu)
            {
                this.AddToMenu();
            }
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Lane clear delegate
        /// </summary>
        public delegate void ClearDelegate();

        /// <summary>
        ///     Last hit delegate
        /// </summary>
        public delegate void FarmDelegate();

        #endregion

        #region Enums

        /// <summary>
        ///     Cast behavior of the specified Champion Spell
        /// </summary>
        public enum CastType
        {
            /// <summary>
            ///     Linear skillshot
            /// </summary>
            Linear, 

            /// <summary>
            ///     Linear skillshot with collision against Obj_AI_Base
            /// </summary>
            LinearCollision, 

            /// <summary>
            ///     Circle skillshot (can also be used for place spells, ie. Malzahar Q)
            /// </summary>
            Circle, 

            /// <summary>
            ///     Cone skillshot
            /// </summary>
            Cone, 

            /// <summary>
            ///     Targeted spell, can only be cast on a Obj_AI_Base
            /// </summary>
            Target, 

            /// <summary>
            ///     Self spell, commonly a powerup spell
            /// </summary>
            Self, 

            /// <summary>
            ///     AA-Powered spell, cast then attack move is issued to the desired Obj_AI_Base
            /// </summary>
            AaAttack, 

            /// <summary>
            ///     Friendly shield cast, for self only use SELF
            /// </summary>
            Shield
        }

        /// <summary>
        ///     Handler type, created in CreateHandler
        /// </summary>
        public enum HandlerType
        {
            /// <summary>
            ///     Automated handle if a nearby unit is immobile
            /// </summary>
            Immobile, 

            /// <summary>
            ///     Automated handle if a nearby unit is dashing
            /// </summary>
            Dash, 

            /// <summary>
            ///     Automated handle if a nearby unit is killable by the specified ChampionSpell
            /// </summary>
            Killable, 

            /// <summary>
            ///     Automated handle if the minimum amount of specified enemies can be hit with the ChampionSpell
            /// </summary>
            OnMinimumHit, 

            /// <summary>
            ///     Automated handle for farming
            /// </summary>
            Farm, 

            /// <summary>
            ///     Automated handle for healing (self)
            /// </summary>
            OnSelfHealthBelow, 

            /// <summary>
            ///     Automated handle for interrupting important spells
            /// </summary>
            OnImportantCast
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Determines if the spell performs damage
        /// </summary>
        public bool CanDamage { get; set; }

        /// <summary>
        ///     Prediction based FROM location
        /// </summary>
        public Vector3 From
        {
            get
            {
                return this.Instance.From;
            }

            set
            {
                this.Instance.UpdateSourcePosition(value, value);
            }
        }

        /// <summary>
        ///     References the specified sub menu created for additional handlers
        /// </summary>
        public Menu HandlerMenu
        {
            get
            {
                return this.SpellMenu.SubMenu("Handler");
            }
        }

        /// <summary>
        ///     Determines if the ChampionSpell is enabled in the spell menu for general casting
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return this.SpellMenu.Item("Enabled").GetValue<bool>();
            }

            set
            {
                this.SpellMenu.Item("Enabled").SetValue<bool>(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is enabled_ harass.
        /// </summary>
        public bool IsEnabled_Harass
        {
            get
            {
                return this.SpellMenu.Item("Harass").GetValue<bool>();
            }

            set
            {
                this.SpellMenu.Item("Harass").SetValue<bool>(value);
            }
        }

        /// <summary>
        ///     Determines if the ChampionSpell is enabled in the spell menu for lane clear
        /// </summary>
        public bool IsEnabled_LaneClear
        {
            get
            {
                return this.SpellMenu.Item("LaneClear").GetValue<bool>();
            }

            set
            {
                this.SpellMenu.Item("LaneClear").SetValue<bool>(value);
            }
        }

        /// <summary>
        ///     Determines if the ChampionSpell is enabled in the spell menu for last hit
        /// </summary>
        public bool IsEnabled_LastHit
        {
            get
            {
                return this.SpellMenu.Item("LastHit").GetValue<bool>();
            }

            set
            {
                this.SpellMenu.Item("LastHit").SetValue<bool>(value);
            }
        }

        /// <summary>
        ///     Converts the local ChampionSpell CastType into LeagueSharp's SkillshotType
        /// </summary>
        public SkillshotType SkillShotType
        {
            get
            {
                switch (this.CastingType)
                {
                    case CastType.Linear:
                    case CastType.LinearCollision:
                        return SkillshotType.SkillshotLine;
                    case CastType.Cone:
                        return SkillshotType.SkillshotCone;
                    case CastType.Circle:
                        return SkillshotType.SkillshotCircle;
                    default:
                        return SkillshotType.SkillshotLine;
                }
            }
        }

        /// <summary>
        ///     Damage type of the spell, used in target selection
        /// </summary>
        /// <summary>
        ///     References the specified sub menu created during instantiation of the ChampionSpell
        /// </summary>
        public Menu SpellMenu
        {
            get
            {
                return Configuration.Spell.SubMenu(this.SpellString);
            }
        }

        /// <summary>
        ///     String instance of the specified ChampionSpell
        /// </summary>
        public string SpellString
        {
            get
            {
                switch (this.Slot)
                {
                    case SpellSlot.Q:
                        return "Q";
                    case SpellSlot.W:
                        return "W";
                    case SpellSlot.E:
                        return "E";
                    case SpellSlot.R:
                        return "R";
                }

                return string.Empty;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates a new instance of ChampionSpell from the local spell library
        /// </summary>
        /// <param name="ChampionName"></param>
        /// <param name="Slot"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static ChampionSpell FromLibrary(string ChampionName, SpellSlot Slot, CastType Type)
        {
            var retrieved = SpellLibrary.GetSpell(ChampionName, Slot);

            return new ChampionSpell(
                Slot, 
                retrieved.Range, 
                Type, 
                retrieved.MissileSpeed, 
                retrieved.Delay == 0 ? 0 : ((float)retrieved.Delay) / 1000, 
                retrieved.Radius);
        }

        /// <summary>
        ///     Attempts to retrieve the champion spell information from the spell library, then in game values.
        /// </summary>
        /// <param name="Slot">Slot of the spell</param>
        /// <param name="CastingType">Casting type of the ability</param>
        /// <param name="Unit">Unit to retrieve spells from</param>
        /// <returns></returns>
        public static ChampionSpell FromLibrary(SpellSlot Slot, CastType CastingType, Obj_AI_Hero Unit = null)
        {
            var unit = Unit ?? ObjectManager.Player;
            var retrieved = SpellLibrary.GetSpell(unit.ChampionName, Slot);

            if (retrieved != null)
            {
                return new ChampionSpell(
                    Slot, 
                    retrieved.Range, 
                    CastingType, 
                    retrieved.MissileSpeed, 
                    ((float)retrieved.Delay) / 1000, 
                    retrieved.Radius);
            }

            var spell = unit.Spellbook.GetSpell(Slot);
            var spellData = spell.SData;

            return new ChampionSpell(
                Slot, 
                spellData.CastRange, 
                CastingType, 
                spellData.MissileSpeed, 
                spellData.SpellCastTime / 1000, 
                spellData.CastRadius);
        }

        /// <summary>
        ///     Create and adds the spell instance menu variables into the Configuration menu
        /// </summary>
        public void AddToMenu()
        {
            Configuration.Spell.AddSubMenu(new Menu(this.SpellString, this.SpellString));

            this.SpellMenu.AddSubMenu(new Menu("Automatic Handlers", "Handler"));

            this.SpellMenu.AddItem(new MenuItem("DisableAll", "Disable all Automatic Handlers").SetValue<bool>(false));
            this.SpellMenu.AddItem(new MenuItem("Enabled", "Enabled").SetValue<bool>(this.Slot != SpellSlot.R));
            this.SpellMenu.AddItem(new MenuItem("LaneClear", "Enable LaneClear")).SetValue<bool>(false);
            this.SpellMenu.AddItem(new MenuItem("LastHit", "Enable LastHit")).SetValue<bool>(false);

            if (this.Slot != SpellSlot.R)
            {
                this.SpellMenu.AddItem(new MenuItem("Harass", "Enable Harass")).SetValue<bool>(false);
            }

            this.SpellMenu.Item("DisableAll").ValueChanged += (object sender, OnValueChangeEventArgs args) =>
                {
                    if (args.GetNewValue<bool>())
                    {
                        foreach (var item in this.HandlerMenu.Items)
                        {
                            if (item.DisplayName.ToLower().Contains("enable"))
                            {
                                item.SetValue<bool>(false);
                            }
                        }
                    }
                };
        }

        /// <summary>
        ///     Cast behavior handle for casting the ChampionSpell against Obj_AI_Base (single target OR AoE)
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="IgnoreCustomCasting"></param>
        public void Cast(Obj_AI_Base unit, bool IgnoreCustomCasting = false, bool IgnoreRangeCheck = false)
        {
            if ((!IgnoreRangeCheck && !unit.IsValidTarget(this.Range)) || !this.Instance.IsReady())
            {
                return;
            }

            if (this.CastCondition != null && !this.CastCondition(unit))
            {
                return;
            }

            if (this.CastFunction != null && !IgnoreCustomCasting)
            {
                this.CastFunction(unit);
                return;
            }

            switch (this.CastingType)
            {
                case CastType.Self:
                    this.Instance.Cast();
                    break;
                case CastType.AaAttack:
                    if (Orbwalking.CanAttack()
                        && unit.ServerPosition.Distance(ObjectManager.Player.ServerPosition)
                        <= ObjectManager.Player.AttackRange)
                    {
                        this.Instance.Cast();
                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
                    }

                    break;
                case CastType.Target:
                    this.Instance.CastOnUnit(unit);
                    break;
                case CastType.Linear:
                case CastType.LinearCollision:
                case CastType.Circle:
                case CastType.Cone:
                    PredictionOutput prediction;

                    if (Configuration.Miscellaneous.Item("CustomPrediction", true).GetValue<bool>())
                    {
                        prediction =
                            MovementPrediction.GetUpdatedPrediction2(
                                new PredictionInput
                                    {
                                        Unit = unit, Delay = this.Delay, Radius = this.Width, Range = this.Range, 
                                        Speed = this.Speed, Collision = this.CastingType == CastType.LinearCollision
                                    });
                    }
                    else
                    {
                        prediction = this.Instance.GetPrediction(unit);
                    }

                    if (prediction != null)
                    {
                        if (prediction.Hitchance > HitChance.Low)
                        {
                            this.Instance.Cast(prediction.CastPosition);
                        }
                    }

                    break;
            }
        }

        /// <summary>
        ///     Create and returns an instanced Handler of the specified type
        /// </summary>
        /// <param name="Handle"></param>
        /// <returns></returns>
        public Handler CreateHandler(HandlerType Handle)
        {
            switch (Handle)
            {
                case HandlerType.Immobile:
                    this.HandlerMenu.AddItem(
                        new MenuItem("Immobile", "[Auto] Enable use on Immobilized champions").SetValue<bool>(false));

                    return new Handler(
                        this.Instance, 
                        (Spell spell) =>
                            {
                                foreach (var unit in
                                    ObjectManager.Get<Obj_AI_Hero>().Where(unit => unit.IsValidTarget(this.Range)))
                                {
                                    spell.CastIfHitchanceEquals(unit, HitChance.Immobile);
                                    break;
                                }
                            }, 
                        (Spell spell) => this.Instance.IsReady() && this.HandlerMenu.Item("Immobile").GetValue<bool>());
                case HandlerType.Dash:
                    this.HandlerMenu.AddItem(
                        new MenuItem("Dashing", "[Auto] Enable use on Dashing champions").SetValue<bool>(false));

                    return new Handler(
                        this.Instance, 
                        (Spell spell) =>
                            {
                                foreach (var unit in
                                    ObjectManager.Get<Obj_AI_Hero>().Where(unit => unit.IsValidTarget(this.Range)))
                                {
                                    spell.CastIfHitchanceEquals(unit, HitChance.Dashing);
                                    break;
                                }
                            }, 
                        (Spell spell) => this.Instance.IsReady() && this.HandlerMenu.Item("Dashing").GetValue<bool>());
                case HandlerType.Killable:
                    this.HandlerMenu.AddItem(new MenuItem("Killsteal", "[Auto] Enable use on kill-able champions (KS)"))
                        .SetValue<bool>(false);

                    return new Handler(
                        this.Instance, 
                        (Spell spell) =>
                            {
                                foreach (var unit in
                                    ObjectManager.Get<Obj_AI_Hero>()
                                        .Where(
                                            unit =>
                                            unit.IsValidTarget(this.Range)
                                            && HealthPrediction.GetHealthPrediction(
                                                unit, 
                                                (int)(this.Speed / unit.Distance(ObjectManager.Player, false)), 
                                                (int)this.Delay) <= this.GetDamage(unit)))
                                {
                                    var prediction = spell.GetPrediction(unit);

                                    if (prediction != null && prediction.Hitchance >= HitChance.Medium)
                                    {
                                        spell.Cast(prediction.CastPosition);
                                    }
                                }
                            }, 
                        (Spell spell) => this.Instance.IsReady() && this.HandlerMenu.Item("Killsteal").GetValue<bool>());
                case HandlerType.OnMinimumHit:
                    this.HandlerMenu.AddItem(new MenuItem("MinimumHit", "[Auto] Enable casting on minimum hit"))
                        .SetValue<bool>(false);
                    this.HandlerMenu.AddItem(new MenuItem("MinimumHitAmount", "-> Amount needed for Minimum Hit"))
                        .SetValue<Slider>(new Slider(1, 0, 5));

                    return new Handler(
                        this.Instance, 
                        (Spell spell) =>
                            {
                                foreach (var unit in
                                    ObjectManager.Get<Obj_AI_Hero>().Where(unit => unit.IsValidTarget(this.Range)))
                                {
                                    spell.CastIfWillHit(
                                        unit, 
                                        this.HandlerMenu.Item("MinimumHitAmount").GetValue<Slider>().Value - 1);
                                    break;
                                }
                            }, 
                        (Spell spell) => this.Instance.IsReady() && this.HandlerMenu.Item("MinimumHit").GetValue<bool>());
                case HandlerType.Farm:
                    this.HandlerMenu.AddItem(new MenuItem("Farm", "[Auto] Enable AFK farming")).SetValue<bool>(false);

                    return new Handler(
                        this.Instance, 
                        (Spell spell) =>
                            {
                                foreach (var unit in
                                    ObjectManager.Get<Obj_AI_Minion>()
                                        .Where(
                                            unit =>
                                            unit.IsValidTarget(this.Range)
                                            && HealthPrediction.GetHealthPrediction(
                                                unit, 
                                                (int)(this.Speed / unit.Distance(ObjectManager.Player, false)), 
                                                (int)this.Delay) <= this.GetDamage(unit)))
                                {
                                    if (this.CastingType == CastType.Linear || this.CastingType == CastType.Cone
                                        || this.CastingType == CastType.Circle)
                                    {
                                        this.LaneClear();
                                    }
                                    else
                                    {
                                        this.Cast(unit);
                                    }

                                    break;
                                }
                            }, 
                        (Spell spell) => this.Instance.IsReady() && this.HandlerMenu.Item("Farm").GetValue<bool>());
                case HandlerType.OnSelfHealthBelow:
                    this.HandlerMenu.AddItem(new MenuItem("SelfHeal", "[Auto] Enable (self) Heal"))
                        .SetValue<bool>(false);
                    this.HandlerMenu.AddItem(new MenuItem("HealPercent", "-> Health Percent"))
                        .SetValue<Slider>(new Slider(40, 0, 100));
                    this.HandlerMenu.AddItem(new MenuItem("ManaPercent", "-> Min Mana Percent"))
                        .SetValue<Slider>(new Slider(30, 0, 100));

                    return new Handler(
                        this.Instance, 
                        (Spell spell) =>
                            {
                                var HealthPercent = this.HandlerMenu.Item("HealPercent").GetValue<Slider>().Value;
                                var ManaPercent = this.HandlerMenu.Item("ManaPercent").GetValue<Slider>().Value;

                                if (HealthPercent < ObjectManager.Player.HealthPercent
                                    && ManaPercent < ObjectManager.Player.ManaPercent)
                                {
                                    spell.Cast();
                                }
                            }, 
                        (Spell spell) => this.Instance.IsReady() && this.HandlerMenu.Item("SelfHeal").GetValue<bool>());
                case HandlerType.OnImportantCast:
                    this.HandlerMenu.AddItem(new MenuItem("Interrupt", "[Auto] Enable Interrupting"))
                        .SetValue<bool>(false);

                    return new Handler(
                        this.Instance, 
                        (Spell spell) =>
                            {
                                var units =
                                    ObjectManager.Get<Obj_AI_Hero>()
                                        .Where(
                                            s =>
                                            s.IsCastingInterruptableSpell()
                                            && s.ServerPosition.Distance(ObjectManager.Player.ServerPosition)
                                            < this.Range);

                                foreach (var unit in
                                    units.OrderBy(s => s.ServerPosition.Distance(ObjectManager.Player.ServerPosition)))
                                {
                                    this.Cast(unit);
                                }
                            }, 
                        (Spell spell) =>
                        this.Instance.IsReady() && this.HandlerMenu.Item("Interrupt").GetValue<bool>()
                        && ObjectManager.Get<Obj_AI_Hero>()
                               .Any(
                                   s =>
                                   s.IsEnemy
                                   && s.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < this.Range
                                   && s.IsCastingInterruptableSpell(true)));
            }

            return null;
        }

        /// <summary>
        ///     Returns the total damage the ChampionSpell can do against the specified Obj_AI_Base
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public double GetDamage(Obj_AI_Base unit)
        {
            return this.CanDamage ? ObjectManager.Player.GetSpellDamage(unit, this.Slot) : 0;
        }

        /// <summary>
        ///     The is ready.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsReady(Obj_AI_Base unit)
        {
            if (!this.Instance.IsReady())
            {
                return false;
            }

            if (this.CastCondition != null && !this.CastCondition(unit))
            {
                return false;
            }

            return ObjectManager.Player.Mana >= this.Instance.Instance.ManaCost;
        }

        /// <summary>
        /// The is ready.
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsReady()
        {
            return this.Instance.IsReady();
        }

        /// <summary>
        ///     Lane clear behavior for the specified ChampionSpell
        /// </summary>
        public void LaneClear()
        {
            if (this.Clear != null)
            {
                this.Clear();
                return;
            }

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, this.Range);
            var position = new MinionManager.FarmLocation();

            switch (this.CastingType)
            {
                case CastType.Linear:
                case CastType.Cone:
                    position = this.Instance.GetLineFarmLocation(minions);
                    break;
                case CastType.Circle:
                    position = this.Instance.GetCircularFarmLocation(minions);
                    break;
                default:
                    var minion = minions.Where(unit => unit.IsValidTarget(this.Range));

                    if (minion.Any())
                    {
                        position.Position = CastingType == CastType.Self
                                                ? ObjectManager.Player.ServerPosition.To2D()
                                                : MEC.GetMec(minion.Select(s => s.ServerPosition.To2D()).ToList()).Center;
                        position.MinionsHit = minion.Count();
                    }

                    break;
            }

            if (position.Position != null && position.MinionsHit > 1)
            {
                this.Instance.Cast(position.Position);
            }
        }

        /// <summary>
        ///     Last hit behavior for the ChampionSpell
        /// </summary>
        public void LastHit()
        {
            if (this.Farm != null)
            {
                this.Farm();
                return;
            }

            var minion =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, 
                    this.Range, 
                    MinionTypes.All, 
                    MinionTeam.Enemy)
                    .First(
                        unit =>
                        unit.IsValidTarget()
                        && HealthPrediction.LaneClearHealthPrediction(
                            unit, 
                            (int)(this.Speed / unit.Distance(ObjectManager.Player, false)), 
                            (int)this.Delay) <= this.GetDamage(unit));

            if (minion != null)
            {
                this.Cast(minion);
            }
        }

        #endregion

        /// <summary>
        ///     Handler creates a simple wrapper that allows easy casting through delegate checks and
        ///     performing actions as necessary in a seperate thread.
        /// </summary>
        public class Handler
        {
            #region Fields

            /// <summary>
            ///     The condition.
            /// </summary>
            public OnSpellCheck Condition;

            /// <summary>
            ///     The process.
            /// </summary>
            public OnGameUpdate Process;

            /// <summary>
            ///     The spell instance.
            /// </summary>
            public Spell SpellInstance;

            /// <summary>
            ///     The type.
            /// </summary>
            public HandleType Type;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Handler" /> class.
            /// </summary>
            /// <param name="SpellInstance">
            ///     The spell instance.
            /// </param>
            /// <param name="Process">
            ///     The process.
            /// </param>
            /// <param name="Condition">
            ///     The condition.
            /// </param>
            /// <param name="Type">
            ///     The type.
            /// </param>
            public Handler(
                Spell SpellInstance, 
                OnGameUpdate Process, 
                OnSpellCheck Condition = null, 
                HandleType Type = HandleType.OnUpdate)
            {
                this.SpellInstance = SpellInstance;
                this.Type = Type;
                this.Process = Process;
                this.Condition = Condition;

                switch (Type)
                {
                    case HandleType.OnUpdate:
                        Game.OnUpdate += (EventArgs args) =>
                            {
                                if (Condition != null && Condition(SpellInstance))
                                {
                                    Process(SpellInstance);
                                }
                            };
                        break;
                }
            }

            #endregion

            #region Delegates

            /// <summary>
            ///     Continual cast method, performed whenever the Conditional is returned true and
            ///     the HandleType method is passed.
            /// </summary>
            /// <param name="spell"></param>
            public delegate void OnGameUpdate(Spell spell);

            /// <summary>
            ///     Conditional check for the handler method
            /// </summary>
            /// <param name="spell"></param>
            /// <returns></returns>
            public delegate bool OnSpellCheck(Spell spell);

            #endregion

            #region Enums

            /// <summary>
            ///     Trigger method for Handler
            /// </summary>
            public enum HandleType
            {
                /// <summary>
                ///     The on update.
                /// </summary>
                OnUpdate
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Default conditional for the method pass through
            /// </summary>
            /// <param name="spell"></param>
            /// <returns></returns>
            public static bool DefaultCondition(Spell spell)
            {
                return spell.IsReady();
            }

            #endregion
        }
    }
}