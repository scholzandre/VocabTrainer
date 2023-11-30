using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class AddViewModel : BaseViewModel {
        private string _selectedItem;
        public string SelectedItem {
            get => _selectedItem;
            set {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
        private ObservableCollection<string> _comboBoxEntries = new ObservableCollection<string>();
        public ObservableCollection<string> ComboBoxEntries {
            get => _comboBoxEntries;
            set {
                _comboBoxEntries = value;
                OnPropertyChanged(nameof(ComboBoxEntries));
            }
        }
        private Dictionary<string, WordlistsList> _comboBoxWordlists = new Dictionary<string, WordlistsList>();
        public Dictionary<string, WordlistsList> ComboBoxWordlists {
            get => _comboBoxWordlists;
            set {
                _comboBoxWordlists = value;
                OnPropertyChanged(nameof(ComboBoxWordlists));
            }
        }
        private string _firstWord = string.Empty;
        public string FirstWord {
            get => _firstWord;
            set {
                _firstWord = value;
                OnPropertyChanged(nameof(FirstWord));
            }
        }
        private string _secondWord = string.Empty;
        public string SecondWord {
            get => _secondWord;
            set {
                _secondWord = value;
                OnPropertyChanged(nameof(SecondWord));
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
        public AddViewModel() {
            List<WordlistsList> tempWordlists = WordlistsList.GetWordlistsList();
            foreach (WordlistsList temp in tempWordlists) {
                if (temp.WordlistName != "Marked" &&
                    temp.WordlistName != "Seen" &&
                    temp.WordlistName != "LastTimeWrong" &&
                    temp.WordlistName != "NotSeen") {
                    string tempString = $"{temp.WordlistName} ({temp.FirstLanguage}, {temp.SecondLanguage})";
                    ComboBoxWordlists.Add(tempString, temp);
                    ComboBoxEntries.Add(tempString);
                }
            }
            SelectedItem = (ComboBoxEntries.Count > 0) ? ComboBoxEntries[ComboBoxEntries.Count - 1] : null;
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand AddEntryCommand => new RelayCommand(AddEntry, CanExecuteCommand);
        private void AddEntry(object obj) {
            VocabularyEntry entry = new VocabularyEntry() {
                German = FirstWord,
                English = SecondWord,
                FirstLanguage = ComboBoxWordlists[SelectedItem].FirstLanguage,
                SecondLanguage = ComboBoxWordlists[SelectedItem].SecondLanguage,
                FilePath = $"{VocabularyEntry.FirstPartFilePath}{ComboBoxWordlists[SelectedItem].WordlistName}_{ComboBoxWordlists[SelectedItem].FirstLanguage}_{ComboBoxWordlists[SelectedItem].SecondLanguage}{VocabularyEntry.SecondPartFilePath}"
            };
            List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
            bool contains = false;
            foreach (VocabularyEntry tempEntry in entries) {
                if (tempEntry.German == entry.German || tempEntry.English == entry.English) {
                    contains = true;
                    break;
                }
            }
            if (!contains) {
                entries.Add(entry);
                VocabularyEntry.WriteData(entry, entries);
                InfoText = $"{FirstWord} and {SecondWord} have been successfully added!";
                FirstWord = string.Empty;
                SecondWord = string.Empty;
            } else {
                InfoText = "This entry already exists.";
            }
        }
    }
}
