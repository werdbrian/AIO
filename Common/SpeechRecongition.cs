using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace AIO.Common
{
    // TODO: Setting modification through speech
    // TODO: Spell cast through speech 
    /// <summary>
    ///     A speech recongition class
    /// </summary>
    public static class SpeechRecongition
    {

        /// <summary>
        ///     A compiled list of all champion names
        /// </summary>
        public static readonly string[] CHAMPION_NAMES = new string[] {
            "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana",
            "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus", "Kassadin", "Katarina", "Kayle", "Kennen",
            "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna", "Ryze", "Sion",
            "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra",
            "Velkoz",  "Blitzcrank", "Janna", "Karma", "Leona", "Lulu", "Nami", "Sona",
            "Soraka", "Thresh", "Zilean", "Amumu", "Chogath", "DrMundo", "Galio", "Hecarim", "Malphite",
            "Maokai", "Nasus", "Rammus", "Sejuani", "Shen", "Singed", "Skarner", "Volibear", "Warwick", "Yorick", "Zac",
            "Nunu", "Taric", "Alistar", "Garen", "Nautilus", "Braum",  "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "KogMaw",
            "MissFortune", "Quinn", "Sivir", "Talon", "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Zed", "Jinx",
            "Yasuo", "Lucian", "Darius", "Elise", "Evelynn", "Fiora", "Gangplank", "Gnar", "Jayce",
            "Pantheon", "Irelia", "JarvanIV", "Jax", "Khazix", "LeeSin", "Nocturne", "Olaf", "Poppy", "Renekton",
            "Rengar", "Riven", "Shyvana", "Trundle", "Tryndamere", "Udyr", "Vi", "MonkeyKing", "XinZhao", "Aatrox",
            "Rumble", "Shaco", "MasterYi", "Kalista", "Reksai", "Bard"
        };

        /// <summary>
        ///     A compiled list of champion spell slots
        /// </summary>
        public static readonly string[] CHAMPION_SPELLS = new string[] { "Cast Q", "Cast W", "Cast E", "Cast R" };

        private static Menu _SpeechConfiguration;

        /// <summary>
        ///     Maintains a sane menu Configuration 
        /// </summary>
        public static Menu SpeechConfiguration
        {
            get
            {
                return (_SpeechConfiguration ?? (_SpeechConfiguration = Configuration.Miscellaneous.AddSubMenu(new Menu("Speech", "Speech"))));
            }
        }

        /// <summary>
        ///     Recongized delegate
        /// </summary>
        /// <param name="output"></param>
        public delegate void OnRecongizedEvent(string output);

        /// <summary>
        ///     Recongize event 
        /// </summary>
        public static event OnRecongizedEvent OnRecongized;

        private static List<string> CustomList = new List<string>();
        private static SpeechRecognitionEngine Engine;

        static SpeechRecongition()
        {
            #region Configuration

            SpeechConfiguration.AddItem(new MenuItem("Press", "Press to Talk")).SetValue<KeyBind>(new KeyBind("N".ToCharArray()[0], KeyBindType.Press, false));
            SpeechConfiguration.AddItem(new MenuItem("Target", "Speech Target Selection")).SetValue<bool>(true);
            SpeechConfiguration.AddItem(new MenuItem("Mode", "Enabled Mode")).SetValue<StringList>(new StringList(new string[] { "Push to Talk", "Always On", "Disabled"  }, 2));
            
            #endregion 

            CustomList.AddRange(CHAMPION_NAMES);
            CustomList.AddRange(CHAMPION_SPELLS);

            var List = new Choices();
            List.Add(CustomList.ToArray());

            Engine = new SpeechRecognitionEngine();
            Engine.SetInputToDefaultAudioDevice();
            Engine.LoadGrammar(new Grammar(new GrammarBuilder(List)));
            Engine.SpeechRecognized += Engine_SpeechRecognized;
            Engine.RecognizeAsync(RecognizeMode.Multiple);
            //Engine.Recognize(); 

            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        private static void Engine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var index = SpeechConfiguration.Item("Mode").GetValue<StringList>().SelectedIndex;

            if (index == 3 || index == 2 && !SpeechConfiguration.Item("Press").GetValue<KeyBind>().Active)
                return;

            if (SpeechConfiguration.Item("Target").GetValue<bool>() && CHAMPION_NAMES.Contains(e.Result.Text))
                FireEvent(e.Result.Text);

            if (CustomList.Contains(e.Result.Text))
                FireEvent(e.Result.Text);

            if (CHAMPION_SPELLS.Contains(e.Result.Text))
                FireEvent(e.Result.Text);
        }

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

        /// <summary>
        ///     Fires the event with the specified output
        /// </summary>
        /// <param name="output"></param>
        private static void FireEvent(string output)
        {
            if (OnRecongized != null)
                OnRecongized(output); 
        }

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
    }
}
