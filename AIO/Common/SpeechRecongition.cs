// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpeechRecongition.cs" company="LeagueSharp">
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
//   The speech recongition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Permissions;
    using System.Speech.Recognition;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The speech recongition.
    /// </summary>
    public static class SpeechRecongition
    {
        #region Static Fields

        /// <summary>
        ///     A compiled list of champion spell slots
        /// </summary>
        public static readonly string[] CHAMPION_SPELLS = new[] { "Cast Q", "Cast W", "Cast E", "Cast R" };

        /// <summary>
        ///     A compiled list of all champion names
        /// </summary>
        private static readonly string[] ChampionNames = new[]
                                                             {
                                                                 "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", 
                                                                 "Diana", "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", 
                                                                 "Karthus", "Kassadin", "Katarina", "Kayle", "Kennen", 
                                                                 "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", 
                                                                 "Morgana", "Nidalee", "Orianna", "Ryze", "Sion", "Swain", 
                                                                 "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", 
                                                                 "Vladimir", "Xerath", "Ziggs", "Zyra", "Velkoz", 
                                                                 "Blitzcrank", "Janna", "Karma", "Leona", "Lulu", "Nami", 
                                                                 "Sona", "Soraka", "Thresh", "Zilean", "Amumu", "Chogath", 
                                                                 "DrMundo", "Galio", "Hecarim", "Malphite", "Maokai", 
                                                                 "Nasus", "Rammus", "Sejuani", "Shen", "Singed", "Skarner", 
                                                                 "Volibear", "Warwick", "Yorick", "Zac", "Nunu", "Taric", 
                                                                 "Alistar", "Garen", "Nautilus", "Braum", "Ashe", 
                                                                 "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", 
                                                                 "KogMaw", "MissFortune", "Quinn", "Sivir", "Talon", 
                                                                 "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Zed", 
                                                                 "Jinx", "Yasuo", "Lucian", "Darius", "Elise", "Evelynn", 
                                                                 "Fiora", "Gangplank", "Gnar", "Jayce", "Pantheon", 
                                                                 "Irelia", "JarvanIV", "Jax", "Khazix", "LeeSin", 
                                                                 "Nocturne", "Olaf", "Poppy", "Renekton", "Rengar", 
                                                                 "Riven", "Shyvana", "Trundle", "Tryndamere", "Udyr", "Vi", 
                                                                 "MonkeyKing", "XinZhao", "Aatrox", "Rumble", "Shaco", 
                                                                 "MasterYi", "Kalista", "Reksai", "Bard"
                                                             };

        /// <summary>
        ///     The _ speech configuration.
        /// </summary>
        private static Menu _SpeechConfiguration;

        /// <summary>
        ///     The custom list.
        /// </summary>
        private static List<string> CustomList = new List<string>();

        /// <summary>
        ///     The engine.
        /// </summary>
        private static SpeechRecognitionEngine Engine;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="SpeechRecongition" /> class.
        /// </summary>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        static SpeechRecongition()
        {
            SpeechConfiguration.AddItem(new MenuItem("Press", "Press to Talk"))
                .SetValue<KeyBind>(new KeyBind("N".ToCharArray()[0], KeyBindType.Press, false));
            SpeechConfiguration.AddItem(new MenuItem("Target", "Speech Target Selection")).SetValue<bool>(true);
            SpeechConfiguration.AddItem(new MenuItem("Mode", "Enabled Mode"))
                .SetValue<StringList>(new StringList(new[] { "Push to Talk", "Always On", "Disabled" }, 2));

            CustomList.AddRange(ChampionNames);
            CustomList.AddRange(CHAMPION_SPELLS);

            var List = new Choices();
            List.Add(CustomList.ToArray());

            Engine = new SpeechRecognitionEngine();
            Engine.SetInputToDefaultAudioDevice();
            Engine.LoadGrammar(new Grammar(new GrammarBuilder(List)));
            Engine.SpeechRecognized += Engine_SpeechRecognized;
            Engine.RecognizeAsync(RecognizeMode.Multiple);

            // Engine.Recognize(); 
            Game.OnUpdate += Game_OnGameUpdate;
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Recongized delegate
        /// </summary>
        /// <param name="output"></param>
        public delegate void OnRecongizedEvent(string output);

        #endregion

        #region Public Events

        /// <summary>
        ///     Recongize event
        /// </summary>
        public static event OnRecongizedEvent OnRecongized;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Maintains a sane menu Configuration
        /// </summary>
        public static Menu SpeechConfiguration
        {
            get
            {
                return _SpeechConfiguration
                       ?? (_SpeechConfiguration = Configuration.Miscellaneous.AddSubMenu(new Menu("Speech", "Speech")));
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Overrides the preset grammar with new input
        /// </summary>
        /// <param name="input"></param>
        public static void OverrideGrammar(params string[] input)
        {
            var List = new Choices();

            CustomList.AddRange(input);
            List.Add(CustomList.ToArray());

            Engine.UnloadAllGrammars();
            Engine.LoadGrammar(new Grammar(new GrammarBuilder(List)));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The engine_ speech recognized.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private static void Engine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var index = SpeechConfiguration.Item("Mode").GetValue<StringList>().SelectedIndex;

            if (index == 3 || (index == 2 && !SpeechConfiguration.Item("Press").GetValue<KeyBind>().Active))
            {
                return;
            }

            if (SpeechConfiguration.Item("Target").GetValue<bool>() && ChampionNames.Contains(e.Result.Text))
            {
                FireEvent(e.Result.Text);
            }

            if (CustomList.Contains(e.Result.Text))
            {
                FireEvent(e.Result.Text);
            }

            if (CHAMPION_SPELLS.Contains(e.Result.Text))
            {
                FireEvent(e.Result.Text);
            }
        }

        /// <summary>
        ///     Fires the event with the specified output
        /// </summary>
        /// <param name="output"></param>
        private static void FireEvent(string output)
        {
            if (OnRecongized != null)
            {
                OnRecongized(output);
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
            try
            {
                Engine.RequestRecognizerUpdate();
            }
            catch (Exception e)
            {
                Logger.Print(e.Message);
            }
        }

        #endregion
    }
}