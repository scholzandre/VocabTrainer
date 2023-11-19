using System.Collections.Generic;
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
        private List<Brush> _backgroundColors = new List<Brush>() {
            Brushes.White,
            Brushes.White,
            Brushes.White,
            Brushes.White,
            Brushes.White,
            Brushes.White
        };
        public List<Brush> BackgroundColors {
            get => _backgroundColors;
            set {
                _backgroundColors = value;
                OnPropertyChanged(nameof(BackgroundColors));
            }
        }

        private readonly LearnViewModel _parent;
        private List<VocabularyEntry> _entries;
        private readonly int _language = 0;
        private readonly int _counter;
        static readonly VocabularyEntry _markEntry = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}Marked{VocabularyEntry.SecondPartFilePath}" };
        private readonly List<VocabularyEntry> _markedEntries = VocabularyEntry.GetData(_markEntry);
        private int _positionCorrectItem;
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
            _positionCorrectItem = _parent.Random.Next(0, 5);
            _entries.Insert(_positionCorrectItem, _parent.Entries[_counter]);
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
        private async void CheckFirstAnswer(object obj) {
            await CheckInput(_entries[0], 1);
        }
        public ICommand CheckSecondAnswerCommand => new RelayCommand(CheckSecondAnswer, CanExecuteCommand);
        private async void CheckSecondAnswer(object obj) {
            await CheckInput(_entries[1], 2);
        }
        public ICommand CheckThirdAnswerCommand => new RelayCommand(CheckThirdAnswer, CanExecuteCommand);
        private async void CheckThirdAnswer(object obj) {
            await CheckInput(_entries[2], 3);
        }
        public ICommand CheckFourthAnswerCommand => new RelayCommand(CheckFourthAnswer, CanExecuteCommand);
        private async void CheckFourthAnswer(object obj) {
            await CheckInput(_entries[3], 4);
        }
        public ICommand CheckFifthAnswerCommand => new RelayCommand(CheckFifthAnswer, CanExecuteCommand);
        private async void CheckFifthAnswer(object obj) {
            await CheckInput(_entries[4], 5);
        }
        private async Task CheckInput(VocabularyEntry choice, int answer) {
            List<Brush> tempList = new List<Brush>(BackgroundColors);
            if (_parent.Entries[_counter].English == choice.English || _parent.Entries[_counter].German == choice.German) {
                _parent.Entries[_counter].Seen = true;
                _parent.Entries[_counter].LastTimeWrong = false;
                _parent.Entries[_counter].Repeated += 1;
                tempList[0] = Brushes.Green;
                tempList[answer] = Brushes.Green;
            } else {
                _parent.Entries[_counter].Seen = true;
                _parent.Entries[_counter].LastTimeWrong = true;
                _parent.Entries[_counter].Repeated = 0;
                tempList[0] = Brushes.DarkRed;
                tempList[answer] = Brushes.DarkRed;
                tempList[_positionCorrectItem+1] = Brushes.Green;
            }

            _parent.Entries[_counter].FilePath = $"{VocabularyEntry.FirstPartFilePath}{_parent.Entries[_counter].WordList}{VocabularyEntry.SecondPartFilePath}";
            VocabularyEntry.WriteData(_parent.Entries[_counter], _parent.Entries);
            BackgroundColors = tempList;
            await ExtraFunctions.Wait();
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