using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using VocabTrainer.Views;
using System.Windows;

namespace VocabTrainer {
    public class WordlistsList {
        public string WordlistName { get; set; }
        public string FirstLanguage { get; set; }
        public string SecondLanguage { get; set; }
        public bool IsTrue { get; set; }
        private static string filePath = $"{VocabularyEntry.FirstPartFilePath}wordlists{VocabularyEntry.SecondPartFilePath}";
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
                wordlist.FilePath = $"{VocabularyEntry.FirstPartFilePath}{list[i].WordlistName}{VocabularyEntry.SecondPartFilePath}";
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

        public static void CheckAvailabilityOfJSONFiles() {
            List<WordlistsList> list = GetWordlists();
            List<string> listNames = list.Select(values => values.WordlistName).ToList();
            string temp = string.Empty;
            string[] files = Directory.GetFiles(VocabularyEntry.FirstPartFilePath);
            List<string> filesList = files.ToList();

            for (int i = 0; i < filesList.Count; i++) {
                temp = filesList[i].Substring(VocabularyEntry.FirstPartFilePath.Length, filesList[i].Length - VocabularyEntry.FirstPartFilePath.Length - 5).Trim();
                if (temp == "settings" || temp == "wordlists"){
                    continue;
                } else if (!listNames.Contains(temp)) {
                    MessageBoxResult result = MessageBox.Show($"Do you want to add {temp}.json to your program?", "Problem with wordlists", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes) {
                        VocabularyEntry tempEntry = new VocabularyEntry();
                        tempEntry.FilePath = VocabularyEntry.FirstPartFilePath+temp+VocabularyEntry.SecondPartFilePath;
                        List<VocabularyEntry> testList = VocabularyEntry.GetData(tempEntry);
                        WordlistsList newEntry = new WordlistsList();
                        Random random = new Random();
                        string defaultValue = string.Empty;
                        for (int j = 0; j < 8; j++) {
                            defaultValue += random.Next(0, 9);
                        }
                        newEntry.WordlistName = temp;
                        newEntry.FirstLanguage = defaultValue;
                        newEntry.SecondLanguage = defaultValue;
                        list.Add(newEntry);
                    } else {
                        File.Delete(filesList[i]);
                    }
                }
            }

            for (int i = 0; i < list.Count; i++) {
                if (!File.Exists(VocabularyEntry.FirstPartFilePath+list[i].WordlistName+VocabularyEntry.SecondPartFilePath)) {
                    list.Remove(list[i]);
                    i--;
                }
            }

            WriteWordlistsList(list);
        }
    }
}