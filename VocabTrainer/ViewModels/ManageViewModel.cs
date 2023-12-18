using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class ManageViewModel : BaseViewModel {
        private ObservableCollection<string> _comboBoxEntries = new ObservableCollection<string>();
        public ObservableCollection<string> ComboBoxEntries {
            get => _comboBoxEntries;
            set {
                _comboBoxEntries = value;
                OnPropertyChanged(nameof(ComboBoxEntries));
            }
        }
        private string _selectedItem;
        public string SelectedItem {
            get => _selectedItem;
            set {
                if (_selectedItem != value) {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                    FillEntriesCollection();
                    AllEntriesCounter = SearchingWords.Count;
                }
            }
        }
        private ObservableCollection<ManageEntryViewModel> _searchingWords = new ObservableCollection<ManageEntryViewModel>();
        public ObservableCollection<ManageEntryViewModel> SearchingWords {
            get => _searchingWords;
            set {
                _searchingWords = value;
                OnPropertyChanged(nameof(SearchingWords));
            }
        }
        private string _searchingWord = "Searching...";
        public string SearchingWord {
            get => _searchingWord;
            set {
                _searchingWord = value;
                OnPropertyChanged(nameof(SearchingWord));
            }
        }
        private int _allEntriesCounter = 0;
        public int AllEntriesCounter {
            get => _allEntriesCounter;
            set {
                _allEntriesCounter = value;
                OnPropertyChanged(nameof(AllEntriesCounter));
                InfoText = $"There are {AllEntriesCounter} Entries";
            }
        }
        private string _infoText;
        public string InfoText {
            get => _infoText;
            set {
                _infoText = value;
                OnPropertyChanged(nameof(InfoText));
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
        public ManageViewModel() {
            List<WordlistsList> tempWordlists = WordlistsList.GetWordlistsList();
            foreach (WordlistsList temp in tempWordlists) 
                if (temp.WordlistName != "Seen" &&
                    temp.WordlistName != "LastTimeWrong" &&
                    temp.WordlistName != "NotSeen") {
                    string tempString = $"{temp.WordlistName} ({temp.FirstLanguage}, {temp.SecondLanguage})";
                    ComboBoxWordlists.Add(tempString, temp);
                    ComboBoxEntries.Add(tempString);
                }
            SelectedItem = (ComboBoxEntries.Count > 0) ? ComboBoxEntries[ComboBoxEntries.Count - 1] : null;
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand SearchCommand => new RelayCommand(Search, CanExecuteCommand);
        private void Search(object obj) {
            FillEntriesCollection();
            if (SearchingWord != "" && SearchingWord != "Searching...")
                for (int i = 0; i < SearchingWords.Count; i++)
                    if (!SearchingWords[i].FirstWord.ToLower().Contains(SearchingWord.ToLower()) && !SearchingWords[i].SecondWord.ToLower().Contains(SearchingWord.ToLower())) {
                        SearchingWords.Remove(SearchingWords[i]);
                        i--;
                    }
        }
        private void FillEntriesCollection() {
            SearchingWords = new ObservableCollection<ManageEntryViewModel>();
            VocabularyEntry entry = new VocabularyEntry();
            if (ComboBoxWordlists[SelectedItem].WordlistName != "Marked") 
                entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{ComboBoxWordlists[SelectedItem].WordlistName}_{ComboBoxWordlists[SelectedItem].FirstLanguage}_{ComboBoxWordlists[SelectedItem].SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
            else 
                entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{ComboBoxWordlists[SelectedItem].WordlistName}{VocabularyEntry.SecondPartFilePath}";
            
            List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
            foreach (VocabularyEntry tempEntry in entries) {
                WordlistsList tempWordlist = new WordlistsList() {
                    WordlistName = $"{ComboBoxWordlists[SelectedItem].WordlistName}_{ComboBoxWordlists[SelectedItem].FirstLanguage}_{ComboBoxWordlists[SelectedItem].SecondLanguage}"
                };
                SearchingWords.Add(new ManageEntryViewModel(SearchingWords, tempEntry, this, tempWordlist));
            }
        }
    }
}
