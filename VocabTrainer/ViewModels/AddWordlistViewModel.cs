using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    internal class AddWordlistViewModel : BaseViewModel {
        private string _wordlistName = string.Empty;
        public string WordlistName{
            get => _wordlistName;
            set { 
                _wordlistName = value;
                OnPropertyChanged(nameof(WordlistName));
            } 
        }
        private string _firstLanguage = string.Empty;
        public string FirstLanguage {
            get => _firstLanguage;
            set {
                _firstLanguage = value;
                OnPropertyChanged(nameof(FirstLanguage));
            }
        }
        private string _secondLanguage = string.Empty;
        public string SecondLanguage {
            get => _secondLanguage;
            set {
                _secondLanguage = value;
                OnPropertyChanged(nameof(SecondLanguage));
            }
        }
        private string _infoText = string.Empty;
        public string InfoText {
            get => _infoText;
            set {
                _infoText = value;
                OnPropertyChanged(nameof(InfoText));
            }
        }
        private List<char> _forbiddenCharacters = new List<char>() { 
            '\\',
            '/',
            ':',
            '*',
            '?',
            '"',
            '<',
            '>',
            '|'
        };
        private MainViewModel _parent;
        public AddWordlistViewModel(MainViewModel parent) {
            _parent = parent;
        }

        private bool CanExecuteCommand(object arg) {
            return (WordlistName != string.Empty && FirstLanguage != string.Empty && SecondLanguage != string.Empty);
        }
        public ICommand AddWordlistCommand => new RelayCommand(AddWordlist, CanExecuteCommand);
        private void AddWordlist(object obj) {
            List<WordlistsList> wordlists = WordlistsList.GetWordlistsList();
            WordlistsList wordlist = new WordlistsList() {
                WordlistName = WordlistName,
                FirstLanguage = FirstLanguage,
                SecondLanguage = SecondLanguage
            };
            bool alreadyExists = false;
            for(int i = 0; i < _forbiddenCharacters.Count; i++) {
                if (WordlistName.Contains(_forbiddenCharacters[i].ToString()) ||
                    FirstLanguage.Contains(_forbiddenCharacters[i].ToString()) ||
                    SecondLanguage.Contains(_forbiddenCharacters[i].ToString())) {
                    InfoText = $"Don't use the following characters: {string.Join(" ", _forbiddenCharacters.Where(x => x != ' '))}";
                    return;
                }
            }
            for (int i = 0; i < wordlists.Count; i++) {
                if (wordlists[i].WordlistName == WordlistName &&
                    wordlists[i].FirstLanguage == FirstLanguage &&
                    wordlists[i].SecondLanguage == SecondLanguage)
                    alreadyExists = true;
            }
            if (!alreadyExists) {
                wordlists.Add(wordlist);
                VocabularyEntry.WriteData(new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}{wordlist.WordlistName}_{wordlist.FirstLanguage}_{wordlist.SecondLanguage}{VocabularyEntry.SecondPartFilePath}" }, new List<VocabularyEntry>());
                InfoText = "Wordlist has been added successfully";
                WordlistName = string.Empty;
                FirstLanguage = string.Empty;
                SecondLanguage = string.Empty;
            } else { 
                InfoText = "This Wordlist already exists";
                return;
            }
            WordlistsList.WriteWordlistsList(wordlists);
            _parent.ListWordlist = WordlistsList.GetWordlistsList();
        }
    }
}
