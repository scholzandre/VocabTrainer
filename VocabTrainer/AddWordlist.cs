using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using VocabTrainer.Views;

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
            List<WordlistsList> wordlistlist = WordlistsList.GetWordlists();
            WordlistsList entry = new WordlistsList();
            entry.WordlistName = Name;
            entry.FirstLanguage = FirstLanguage;
            entry.SecondLanguage = SecondLanguage;
            wordlistlist.Add(entry);
            WordlistsList.WriteWordlistsList(wordlistlist);
        }

        public string CheckInput(string name, string firstLanguage, string secondLanguage) {
            Name = name.Trim();
            FirstLanguage = firstLanguage.Trim();
            SecondLanguage = secondLanguage.Trim();

            if (Name == "" || FirstLanguage == "" || SecondLanguage == "") {
                return $"Adding was not successful because one input box is empty";
            } else {
                List<WordlistsList> wordlists = WordlistsList.GetWordlists();
                for (int i = 0; i < wordlists.Count; i++) {
                    if (wordlists[i].WordlistName == Name && wordlists[i].FirstLanguage == FirstLanguage && wordlists[i].SecondLanguage == SecondLanguage)
                        return "Adding was not successful because this wordlist already exists";
                }
                WriteNewWordList();
                return "Adding was successful.";
            }
        }
    }
}
