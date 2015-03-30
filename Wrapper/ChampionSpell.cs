using AIO.Library;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Wrapper
{
    /// <summary>
    ///     Represents an in game Champion spell, this wrapper automates cast, last hit, and lane clear behavior.
    /// </summary>
    public class ChampionSpell
    {

        /// <summary>
        ///     Handler creates a simple wrapper that allows easy casting through delegate checks and 
        ///     performing actions as necessary in a seperate thread.
        /// </summary>
        public class Handler
        {
            /// <summary>
            ///     Trigger method for Handler
            /// </summary>
            public enum HandleType
            {
                OnUpdate
            }

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

            public HandleType Type;
            public Spell SpellInstance;
            public OnGameUpdate Process;
            public OnSpellCheck Condition;

            public Handler(Spell SpellInstance, OnGameUpdate Process, OnSpellCheck Condition = null, HandleType Type = HandleType.OnUpdate)
            {
                this.SpellInstance = SpellInstance;
                this.Type = Type;
                this.Process = Process;
                this.Condition = Condition;

                switch (Type)
                {
                    case HandleType.OnUpdate:
                        Game.OnGameUpdate += (EventArgs args) =>
                        {
                            if (Condition != null && Condition(SpellInstance))
                            {
                                Process(SpellInstance);
                            }
                        };
                        break;
                }
            }

            /// <summary>
            ///     Default conditional for the method pass through 
            /// </summary>
            /// <param name="spell"></param>
            /// <returns></returns>
            public static bool DefaultCondition(Spell spell)
            {
                return spell.IsReady();
            }
        }

        /// <summary>
        ///     Handler type, created in CreateHandler 
        /// </summary>
        public enum HandlerType
        {
            /// <summary>
            ///     Automated handle if a nearby unit is immobile
            /// </summary>
            IMMOBILE,
            /// <summary>
            ///     Automated handle if a nearby unit is dashing
            /// </summary>
            DASH,
            /// <summary>
            ///     Automated handle if a nearby unit is killable by the specified ChampionSpell
            /// </summary>
            KILLABLE,
            /// <summary>
            ///     Automated handle if the minimum amount of specified enemies can be hit with the ChampionSpell 
            /// </summary>
            ON_MINIMUM_HIT,
            /// <summary>
            ///     Automated handle for farming
            /// </summary>
            FARM,
            /// <summary>
            ///     Automated handle for healing (self) 
            /// </summary>
            ON_SELF_HEALTH_BELOW
        }

        /// <summary>
        ///     Cast behavior of the specified Champion Spell 
        /// </summary>
        public enum CastType
        {
            /// <summary>
            ///     Linear skillshot
            /// </summary>
            LINEAR,
            /// <summary>
            ///     Linear skillshot with collision against Obj_AI_Base
            /// </summary>
            LINEAR_COLLISION,
            /// <summary>
            ///     Circle skillshot (can also be used for place spells, ie. Malzahar Q) 
            /// </summary>
            CIRCLE,
            /// <summary>
            ///     Cone skillshot 
            /// </summary>
            CONE,
            /// <summary>
            ///     Targeted spell, can only be cast on a Obj_AI_Base
            /// </summary>
            TARGET,
            /// <summary>
            ///     Self spell, commonly a powerup spell 
            /// </summary>
            SELF,
            /// <summary>
            ///     AA-Powered spell, cast then attack move is issued to the desired Obj_AI_Base
            /// </summary>
            AA_ATTACK,
            /// <summary>
            ///     Friendly shield cast, for self only use SELF 
            /// </summary>
            SHIELD
        }

        #region Class variables
        public Spell Instance;
        public SpellSlot Slot;
        public CastType CastingType;
        public int Range;
        public int Speed;
        public float Delay;
        public int Width;

        #endregion 

        /// <summary>
        ///     Determines if the spell performs damage 
        /// </summary>
        public bool CanDamage { get; set; }

        /// <summary>
        ///     Lane clear delegate
        /// </summary>
        public delegate void ClearDelegate();

        /// <summary>
        ///     Last hit delegate 
        /// </summary>
        public delegate void FarmDelegate();
        public ClearDelegate Clear;
        public FarmDelegate Farm;

        /// <summary>
        ///     Custom cast conditional, checked on call of Cast
        /// </summary>
        public Func<Obj_AI_Base, bool> CastCondition;

        /// <summary>
        ///     Cast method, can be bypassed through Cast parameter 
        /// </summary>
        public Action<Obj_AI_Base> CastFunction;

        /// <summary>
        ///     Damage type of the spell, used in target selection 
        /// </summary>
        //public SimpleTs.DamageType TSDamageType = SimpleTs.DamageType.Physical;

        /// <summary>
        ///     References the specified sub menu created during instantiation of the ChampionSpell 
        /// </summary>
        public Menu SpellMenu
        {
            get
            {
                return Configuration.Spell.SubMenu(SpellString);
            }
        }

        /// <summary>
        ///     References the specified sub menu created for additional handlers
        /// </summary>
        public Menu HandlerMenu
        {
            get
            {
                return SpellMenu.SubMenu("Handler");
            }
        }

        /// <summary>
        ///     Determines if the ChampionSpell is enabled in the spell menu for general casting
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return SpellMenu.Item("Enabled").GetValue<bool>();
            }
            set
            {
                SpellMenu.Item("Enabled").SetValue<bool>(value);
            }
        }

        /// <summary>
        ///     Determines if the ChampionSpell is enabled in the spell menu for lane clear
        /// </summary>
        public bool IsEnabled_LaneClear
        {
            get
            {
                return SpellMenu.Item("LaneClear").GetValue<bool>();
            }
            set
            {
                SpellMenu.Item("LaneClear").SetValue<bool>(value);
            }
        }

        /// <summary>
        ///     Determines if the ChampionSpell is enabled in the spell menu for last hit
        /// </summary>
        public bool IsEnabled_LastHit
        {
            get
            {
                return SpellMenu.Item("LastHit").GetValue<bool>();
            }
            set
            {
                SpellMenu.Item("LastHit").SetValue<bool>(value);
            }
        }

        /// <summary>
        ///     String instance of the specified ChampionSpell 
        /// </summary>
        public string SpellString
        {
            get
            {
                switch (Slot)
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
                return "";
            }
        }

        /// <summary>
        ///     Prediction based FROM location
        /// </summary>
        public Vector3 From
        {
            get
            {
                return Instance.From;
            }
            set
            {
                Instance.UpdateSourcePosition(value);
            }
        }

        /// <summary>
        ///     Converts the local ChampionSpell CastType into LeagueSharp's SkillshotType 
        /// </summary>
        public SkillshotType SkillShotType
        {
            get
            {
                switch (CastingType)
                {
                    case CastType.LINEAR:
                    case CastType.LINEAR_COLLISION:
                        return SkillshotType.SkillshotLine;
                    case CastType.CONE:
                        return SkillshotType.SkillshotCone;
                    case CastType.CIRCLE:
                        return SkillshotType.SkillshotCircle;
                    default:
                        return SkillshotType.SkillshotLine;
                }
            }
        }

        public ChampionSpell(SpellSlot Slot, int Range, CastType CastingType, int Speed = int.MaxValue, float Delay = 0.250f, int Width = 50, bool AddToMenu = true)
        {
            this.Slot = Slot;
            this.Range = Range;
            this.CastingType = CastingType;
            this.Speed = Speed;
            this.Delay = Delay;
            this.Width = Width;

            CanDamage = true;
            Instance = new Spell(Slot, Range);

            if (CastingType == CastType.LINEAR || CastingType == CastType.LINEAR_COLLISION || CastingType == CastType.CIRCLE || CastingType == CastType.CONE)
            {
                Instance.SetSkillshot(Delay, Width, Speed, (CastingType == CastType.LINEAR_COLLISION), SkillShotType);
            }
            else if (CastingType == CastType.TARGET)
            {
                Instance.SetTargetted(Delay, Speed);
            }


            if (AddToMenu)
            {
                this.AddToMenu();
            }
        }

        /// <summary>
        ///     Create and adds the spell instance menu variables into the Configuration menu 
        /// </summary>
        public void AddToMenu()
        {
            Configuration.Spell.AddSubMenu(new Menu(SpellString, SpellString));

            SpellMenu.AddSubMenu(new Menu("Automatic Handlers", "Handler"));

            SpellMenu.AddItem(new MenuItem("DisableAll", "Disable all Automatic Handlers").SetValue<bool>(false));
            SpellMenu.AddItem(new MenuItem("Enabled", "Enabled").SetValue<bool>(Slot != SpellSlot.R ? true : false));
            SpellMenu.AddItem(new MenuItem("LaneClear", "Enable LaneClear")).SetValue<bool>(false);
            SpellMenu.AddItem(new MenuItem("LastHit", "Enable LastHit")).SetValue<bool>(false);

            SpellMenu.Item("DisableAll").ValueChanged += (object sender, OnValueChangeEventArgs args) =>
            {
                if (args.GetNewValue<bool>())
                {
                    foreach (var item in HandlerMenu.Items)
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
        ///     Determines if the ChampionSpell's instance is ready to cast & cast conditional is ready 
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool IsReady(Obj_AI_Base unit)
        {
            if (!Instance.IsReady())
                return false;

            if (CastCondition != null && !CastCondition(unit))
                return false;

            return ObjectManager.Player.Mana >= Instance.Instance.ManaCost; 
        }

        /// <summary>
        ///     Cast behavior handle for casting the ChampionSpell against Obj_AI_Base (single target OR AoE) 
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="IgnoreCustomCasting"></param>
        public void Cast(Obj_AI_Base unit, bool IgnoreCustomCasting = false)
        {
            if (!unit.IsValidTarget(Range) || !Instance.IsReady())
                return;

            if (CastCondition != null && !CastCondition(unit))
                return;

            if (CastFunction != null && !IgnoreCustomCasting)
            {
                CastFunction(unit);
                return;
            }

            switch (CastingType)
            {
                case CastType.SELF:
                    Instance.Cast();
                    break;
                case CastType.AA_ATTACK:
                    /*var encoded = Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(unit.ServerPosition.X, unit.ServerPosition.Y, 3, unit.NetworkId));

                    if (Orbwalking.CanAttack())
                    {
                        encoded.Send();
                    }*/

                    if (Orbwalking.CanAttack())
                    {
                        Instance.Cast();
                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, unit); 
                    }
                    break;
                case CastType.TARGET:
                    Instance.CastOnUnit(unit);
                    break;
                case CastType.LINEAR:
                case CastType.LINEAR_COLLISION:
                case CastType.CIRCLE:
                case CastType.CONE:

                    var prediction = Instance.GetPrediction(unit, (CastingType != CastType.LINEAR_COLLISION));

                    if (prediction != null)
                    {
                        if (prediction.Hitchance > HitChance.Low)
                        {
                            Instance.Cast(prediction.CastPosition);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        ///     Lane clear behavior for the specified ChampionSpell 
        /// </summary>
        public void LaneClear()
        {
            if (Clear != null)
            {
                Clear();
                return;
            }

            List<Obj_AI_Base> minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Range);
            MinionManager.FarmLocation position = new MinionManager.FarmLocation();

            switch (CastingType)
            {
                case CastType.LINEAR:
                case CastType.CONE:
                    position = Instance.GetLineFarmLocation(minions);
                    break;
                case CastType.CIRCLE:
                    position = Instance.GetCircularFarmLocation(minions);
                    break;
                default:
                    var minion = minions.First(unit => unit.IsValidTarget() && HealthPrediction.LaneClearHealthPrediction(unit, (int)(Speed / unit.Distance(ObjectManager.Player, false)), (int)Delay) <= GetDamage(unit));

                    if (minion != null)
                    {
                        position.Position = minion.ServerPosition.To2D();
                        position.MinionsHit = 2;
                    }
                    break;
            }

            if (position.Position != null && position.MinionsHit > 1)
            {
                Instance.Cast(position.Position);
            }
        }

        /// <summary>
        ///     Last hit behavior for the ChampionSpell 
        /// </summary>
        public void LastHit()
        {
            if (Farm != null)
            {
                Farm();
                return;
            }

            Obj_AI_Base minion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Range, MinionTypes.All, MinionTeam.Enemy).First(unit => unit.IsValidTarget() && HealthPrediction.LaneClearHealthPrediction(unit, (int)(Speed / unit.Distance(ObjectManager.Player, false)), (int)Delay) <= GetDamage(unit));

            if (minion != null)
            {
                Cast(minion);
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
                case HandlerType.IMMOBILE:
                    HandlerMenu.AddItem(new MenuItem("Immobile", "[Auto] Enable use on Immobilized champions").SetValue<bool>(false));

                    return new Handler(Instance, (Spell spell) =>
                    {
                        foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(unit => unit.IsValidTarget(Range)))
                        {
                            spell.CastIfHitchanceEquals(unit, HitChance.Immobile);
                            break;
                        }
                    }, (Spell spell) => Instance.IsReady() && HandlerMenu.Item("Immobile").GetValue<bool>());
                case HandlerType.DASH:
                    HandlerMenu.AddItem(new MenuItem("Dashing", "[Auto] Enable use on Dashing champions").SetValue<bool>(false));

                    return new Handler(Instance, (Spell spell) =>
                    {
                        foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(unit => unit.IsValidTarget(Range)))
                        {
                            spell.CastIfHitchanceEquals(unit, HitChance.Dashing);
                            break;
                        }
                    }, (Spell spell) => Instance.IsReady() && HandlerMenu.Item("Dashing").GetValue<bool>());
                case HandlerType.KILLABLE:
                    HandlerMenu.AddItem(new MenuItem("Killsteal", "[Auto] Enable use on kill-able champions")).SetValue<bool>(false);

                    return new Handler(Instance, (Spell spell) =>
                    {
                        foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(unit => unit.IsValidTarget(Range) && HealthPrediction.GetHealthPrediction(unit, (int)(Speed / unit.Distance(ObjectManager.Player, false)), (int)Delay) <= GetDamage(unit)))
                        {
                            var prediction = spell.GetPrediction(unit);

                            if (prediction != null && prediction.Hitchance >= HitChance.Medium)
                            {
                                spell.Cast(prediction.CastPosition);
                            }
                        }
                    }, (Spell spell) => Instance.IsReady() && HandlerMenu.Item("Killsteal").GetValue<bool>());
                case HandlerType.ON_MINIMUM_HIT:
                    HandlerMenu.AddItem(new MenuItem("MinimumHit", "[Auto] Enable casting on minimum hit")).SetValue<bool>(false);
                    HandlerMenu.AddItem(new MenuItem("MinimumHitAmount", "-> Amount needed for Minimum Hit")).SetValue<Slider>(new Slider(1, 0, 5));

                    return new Handler(Instance, (Spell spell) =>
                    {
                        foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(unit => unit.IsValidTarget(Range)))
                        {
                            spell.CastIfWillHit(unit, HandlerMenu.Item("MinimumHitAmount").GetValue<Slider>().Value - 1);
                            break;
                        }
                    }, (Spell spell) => Instance.IsReady() && HandlerMenu.Item("MinimumHit").GetValue<bool>());
                case HandlerType.FARM:
                    HandlerMenu.AddItem(new MenuItem("Farm", "[Auto] Enable AFK farming")).SetValue<bool>(false);

                    return new Handler(Instance, (Spell spell) =>
                    {
                        foreach (var unit in ObjectManager.Get<Obj_AI_Minion>().Where(unit => unit.IsValidTarget(Range) && HealthPrediction.GetHealthPrediction(unit, (int)(Speed / unit.Distance(ObjectManager.Player, false)), (int)Delay) <= GetDamage(unit)))
                        {
                            if (CastingType == CastType.LINEAR || CastingType == CastType.CONE || CastingType == CastType.CIRCLE)
                            {
                                LaneClear();
                            }
                            else
                            {
                                Cast(unit);
                            }

                            break;
                        }
                    }, (Spell spell) => Instance.IsReady() && HandlerMenu.Item("Farm").GetValue<bool>());
                case HandlerType.ON_SELF_HEALTH_BELOW:
                    HandlerMenu.AddItem(new MenuItem("SelfHeal", "[Auto] Enable (self) Heal")).SetValue<bool>(false);
                    HandlerMenu.AddItem(new MenuItem("HealPercent", "-> Health Percent")).SetValue<Slider>(new Slider(40, 0, 100));
                    HandlerMenu.AddItem(new MenuItem("ManaPercent", "-> Min Mana Percent")).SetValue<Slider>(new Slider(30, 0, 100));

                    return new Handler(Instance, (Spell spell) =>
                    {
                        var HealthPercent = HandlerMenu.Item("HealPercent").GetValue<Slider>().Value;
                        var ManaPercent = HandlerMenu.Item("ManaPercent").GetValue<Slider>().Value;

                        if (HealthPercent < ObjectManager.Player.HealthPercentage() && ManaPercent < ObjectManager.Player.ManaPercentage())
                        {
                            spell.Cast(); 
                        }
                    }, (Spell spell) => Instance.IsReady() && HandlerMenu.Item("SelfHeal").GetValue<bool>()); 
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
            return CanDamage ? ObjectManager.Player.GetSpellDamage(unit, Slot) : 0;
        }

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

            return new ChampionSpell(Slot, retrieved.Range, Type, retrieved.MissileSpeed, retrieved.Delay == 0 ? 0 : ((float)retrieved.Delay) / 1000, retrieved.Radius);
        }
    }
}
