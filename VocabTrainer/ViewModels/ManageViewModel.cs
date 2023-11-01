using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels
{
    public class ManageViewModel : BaseViewModel {
        private ObservableCollection<ManageEntryViewModel> _entries;
        public ObservableCollection<ManageEntryViewModel> Entries {
            get => _entries;
            set {
                _entries = value;
                SearchingWords = Entries;
                OnPropertyChanged(nameof(Entries));
            }
        }
        private ObservableCollection<WordlistsList> _comboBoxEntries;
        public ObservableCollection<WordlistsList> ComboBoxEntries {
            get => _comboBoxEntries;
            set {
                _comboBoxEntries = value;
                OnPropertyChanged(nameof(ComboBoxEntries));
            }
        }
        private WordlistsList _selectedItem;
        public WordlistsList SelectedItem {
            get => _selectedItem;
            set {
                if (_selectedItem != value) {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                    FillEntriesCollection();
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
        public ManageViewModel() {
            ComboBoxEntries = new ObservableCollection<WordlistsList>(WordlistsList.GetWordlistsList());
            SelectedItem = ComboBoxEntries.Count > 0 ? ComboBoxEntries[0] : null;
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand SearchCommand => new RelayCommand(Search, CanExecuteCommand);
        private void Search(object obj) {
            SearchingWords = new ObservableCollection<ManageEntryViewModel>(Entries);
            if (SearchingWord != "" && SearchingWord != "Searching...")
                for (int i = 0; i < SearchingWords.Count; i++)
                    if (!SearchingWords[i].FirstWord.ToLower().Contains(SearchingWord.ToLower()) && !SearchingWords[i].SecondWord.ToLower().Contains(SearchingWord.ToLower())) {
                        SearchingWords.Remove(SearchingWords[i]);
                        i--;
                    }
        }
        private void FillEntriesCollection() {
            Entries = new ObservableCollection<ManageEntryViewModel>();
            VocabularyEntry entry = new VocabularyEntry() { 
                FilePath = VocabularyEntry.FirstPartFilePath + SelectedItem.WordlistName + VocabularyEntry.SecondPartFilePath
            };
            List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
            foreach (VocabularyEntry tempEntry in entries) {
                Entries.Add(new ManageEntryViewModel(Entries, tempEntry));
            }
        }
    }
}
