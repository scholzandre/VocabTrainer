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

namespace VocabTrainer.ViewModels {
    public class ManageEntryViewModel : BaseViewModel {
        public string FirstWord { get; set; }
        public string SecondWord { get; set; }
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
        private VocabularyEntry _entry;
        private ObservableCollection<UserControl> _views;
        public ManageEntryViewModel(ObservableCollection<UserControl> views, VocabularyEntry entry) {
            FirstWord = entry.German;
            FirstWordWritable = false;
            SecondWord = entry.English;
            SecondWordWritable = false;
            EditButtonText = "🖉";
            DeleteButtonText = "🗑";
            _entry = entry;
            _entry.FilePath = VocabularyEntry.FirstPartFilePath + entry.WordList + VocabularyEntry.SecondPartFilePath;
            _views = views;
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand ChangeTextCommand => new RelayCommand(ChangeText, CanExecuteCommand);
        private void ChangeText(object obj) {
            if (EditButtonText == "🖉") {
                EditButtonText = "💾";
                FirstWordWritable = true;
                SecondWordWritable = true;
            } else {
                EditButtonText = "🖉";
                FirstWordWritable = false;
                SecondWordWritable = false;
                List<VocabularyEntry> entries = VocabularyEntry.GetData(_entry);
                for (int i = 0; i < entries.Count; i++) {
                    if (entries[i].German == _entry.German && entries[i].English == _entry.English) {
                        entries[i].German = FirstWord;
                        entries[i].English = SecondWord;
                    }
                }
                VocabularyEntry.WriteData(_entry, entries);
            }
        }
        public ICommand DeleteEntryCommand => new RelayCommand(DeleteEntry, CanExecuteCommand);
        private void DeleteEntry(object obj) {
            if (DeleteButtonText == "🗑") {
                DeleteButtonText = "✓";
            } else {
                DeleteButtonText = "🗑";
                List<VocabularyEntry> entries = VocabularyEntry.GetData(_entry);
                foreach (VocabularyEntry entry in entries) {
                    if (entry.German == FirstWord && entry.English == SecondWord) {
                        entries.Remove(entry);
                        break;
                    }
                }
                foreach (UserControl view in _views) {
                    int firstViewModel = view.DataContext.GetHashCode();
                    int secondViewModel = new ManageEntryViewModel(_views, _entry).GetHashCode();
                    if (firstViewModel == secondViewModel) {
                        _views.Remove(view);
                        break;
                    }
                }
                VocabularyEntry.WriteData(_entry, entries);
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