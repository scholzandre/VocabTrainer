using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class ManageWordlistViewModel : BaseViewModel {
        private string _editButtonText;
        public string EditButtonText {
            get => _editButtonText;
            set {
                _editButtonText = value;
                OnPropertyChanged(nameof(EditButtonText));
            }
        }

        private string _deleteButtonText;
        public string DeleteButtonText {
            get => _deleteButtonText;
            set {
                _deleteButtonText = value;
                OnPropertyChanged(nameof(DeleteButtonText));
            }
        }

        private WordlistsList _wordlist;
        public WordlistsList Wordlist {
            get => _wordlist;
            set {
                _wordlist = value;
                OnPropertyChanged(nameof(Wordlist));
            }
        }

        private string _wordlistName;
        public string WordlistName {
            get => _wordlistName;
            set {
                _wordlistName = value;
                OnPropertyChanged(nameof(WordlistName));
            }
        }

        private string _firstLanguage;
        public string FirstLanguage {
            get => _firstLanguage;
            set {
                _firstLanguage = value;
                OnPropertyChanged(nameof(FirstLanguage));
            }
        }

        private string _secondLanguage;
        public string SecondLanguage {
            get => _secondLanguage;
            set {
                _secondLanguage = value;
                OnPropertyChanged(nameof(SecondLanguage));
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

        private bool _editable;
        public bool Editable {
            get => _editable;
            set {
                _editable = value;
                OnPropertyChanged(nameof(Editable));
            }
        }
        private bool _writable;
        public bool Writable {
            get => _writable;
            set {
                _writable = value;
                OnPropertyChanged(nameof(Writable));
            }
        }
        private bool _deletable;
        public bool Deletable {
            get => _deletable;
            set {
                _deletable = value;
                OnPropertyChanged(nameof(Deletable));
            }
        }
        private WordlistsList _original;
        ObservableCollection<ManageWordlistViewModel> _views;
        public ManageWordlistViewModel(WordlistsList wordlist, List<WordlistsList> allWordlists, ObservableCollection<ManageWordlistViewModel> views) {
            _original = wordlist;
            Wordlist = wordlist;
            SetOriginalValues();
            AllWordlists = allWordlists;
            _views = views;
            Writable = false;
            EditButtonText = ButtonIcons.GetIconString(IconType.Edit);
            DeleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
            if (Wordlist.WordlistName == "Marked" ||
                        Wordlist.WordlistName == "Seen" ||
                        Wordlist.WordlistName == "NotSeen" ||
                        Wordlist.WordlistName == "LastTimeWrong") {
                Editable = false;
                Deletable = false;
            } else {
                Editable = true;
                Deletable = true;
            }
        }

        private void SetOriginalValues() {
            WordlistName = _original.WordlistName;
            FirstLanguage = _original.FirstLanguage;
            SecondLanguage = _original.SecondLanguage;
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand ChangeTextCommand => new RelayCommand(ChangeText, CanExecuteCommand);
        private void ChangeText(object obj) {
            if (EditButtonText == ButtonIcons.GetIconString(IconType.Edit)) {
                Writable = true;
                EditButtonText = ButtonIcons.GetIconString(IconType.Save);
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Cancel);
            } else if (EditButtonText == ButtonIcons.GetIconString(IconType.Save)) {
                Writable = false;
                EditButtonText = ButtonIcons.GetIconString(IconType.Edit);
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
                if (File.Exists($"{VocabularyEntry.FirstPartFilePath}{_original.WordlistName}_{_original.FirstLanguage}_{_original.SecondLanguage}{VocabularyEntry.SecondPartFilePath}")) {
                    File.Move($"{VocabularyEntry.FirstPartFilePath}{_original.WordlistName}_{_original.FirstLanguage}_{_original.SecondLanguage}{VocabularyEntry.SecondPartFilePath}",
                              $"{VocabularyEntry.FirstPartFilePath}{WordlistName}_{FirstLanguage}_{SecondLanguage}{VocabularyEntry.SecondPartFilePath}");
                }
                Wordlist.WordlistName = WordlistName;
                Wordlist.FirstLanguage = FirstLanguage;
                Wordlist.SecondLanguage = SecondLanguage;
                for (int i = 0; i < AllWordlists.Count; i++) 
                    if (AllWordlists[i] == _original) {
                        AllWordlists[i] = Wordlist;
                        break;
                    }
                for (int i = 0; i < _views.Count; i++) 
                    if (_views[i].Wordlist.WordlistName == _original.WordlistName &&
                        _views[i].Wordlist.FirstLanguage == _original.FirstLanguage &&
                        _views[i].Wordlist.SecondLanguage == _original.SecondLanguage) {
                        _views[i].Wordlist = Wordlist;
                        break;
                    }
            } else {
                EditButtonText = ButtonIcons.GetIconString(IconType.Edit);
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
            }
            WordlistsList.WriteWordlistsList(AllWordlists);
        }
        public ICommand DeleteEntryCommand => new RelayCommand(DeleteEntry, CanExecuteCommand);
        private void DeleteEntry(object obj) {
            if (DeleteButtonText == ButtonIcons.GetIconString(IconType.Delete)) {
                EditButtonText = ButtonIcons.GetIconString(IconType.Cancel);
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Approve);
            } else if (DeleteButtonText == ButtonIcons.GetIconString(IconType.Approve)) {
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
                EditButtonText = ButtonIcons.GetIconString(IconType.Edit);
                for (int i = 0; i < _views.Count; i++) 
                    if (_views[i].Wordlist.WordlistName == _original.WordlistName &&
                        _views[i].Wordlist.FirstLanguage == _original.FirstLanguage &&
                        _views[i].Wordlist.SecondLanguage == _original.SecondLanguage) {
                        _views.Remove(this);
                        AllWordlists.Remove(_original);
                        break;
                    }
                if (File.Exists($"{VocabularyEntry.FirstPartFilePath}{WordlistName}_{FirstLanguage}_{SecondLanguage}{VocabularyEntry.SecondPartFilePath}"))
                    File.Delete($"{VocabularyEntry.FirstPartFilePath}{WordlistName}_{FirstLanguage}_{SecondLanguage}{VocabularyEntry.SecondPartFilePath}");
            } else {
                Writable = false;
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
                EditButtonText = ButtonIcons.GetIconString(IconType.Edit);
                SetOriginalValues();
            }
            WordlistsList.WriteWordlistsList(AllWordlists);
        }
    }
}
