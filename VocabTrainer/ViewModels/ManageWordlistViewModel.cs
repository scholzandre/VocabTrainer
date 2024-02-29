using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class ManageWordlistViewModel : BaseViewModel {
        private static readonly string _normalTextboxBackground = "white";
        private static readonly string _deleteTextboxBackground = "red";
        private string _backgroundTextbox = _normalTextboxBackground;
        public string BackgroundTextbox {
            get => _backgroundTextbox;
            set {
                _backgroundTextbox = value;
                OnPropertyChanged(nameof(BackgroundTextbox));
            }
        }
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
        private static readonly List<VocabularyEntry> _entrySpecialLists = new List<VocabularyEntry> {
            new VocabularyEntry() { FilePath =  $"{VocabularyEntry.FirstPartFilePath}Marked{VocabularyEntry.SecondPartFilePath}" },
            new VocabularyEntry() { FilePath =  $"{VocabularyEntry.FirstPartFilePath}Seen{VocabularyEntry.SecondPartFilePath}" },
            new VocabularyEntry() { FilePath =  $"{VocabularyEntry.FirstPartFilePath}NotSeen{VocabularyEntry.SecondPartFilePath}" },
            new VocabularyEntry() { FilePath =  $"{VocabularyEntry.FirstPartFilePath}LastTimeWrong{VocabularyEntry.SecondPartFilePath}" }
        };

        private readonly List<List<VocabularyEntry>> _entriesSpecialLists = new List<List<VocabularyEntry>> {
            VocabularyEntry.GetData(_entrySpecialLists[0]),
            VocabularyEntry.GetData(_entrySpecialLists[1]),
            VocabularyEntry.GetData(_entrySpecialLists[2]),
            VocabularyEntry.GetData(_entrySpecialLists[3])
        };
        ManageWordlistsViewModel _parent;
        public int Index { get; set; }
        public ManageWordlistViewModel(ManageWordlistsViewModel parent, WordlistsList wordlist, List<WordlistsList> allWordlists, ObservableCollection<ManageWordlistViewModel> views, int index) {
            _parent = parent;
            Index = index;
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
                VocabularyEntry.UpdateSpecialLists();
                Writable = false;
                EditButtonText = ButtonIcons.GetIconString(IconType.Edit);
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
                if (File.Exists($"{VocabularyEntry.FirstPartFilePath}{_original.WordlistName}_{_original.FirstLanguage}_{_original.SecondLanguage}{VocabularyEntry.SecondPartFilePath}")) {
                    File.Move($"{VocabularyEntry.FirstPartFilePath}{_original.WordlistName}_{_original.FirstLanguage}_{_original.SecondLanguage}{VocabularyEntry.SecondPartFilePath}",
                              $"{VocabularyEntry.FirstPartFilePath}{WordlistName}_{FirstLanguage}_{SecondLanguage}{VocabularyEntry.SecondPartFilePath}");
                }
                for (int i = 0; i < VocabularyEntry.EntriesSpecialWordlists.Count; i++) {
                    for (int j = 0; j < VocabularyEntry.EntriesSpecialWordlists[i].Count; j++) {
                        if (VocabularyEntry.EntriesSpecialWordlists[i][j].WordList == Wordlist.WordlistName &&
                            VocabularyEntry.EntriesSpecialWordlists[i][j].FirstLanguage == Wordlist.FirstLanguage &&
                            VocabularyEntry.EntriesSpecialWordlists[i][j].SecondLanguage == Wordlist.SecondLanguage) {
                            VocabularyEntry.EntriesSpecialWordlists[i][j].WordList = WordlistName;
                            VocabularyEntry.EntriesSpecialWordlists[i][j].FirstLanguage = FirstLanguage;
                            VocabularyEntry.EntriesSpecialWordlists[i][j].SecondLanguage = SecondLanguage;
                        }
                    }
                    VocabularyEntry.WriteData(VocabularyEntry.EntrySpecialWordlists[i], VocabularyEntry.EntriesSpecialWordlists[i]);
                }
                WordlistsList firstTempList = new WordlistsList() { 
                    WordlistName = Wordlist.WordlistName,
                    FirstLanguage = Wordlist.FirstLanguage,
                    SecondLanguage = Wordlist.SecondLanguage,
                };
                WordlistsList secondTempList = new WordlistsList() {
                    WordlistName = WordlistName,
                    FirstLanguage = FirstLanguage,
                    SecondLanguage = SecondLanguage,
                };
                Wordlist.WordlistName = WordlistName;
                Wordlist.FirstLanguage = FirstLanguage;
                Wordlist.SecondLanguage = SecondLanguage;
                _parent.UndoList.Add((Index, firstTempList, secondTempList, new List<(bool, VocabularyEntry)>()));
                _parent.RedoList = new List<(int index, WordlistsList before, WordlistsList after, List<(bool, VocabularyEntry)>)>();

                VocabularyEntry entry = new VocabularyEntry() { 
                    FilePath = $"{VocabularyEntry.FirstPartFilePath}{WordlistName}_{FirstLanguage}_{SecondLanguage}{VocabularyEntry.SecondPartFilePath}"
                };
                List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
                for (int i = 0; i < entries.Count; i++) {
                    entries[i].WordList = WordlistName;
                    entries[i].FirstLanguage = FirstLanguage;
                    entries[i].SecondLanguage = SecondLanguage;
                }
                VocabularyEntry.WriteData(entry, entries);
                
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
                WordlistsList.WriteWordlistsList(AllWordlists);
                _parent.UpdateViewModels(Wordlist);
            } else {
                BackgroundTextbox = _normalTextboxBackground;
                EditButtonText = ButtonIcons.GetIconString(IconType.Edit);
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
            }
        }
        public ICommand DeleteEntryCommand => new RelayCommand(DeleteEntry, CanExecuteCommand);
        private void DeleteEntry(object obj) {
            if (DeleteButtonText == ButtonIcons.GetIconString(IconType.Delete)) {
                BackgroundTextbox = _deleteTextboxBackground;
                EditButtonText = ButtonIcons.GetIconString(IconType.Cancel);
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Approve);
            } else if (DeleteButtonText == ButtonIcons.GetIconString(IconType.Approve)) {
                VocabularyEntry.UpdateSpecialLists();
                if (!_parent.DeletedWordlists.ContainsKey(Wordlist))
                    _parent.DeletedWordlists.Add(Wordlist, VocabularyEntry.GetData(new VocabularyEntry() { 
                        FilePath = $"{VocabularyEntry.FirstPartFilePath}{WordlistName}_{FirstLanguage}_{SecondLanguage}{VocabularyEntry.SecondPartFilePath}"
                    }));
                WordlistsList tempList = new WordlistsList() {
                    WordlistName = WordlistName,
                    FirstLanguage = FirstLanguage,
                    SecondLanguage = SecondLanguage,
                };
                VocabularyEntry tempEntry = new VocabularyEntry() {
                    FilePath = $"{VocabularyEntry.FirstPartFilePath}{WordlistName}_{FirstLanguage}_{SecondLanguage}{VocabularyEntry.SecondPartFilePath}"
                };
                List<VocabularyEntry> entries = VocabularyEntry.GetData(tempEntry);
                List<(bool, VocabularyEntry)> entriesUndo = new List<(bool, VocabularyEntry)>();
                foreach (VocabularyEntry tempUndoEntry in entries) {
                    if (VocabularyEntry.EntriesSpecialWordlists[0].Contains(tempUndoEntry))
                        entriesUndo.Add((true, tempUndoEntry));
                    else
                        entriesUndo.Add((false, tempUndoEntry));
                }

                _parent.UndoList.Add((Index, tempList, tempList, entriesUndo));
                _parent.RedoList = new List<(int index, WordlistsList before, WordlistsList after, List<(bool, VocabularyEntry)>)>();
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
                EditButtonText = ButtonIcons.GetIconString(IconType.Edit);
                int tempIndex = 0;
                for (int i = 0; i < _views.Count; i++) 
                    if (_views[i].Wordlist.WordlistName == _original.WordlistName &&
                        _views[i].Wordlist.FirstLanguage == _original.FirstLanguage &&
                        _views[i].Wordlist.SecondLanguage == _original.SecondLanguage) {
                        tempIndex = i;
                        _views.Remove(this);
                        AllWordlists.Remove(_original);
                        break;
                    }
                for (; tempIndex < _views.Count; tempIndex++) {
                    _views[tempIndex].Index = _views[tempIndex].Index-1;
                }
                for (int i = 0; i < entries.Count; i++) 
                    for (int j = 0; j < _entriesSpecialLists.Count; j++) 
                        _entriesSpecialLists[j].Remove(entries[i]);

                for (int i = 0; i < _entrySpecialLists.Count; i++)
                    VocabularyEntry.WriteData(_entrySpecialLists[i], _entriesSpecialLists[i]);
                
                if (File.Exists($"{VocabularyEntry.FirstPartFilePath}{WordlistName}_{FirstLanguage}_{SecondLanguage}{VocabularyEntry.SecondPartFilePath}"))
                    File.Delete($"{VocabularyEntry.FirstPartFilePath}{WordlistName}_{FirstLanguage}_{SecondLanguage}{VocabularyEntry.SecondPartFilePath}");
                WordlistsList.WriteWordlistsList(AllWordlists);
                _parent.UpdateViewModels(Wordlist);
            } else {
                Writable = false;
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
                EditButtonText = ButtonIcons.GetIconString(IconType.Edit);
                SetOriginalValues();
            }
        }
    }
}
