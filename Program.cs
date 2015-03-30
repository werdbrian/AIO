using AIO.Champions;
using AIO.Common;
using AIO.Wrapper;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using Timer = System.Timers.Timer;
using Render = LeagueSharp.Common.Render;
using AIO.Common.Rendering; 

/**
 * uAIO;
 * 
 * == Change Log == 
 * 
 * 12/16/2014
 * - Added API documentation 
 * - Updated spell library 
 * - Updated internal API
 * - Added Dr Mundo, Malzahar, Shyvana, Skarner, Gangplank 
 * - Version updated to 0.003 
 * 
 * 12/24/2014
 * - Added a special Christmas surprise 
 * - Version 0.003 maintained
 * 
 * 12/26/2014
 * - Updated for 4.21 (Beta & Common) 
 *      - Removed SimpleTs and replaced with TargetSelector, should now be much more accurate 
 *      - Fixed Spell references
 *      - Updated internal API to match LeagueSharp#Common and LeagueSharp core
 * - Version updated to 0.004 
 * 
 * 01/06/2015
 * - Removed Utility (moved methods to Program) 
 * - Removed CustomRender (moved methods to own classes) 
 * - Replaced DrawCircle for Render.Circle 
 * 
 * 01/12/2015
 * - Added Kassadin
 * - Added mana check for IsReady 
 * - Version updated to 0.005 
 * 
 * 02/21/2015
 * - Added Rengar, Maokai
 * - Back in work, again... Version updated to 0.006
 * - Removed Winter snow effect 
 **/
namespace AIO
{
    public class Program
    {
        private const double VERSION = 0.006;

        private static Random _Random;
        private static HpDamageIndicator DamageIndicator;
        private static Champion _Champion;
        private static Action OnLoad;

        public static Orbwalking.Orbwalker Orbwalker;

        public static Champion Champion
        {
            get
            {
                return (_Champion ?? (_Champion = RetrieveInstance()));
            }
        }

        private static void Main(string[] args)
        {
            /*if ((Game.ClockTime / 60) < 2)
            {
                var winter = new Timer(1000 / 60); // 60 FPS ~ Should be fine on most computers, even old wooden ones 
                var SnowEffect = new SnowEffect(Program.Random.Next(15, 40), 500, 5);

                winter.Start();
                winter.Elapsed += (a, b) =>
                {
                    if ((Game.ClockTime / 60) < 2) 
                        SnowEffect.Step();
                    else
                    {
                        SnowEffect.RemoveAll(); 
                        winter.Stop();
                    }
                };
            }*/

            CustomEvents.Game.OnGameLoad += (a) =>
            {
                if (OnLoad == null)
                {
                    OnLoad = new Action(Game_OnGameLoad);
                    OnLoad();

                    Logger.Debug("OnLoad passed, Game_OnGameLoad has been instanced and created.");
                }
            };
        }

        private static void Game_OnGameLoad()
        {
            #region Ensure Debug-Menu Sanity
            Configuration.Miscellaneous.AddItem(new MenuItem("Debug", "Debug Logs")).SetValue<bool>(false); // CHANGE THIS IF YOU WANT DEBUG ENABLED ON FIRST LOAD
            #endregion 

            Logger.Print(string.Format("AIO Version {0} Loaded.", VERSION));

            if (Champion != null)
            {
                #region Menu OnLoad 
                Orbwalker = new Orbwalking.Orbwalker(Configuration.Main.AddSubMenu(new Menu("Orbwalk", "Orbwalk")));

                Configuration.Draw.AddItem(new MenuItem("Indicator", "HP Damage Indicator")).SetValue<bool>(true);
                Configuration.Draw.AddItem(new MenuItem("IndicatorColor", "Damage Indicator Color")).SetValue<Color>(Color.Green);
                Configuration.Miscellaneous.AddItem(new MenuItem("BuiltIn", "Disable Built-In Orbwalker").SetValue<bool>(false)).ValueChanged += (a, b) =>
                {
                    if (b.GetNewValue<bool>())
                        Logger.Print("Built-In orbwalker disabled, feel free to use any external orb walker of your choice");

                    Orbwalker.SetMovement(!b.GetNewValue<bool>());
                    Orbwalker.SetAttack(!b.GetNewValue<bool>());
                };

                foreach (var spell in Champion.GetList())
                {
                    if (spell.CastingType == ChampionSpell.CastType.LINEAR_COLLISION || spell.CastingType == ChampionSpell.CastType.CIRCLE)
                    {
                        spell.CreateHandler(ChampionSpell.HandlerType.IMMOBILE);
                        spell.CreateHandler(ChampionSpell.HandlerType.DASH);
                    }

                    spell.CreateHandler(ChampionSpell.HandlerType.KILLABLE);
                    spell.CreateHandler(ChampionSpell.HandlerType.FARM);
                    spell.CreateHandler(ChampionSpell.HandlerType.ON_MINIMUM_HIT);

                    #region Range Circle
                    var circle = new Render.Circle(ObjectManager.Player, spell.Range, Color.AliceBlue);

                    Configuration.Draw.AddItem(new MenuItem(spell.SpellString, string.Format("Draw {0}", spell.SpellString))).SetValue<Circle>(new Circle(false, Color.Aqua, spell.Range)).ValueChanged += (a, b) =>
                    {
                        circle.Color = b.GetNewValue<Circle>().Color; 
                    };
                    
                    circle.VisibleCondition = (a) =>
                    {
                        return Configuration.Draw.Item(spell.SpellString).GetValue<Circle>().Active && ObjectManager.Player.IsVisible;
                    };

                    circle.Add();
                    #endregion 
                }
                
                #endregion

                DamageIndicator = new HpDamageIndicator();

                SpeechRecongition.OnRecongized += SpeechRecongition_OnRecongized;
                Configuration.Main.AddToMainMenu();
                Game.OnUpdate += Game_OnGameUpdate;
                Drawing.OnEndScene += Drawing_OnEndScene;

                Logger.Debug(string.Format("Game_OnGameLoad passed, Champion instance created for {0}", ObjectManager.Player.ChampionName));
            }
            else
                Logger.Debug(string.Format("Game_OnGameLoad passed, Champion {0} could not be instanced and is not supported.", ObjectManager.Player.ChampionName));
        }

        private static void SpeechRecongition_OnRecongized(string output)
        {
            if (SpeechRecongition.CHAMPION_SPELLS.Contains(output))
            {
                Logger.Debug(string.Format("Speech {0} recongized as a Champion Spell...", output));

                var Spell = Champion.GetList().FirstOrDefault(s => s.SpellString.Equals(output.Replace("Cast ", "")));

                if (Spell != null && Spell.IsEnabled && Spell.Instance.IsReady())
                {
                    Logger.Debug(string.Format("Recongized spell {0} from speech... Found spell & IsEnabled & IsReady", Spell.SpellString));

                    var Target = Selector.GetTarget((float)Champion.LongestRange); 

                    if (Target != null)
                    {
                        Spell.Cast(Target);
                    }
                }
                else if (Spell != null)
                    Logger.Debug(string.Format("Recongized spell {0} from speech... Is not ready or enabled, thus ignored.", Spell.SpellString));
                else
                    Logger.Debug("Spell was not recongized from output...");
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                /*foreach (var spell in Champion.GetList()) // No longer needed... 
                {
                    var circle = Configuration.Draw.Item(spell.SpellString).GetValue<Circle>();

                    if (circle.Active)
                        Utility.DrawCircle(ObjectManager.Player.Position, circle.Radius, circle.Color); 
                }*/ 

                if (Configuration.Draw.Item("Indicator").GetValue<bool>())
                {
                    var color = Configuration.Draw.Item("IndicatorColor").GetValue<Color>(); 

                    foreach (var unit in ObjectManager.Get<Obj_AI_Hero>().Where(u => !u.IsDead && u.IsEnemy && u.IsVisible)) 
                    {
                        DamageIndicator.DrawDamage(unit, (float)ObjectManager.Player.GetComboDamage(unit, Champion.GetList().Where(s => s.Instance.IsReady()).Select(s => s.Slot)), color);
                    }
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    var Target = Selector.GetTarget((float)Champion.LongestRange);

                    if (Target != null)
                    {
                        var Spell = Champion.AsOrderedCombo().First(spell => spell.IsReady(Target));

                        if (Spell != null)
                        {
                            Spell.Cast(Target);
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    var LaneClearSpell = Champion.AsLaneClear().First();

                    if (LaneClearSpell != null)
                    {
                        LaneClearSpell.LaneClear();
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    var LastHitSpell = Champion.AsLaneClear().First();

                    if (LastHitSpell != null)
                    {
                        LastHitSpell.LaneClear();
                    }
                    break;
            }
        }

        /// <summary>
        ///     Retrieves the instance of the current champion, internal usage only 
        /// </summary>
        /// <returns></returns>
        private static Champion RetrieveInstance()
        {
            Logger.Debug("AIO.Champions." + ObjectManager.Player.ChampionName);
            try
            {
                var instance = System.Activator.CreateInstance(null, "AIO.Champions." + ObjectManager.Player.ChampionName);
                return (Champion) instance.Unwrap();
            }
            catch (TargetInvocationException e)
            {
                Logger.Print(e.InnerException.Message, Logger.LogType.Danger);
                return null;
            }
            catch (Exception e)
            {
                Logger.Print(e.Message, Logger.LogType.Danger);
                return null; 
            }
        }

        /// <summary>
        ///     Random Instance, used to ensure unique random generation by keeping time-seed 
        ///     @See Random documentation 
        /// </summary>
        public static Random Random
        {
            get
            {
                return (_Random ?? (_Random = new Random(DateTime.Now.Millisecond)));
            }
        }
    }
}
