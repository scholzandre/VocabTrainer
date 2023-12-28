using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using VocabTrainer.Views;

namespace VocabTrainer {
    public class Settings {
        public string ButtonsForeground { get; set; }
        public string ButtonsBackground { get; set; }
        public string BorderBrush { get; set; }
        public string BorderBackground { get; set; }
        public string NavBarBackground { get; set; }
        public string Condition { get; set; }
        public bool IsTrue { get; set; }
        public int LearningMode { get; set; }
        public bool IsLearningMode { get; set; }
        private static string _filePath = $"{VocabularyEntry.FirstPartFilePath}settings{VocabularyEntry.SecondPartFilePath}";
        public static List<Settings> GetSettings() {
            List<Settings> settings = new List<Settings>();
            if (File.Exists(_filePath)) {
                string jsonData = File.ReadAllText(_filePath);
                settings = JsonConvert.DeserializeObject<List<Settings>>(jsonData);
            }
            return settings;
        }
        public static void WriteSettings(List<Settings> settings) {
            if (settings[0].IsTrue == false && settings[1].IsTrue == false) {
                settings[1].IsTrue = true;
            } else if (settings[0].IsTrue == true && settings[1].IsTrue == true) {
                settings[0].IsTrue = true;
                settings[1].IsTrue = false;
            }
            bool learningModeAvailable = false;
            for (int i = 0; i < settings.Count(); i++) {
                if (settings[i].IsLearningMode && settings[i].IsTrue) {
                    learningModeAvailable = true;
                    break;
                }
            }
            if (!learningModeAvailable) settings[2].IsTrue = true;

            List<(VocabularyEntry entry, string firstLanguage, string secondLanguage, string file)> allWords = new WordlistsList().GetAllWords();
            if (allWords.Count == 0) {
                foreach (Settings setting in settings) { 
                    setting.IsTrue = false;
                }
            }
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public static void CheckSettingsFile() {
            if (!File.Exists(_filePath)) { 
                List<Settings> settings = new List<Settings> {
                    new Settings() { ButtonsForeground = null, ButtonsBackground = null, BorderBrush = null, BorderBackground = null, NavBarBackground = null, Condition = "random order", IsTrue = false, LearningMode = 0, IsLearningMode = false },
                    new Settings() { ButtonsForeground = null, ButtonsBackground = null, BorderBrush = null, BorderBackground = null, NavBarBackground = null, Condition = "intelligent order", IsTrue = true, LearningMode = 0, IsLearningMode = false },
                    new Settings() { ButtonsForeground = null, ButtonsBackground = null, BorderBrush = null, BorderBackground = null, NavBarBackground = null, Condition = "Learning Mode One", IsTrue = true, LearningMode = 1, IsLearningMode = true },
                    new Settings() { ButtonsForeground = null, ButtonsBackground = null, BorderBrush = null, BorderBackground = null, NavBarBackground = null, Condition = "Learning Mode Two", IsTrue = false, LearningMode = 2, IsLearningMode = true },
                    new Settings() { ButtonsForeground = null, ButtonsBackground = null, BorderBrush = null, BorderBackground = null, NavBarBackground = null, Condition = "Learning Mode Three", IsTrue = false, LearningMode = 3, IsLearningMode = true },
                    new Settings() { ButtonsForeground = null, ButtonsBackground = null, BorderBrush = null, BorderBackground = null, NavBarBackground = null, Condition = "Learning Mode Four", IsTrue = false, LearningMode = 4, IsLearningMode = true },
                    new Settings() { ButtonsForeground = "#FFFFFF", ButtonsBackground = null, BorderBrush = null, BorderBackground = null, NavBarBackground = null, Condition = "Buttons foreground", IsTrue = false, LearningMode = 0, IsLearningMode = false },
                    new Settings() { ButtonsForeground = null, ButtonsBackground = "#000000", BorderBrush = null, BorderBackground = null, NavBarBackground = null, Condition = "Buttons background", IsTrue = false, LearningMode = 0, IsLearningMode = false },
                    new Settings() { ButtonsForeground = null, ButtonsBackground = null, BorderBrush = "#FFFFFF", BorderBackground = null, NavBarBackground = null, Condition = "Border color", IsTrue = false, LearningMode = 0, IsLearningMode = false },
                    new Settings() { ButtonsForeground = null, ButtonsBackground = null, BorderBrush = null, BorderBackground = "#000000", NavBarBackground = null, Condition = "Background color", IsTrue = false, LearningMode = 0, IsLearningMode = false },
                    new Settings() { ButtonsForeground = null, ButtonsBackground = null, BorderBrush = null, BorderBackground = null, NavBarBackground = "#AACCFF", Condition = "Navigationbar Background", IsTrue = false, LearningMode = 0, IsLearningMode = false }
                };
                WriteSettings(settings);
            }
        }
    }
}