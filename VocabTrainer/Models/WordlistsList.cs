using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using VocabTrainer.Views;
using System.Windows;

namespace VocabTrainer {
    public class WordlistsList {
        public string WordlistName { get; set; }
        public string FirstLanguage { get; set; }
        public string SecondLanguage { get; set; }
        public bool IsTrue { get; set; }
        private static string _filePath = $"{VocabularyEntry.FirstPartFilePath}wordlists{VocabularyEntry.SecondPartFilePath}";
        private static List<string> _specialWordlists = new List<string>() {
            "Marked",
            "Seen",
            "NotSeen",
            "LastTimeWrong"
        };

        public static List<WordlistsList> GetWordlistsList() {
            List<WordlistsList> wordlists = new List<WordlistsList>();
            if (File.Exists(_filePath)) {
                string jsonData = File.ReadAllText(_filePath);
                wordlists = JsonConvert.DeserializeObject<List<WordlistsList>>(jsonData);
            }
            return wordlists;
        }
        public static void WriteWordlistsList(List<WordlistsList> wordlistsList) {
            string json = JsonConvert.SerializeObject(wordlistsList, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
        public static (bool, int) AlreadyThere(string name, string firstLanguage, string secondLanguage) {
            List<WordlistsList> wordlistsList = GetWordlistsList();
            for (int i = 0; i < wordlistsList.Count(); i++)
                if (wordlistsList[i].WordlistName.ToLower() == name.ToLower() &&
                    wordlistsList[i].FirstLanguage.ToLower() == firstLanguage.ToLower() &&
                    wordlistsList[i].SecondLanguage.ToLower() == secondLanguage.ToLower()) {
                    return (true, i);
                }
            return (false, -1);
        }
        public List<(VocabularyEntry, string, string, string)> GetAllWords() {
            List<(VocabularyEntry, string, string, string)> words = new List<(VocabularyEntry, string, string, string)>();
            List<WordlistsList> checkedLists = GetCheckedLists();

            for (int i = 0; i < checkedLists.Count; i++) {
                VocabularyEntry checkedList = new VocabularyEntry();
                if (checkedLists[i].WordlistName != "Marked" &&
                    checkedLists[i].WordlistName != "Seen" &&
                    checkedLists[i].WordlistName != "NotSeen" &&
                    checkedLists[i].WordlistName != "LastTimeWrong") {
                    checkedList.FilePath = $"{VocabularyEntry.FirstPartFilePath}{checkedLists[i].WordlistName}_{checkedLists[i].FirstLanguage}_{checkedLists[i].SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
                } else {
                    checkedList.FilePath = $"{VocabularyEntry.FirstPartFilePath}{checkedLists[i].WordlistName}{VocabularyEntry.SecondPartFilePath}";
                }
                List<VocabularyEntry> vocabulary = VocabularyEntry.GetData(checkedList);

                foreach (VocabularyEntry entry in vocabulary)
                    words.Add((entry, checkedLists[i].FirstLanguage, checkedLists[i].SecondLanguage, checkedLists[i].WordlistName));

            }
            return (words);
        }

        public List<WordlistsList> GetCheckedLists() {
            List<WordlistsList> list = GetWordlistsList();
            for (int i = list.Count - 1; i >= 0; i--)
                if (!list[i].IsTrue)
                    list.Remove(list[i]);
            return list;
        }

        public static void CheckAvailabilityOfJSONFiles() {
            List<WordlistsList> allWordLists = GetWordlistsList();
            List<string> namesList = allWordLists.Select(values => values.WordlistName).ToList();
            List<string> filenameList = Directory.GetFiles(VocabularyEntry.FirstPartFilePath).ToList();
            string file = string.Empty;
            string wordlistName = string.Empty;

            for (int i = 0; i < filenameList.Count; i++)
                if (filenameList[i].Contains("_")) {
                    file = filenameList[i].Substring(VocabularyEntry.FirstPartFilePath.Length);
                    wordlistName = file.Substring(0, file.IndexOf("_"));
                    if (!namesList.Contains(wordlistName)) {
                        MessageBoxResult result = MessageBox.Show($"Do you want to add {file} to your program?", "Problem with wordlists", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes) {
                            VocabularyEntry tempEntry = new VocabularyEntry() { FilePath = VocabularyEntry.FirstPartFilePath + file };
                            WordlistsList newWordlist = new WordlistsList();
                            if (file.Contains('_')) {
                                newWordlist.FirstLanguage = file.Substring(file.IndexOf('_') + 1, file.LastIndexOf('_') - 1 - file.IndexOf('_'));
                                newWordlist.SecondLanguage = file.Substring(file.LastIndexOf('_') + 1, file.Length - file.LastIndexOf('_') - 1 - VocabularyEntry.SecondPartFilePath.Length);
                            }
                            newWordlist.WordlistName = wordlistName;
                            allWordLists.Add(newWordlist);

                            VocabularyEntry entry = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}NotSeen{VocabularyEntry.SecondPartFilePath}" };
                            List<VocabularyEntry> notSeen = VocabularyEntry.GetData(entry);
                            notSeen.AddRange(VocabularyEntry.GetData(tempEntry));
                            VocabularyEntry.WriteData(entry, notSeen);
                        } else
                            File.Delete(filenameList[i]);
                    }
                }

            for (int i = 0; i < allWordLists.Count; i++)
                if (allWordLists[i].FirstLanguage != "-" && allWordLists[i].SecondLanguage != "-")
                    if (!File.Exists($"{VocabularyEntry.FirstPartFilePath}{allWordLists[i].WordlistName}_{allWordLists[i].FirstLanguage}_{allWordLists[i].SecondLanguage}{VocabularyEntry.SecondPartFilePath}")) {
                        allWordLists.Remove(allWordLists[i]);
                        i--;
                    }

            WriteWordlistsList(allWordLists);
        }


        public static void CheckSpecialWordlists() {
            string filePathWordlists = VocabularyEntry.FirstPartFilePath;
            List<string> allFiles = Directory.GetFiles(filePathWordlists).ToList();
            for (int i = 0; i < allFiles.Count; i++) {
                string fileName = allFiles[i].Substring(allFiles[i].LastIndexOf("\\") + 1);
                allFiles[i] = fileName;
            }

            for (int i = 0; i < _specialWordlists.Count; i++) {
                if (!allFiles.Contains(_specialWordlists[i] + VocabularyEntry.SecondPartFilePath)) {
                    VocabularyEntry tempEntry = new VocabularyEntry() { 
                        FilePath = VocabularyEntry.FirstPartFilePath + _specialWordlists[i] + VocabularyEntry.SecondPartFilePath
                    };
                    VocabularyEntry.WriteData(tempEntry, new List<VocabularyEntry>());
                    WordlistsList wordlist = new WordlistsList() {
                        WordlistName = _specialWordlists[i],
                        FirstLanguage = "-",
                        SecondLanguage = "-",
                        IsTrue = false
                    };
                    List<WordlistsList> wordlistsLists = WordlistsList.GetWordlistsList();
                    if (!wordlistsLists.Contains(wordlist)) {
                        wordlistsLists.Add(wordlist);
                        WriteWordlistsList(wordlistsLists);
                    }

                    if (_specialWordlists[i] == "NotSeen") {
                        VocabularyEntry seen = new VocabularyEntry() { 
                            FilePath = $"{VocabularyEntry.FirstPartFilePath}Seen{VocabularyEntry.SecondPartFilePath}"
                        };
                        VocabularyEntry notSeen = new VocabularyEntry() {
                            FilePath = $"{VocabularyEntry.FirstPartFilePath}NotSeen{VocabularyEntry.SecondPartFilePath}"
                        };
                        List<VocabularyEntry> tempSeenEntries = VocabularyEntry.GetData(seen);
                        List<VocabularyEntry> tempNotSeenEntries = new List<VocabularyEntry>();
                        List<WordlistsList> wordlists = GetWordlistsList();
                        for (int j = 0; j < wordlists.Count; j++) {
                            List<VocabularyEntry> tempEntries = VocabularyEntry.GetData(new VocabularyEntry() {
                                FilePath = $"{VocabularyEntry.FirstPartFilePath}{wordlists[j].WordlistName}" +
                                $"_{wordlists[j].FirstLanguage}_{wordlists[j].SecondLanguage}{VocabularyEntry.SecondPartFilePath}"
                            });
                            for (int z = 0; z < tempEntries.Count; z++) 
                                if (!tempSeenEntries.Contains(tempEntries[z]))
                                    tempNotSeenEntries.Add(tempEntries[z]);
                        }
                        VocabularyEntry.WriteData(notSeen, tempNotSeenEntries);
                    }
                }
            }

        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            WordlistsList otherEntry = (WordlistsList)obj;
            return this.WordlistName == otherEntry.WordlistName &&
                   this.FirstLanguage == otherEntry.FirstLanguage &&
                   this.SecondLanguage == otherEntry.SecondLanguage;
        }
    }
}