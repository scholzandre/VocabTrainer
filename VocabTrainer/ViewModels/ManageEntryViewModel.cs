using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class ManageEntryViewModel : BaseViewModel {
        private string _firstWord;
        public string FirstWord { 
            get => _firstWord;
            set {
                _firstWord = value;
                OnPropertyChanged(nameof(FirstWord));
            } 
        }
        private string _secondWord;
        public string SecondWord { 
            get => _secondWord;
            set {
                _secondWord = value;
                OnPropertyChanged(nameof(SecondWord));
            }
        }
        private bool _firstWordWritable = false;
        public bool FirstWordWritable { 
            get => _firstWordWritable;
            set {
                _firstWordWritable = value;
                OnPropertyChanged(nameof(FirstWordWritable));
            } 
        }
        private bool _secondWordWritable = false;
        public bool SecondWordWritable { 
            get => _secondWordWritable;
            set { 
                _secondWordWritable = value;
                OnPropertyChanged(nameof(SecondWordWritable));
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
        private string _editButtonText = ButtonIcons.GetIconString(IconType.Edit);
        public string EditButtonText {
            get => _editButtonText;
            set {
                _editButtonText = value;
                OnPropertyChanged(nameof(EditButtonText));
            }
        }
        private string _deleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
        public string DeleteButtonText {
            get => _deleteButtonText;
            set {
                _deleteButtonText = value;
                OnPropertyChanged(nameof(DeleteButtonText));
            }
        }
        private readonly List<string> filePathsSpecialLists = new List<string> { 
            $"{VocabularyEntry.FirstPartFilePath}Marked{VocabularyEntry.SecondPartFilePath}",
            $"{VocabularyEntry.FirstPartFilePath}Seen{VocabularyEntry.SecondPartFilePath}",
            $"{VocabularyEntry.FirstPartFilePath}NotSeen{VocabularyEntry.SecondPartFilePath}",
            $"{VocabularyEntry.FirstPartFilePath}LastTimeWrong{VocabularyEntry.SecondPartFilePath}"
        };

        private VocabularyEntry _entry;
        private ObservableCollection<ManageEntryViewModel> _views;
        private ManageViewModel _parent;
        private WordlistsList _selectedItem;
        public ManageEntryViewModel(ObservableCollection<ManageEntryViewModel> views, VocabularyEntry entry, ManageViewModel parent, WordlistsList selectedItem) {
            FirstWord = entry.German;
            SecondWord = entry.English;
            _entry = entry;
            _views = views;
            _parent = parent;
            _selectedItem = selectedItem;
            Editable = (_selectedItem.WordlistName == "Marked_-_-") ? false : true;
            _entry.FilePath = (_selectedItem.WordlistName == "Marked_-_-")? VocabularyEntry.FirstPartFilePath + "Marked" + VocabularyEntry.SecondPartFilePath : VocabularyEntry.FirstPartFilePath + _selectedItem.WordlistName + VocabularyEntry.SecondPartFilePath;
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand ChangeTextCommand => new RelayCommand(ChangeText, CanExecuteCommand);
        private void ChangeText(object obj) {
            if (EditButtonText == ButtonIcons.GetIconString(IconType.Edit)) {
                EditButtonText = ButtonIcons.GetIconString(IconType.Save);
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Cancel);
                FirstWordWritable = true;
                SecondWordWritable = true;
            } else if (EditButtonText == ButtonIcons.GetIconString(IconType.Save)) {
                EditButtonText = ButtonIcons.GetIconString(IconType.Edit);
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
                FirstWordWritable = false;
                SecondWordWritable = false;
                bool alreadyExists = false;
                List<VocabularyEntry> entries = VocabularyEntry.GetData(_entry);
                foreach (VocabularyEntry entry in entries) 
                    if ((entry.German == FirstWord && FirstWord != _entry.German) || (entry.English == SecondWord && SecondWord != _entry.English)) {
                        alreadyExists = true;
                        break;
                    }
                if (!alreadyExists) {
                    for (int i = 0; i < entries.Count; i++) 
                        if (entries[i].German == _entry.German && entries[i].English == _entry.English) {
                            entries[i].German = FirstWord;
                            entries[i].English = SecondWord;
                            break;
                        }
                    VocabularyEntry.WriteData(_entry, entries);

                    for (int i = 0; i < filePathsSpecialLists.Count; i++) {
                        VocabularyEntry tempEntry = new VocabularyEntry() {
                            FilePath = filePathsSpecialLists[i]
                        };
                        List<VocabularyEntry> tempEntries = VocabularyEntry.GetData(tempEntry);
                        for (int j = 0; j < tempEntries.Count; j++) 
                            if (tempEntries[j].German == _entry.German && tempEntries[j].English == _entry.English) {
                                tempEntries[j].German = FirstWord;
                                tempEntries[j].English = SecondWord;
                                break;
                            }
                        VocabularyEntry.WriteData(tempEntry, tempEntries);
                    }
                    _entry.German = FirstWord;
                    _entry.English = SecondWord;
                } else { 
                    FirstWord = _entry.German;
                    SecondWord = _entry.English;
                } 
            } else { 
                EditButtonText = ButtonIcons.GetIconString(IconType.Edit);
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
                Editable = (_selectedItem.WordlistName == "Marked_-_-") ? false : true;
            }
        }
        public ICommand DeleteEntryCommand => new RelayCommand(DeleteEntry, CanExecuteCommand);
        private void DeleteEntry(object obj) {
            if (DeleteButtonText == ButtonIcons.GetIconString(IconType.Delete)) {
                Editable = true;
                EditButtonText = ButtonIcons.GetIconString(IconType.Cancel);
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Approve);
            } else if (DeleteButtonText == ButtonIcons.GetIconString(IconType.Approve)) {
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
                List<VocabularyEntry> entries = VocabularyEntry.GetData(_entry);
                entries.Remove(_entry);
                VocabularyEntry.WriteData(_entry, entries);

                int secondViewModel = new ManageEntryViewModel(_views, _entry, _parent, _selectedItem).GetHashCode();
                foreach (ManageEntryViewModel view in _views) {
                    int firstViewModel = view.GetHashCode();
                    if (firstViewModel == secondViewModel) {
                        _views.Remove(view);
                        _parent.AllEntriesCounter = _views.Count();
                        break;
                    }
                }

                if (_selectedItem.WordlistName != "Marked_-_-") 
                    for (int i = 0; i < filePathsSpecialLists.Count; i++) {
                        VocabularyEntry tempEntry = new VocabularyEntry() {
                            FilePath = filePathsSpecialLists[i]
                        };
                        List<VocabularyEntry> tempEntries = VocabularyEntry.GetData(tempEntry);
                        if (tempEntries.Contains(_entry)) tempEntries.Remove(_entry);
                        VocabularyEntry.WriteData(tempEntry, tempEntries);    
                    }
            } else {
                DeleteButtonText = ButtonIcons.GetIconString(IconType.Delete);
                EditButtonText = ButtonIcons.GetIconString(IconType.Edit);
                FirstWordWritable = false;
                SecondWordWritable = false;
                FirstWord = _entry.German;
                SecondWord = _entry.English;
            }
        }
        public override int GetHashCode() {
            unchecked {
                int hashCode = 17;
                hashCode = (hashCode * 23) + (FirstWord?.GetHashCode() ?? 0);
                hashCode = (hashCode * 23) + (SecondWord?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}