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
        [JsonIgnore]
        public string WordList { get; set; }
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
            germanWord = germanWord.Trim();
            englishWord = englishWord.Trim();


            if (germanWord == "" || englishWord == "") {
                return (false, $"Adding was not successful because one input box is empty");
            } else {
                List<VocabularyEntry> vocabulary = GetData(entry);
                (bool isTrue, int index, int error, string word) returnedTupel = alreadyThere(vocabulary, germanWord, englishWord);
                if (!returnedTupel.isTrue) {
                    vocabulary.Add(new VocabularyEntry(germanWord, englishWord));
                    WriteData(entry, vocabulary);
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
    }
}