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
        private Dictionary<WordlistsList, List<(int index, VocabularyEntry before, VocabularyEntry after, int indexM, List<bool> availability)>> _undoList = new Dictionary<WordlistsList, List<(int, VocabularyEntry, VocabularyEntry, int indexM, List<bool>)>>();
        public Dictionary<WordlistsList, List<(int index, VocabularyEntry before, VocabularyEntry after, int indexM, List<bool> availability)>> UndoList {
            get => _undoList;
            set {
                _undoList = value;
                OnPropertyChanged(nameof(UndoList));
            }
        }
        private Dictionary<WordlistsList, List<(int index, VocabularyEntry before, VocabularyEntry after, int indexM, List<bool> availability)>> _redoList = new Dictionary<WordlistsList, List<(int index, VocabularyEntry, VocabularyEntry, int indexM, List<bool> availability)>>();
        public Dictionary<WordlistsList, List<(int index, VocabularyEntry before, VocabularyEntry after, int indexM, List<bool> availability)>> RedoList {
            get => _redoList;
            set {
                _redoList = value;
                OnPropertyChanged(nameof(RedoList));
            }
        }
        private string _undoString = ButtonIcons.GetIconString(IconType.Undo);
        public string UndoString {
            get => _undoString;
            set {
                _undoString = value;
                OnPropertyChanged(nameof(UndoString));
            }
        }
        private string _redoString = ButtonIcons.GetIconString(IconType.Redo);
        public string RedoString {
            get => _redoString;
            set {
                _redoString = value;
                OnPropertyChanged(nameof(RedoString));
            }
        }
        private bool _canUndo = false;
        public bool CanUndo {
            get => _canUndo;
            set {
                _canUndo = value;
                OnPropertyChanged(nameof(CanUndo));
            }
        }
        private bool _canRedo = false;
        public bool CanRedo {
            get => _canRedo;
            set {
                _canRedo = value;
                OnPropertyChanged(nameof(CanRedo));
            }
        }
        private string _selectedItem;
        public string SelectedItem {
            get => _selectedItem;
            set {
                if (_selectedItem != value) {
                    _selectedItem = value;
                    _parent.ManageEntriesOpenedWordlist = SelectedItem;
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
        private MainViewModel _parent;
        private string _openedWordlist;
        public ManageViewModel(MainViewModel parent, string openedWordlist) {
            VocabularyEntry.UpdateSpecialLists();
            _parent = parent;
            _openedWordlist = openedWordlist;
            List<WordlistsList> tempWordlists = WordlistsList.GetWordlistsList();
            foreach (WordlistsList temp in tempWordlists)
                if (temp.WordlistName != "Seen" &&
                    temp.WordlistName != "LastTimeWrong" &&
                    temp.WordlistName != "NotSeen") {
                    string tempString = $"{temp.WordlistName} ({temp.FirstLanguage}, {temp.SecondLanguage})";
                    ComboBoxWordlists.Add(tempString, temp);
                    ComboBoxEntries.Add(tempString);
                    UndoList.Add(temp, new List<(int, VocabularyEntry, VocabularyEntry, int, List<bool>)>());
                    RedoList.Add(temp, new List<(int, VocabularyEntry, VocabularyEntry, int, List<bool>)>());
                }
            if (_openedWordlist != "" && ComboBoxEntries.Contains(_openedWordlist))
                SelectedItem = ComboBoxEntries[ComboBoxEntries.IndexOf(_openedWordlist)];
            else
                SelectedItem = (ComboBoxEntries.Count > 0) ? ComboBoxEntries[ComboBoxEntries.Count - 1] : null;
        }
        private bool CanExecuteSearchCommand(object arg) {
            return true;
        }
        public ICommand SearchCommand => new RelayCommand(Search, CanExecuteSearchCommand);
        private void Search(object obj) {
            FillEntriesCollection();
            if (SearchingWord != "" && SearchingWord != "Searching...")
                for (int i = 0; i < SearchingWords.Count; i++)
                    if (!SearchingWords[i].FirstWord.ToLower().Contains(SearchingWord.ToLower()) && !SearchingWords[i].SecondWord.ToLower().Contains(SearchingWord.ToLower())) {
                        SearchingWords.Remove(SearchingWords[i]);
                        i--;
                    }
            AllEntriesCounter = SearchingWords.Count;
        }

        private bool CanExecuteUndoCommand(object arg) {
            return UndoList[ComboBoxWordlists[SelectedItem]].Count > 0;
        }
        public ICommand UndoCommand => new RelayCommand(Undo, CanExecuteUndoCommand);
        private void Undo(object obj) {
            int index = UndoList[ComboBoxWordlists[SelectedItem]][UndoList[ComboBoxWordlists[SelectedItem]].Count-1].index;
            VocabularyEntry beforeTempEntry = UndoList[ComboBoxWordlists[SelectedItem]][UndoList[ComboBoxWordlists[SelectedItem]].Count - 1].before;
            VocabularyEntry afterTempEntry = UndoList[ComboBoxWordlists[SelectedItem]][UndoList[ComboBoxWordlists[SelectedItem]].Count - 1].after;
            VocabularyEntry tempEntry = new VocabularyEntry();
            if (SelectedItem == "Marked (-, -)")
                tempEntry.FilePath = $"{VocabularyEntry.FirstPartFilePath}Marked{VocabularyEntry.SecondPartFilePath}";
            else
                tempEntry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{ComboBoxWordlists[SelectedItem].WordlistName}_{ComboBoxWordlists[SelectedItem].FirstLanguage}_{ComboBoxWordlists[SelectedItem].SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
            List<VocabularyEntry> tempList = VocabularyEntry.GetData(tempEntry);
            if (beforeTempEntry == afterTempEntry) {
                UpdateIndex(1, index);
                ManageEntryViewModel tempManageEntryView = new ManageEntryViewModel(SearchingWords, afterTempEntry, this, ComboBoxWordlists[SelectedItem], index, _parent);
                tempManageEntryView.CheckAvailability(beforeTempEntry);
                if (tempList.Count > 0) {
                    tempList.Insert(index, afterTempEntry);
                    SearchingWords.Insert(index, tempManageEntryView);
                } else {
                    tempList.Add(afterTempEntry);
                    SearchingWords.Add(tempManageEntryView);
                }

                for (int i = 0; i < UndoList[ComboBoxWordlists[SelectedItem]][UndoList[ComboBoxWordlists[SelectedItem]].Count - 1].availability.Count; i++) {
                    if (UndoList[ComboBoxWordlists[SelectedItem]][UndoList[ComboBoxWordlists[SelectedItem]].Count - 1].availability[i]) {
                        if (i == 0 && VocabularyEntry.EntriesSpecialWordlists[i].Count > 0) {
                            VocabularyEntry.EntriesSpecialWordlists[i].Insert(UndoList[ComboBoxWordlists[SelectedItem]][UndoList[ComboBoxWordlists[SelectedItem]].Count - 1].indexM, beforeTempEntry);
                        } else {
                            VocabularyEntry.EntriesSpecialWordlists[i].Add(beforeTempEntry);
                        }
                        VocabularyEntry.WriteData(VocabularyEntry.EntrySpecialWordlists[i], VocabularyEntry.EntriesSpecialWordlists[i]);
                    }
                }

            } else {
                tempList.Remove(tempList[index]);
                SearchingWords.Remove(SearchingWords[index]);
                ManageEntryViewModel tempManageEntryView = new ManageEntryViewModel(SearchingWords, beforeTempEntry, this, ComboBoxWordlists[SelectedItem], index, _parent);
                tempManageEntryView.CheckAvailability(afterTempEntry);
                if (tempList.Count > 0) {
                    tempList.Insert(index, beforeTempEntry);
                    SearchingWords.Insert(index, tempManageEntryView);
                } else {
                    tempList.Add(beforeTempEntry);
                    SearchingWords.Add(tempManageEntryView);
                }

                for (int i = 0; i < UndoList[ComboBoxWordlists[SelectedItem]][UndoList[ComboBoxWordlists[SelectedItem]].Count - 1].availability.Count; i++) {
                    if (UndoList[ComboBoxWordlists[SelectedItem]][UndoList[ComboBoxWordlists[SelectedItem]].Count - 1].availability[i]) {
                        VocabularyEntry.EntriesSpecialWordlists[i].Remove(afterTempEntry);
                        VocabularyEntry.EntriesSpecialWordlists[i].Add(beforeTempEntry);
                        VocabularyEntry.WriteData(VocabularyEntry.EntrySpecialWordlists[i], VocabularyEntry.EntriesSpecialWordlists[i]);
                    }
                }
            }
            VocabularyEntry.WriteData(tempEntry, tempList);
            UpdateViewModels(ComboBoxWordlists[SelectedItem]);
            RedoList[ComboBoxWordlists[SelectedItem]].Add(UndoList[ComboBoxWordlists[SelectedItem]][UndoList[ComboBoxWordlists[SelectedItem]].Count - 1]);
            UndoList[ComboBoxWordlists[SelectedItem]].Remove(UndoList[ComboBoxWordlists[SelectedItem]][UndoList[ComboBoxWordlists[SelectedItem]].Count - 1]);
        }

        private bool CanExecuteRedoCommand(object arg) {
            return RedoList[ComboBoxWordlists[SelectedItem]].Count > 0;
        }
        public ICommand RedoCommand => new RelayCommand(Redo, CanExecuteRedoCommand);
        private void Redo(object obj) {
            int index = RedoList[ComboBoxWordlists[SelectedItem]][RedoList[ComboBoxWordlists[SelectedItem]].Count - 1].index;
            VocabularyEntry beforeTempEntry = RedoList[ComboBoxWordlists[SelectedItem]][RedoList[ComboBoxWordlists[SelectedItem]].Count - 1].before;
            VocabularyEntry afterTempEntry = RedoList[ComboBoxWordlists[SelectedItem]][RedoList[ComboBoxWordlists[SelectedItem]].Count - 1].after;
            VocabularyEntry tempEntry = new VocabularyEntry();
            if (SelectedItem == "Marked (-, -)")
                tempEntry.FilePath = $"{VocabularyEntry.FirstPartFilePath}Marked{VocabularyEntry.SecondPartFilePath}";
            else
                tempEntry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{ComboBoxWordlists[SelectedItem].WordlistName}_{ComboBoxWordlists[SelectedItem].FirstLanguage}_{ComboBoxWordlists[SelectedItem].SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
            List<VocabularyEntry> tempList = VocabularyEntry.GetData(tempEntry);
            tempList.Remove(tempList[index]);

            SearchingWords.Remove(SearchingWords[index]);
            ManageEntryViewModel tempManageEntryView = new ManageEntryViewModel(SearchingWords, afterTempEntry, this, ComboBoxWordlists[SelectedItem], index, _parent);
            tempManageEntryView.CheckAvailability(beforeTempEntry);
            if (beforeTempEntry != afterTempEntry) {
                if (tempList.Count > 0) {
                    tempList.Insert(index, afterTempEntry);
                    SearchingWords.Insert(index, tempManageEntryView);
                } else {
                    tempList.Add(afterTempEntry);
                    SearchingWords.Add(tempManageEntryView);
                }
            } else
                UpdateIndex(-1, index);
            VocabularyEntry.WriteData(tempEntry, tempList);

            for (int i = 0; i < RedoList[ComboBoxWordlists[SelectedItem]][RedoList[ComboBoxWordlists[SelectedItem]].Count - 1].availability.Count; i++) {
                if (RedoList[ComboBoxWordlists[SelectedItem]][RedoList[ComboBoxWordlists[SelectedItem]].Count - 1].availability[i]) {
                    VocabularyEntry.EntriesSpecialWordlists[i].Remove(beforeTempEntry);
                    if (beforeTempEntry != afterTempEntry) { 
                        if (i == 0 && VocabularyEntry.EntriesSpecialWordlists[i].Count > 0)
                            VocabularyEntry.EntriesSpecialWordlists[i].Insert(RedoList[ComboBoxWordlists[SelectedItem]][RedoList[ComboBoxWordlists[SelectedItem]].Count - 1].indexM, afterTempEntry);
                        else
                            VocabularyEntry.EntriesSpecialWordlists[i].Add(afterTempEntry);
                    }
                    VocabularyEntry.WriteData(VocabularyEntry.EntrySpecialWordlists[i], VocabularyEntry.EntriesSpecialWordlists[i]);
                }
            }
            UpdateViewModels(ComboBoxWordlists[SelectedItem]);
            UndoList[ComboBoxWordlists[SelectedItem]].Add(RedoList[ComboBoxWordlists[SelectedItem]][RedoList[ComboBoxWordlists[SelectedItem]].Count - 1]);
            RedoList[ComboBoxWordlists[SelectedItem]].Remove(RedoList[ComboBoxWordlists[SelectedItem]][RedoList[ComboBoxWordlists[SelectedItem]].Count - 1]);
        }

        public void UpdateIndex(int number, int index) {
            for (int i = index; i < SearchingWords.Count; i++)
                SearchingWords[i].Index += number;
        }

        private void FillEntriesCollection() {
            SearchingWords = new ObservableCollection<ManageEntryViewModel>();
            VocabularyEntry entry = new VocabularyEntry();
            if (ComboBoxWordlists[SelectedItem].WordlistName != "Marked")
                entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{ComboBoxWordlists[SelectedItem].WordlistName}_{ComboBoxWordlists[SelectedItem].FirstLanguage}_{ComboBoxWordlists[SelectedItem].SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
            else
                entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{ComboBoxWordlists[SelectedItem].WordlistName}{VocabularyEntry.SecondPartFilePath}";

            List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
            int index = 0;
            foreach (VocabularyEntry tempEntry in entries) {
                SearchingWords.Add(new ManageEntryViewModel(SearchingWords, tempEntry, this, ComboBoxWordlists[SelectedItem], index, _parent));
                index++;
            }
        }

        public void UpdateViewModels(WordlistsList wordlist) {
            if (_parent.ListWordlist.Contains(wordlist) && _parent.ListWordlist[_parent.ListWordlist.IndexOf(wordlist)].IsTrue) {
                _parent.LearnEntriesViewModel = new LearnViewModel(_parent);
                _parent.SettingsViewModel = new SettingsViewModel(_parent);
            }
        }
    }
}
