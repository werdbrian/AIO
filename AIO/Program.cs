// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="LeagueSharp">
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
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;

    using AIO.Common;
    using AIO.Wrapper;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The program.
    /// </summary>
    public class Program
    {
        #region Constants

        /// <summary>
        ///     The version.
        /// </summary>
        private const double Version = 0.010;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The orbwalker.
        /// </summary>
        public static Orbwalking.Orbwalker Orbwalker { get; private set; }

        /// <summary>
        ///     The _ champion.
        /// </summary>
        private static Champion champion;

        /// <summary>
        /// The damage indicator.
        /// </summary>
        public static HpBarIndicator DamageIndicator { get; private set; }

        /// <summary>
        ///     The on load.
        /// </summary>
        private static Action onLoad;

        /// <summary>
        ///     The _ random.
        /// </summary>
        private static Random random;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Random Instance, used to ensure unique random generation by keeping time-seed
        ///     @See Random documentation
        /// </summary>
        public static Random Random
        {
            get
            {
                return random ?? (random = new Random(DateTime.Now.Millisecond));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the champion.
        /// </summary>
        private static Champion Champion
        {
            get
            {
                return champion ?? (champion = RetrieveInstance());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The game_ on game load.
        /// </summary>
        private static void Game_OnGameLoad()
        {
            Configuration.Miscellaneous.AddItem(new MenuItem("Debug", "Debug Mode")).SetValue<bool>(false);

            // CHANGE THIS IF YOU WANT DEBUG ENABLED ON FIRST LOAD
            Logger.Print(string.Format("AIO Version {0} Loaded.", Version));

            if (Champion != null)
            {

                #region Menu OnLoad 
                DamageIndicator = new HpBarIndicator(ObjectManager.Player);
                Orbwalker = new Orbwalking.Orbwalker(Configuration.Main.AddSubMenu(new Menu("Orbwalk", "Orbwalk")));

                Configuration.Draw.AddItem(new MenuItem("Indicator", "HP Damage Indicator"))
                    .SetValue<bool>(true)
                    .ValueChanged += (a, b) =>
                        {
                            if (b.GetNewValue<bool>())
                            {
                                Logger.Print("Indicator has been disabled.");
                            }

                            DamageIndicator.Enabled = b.GetNewValue<bool>();
                        };
                Configuration.Draw.AddItem(new MenuItem("IndicatorColor", "Damage Indicator Color"))
                    .SetValue<Color>(Color.Green)
                    .ValueChanged += (a, b) =>
                        {
                            var color = b.GetNewValue<Color>();
                            DamageIndicator.DrawColor = new SharpDX.Color(color.R, color.G, color.B, color.A);
                        };
                Configuration.Miscellaneous.AddItem(
                    new MenuItem("BuiltIn", "Disable Built-In Orbwalker").SetValue<bool>(false)).ValueChanged +=
                    (a, b) =>
                        {
                            if (b.GetNewValue<bool>())
                            {
                                Logger.Print(
                                    "Built-In orbwalker disabled, feel free to use any external orb walker of your choice");
                            }

                            Orbwalker.SetMovement(!b.GetNewValue<bool>());
                            Orbwalker.SetAttack(!b.GetNewValue<bool>());
                        };
                Configuration.Miscellaneous.AddItem(new MenuItem("CustomPrediction", "Use Custom Prediction", true))
                    .SetValue<bool>(true)
                    .ValueChanged += (a, b) =>
                        {
                            if (b.GetNewValue<bool>())
                            {
                                Logger.Print("Custom prediction enabled, turn on Debug to see prediction circle");
                            }
                        };

                foreach (var spell in Champion.GetList())
                {
                    if (spell.CastingType == ChampionSpell.CastType.LinearCollision
                        || spell.CastingType == ChampionSpell.CastType.Circle)
                    {
                        spell.CreateHandler(ChampionSpell.HandlerType.Immobile);
                        spell.CreateHandler(ChampionSpell.HandlerType.Dash);
                    }

                    spell.CreateHandler(ChampionSpell.HandlerType.Killable);
                    spell.CreateHandler(ChampionSpell.HandlerType.Farm);
                    spell.CreateHandler(ChampionSpell.HandlerType.OnMinimumHit);

                    #region Range Circle

                    var circle = new Render.Circle(ObjectManager.Player, spell.Range, Color.AliceBlue);

                    Configuration.Draw.AddItem(
                        new MenuItem(spell.SpellString, string.Format("Draw {0}", spell.SpellString)))
                        .SetValue<Circle>(new Circle(false, Color.Aqua, spell.Range))
                        .ValueChanged += (a, b) => { circle.Color = b.GetNewValue<Circle>().Color; };

                    circle.VisibleCondition =
                        (a) =>
                        Configuration.Draw.Item(spell.SpellString).GetValue<Circle>().Active
                        && ObjectManager.Player.IsVisible;

                    circle.Add();

                    #endregion

                    #region Damage Indicator 

                    DamageIndicator.AddSpell(spell.Slot);

                    #endregion
                }

                #endregion

                #region Champion#OnLoad call 

                Champion.OnLoad().Invoke();

                #endregion

                #region Event registration 

                // SpeechRecongition.OnRecongized += SpeechRecongition_OnRecongized;
                Game.OnUpdate += Game_OnGameUpdate;

                #endregion

                Configuration.Main.AddToMainMenu();

                Logger.Debug(
                    string.Format(
                        "Game_OnGameLoad passed, Champion instance created for {0}", 
                        ObjectManager.Player.ChampionName));
            }
            else
            {
                Logger.Debug(
                    string.Format(
                        "Game_OnGameLoad passed, Champion {0} could not be instanced and is not supported.", 
                        ObjectManager.Player.ChampionName));
            }
        }

        /// <summary>
        ///     The game_ on game update.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Champion.OnTick() != null)
            {
                Champion.OnTick().Invoke();
                return;
            }

            var target = Selector.GetTarget((float)Champion.LongestRange);

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (target == null)
                    {
                        return;
                    }

                    foreach (var spell in Champion.Spells.Where(spell => spell.Instance.IsReady() && spell.IsEnabled))
                    {
                        spell.Cast(target);
                    }

                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    var laneClearSpell = Champion.AsLaneClear().FirstOrDefault();

                    if (laneClearSpell != null)
                    {
                        laneClearSpell.LaneClear();
                    }

                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    var lastHitSpell = Champion.AsLaneClear().FirstOrDefault();

                    if (lastHitSpell != null)
                    {
                        lastHitSpell.LaneClear();
                    }

                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (target == null)
                    {
                        return;
                    }

                    foreach (
                        var spell in Champion.Spells.Where(spell => spell.Instance.IsReady() && spell.IsEnabled_Harass))
                    {
                        spell.Cast(target);
                    }

                    break;
            }

            if (target != null)
            {
                DamageIndicator.Target = target;
            }
        }

        /// <summary>
        ///     The main.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += (a) =>
                {
                    if (onLoad == null)
                    {
                        onLoad = Game_OnGameLoad;
                        onLoad();
                        Logger.Debug("OnLoad passed, Game_OnGameLoad has been instanced and created.");
                    }
                };
        }

        /// <summary>
        ///     Retrieves the instance of the current champion, internal usage only
        /// </summary>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static Champion RetrieveInstance()
        {
            Logger.Debug("AIO.Champions." + ObjectManager.Player.ChampionName);

            try
            {
                /*                // Below is the proper System Activator usage, however with the LeagueSharp sandbox we cannot use this method, thus we must 
                // use a customized Dynamic activator 

                // var instance = System.Activator.CreateInstance(null, "AIO.Champions." + ObjectManager.Player.ChampionName);
                var type = Type.GetType("AIO.Champions." + ObjectManager.Player.ChampionName);

                if (type != null)
                {
                    var instance = DynamicInitializer.CreateInstance(type);

                    return (Champion)instance;
                }*/
                var instance = Activator.CreateInstance(null, "AIO.Champions." + ObjectManager.Player.ChampionName);

                if (instance != null)
                {
                    return (Champion)instance.Unwrap();
                }
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

            return null;
        }

        /// <summary>
        /// The speech recongition_ on recongized.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        private static void SpeechRecongition_OnRecongized(string output)
        {
            if (SpeechRecongition.CHAMPION_SPELLS.Contains(output))
            {
                Logger.Debug(string.Format("Speech {0} recongized as a Champion Spell...", output));

                var Spell = Champion.GetList().FirstOrDefault(s => s.SpellString.Equals(output.Replace("Cast ", string.Empty)));

                if (Spell != null && Spell.IsEnabled && Spell.Instance.IsReady())
                {
                    Logger.Debug(
                        string.Format(
                            "Recongized spell {0} from speech... Found spell & IsEnabled & IsReady", 
                            Spell.SpellString));

                    var Target = Selector.GetTarget((float)Champion.LongestRange);

                    if (Target != null)
                    {
                        Spell.Cast(Target);
                    }
                }
                else if (Spell != null)
                {
                    Logger.Debug(
                        string.Format(
                            "Recongized spell {0} from speech... Is not ready or enabled, thus ignored.", 
                            Spell.SpellString));
                }
                else
                {
                    Logger.Debug("Spell was not recongized from output...");
                }
            }
        }

        #endregion
    }
}