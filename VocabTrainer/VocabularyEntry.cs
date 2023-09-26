using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Windows.Navigation;

namespace VocabTrainer.Views {
    public class VocabularyEntry {
        public string English { get; set; }
        public string German { get; set; }
        public bool Seen { get; set; }
        public bool LastTimeWrong { get; set; }
        public int Repeated { get; set; }
        public string WordList { get; set; }
        public string FirstLanguage { get; set; }
        public string SecondLanguage { get; set; }
        [JsonIgnore]
        public string FilePath { get; set; }

        [JsonIgnore]
        public static string FirstPartFilePath { get => Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().Length-9)+"jsons\\"; }
        [JsonIgnore]
        public static string SecondPartFilePath { get => ".json"; }
        public VocabularyEntry() { }
        public VocabularyEntry(string german, string english) {
            German = german;
            English = english;
        }

        public static List<VocabularyEntry> GetData(VocabularyEntry entry) {
            List<VocabularyEntry> vocabulary = new List<VocabularyEntry>();
            if (File.Exists(entry.FilePath)) {
                string jsonData = File.ReadAllText(entry.FilePath);
                vocabulary = JsonConvert.DeserializeObject<List<VocabularyEntry>>(jsonData);
            }
            return vocabulary;
        }

        public static void WriteData(VocabularyEntry entry, List<VocabularyEntry> vocabulary) {
            for (int i = 0; i < vocabulary.Count; i++) {
                int index = entry.FilePath.LastIndexOf('\\');
                string tempWordlist = entry.FilePath.Substring(index + 1, entry.FilePath.Count() - (index + 6)); 
                string languages = tempWordlist.Substring(tempWordlist.IndexOf('_') + 1);

                if (languages != "Marked" &&
                    languages != "Seen" &&
                    languages != "NotSeen" &&
                    languages != "LastTimeWrong") { 
                    vocabulary[i].WordList = tempWordlist;
                    vocabulary[i].FirstLanguage = languages.Substring(0, languages.IndexOf('_'));
                    vocabulary[i].SecondLanguage = languages.Substring(languages.IndexOf('_')+1);
                }
            }
            string json = JsonConvert.SerializeObject(vocabulary, Formatting.Indented);
            File.WriteAllText(entry.FilePath, json);
        }

        public static void WriteData(List<VocabularyEntry> vocabulary) {
            VocabularyEntry entry = new VocabularyEntry();
            string json = JsonConvert.SerializeObject(vocabulary, Formatting.Indented);
            File.WriteAllText(entry.FilePath, json);
        }

        public static (bool, string) CheckInput(string wordlist, string germanWord, string englishWord) {
            int index = wordlist.IndexOf("(");
            string fileName = wordlist.Substring(0, index - 1);
            VocabularyEntry entry = new VocabularyEntry();
            entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{fileName}{VocabularyEntry.SecondPartFilePath}";
            entry.German = germanWord.Trim();
            entry.English = englishWord.Trim();

            if (germanWord == "" || englishWord == "") {
                return (false, $"Adding was not successful because one input box is empty");
            } else {
                List<VocabularyEntry> vocabulary = GetData(entry);
                (bool isTrue, int index, int error, string word) returnedTupel = alreadyThere(vocabulary, entry.German, entry.English);
                if (!returnedTupel.isTrue) {
                    vocabulary.Add(entry);
                    WriteData(entry, vocabulary);
                    MoveWords("NotSeen", entry);
                    return (true, $"'{germanWord}' and '{englishWord}' were successfully added");
                } else {
                    if (returnedTupel.error == 1) {
                        return (false, $"Adding was not successful because '{returnedTupel.word}' already exists in the program");
                    } else if (returnedTupel.error == 2) {
                        return (false, $"Adding was not successful because the entry '{germanWord}' and '{englishWord}' already exists in the program");
                    } else if (returnedTupel.error == 3) {
                        return (false, $"Adding was not successful because '{returnedTupel.word}' already exists in the program");
                    } else {
                        return (false, "");
                    }
                }
            }
        }
        public static (bool, int, int, string) alreadyThere(List<VocabularyEntry> vocabulary, string germanWord, string englishWord) {
            for (int i = 0; i < vocabulary.Count(); i++) {
                if (vocabulary[i].German.ToLower() == germanWord.ToLower()) {
                    return (true, i, 1, germanWord);
                } else if (vocabulary[i].English.ToLower() == englishWord.ToLower()) {
                    return (true, i, 1, englishWord);
                } else if (vocabulary[i].German.ToLower().ToLower() == englishWord && vocabulary[i].English.ToLower() == germanWord) {
                    return (true, i, 2, "");
                } else if (vocabulary[i].German.ToLower().ToLower() == englishWord) {
                    return (true, i, 3, englishWord);
                } else if (vocabulary[i].English.ToLower() == germanWord) {
                    return (true, i, 3, germanWord);
                }
            }
            return (false, -1, -1, "");
        }

        public static (bool, int, int, string) alreadyThere( string germanWord, string englishWord) {
            List<VocabularyEntry> vocabulary = new List<VocabularyEntry>();
            for (int i = 0; i < vocabulary.Count(); i++) {
                if (vocabulary[i].German.ToLower() == germanWord.ToLower()) {
                    return (true, i, 1, germanWord);
                } else if (vocabulary[i].English.ToLower() == englishWord.ToLower()) {
                    return (true, i, 1, englishWord);
                } else if (vocabulary[i].German.ToLower().ToLower() == englishWord && vocabulary[i].English.ToLower() == germanWord) {
                    return (true, i, 2, "");
                } else if (vocabulary[i].German.ToLower().ToLower() == englishWord) {
                    return (true, i, 3, englishWord);
                } else if (vocabulary[i].English.ToLower() == germanWord) {
                    return (true, i, 3, germanWord);
                }
            }
            return (false, -1, -1, "");
        }
        public static List<string> checkEmpty(List<VocabularyEntry> vocabulary) {
            if (vocabulary.Count == 0) {
                return new List<string> { "Es sind keine Wörter verfügbar", "There are no words available" };
                ;
            } else return new List<string> { string.Empty, string.Empty };
        }

        public static void MoveWords(string wordlist, VocabularyEntry words) {
            VocabularyEntry entry = words;
            entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{wordlist}{VocabularyEntry.SecondPartFilePath}";
            List<VocabularyEntry> entries = GetData(entry);
            entries.Add(entry);
            WriteData(entry, entries);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            VocabularyEntry otherEntry = (VocabularyEntry)obj;
            return this.WordList == otherEntry.WordList &&
                   this.German == otherEntry.German &&
                   this.English == otherEntry.English;
        }

        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 31 + WordList.GetHashCode();
            hash = hash * 31 + (German ?? "").GetHashCode();
            hash = hash * 31 + (English ?? "").GetHashCode();
            return hash;
        }
    }
}