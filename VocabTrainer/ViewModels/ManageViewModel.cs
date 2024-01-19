﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
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
        private List<(int index, VocabularyEntry, VocabularyEntry)> _undoList = new List<(int, VocabularyEntry, VocabularyEntry)>();
        public List<(int index, VocabularyEntry before, VocabularyEntry after)> UndoList {
            get => _undoList;
            set {
                _undoList = value;
                OnPropertyChanged(nameof(UndoList));
            }
        }
        private List<(int index, VocabularyEntry, VocabularyEntry)> _redoList = new List<(int, VocabularyEntry, VocabularyEntry)>();
        public List<(int index, VocabularyEntry before, VocabularyEntry after)> RedoList {
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
                    _parent.ManageEntryOpenedWordlist = SelectedItem;
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
            return UndoList.Count > 0;
        }
        public ICommand UndoCommand => new RelayCommand(Undo, CanExecuteUndoCommand);
        private void Undo(object obj) {
            int index = UndoList[UndoList.Count - 1].index;
            VocabularyEntry beforeTempEntry = UndoList[UndoList.Count - 1].before;
            VocabularyEntry afterTempEntry = UndoList[UndoList.Count - 1].after;
            WordlistsList tempWordlist = new WordlistsList {
                WordlistName = $"{afterTempEntry.WordList}_{afterTempEntry.FirstLanguage}_{afterTempEntry.SecondLanguage}"
            };
            if (beforeTempEntry == afterTempEntry) { 
                SearchingWords.Insert(index, new ManageEntryViewModel(SearchingWords, afterTempEntry, this, tempWordlist, index));
                UndoList.Remove(UndoList[UndoList.Count - 1]);
            }
        }

        private bool CanExecuteRedoCommand(object arg) {
            return RedoList.Count > 0;
        }
        public ICommand RedoCommand => new RelayCommand(Redo, CanExecuteRedoCommand);
        private void Redo(object obj) {
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
                WordlistsList tempWordlist = new WordlistsList() {
                    WordlistName = $"{ComboBoxWordlists[SelectedItem].WordlistName}_{ComboBoxWordlists[SelectedItem].FirstLanguage}_{ComboBoxWordlists[SelectedItem].SecondLanguage}"
                };
                SearchingWords.Add(new ManageEntryViewModel(SearchingWords, tempEntry, this, tempWordlist, index));
                index++;
            }
        }
    }
}
