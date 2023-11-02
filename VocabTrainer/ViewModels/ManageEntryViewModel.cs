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
        private bool _firstWordWritable;
        public bool FirstWordWritable { 
            get => _firstWordWritable;
            set {
                _firstWordWritable = value;
                OnPropertyChanged(nameof(FirstWordWritable));
            } 
        }
        private bool _secondWordWritable;
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
            FirstWordWritable = false;
            SecondWord = entry.English;
            SecondWordWritable = false;
            EditButtonText = "🖉";
            DeleteButtonText = "🗑";
            _entry = entry;
            _views = views;
            _parent = parent;
            _selectedItem = selectedItem;
            Editable = (_selectedItem.WordlistName == "Marked") ? false : true;
            _entry.FilePath = VocabularyEntry.FirstPartFilePath + _selectedItem.WordlistName + VocabularyEntry.SecondPartFilePath;
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand ChangeTextCommand => new RelayCommand(ChangeText, CanExecuteCommand);
        private void ChangeText(object obj) {
            if (EditButtonText == "🖉") {
                EditButtonText = "💾";
                DeleteButtonText = "🗙";
                FirstWordWritable = true;
                SecondWordWritable = true;
            } else if (EditButtonText == "💾") {
                EditButtonText = "🖉";
                DeleteButtonText = "🗑";
                FirstWordWritable = false;
                SecondWordWritable = false;
                bool alreadyExists = false;
                List<VocabularyEntry> entries = VocabularyEntry.GetData(_entry);
                foreach (VocabularyEntry entry in entries) {
                    if ((entry.German == FirstWord && FirstWord != _entry.German) || (entry.English == SecondWord && SecondWord != _entry.English)) {
                        alreadyExists = true;
                        break;
                    }
                }
                if (!alreadyExists) {
                    for (int i = 0; i < entries.Count; i++) {
                        if (entries[i].German == _entry.German && entries[i].English == _entry.English) {
                            entries[i].German = FirstWord;
                            entries[i].English = SecondWord;
                            _entry.German = FirstWord;
                            _entry.English = SecondWord;
                        }
                    }
                    VocabularyEntry.WriteData(_entry, entries);
                } else { 
                    FirstWord = _entry.German;
                    SecondWord = _entry.English;
                } 
            } else { 
                EditButtonText = "🖉";
                DeleteButtonText = "🗑";
                Editable = (_selectedItem.WordlistName == "Marked") ? false : true;
            }
        }
        public ICommand DeleteEntryCommand => new RelayCommand(DeleteEntry, CanExecuteCommand);
        private void DeleteEntry(object obj) {
            if (DeleteButtonText == "🗑") {
                Editable = true;
                EditButtonText = "🗙";
                DeleteButtonText = "✓";
            } else if (DeleteButtonText == "✓") {
                DeleteButtonText = "🗑";
                List<VocabularyEntry> entries = VocabularyEntry.GetData(_entry);
                foreach (VocabularyEntry entry in entries) {
                    if (entry.German == FirstWord && entry.English == SecondWord) {
                        entries.Remove(entry);
                        break;
                    }
                }
                foreach (ManageEntryViewModel view in _views) {
                    int firstViewModel = view.GetHashCode();
                    int secondViewModel = new ManageEntryViewModel(_views, _entry, _parent, _selectedItem).GetHashCode();
                    if (firstViewModel == secondViewModel) {
                        _views.Remove(view);
                        _parent.AllEntriesCounter = _views.Count();
                        break;
                    }
                }
                VocabularyEntry.WriteData(_entry, entries);
            } else {
                DeleteButtonText = "🗑";
                EditButtonText = "🖉";
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