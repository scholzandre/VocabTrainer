using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

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
        public Dictionary<WordlistsList, List<VocabularyEntry>> DeletedWordlists { get; set; } = new Dictionary<WordlistsList, List<VocabularyEntry>>();
        private List<(int index, WordlistsList, WordlistsList, List<(bool, VocabularyEntry)>)> _undoList = new List<(int, WordlistsList, WordlistsList, List<(bool, VocabularyEntry)>)>();
        public List<(int index, WordlistsList before, WordlistsList after, List<(bool, VocabularyEntry)> Entries)> UndoList {
            get => _undoList;
            set {
                _undoList = value;
                OnPropertyChanged(nameof(UndoList));
            }
        }
        private List<(int index, WordlistsList, WordlistsList, List<(bool, VocabularyEntry)>)> _redoList = new List<(int, WordlistsList, WordlistsList, List<(bool, VocabularyEntry)>)>();
        public List<(int index, WordlistsList before, WordlistsList after, List<(bool, VocabularyEntry)> Entries)> RedoList {
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
        MainViewModel _parent;
        public ManageWordlistsViewModel(MainViewModel parent) {
            _parent = parent;
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
            VocabularyEntry.UpdateSpecialLists();
            List<WordlistsList> allWordlists = WordlistsList.GetWordlistsList();
            if (UndoList[UndoList.Count - 1].before == UndoList[UndoList.Count - 1].after) {
                for (int i = UndoList[UndoList.Count - 1].index; i < Wordlists.Count; i++)
                    Wordlists[i].Index = Wordlists[i].Index + 1;
                Wordlists.Insert(UndoList[UndoList.Count - 1].index, new ManageWordlistViewModel(this, UndoList[UndoList.Count - 1].before, AllWordlists, Wordlists, UndoList[UndoList.Count - 1].index));
                WordlistsList wordlist = UndoList[UndoList.Count - 1].after;
                VocabularyEntry tempEntry = new VocabularyEntry() {
                    FilePath = $"{VocabularyEntry.FirstPartFilePath}{wordlist.WordlistName}_{wordlist.FirstLanguage}_{wordlist.SecondLanguage}{VocabularyEntry.SecondPartFilePath}"
                };
                VocabularyEntry.WriteData(tempEntry, DeletedWordlists[wordlist]);
                foreach ((bool boolean, VocabularyEntry entry) in UndoList[UndoList.Count-1].Entries) {
                    if (boolean && !VocabularyEntry.EntriesSpecialWordlists[0].Contains(entry))
                        VocabularyEntry.EntriesSpecialWordlists[0].Add(entry);

                    if (entry.Seen && !VocabularyEntry.EntriesSpecialWordlists[1].Contains(entry))
                        VocabularyEntry.EntriesSpecialWordlists[1].Add(entry);
                    else if (!entry.Seen && !VocabularyEntry.EntriesSpecialWordlists[2].Contains(entry))
                        VocabularyEntry.EntriesSpecialWordlists[2].Add(entry);
                    else if (!VocabularyEntry.EntriesSpecialWordlists[3].Contains(entry))
                        VocabularyEntry.EntriesSpecialWordlists[3].Add(entry);
                }
                for (int i = 0; i < VocabularyEntry.EntriesSpecialWordlists.Count; i++)
                    VocabularyEntry.WriteData(VocabularyEntry.EntrySpecialWordlists[i], VocabularyEntry.EntriesSpecialWordlists[i]);

                allWordlists.Insert(UndoList[UndoList.Count - 1].index, wordlist);
            } else {
                allWordlists.Remove(UndoList[UndoList.Count - 1].after);
                allWordlists.Insert(UndoList[UndoList.Count - 1].index, UndoList[UndoList.Count - 1].before);
                Wordlists.Remove(Wordlists[UndoList[UndoList.Count - 1].index]);
                Wordlists.Insert(UndoList[UndoList.Count - 1].index, new ManageWordlistViewModel(this, UndoList[UndoList.Count - 1].before, AllWordlists, Wordlists, UndoList[UndoList.Count - 1].index));
                string oldFilePath = $"{VocabularyEntry.FirstPartFilePath}{UndoList[UndoList.Count - 1].after.WordlistName}_{UndoList[UndoList.Count - 1].after.FirstLanguage}_{UndoList[UndoList.Count - 1].after.SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
                string newFilePath = $"{VocabularyEntry.FirstPartFilePath}{UndoList[UndoList.Count - 1].before.WordlistName}_{UndoList[UndoList.Count - 1].before.FirstLanguage}_{UndoList[UndoList.Count - 1].before.SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
                File.Move(oldFilePath, newFilePath);
                VocabularyEntry tempEntry = new VocabularyEntry() {
                    FilePath = newFilePath
                };
                List<VocabularyEntry> tempList = VocabularyEntry.GetData(tempEntry);
                for (int i = 0; i < tempList.Count; i++) {
                    tempList[i].WordList = UndoList[UndoList.Count - 1].before.WordlistName;
                    tempList[i].FirstLanguage = UndoList[UndoList.Count - 1].before.FirstLanguage;
                    tempList[i].SecondLanguage = UndoList[UndoList.Count - 1].before.SecondLanguage;
                }
                VocabularyEntry.WriteData(tempEntry, tempList);
            }
            WordlistsList.WriteWordlistsList(allWordlists);
            UpdateViewModels(UndoList[UndoList.Count - 1].after);
            RedoList.Add(UndoList[UndoList.Count - 1]);
            UndoList.Remove(UndoList[UndoList.Count - 1]);
        }
        private bool CanExecuteRedoCommand(object arg) {
            return RedoList.Count > 0;
        }
        public ICommand RedoCommand => new RelayCommand(Redo, CanExecuteRedoCommand);
        private void Redo(object obj) {
            List<WordlistsList> allWordlists = WordlistsList.GetWordlistsList();
            if (RedoList[RedoList.Count - 1].before == RedoList[RedoList.Count - 1].after) {
                for (int i = RedoList[RedoList.Count - 1].index; i < Wordlists.Count; i++)
                    Wordlists[i].Index = Wordlists[i].Index - 1;
                Wordlists.Remove(Wordlists[RedoList[RedoList.Count - 1].index]);
                allWordlists.Remove(RedoList[RedoList.Count - 1].before);
                File.Delete($"{VocabularyEntry.FirstPartFilePath}{RedoList[RedoList.Count - 1].before.WordlistName}_{RedoList[RedoList.Count - 1].before.FirstLanguage}_{RedoList[RedoList.Count - 1].before.SecondLanguage}{VocabularyEntry.SecondPartFilePath}");
                foreach ((bool boolean, VocabularyEntry entry) in RedoList[RedoList.Count - 1].Entries) {
                    if (boolean && VocabularyEntry.EntriesSpecialWordlists[0].Contains(entry))
                        VocabularyEntry.EntriesSpecialWordlists[0].Remove(entry);

                    if (entry.Seen && VocabularyEntry.EntriesSpecialWordlists[1].Contains(entry))
                        VocabularyEntry.EntriesSpecialWordlists[1].Remove(entry);
                    else if (!entry.Seen && VocabularyEntry.EntriesSpecialWordlists[2].Contains(entry))
                        VocabularyEntry.EntriesSpecialWordlists[2].Remove(entry);
                    else if (VocabularyEntry.EntriesSpecialWordlists[3].Contains(entry))
                        VocabularyEntry.EntriesSpecialWordlists[3].Remove(entry);
                }
                for (int i = 0; i < VocabularyEntry.EntriesSpecialWordlists.Count; i++)
                    VocabularyEntry.WriteData(VocabularyEntry.EntrySpecialWordlists[i], VocabularyEntry.EntriesSpecialWordlists[i]);
            } else {
                allWordlists.Remove(RedoList[RedoList.Count - 1].before);
                allWordlists.Insert(RedoList[RedoList.Count - 1].index, RedoList[RedoList.Count - 1].after);
                Wordlists.Remove(Wordlists[RedoList[RedoList.Count - 1].index]);
                Wordlists.Insert(RedoList[RedoList.Count - 1].index, new ManageWordlistViewModel(this, RedoList[RedoList.Count - 1].after, AllWordlists, Wordlists, RedoList[RedoList.Count - 1].index));
                string oldFilePath = $"{VocabularyEntry.FirstPartFilePath}{RedoList[RedoList.Count - 1].before.WordlistName}_{RedoList[RedoList.Count - 1].before.FirstLanguage}_{RedoList[RedoList.Count - 1].before.SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
                string newFilePath = $"{VocabularyEntry.FirstPartFilePath}{RedoList[RedoList.Count - 1].after.WordlistName}_{RedoList[RedoList.Count - 1].after.FirstLanguage}_{RedoList[RedoList.Count - 1].after.SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
                File.Move(oldFilePath, newFilePath);
                VocabularyEntry tempEntry = new VocabularyEntry() {
                    FilePath = newFilePath
                };
                List<VocabularyEntry> tempList = VocabularyEntry.GetData(tempEntry);
                for (int i = 0; i < tempList.Count; i++) {
                    tempList[i].WordList = RedoList[RedoList.Count - 1].after.WordlistName;
                    tempList[i].FirstLanguage = RedoList[RedoList.Count - 1].after.FirstLanguage;
                    tempList[i].SecondLanguage = RedoList[RedoList.Count - 1].after.SecondLanguage;
                }
                VocabularyEntry.WriteData(tempEntry, tempList);
            }
            UpdateViewModels(RedoList[RedoList.Count - 1].before);
            UndoList.Add(RedoList[RedoList.Count - 1]);
            RedoList.Remove(RedoList[RedoList.Count - 1]);
            WordlistsList.WriteWordlistsList(allWordlists);
        }
        public void UpdateViewModels(WordlistsList wordlist) {
            if (_parent.ListWordlist.Contains(wordlist) && _parent.ListWordlist[_parent.ListWordlist.IndexOf(wordlist)].IsTrue) {
                _parent.LearnEntriesViewModel = new LearnViewModel(_parent);
                _parent.SettingsViewModel = new SettingsViewModel(_parent);
                _parent.TranslatorViewModel = new TranslatorViewModel(_parent, _parent.TranslatorOpenedWordlist);
                _parent.ManageEntriesViewModel = new ManageViewModel(_parent, _parent.ManageEntriesOpenedWordlist);
                _parent.AddWordlistViewModel = new AddWordlistViewModel(_parent);
            }
        }
    }
}
