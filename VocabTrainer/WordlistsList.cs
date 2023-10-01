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

        public static List<WordlistsList> GetWordlistsList() { 
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
        public static (bool, int) AlreadyThere(string name, string firstLanguage, string secondLanguage) {
            List<WordlistsList> wordlistsList = GetWordlistsList();
            for (int i = 0; i < wordlistsList.Count(); i++) {
                if (wordlistsList[i].WordlistName.ToLower() == name.ToLower() &&
                    wordlistsList[i].FirstLanguage.ToLower() == firstLanguage.ToLower() &&
                    wordlistsList[i].SecondLanguage.ToLower() == secondLanguage.ToLower()) {
                    return (true, i);
                } 
            }
            return (false, -1);
        }
        public List<(VocabularyEntry, string, string, string)> GetAllWords() {
            List<(VocabularyEntry, string, string, string)> words = new List<(VocabularyEntry, string, string, string)>();
            List<WordlistsList> checkedLists = GetCheckedLists();

            for (int i = 0; i < checkedLists.Count; i++) {
                VocabularyEntry checkedList = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}{checkedLists[i].WordlistName}{VocabularyEntry.SecondPartFilePath}"};
                List<VocabularyEntry> vocabulary = VocabularyEntry.GetData(checkedList);

                foreach (VocabularyEntry entry in vocabulary) {
                    words.Add((entry, checkedLists[i].FirstLanguage, checkedLists[i].SecondLanguage, checkedLists[i].WordlistName));
                }
            }
            return (words);
        }

        public List<WordlistsList> GetCheckedLists() {
            List<WordlistsList> list = GetWordlistsList();
            for (int i = list.Count - 1; i >= 0; i--) {
                if (!list[i].IsTrue) list.Remove(list[i]);
            }
            return list;
        }

        public static void CheckAvailabilityOfJSONFiles() {
            List<WordlistsList> allWordLists = GetWordlistsList();
            List<string> namesList = allWordLists.Select(values => values.WordlistName).ToList();
            List<string> filenameList = Directory.GetFiles(VocabularyEntry.FirstPartFilePath).ToList();
            string temp = string.Empty;

            for (int i = 0; i < filenameList.Count; i++) {
                temp = filenameList[i].Substring(VocabularyEntry.FirstPartFilePath.Length, filenameList[i].Length - VocabularyEntry.FirstPartFilePath.Length - VocabularyEntry.SecondPartFilePath.Length).Trim();
                if (temp == "settings" || temp == "wordlists"){
                    continue;
                } else if (!namesList.Contains(temp)) {
                    MessageBoxResult result = MessageBox.Show($"Do you want to add {temp}.json to your program?", "Problem with wordlists", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes) {
                        VocabularyEntry tempEntry = new VocabularyEntry() { FilePath = VocabularyEntry.FirstPartFilePath + temp + VocabularyEntry.SecondPartFilePath };
                        WordlistsList newWordlist = new WordlistsList();
                        Random random = new Random();
                        string defaultValue = string.Empty;
                        for (int j = 0; j < 8; j++) {
                            defaultValue += random.Next(0, 9);
                        }
                        newWordlist.WordlistName = temp;
                        newWordlist.FirstLanguage = defaultValue;
                        newWordlist.SecondLanguage = defaultValue;
                        allWordLists.Add(newWordlist);

                        VocabularyEntry entry = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}NotSeen{VocabularyEntry.SecondPartFilePath}" };
                        List<VocabularyEntry> notSeen = VocabularyEntry.GetData(entry);
                        notSeen.AddRange(VocabularyEntry.GetData(tempEntry));
                        VocabularyEntry.WriteData(entry, notSeen);
                    } else {
                        File.Delete(filenameList[i]);
                    }
                }
            }

            for (int i = 0; i < allWordLists.Count; i++) {
                if (!File.Exists(VocabularyEntry.FirstPartFilePath+ allWordLists[i].WordlistName+VocabularyEntry.SecondPartFilePath)) {
                    allWordLists.Remove(allWordLists[i]);
                    i--;
                }
            }

            WriteWordlistsList(allWordLists);
        }
    }
}