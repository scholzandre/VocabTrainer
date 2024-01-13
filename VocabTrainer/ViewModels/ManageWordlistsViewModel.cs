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
            foreach (WordlistsList wordlist in AllWordlists) 
                Wordlists.Add(new ManageWordlistViewModel(wordlist, AllWordlists, Wordlists));
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
            return true;
        }
        public ICommand UndoCommand => new RelayCommand(Undo, CanExecuteUndoCommand);
        private void Undo(object obj) {
        }
        private bool CanExecuteRedoCommand(object arg) {
            return true;
        }
        public ICommand RedoCommand => new RelayCommand(Redo, CanExecuteRedoCommand);
        private void Redo(object obj) {
        }
    }
}
