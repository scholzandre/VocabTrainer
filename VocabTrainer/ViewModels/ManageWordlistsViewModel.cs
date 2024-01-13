using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using VocabTrainer.Models;

namespace VocabTrainer.ViewModels {
    public class ManageWordlistsViewModel : BaseViewModel {
        private ObservableCollection<ManageWordlistViewModel> _wordlists = new ObservableCollection<ManageWordlistViewModel>();
        public ObservableCollection<ManageWordlistViewModel> Wordlists {
            get => _wordlists;
            set {
                _wordlists = value;
                OnPropertyChanged(nameof(Wordlists));
            }
        }
        private List<(int index, WordlistsList, WordlistsList)> _undoList = new List<(int, WordlistsList, WordlistsList)>();
        public List<(int index, WordlistsList before, WordlistsList after)> UndoList {
            get => _undoList;
            set {
                _undoList = value;
                OnPropertyChanged(nameof(UndoList));
            }
        }
        private List<(int index, WordlistsList, WordlistsList)> _redoList = new List<(int, WordlistsList, WordlistsList)>();
        public List<(int index, WordlistsList before, WordlistsList after)> RedoList {
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
        private List<WordlistsList> _allWordlists;
        public List<WordlistsList> AllWordlists {
            get => _allWordlists;
            set {
                _allWordlists = value;
                OnPropertyChanged(nameof(AllWordlists));
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
        public ManageWordlistsViewModel() {
            FillList();
        }

        private void FillList() {
            AllWordlists = new List<WordlistsList>();
            Wordlists = new ObservableCollection<ManageWordlistViewModel>();
            AllWordlists = WordlistsList.GetWordlistsList();
            int counter = 0;
            foreach (WordlistsList wordlist in AllWordlists) {
                Wordlists.Add(new ManageWordlistViewModel(this, wordlist, AllWordlists, Wordlists, counter));
                counter++;
            }
        }

        private bool CanExecuteSearchCommand(object arg) {
            return true;
        }
        public ICommand SearchCommand => new RelayCommand(Search, CanExecuteSearchCommand);
        private void Search(object obj) {
            FillList();
            if (SearchingWord != "" && SearchingWord != "Searching...")
                for (int i = 0; i < Wordlists.Count; i++)
                    if (!Wordlists[i].WordlistName.ToLower().Contains(SearchingWord.ToLower()) &&
                        !Wordlists[i].FirstLanguage.ToLower().Contains(SearchingWord.ToLower()) &&
                        !Wordlists[i].SecondLanguage.ToLower().Contains(SearchingWord.ToLower())) {
                        Wordlists.Remove(Wordlists[i]);
                        i--;
                    }
        }
        private bool CanExecuteUndoCommand(object arg) {
            return UndoList.Count > 0;
        }
        public ICommand UndoCommand => new RelayCommand(Undo, CanExecuteUndoCommand);
        private void Undo(object obj) {
            if (UndoList[UndoList.Count - 1].before == UndoList[UndoList.Count - 1].after) {
                for (int i = UndoList[UndoList.Count - 1].index; i < Wordlists.Count; i++)
                    Wordlists[i].Index = Wordlists[i].Index + 1;
                Wordlists.Insert(UndoList[UndoList.Count - 1].index, new ManageWordlistViewModel(this, UndoList[UndoList.Count - 1].before, AllWordlists, Wordlists, UndoList[UndoList.Count - 1].index));
            } else {
                Wordlists.Remove(Wordlists[UndoList[UndoList.Count - 1].index]);
                Wordlists.Insert(UndoList[UndoList.Count - 1].index, new ManageWordlistViewModel(this, UndoList[UndoList.Count - 1].before, AllWordlists, Wordlists, UndoList[UndoList.Count - 1].index));

            }
            RedoList.Add(UndoList[UndoList.Count - 1]);
            UndoList.Remove(UndoList[UndoList.Count - 1]);
        }
        private bool CanExecuteRedoCommand(object arg) {
            return RedoList.Count > 0;
        }
        public ICommand RedoCommand => new RelayCommand(Redo, CanExecuteRedoCommand);
        private void Redo(object obj) {
            if (RedoList[RedoList.Count - 1].before == RedoList[RedoList.Count - 1].after) {
                for (int i = RedoList[RedoList.Count - 1].index; i < Wordlists.Count; i++)
                    Wordlists[i].Index = Wordlists[i].Index - 1;
                Wordlists.Remove(Wordlists[RedoList[RedoList.Count - 1].index]);
            } else {
                Wordlists.Remove(Wordlists[RedoList[RedoList.Count - 1].index]);
                Wordlists.Insert(RedoList[RedoList.Count - 1].index, new ManageWordlistViewModel(this, RedoList[RedoList.Count - 1].after, AllWordlists, Wordlists, RedoList[RedoList.Count - 1].index));

            }
            UndoList.Add(RedoList[RedoList.Count - 1]);
            RedoList.Remove(RedoList[RedoList.Count - 1]);
        }
    }
}
