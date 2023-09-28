using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using VocabTrainer.Views;

namespace VocabTrainer {
    public class Settings {
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

            (int allWordsCounter, List<(VocabularyEntry entry, string firstLanguage, string secondLanguage, string file)> allWords) = new WordlistsList().GetAllWords();
            if (allWordsCounter == 0) {
                foreach (Settings setting in settings) { 
                    setting.IsTrue = false;
                }
            }
            if (allWordsCounter < 2) {
                settings[0].IsTrue = false; // random mode
                settings[4].IsTrue = false; // learning mode three
                settings[5].IsTrue = false; // learning mode four
            } else if (allWordsCounter < 5) {
                settings[4].IsTrue = false; // learning mode three
                settings[5].IsTrue = false; // learning mode four
            }
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}