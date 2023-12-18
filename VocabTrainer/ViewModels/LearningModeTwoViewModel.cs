using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class LearningModeTwoViewModel : BaseViewModel {
        private string _firstWord;  
        private string _secondWord;
        private string _firstWordAnswer = string.Empty;
        public string FirstWordAnswer {
            get => _firstWordAnswer;
            set {
                _firstWordAnswer = value;
                OnPropertyChanged(nameof(FirstWordAnswer));
            }
        }
        private string _secondWordAnswer = string.Empty;
        public string SecondWordAnswer {
            get => _secondWordAnswer;
            set {
                _secondWordAnswer = value;
                OnPropertyChanged(nameof(SecondWordAnswer));
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
        private string _infoText;
        public string InfoText {
            get => _infoText;
            set {
                _infoText = value;
                OnPropertyChanged(nameof(InfoText));
            }
        }
        private string _buttonText;
        public string ButtonText {
            get => _buttonText;
            set {
                _buttonText = value;
                OnPropertyChanged(nameof(ButtonText));
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
        private SolidColorBrush _firstWordForeground = Brushes.Black;
        public SolidColorBrush FirstWordForeground {
            get => _firstWordForeground;
            set {
                _firstWordForeground = value;
                OnPropertyChanged(nameof(FirstWordForeground));
            }
        }
        private SolidColorBrush _secondWordForeground = Brushes.Black;
        public SolidColorBrush SecondWordForeground {
            get => _secondWordForeground;
            set {
                _secondWordForeground = value;
                OnPropertyChanged(nameof(SecondWordForeground));
            }
        }
        public bool IsMarked { get; set; }
        private readonly LearnViewModel _parent;
        static VocabularyEntry markEntry = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}Marked{VocabularyEntry.SecondPartFilePath}" };
        private List<VocabularyEntry> markedEntries = VocabularyEntry.GetData(markEntry);
        private int _counter;
        private string _checkText = "check answer";
        private string _nextText = "next";
        private int _hints = 0;
        public LearningModeTwoViewModel(LearnViewModel parent) {
            _parent = parent;
            _counter = _parent.Counter;
            _firstWord = _parent.Entries[_counter].German;
            _secondWord = _parent.Entries[_counter].English;
            FirstLanguage = _parent.Entries[_counter].FirstLanguage;
            SecondLanguage = _parent.Entries[_counter].SecondLanguage;
            Star = (markedEntries.Contains(_parent.Entries[_counter])) ? "★" : "☆";
            ButtonText = _checkText;
            if (_parent.Random.Next(2) == 1) {
                FirstWordWritable = true;
                SecondWordAnswer = _secondWord;
            } else { 
                SecondWordWritable = true;
                FirstWordAnswer = _firstWord;
            }
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand CheckAnswerCommand => new RelayCommand(CheckAnswer, CanExecuteCommand);
        private void CheckAnswer(object obj) {
            if (ButtonText == _checkText) {
                bool isPartyCorrect = false;
                bool isCorrect = false;
                bool isWrong = false;
                string[] answers = SplitAnswer((FirstWordWritable) ? FirstWordAnswer : SecondWordAnswer);
                string[] correctAnswers = SplitAnswer((FirstWordWritable) ? _firstWord : _secondWord);
                VocabularyEntry tempEntry = _parent.Entries[_counter];
                tempEntry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{tempEntry.WordList}{VocabularyEntry.SecondPartFilePath}";
                List<VocabularyEntry> entries = VocabularyEntry.GetData(_parent.Entries[_counter]);
                int index = entries.IndexOf(_parent.Entries[_counter]);
                if (answers.Length > correctAnswers.Length) isPartyCorrect = true;
                for (int i = 0; i < answers.Length; i++) { 
                    if (correctAnswers.Contains(answers[i])) isCorrect = true;
                    else isWrong = true;
                }
                if (isCorrect && isWrong) isPartyCorrect = true;

                if (isPartyCorrect || _hints > 3) {
                    if (FirstWordWritable) FirstWordForeground = Brushes.Orange;
                    else SecondWordForeground = Brushes.Orange;
                    entries[index].Seen = true;
                    InfoText = $"The correct answer would have been\n{FirstLanguage}\n{_firstWord}\n\n{SecondLanguage}\n{_secondWord}";
                    VocabularyEntry.RemoveEntry("NotSeen", entries[index]);
                    VocabularyEntry.AddEntry("Seen", entries[index]);
                    VocabularyEntry.WriteData(tempEntry, entries);
                } else if (isCorrect && !isWrong) {
                    if (FirstWordWritable) FirstWordForeground = Brushes.Green;
                    else SecondWordForeground = Brushes.Green;
                    VocabularyEntry.CheckAnswer(entries[index], entries[index]);
                    VocabularyEntry.RemoveEntry("LastTimeWrong", _parent.Entries[_counter]);
                } else if (isWrong && !isCorrect) { 
                    if (FirstWordWritable) FirstWordForeground = Brushes.Red;
                    else SecondWordForeground = Brushes.Red;
                    VocabularyEntry.CheckAnswer(entries[index], new VocabularyEntry());
                    VocabularyEntry.AddEntry("LastTimeWrong", entries[index]);
                    InfoText = $"The correct answer would have been\n{FirstLanguage}\n{_firstWord}\n\n{SecondLanguage}\n{_secondWord}";
                }
                
                FirstWordWritable = false;
                SecondWordWritable = false;
                ButtonText = _nextText;
            } else { 
                _parent.ShowLearnMode();
            }
        }

        public ICommand MarkEntryCommand => new RelayCommand(MarkEntry, CanExecuteCommand);
        private void MarkEntry(object obj) {
            if (Star == "☆") {
                Star = "★";
                markedEntries.Add(_parent.Entries[_counter]);
            } else {
                Star = "☆";
                markedEntries.Remove(_parent.Entries[_counter]);
            }
            VocabularyEntry.WriteData(markEntry, markedEntries);
        }

        public ICommand HintCommand => new RelayCommand(Hint, CanExecuteCommand);
        private void Hint(object obj) {
            _hints++;
            if (FirstWordWritable && FirstWordAnswer.Length < _firstWord.Length)
                FirstWordAnswer = _firstWord.Substring(0, FirstWordAnswer.Length+1);
            else if (SecondWordWritable && SecondWordAnswer.Length < _secondWord.Length)
                SecondWordAnswer = _secondWord.Substring(0, SecondWordAnswer.Length+1);
        }
        private string[] SplitAnswer(string answer) {
            string[] splittedAnswer = answer.Split(',');
            for (int i = 0; i < splittedAnswer.Length; i++) {
                splittedAnswer[i] =  splittedAnswer[i].Trim();
            }
            return splittedAnswer;
        }
    }
}
