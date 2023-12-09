using System.Collections.Generic;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class LearningModeOneViewModel : BaseViewModel{
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
        private string _star;
        public string Star {
            get => _star;
            set {
                _star = value;
                OnPropertyChanged(nameof(Star));
            }
        }
        public bool IsMarked { get; set; }
        private readonly LearnViewModel _parent;
        static VocabularyEntry _markEntry = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}Marked{VocabularyEntry.SecondPartFilePath}" };
        private List<VocabularyEntry> _markedEntries = VocabularyEntry.GetData(_markEntry);
        private int _counter;
        public LearningModeOneViewModel(LearnViewModel parent) { 
            _parent = parent;
            _counter = _parent.Counter;
            FirstWord = _parent.Entries[_counter].German;
            SecondWord = _parent.Entries[_counter].English;
            FirstLanguage = _parent.Entries[_counter].FirstLanguage;
            SecondLanguage = _parent.Entries[_counter].SecondLanguage;
            Star = (_markedEntries.Contains(_parent.Entries[_counter])) ? "★" : "☆";
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand NextWordCommand => new RelayCommand(NextWord, CanExecuteCommand);
        private void NextWord(object obj) {
            VocabularyEntry.RemoveEntry("NotSeen", _parent.Entries[_counter]);
            VocabularyEntry.AddEntry("Seen", _parent.Entries[_counter]);
            _parent.Entries[_counter].Seen = true;
            _parent.Entries[_counter].Repeated += 1;
            VocabularyEntry tempEntry = new VocabularyEntry() {
                FilePath = $"{VocabularyEntry.FirstPartFilePath}{_parent.Entries[_counter].WordList}{VocabularyEntry.SecondPartFilePath}",
                FirstLanguage = _parent.Entries[_counter].FirstLanguage,
                SecondLanguage = _parent.Entries[_counter].SecondLanguage,
            };
            VocabularyEntry.WriteData(tempEntry, _parent.Entries);
            _parent.ShowLearnMode();
        }

        public ICommand MarkEntryCommand => new RelayCommand(MarkEntry, CanExecuteCommand);
        private void MarkEntry(object obj) {
            if (Star == "☆") {
                Star = "★";
                _markedEntries.Add(_parent.Entries[_counter]);
            } else {
                Star = "☆";
                _markedEntries.Remove(_parent.Entries[_counter]);
            }
            VocabularyEntry.WriteData(_markEntry, _markedEntries);
        }
    }
}
