using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Brush FirstWordBackground {
            get => _firstAnswerBackground;
            set {
                _firstAnswerBackground = value;
                OnPropertyChanged(nameof(FirstWordBackground));
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
        public LearningModeThreeViewModel(LearnViewModel parent) {
            _parent = parent;
            FillProperties(_parent.Random.Next(1, 3));
        }
        private void FillProperties(int language) {
            _entries = new List<VocabularyEntry>();
            for (int i = 0; i < 4; i++) {
                int position = _parent.Random.Next(0, _parent.Entries.Count);
                if (_entries.Contains(_parent.Entries[position])) {
                    i--;
                    continue;
                } else {
                    _entries.Add(_parent.Entries[position]);
                }
            }
            int positionCorrectAnswer = _parent.Random.Next(0, 5);
            _entries.Insert(positionCorrectAnswer, _parent.Entries[_parent.Counter]);
            if (language == 1) {
                FirstWord = _parent.Entries[_parent.Counter].German;
                FirstAnswer = _entries[0].English;
                SecondAnswer = _entries[1].English;
                ThirdAnswer = _entries[2].English;
                FourthAnswer = _entries[3].English;
                FifthAnswer = _entries[4].English;
            } else {
                FirstWord = _parent.Entries[_parent.Counter].English;
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
            CheckInput(_entries[0]);
        }
        public ICommand CheckSecondAnswerCommand => new RelayCommand(CheckSecondAnswer, CanExecuteCommand);
        private void CheckSecondAnswer(object obj) {
            CheckInput(_entries[1]);
        }
        public ICommand CheckThirdAnswerCommand => new RelayCommand(CheckThirdAnswer, CanExecuteCommand);
        private void CheckThirdAnswer(object obj) {
            CheckInput(_entries[2]);
        }
        public ICommand CheckFourthAnswerCommand => new RelayCommand(CheckFourthAnswer, CanExecuteCommand);
        private void CheckFourthAnswer(object obj) {
            CheckInput(_entries[3]);
        }
        public ICommand CheckFifthAnswerCommand => new RelayCommand(CheckFifthAnswer, CanExecuteCommand);
        private void CheckFifthAnswer(object obj) {
            CheckInput(_entries[4]);
        }
        private void CheckInput(VocabularyEntry choice) {
            _parent.ShowLearnMode();
        }
    }
}
