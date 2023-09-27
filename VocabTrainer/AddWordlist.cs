using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using VocabTrainer.Views;
using System.Globalization;
using System.Windows.Input;

namespace VocabTrainer {
    public class AddWordlist {
        public string FirstLanguage { get; set; }
        public string SecondLanguage { get; set; }
        public string Name { get; set; }

        public void CreateWordlists(string name) {
            string wordListName = Name;
            string jsonData = JsonConvert.SerializeObject("", Formatting.Indented);
            string filePath = $"{VocabularyEntry.FirstPartFilePath}{wordListName}{VocabularyEntry.SecondPartFilePath}";
            File.WriteAllText(filePath, jsonData);
        }

        public void WriteNewWordList() {
            List<WordlistsList> wordlistlist = WordlistsList.GetWordlistsList();
            WordlistsList entry = new WordlistsList();
            entry.WordlistName = Name;
            entry.FirstLanguage = FirstLanguage;
            entry.SecondLanguage = SecondLanguage;
            wordlistlist.Add(entry);
            WordlistsList.WriteWordlistsList(wordlistlist);
        }

        public string CheckInput(string name, string firstLanguage, string secondLanguage) {
            Name = name.Trim();
            FirstLanguage = PascalCase(firstLanguage).Trim();
            SecondLanguage = PascalCase(secondLanguage).Trim();

            List<VocabularyEntry> entries = new List<VocabularyEntry>();

            if (Name == "" || FirstLanguage == "" || SecondLanguage == "") {
                return $"Adding was not successful because one input box is empty";
            } else {
                List<WordlistsList> wordlists = WordlistsList.GetWordlistsList();
                for (int i = 0; i < wordlists.Count; i++) {
                    if (wordlists[i].WordlistName == Name && wordlists[i].FirstLanguage == FirstLanguage && wordlists[i].SecondLanguage == SecondLanguage)
                        return "Adding was not successful because this wordlist already exists";
                }
                WriteNewWordList();
                VocabularyEntry.WriteData(new VocabularyEntry { FilePath = $"{VocabularyEntry.FirstPartFilePath}{Name}{VocabularyEntry.SecondPartFilePath}" }, entries);
                return "Adding was successful.";
            }
        }

        public string PascalCase(string input) {
            if (!string.IsNullOrEmpty(input) && input.Length > 1) {
                return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
            } else {
                return input;
            }
        }
    }
}
