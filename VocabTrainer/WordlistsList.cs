using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using VocabTrainer.Views;
using System.Windows.Documents;

namespace VocabTrainer {
    public class WordlistsList {
        public string WordlistName { get; set; }
        public string FirstLanguage { get; set; }
        public string SecondLanguage { get; set; }
        public bool IsTrue { get; set; }
        private const string filePath = "./../../wordlists.json";
        public static List<WordlistsList> GetWordlists() { 
            List<WordlistsList> wordlists = new List<WordlistsList>();
            if (File.Exists(filePath)) {
                string jsonData = File.ReadAllText(filePath);
                wordlists = JsonConvert.DeserializeObject<List<WordlistsList>>(jsonData);
            }
            return wordlists;
        }

        public static void WriteWordlistsList(List<WordlistsList> wordlistsList) {
            string json = JsonConvert.SerializeObject(wordlistsList, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static (bool, int, int, string) alreadyThere(string name, string firstLanguage, string secondLanguage) {
            List<WordlistsList> wordlistslist = GetWordlists();
            for (int i = 0; i < wordlistslist.Count(); i++) {
                if (wordlistslist[i].WordlistName.ToLower() == name.ToLower() &&
                    wordlistslist[i].FirstLanguage.ToLower() == firstLanguage.ToLower() &&
                    wordlistslist[i].SecondLanguage.ToLower() == secondLanguage.ToLower()) {
                    return (true, i, 1, "");
                } 
            }
            return (false, -1, -1, "");
        }

        public (int, List<(VocabularyEntry, string, string, string)>) GetAllWords() {
            List<(VocabularyEntry, string, string, string)> words = new List<(VocabularyEntry, string, string, string)>();
            List<WordlistsList> list = GetLists();
            int allWordsCounter = 0;

            for (int i = 0; i < list.Count; i++) {
                VocabularyEntry wordlist = new VocabularyEntry();
                wordlist.WordList = list[i].WordlistName;
                wordlist.FilePath = $"./../../{list[i].WordlistName}.json";
                List<VocabularyEntry> vocabulary = VocabularyEntry.GetData(wordlist);
                foreach (VocabularyEntry entry in vocabulary) {
                    words.Add((entry, list[i].FirstLanguage, list[i].SecondLanguage, list[i].WordlistName));
                }
                allWordsCounter += vocabulary.Count();
            }
            return (allWordsCounter, words);
        }

        public List<WordlistsList> GetLists() {
            List<WordlistsList> list = GetWordlists();
            for (int i = list.Count - 1; i >= 0; i--) { // Removes where wordlist is unchecked
                if (!list[i].IsTrue) {
                    list.Remove(list[i]);
                }
            }
            return list;
        }
    }
}
