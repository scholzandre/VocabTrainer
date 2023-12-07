using System.Collections.Generic;
using System.Windows.Input;
using VocabTrainer.Models;

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
        public AddWordlistViewModel() {}

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
            for (int i = 0; i < wordlists.Count; i++) {
                if (wordlists[i].WordlistName == WordlistName &&
                    wordlists[i].FirstLanguage == FirstLanguage &&
                    wordlists[i].SecondLanguage == SecondLanguage)
                    alreadyExists = true;
            }
            if (!alreadyExists) {
                wordlists.Add(wordlist);
                InfoText = "Wordlist has been added successfully";
                WordlistName = string.Empty;
                FirstLanguage = string.Empty;
                SecondLanguage = string.Empty;
            } else { 
                InfoText = "This Wordlist already exists";
            }
            WordlistsList.WriteWordlistsList(wordlists);
        }
    }
}
