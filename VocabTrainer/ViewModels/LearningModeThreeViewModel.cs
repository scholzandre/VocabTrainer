using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class LearningModeThreeViewModel : BaseViewModel {
        private string _firstWord;
        public string FirstWord {
            get => _firstWord;
            set {
                _firstWord = value;
                OnPropertyChanged(nameof(FirstWord));
            }
        }
        private string _firstAnswer;
        public string FirstAnswer {
            get => _firstAnswer;
            set {
                _firstAnswer = value;
                OnPropertyChanged(nameof(FirstAnswer));
            }
        }
        private string _secondAnswer;
        public string SecondAnswer {
            get => _secondAnswer;
            set {
                _secondAnswer = value;
                OnPropertyChanged(nameof(SecondAnswer));
            }
        }
        private string _thirdAnswer;
        public string ThirdAnswer {
            get => _thirdAnswer;
            set {
                _thirdAnswer = value;
                OnPropertyChanged(nameof(ThirdAnswer));
            }
        }
        private string _fourthAnswer;
        public string FourthAnswer {
            get => _fourthAnswer;
            set {
                _fourthAnswer = value;
                OnPropertyChanged(nameof(FourthAnswer));
            }
        }
        private string _fifthAnswer;
        public string FifthAnswer {
            get => _fifthAnswer;
            set {
                _fifthAnswer = value;
                OnPropertyChanged(nameof(FifthAnswer));
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
        private Brush _correctWordBackground = Brushes.White;
        public Brush CorrectWordBackground {
            get => _correctWordBackground;
            set {
                _correctWordBackground = value;
                OnPropertyChanged(nameof(CorrectWordBackground));
            }
        }
        private Brush _firstAnswerBackground = Brushes.White;
        public Brush FirstAnswerBackground {
            get => _firstAnswerBackground;
            set {
                _firstAnswerBackground = value;
                OnPropertyChanged(nameof(FirstAnswerBackground));
            }
        }
        private Brush _secondAnswerBackground = Brushes.White;
        public Brush SecondAnswerBackground {
            get => _secondAnswerBackground;
            set {
                _secondAnswerBackground = value;
                OnPropertyChanged(nameof(SecondAnswerBackground));
            }
        }
        private Brush _thirdAnswerBackground = Brushes.White;
        public Brush ThirdAnswerBackground {
            get => _thirdAnswerBackground;
            set {
                _thirdAnswerBackground = value;
                OnPropertyChanged(nameof(ThirdAnswerBackground));
            }
        }
        private Brush _fourthAnswerBackground = Brushes.White;
        public Brush FourthAnswerBackground {
            get => _fourthAnswerBackground;
            set {
                _fourthAnswerBackground = value;
                OnPropertyChanged(nameof(FourthAnswerBackground));
            }
        }
        private Brush _fifthAnswerBackground = Brushes.White;
        public Brush FifthAnswerBackground {
            get => _fifthAnswerBackground;
            set {
                _fifthAnswerBackground = value;
                OnPropertyChanged(nameof(FifthAnswerBackground));
            }
        }
        private LearnViewModel _parent;
        private List<VocabularyEntry> _entries;
        private int _language = 0;
        private int _counter;
        static VocabularyEntry _markEntry = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}Marked{VocabularyEntry.SecondPartFilePath}" };
        private List<VocabularyEntry> _markedEntries = VocabularyEntry.GetData(_markEntry);
        public LearningModeThreeViewModel(LearnViewModel parent) {
            _parent = parent;
            _counter = _parent.Counter;
            _language = _parent.Random.Next(1, 3);
            FillProperties(_language);
            Star = (_markedEntries.Contains(_parent.Entries[_counter])) ? "★" : "☆";
        }
        private void FillProperties(int language) {
            _entries = new List<VocabularyEntry>();
            for (int i = 0; i < 4; i++) {
                int position = _parent.Random.Next(0, _parent.Entries.Count);
                if (_entries.Contains(_parent.Entries[position]) || _parent.Entries[position] == _parent.Entries[_parent.Counter]) {
                    i--;
                    continue;
                } else {
                    _entries.Add(_parent.Entries[position]);
                }
            }
            int positionCorrectAnswer = _parent.Random.Next(0, 5);
            _entries.Insert(positionCorrectAnswer, _parent.Entries[_counter]);
            if (language == 1) {
                FirstWord = _parent.Entries[_counter].German;
                FirstAnswer = _entries[0].English;
                SecondAnswer = _entries[1].English;
                ThirdAnswer = _entries[2].English;
                FourthAnswer = _entries[3].English;
                FifthAnswer = _entries[4].English;
            } else {
                FirstWord = _parent.Entries[_counter].English;
                FirstAnswer = _entries[0].German;
                SecondAnswer = _entries[1].German;
                ThirdAnswer = _entries[2].German;
                FourthAnswer = _entries[3].German;
                FifthAnswer = _entries[4].German;
            }
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand CheckFirstAnswerCommand => new RelayCommand(CheckFirstAnswer, CanExecuteCommand);
        private void CheckFirstAnswer(object obj) {
            CheckInput(_entries[0], FirstAnswerBackground);
        }
        public ICommand CheckSecondAnswerCommand => new RelayCommand(CheckSecondAnswer, CanExecuteCommand);
        private void CheckSecondAnswer(object obj) {
            CheckInput(_entries[1], SecondAnswerBackground);
        }
        public ICommand CheckThirdAnswerCommand => new RelayCommand(CheckThirdAnswer, CanExecuteCommand);
        private void CheckThirdAnswer(object obj) {
            CheckInput(_entries[2], ThirdAnswerBackground);
        }
        public ICommand CheckFourthAnswerCommand => new RelayCommand(CheckFourthAnswer, CanExecuteCommand);
        private void CheckFourthAnswer(object obj) {
            CheckInput(_entries[3], FourthAnswerBackground);
        }
        public ICommand CheckFifthAnswerCommand => new RelayCommand(CheckFifthAnswer, CanExecuteCommand);
        private void CheckFifthAnswer(object obj) {
            CheckInput(_entries[4], FifthAnswerBackground);
        }
        private void CheckInput(VocabularyEntry choice, Brush backgroundColor) {
            if (_language == 1) {
                if (_parent.Entries[_counter].English == choice.English) {
                    _parent.Entries[_counter].Seen = true;
                    _parent.Entries[_counter].LastTimeWrong = false;
                    _parent.Entries[_counter].Repeated += 1;
                    CorrectWordBackground = Brushes.Green; // why does it not work 
                    backgroundColor = Brushes.Green;
                }   // else is missing
            } else {
                if (_parent.Entries[_counter].German == choice.German) {
                    _parent.Entries[_counter].Seen = true;
                    _parent.Entries[_counter].LastTimeWrong = true;
                    _parent.Entries[_counter].Repeated = 0;
                }   
            }

            _parent.Entries[_counter].FilePath = $"{VocabularyEntry.FirstPartFilePath}{_parent.Entries[_counter].WordList}{VocabularyEntry.SecondPartFilePath}";
            VocabularyEntry.WriteData(_parent.Entries[_counter], _parent.Entries);
            //Thread.Sleep(1000);
            //_parent.ShowLearnMode();
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
