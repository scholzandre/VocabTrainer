using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class LearningModeFourViewModel : BaseViewModel {
        private List<string> _answerText = new List<string>();
        public List<string> AnswerText {
            get => _answerText;
            set {
                _answerText = value;
                OnPropertyChanged(nameof(AnswerText));
            }
        }
        private List<string> _questionText = new List<string>();
        public List<string > QuestionText {
            get => _questionText;
            set {
                _questionText = value;
                OnPropertyChanged(nameof(QuestionText));
            }
        }

        private static readonly List<Brush> _standardColors = new List<Brush>() {
            Brushes.White,
            Brushes.White,
            Brushes.White,
            Brushes.White,
            Brushes.White
        };
        private List<Brush> _backgroundColorsQuestion = new List<Brush>(_standardColors);
        public List<Brush> BackgroundColorsQuestion {
            get => _backgroundColorsQuestion;
            set {
                _backgroundColorsQuestion = value;
                OnPropertyChanged(nameof(BackgroundColorsQuestion));
            }
        }
        private List<Brush> _backgroundColorsAnswer = new List<Brush>(_standardColors);
        public List<Brush> BackgroundColorsAnswer {
            get => _backgroundColorsAnswer;
            set {
                _backgroundColorsAnswer = value;
                OnPropertyChanged(nameof(BackgroundColorsAnswer));
            }
        }
        private Dictionary<string, string> _answers = new Dictionary<string, string>();
        private Dictionary<string, string> _questions = new Dictionary<string, string>();
        private string _question = string.Empty;
        private string _answer = string.Empty;
        private readonly LearnViewModel _parent;
        private List<VocabularyEntry> _entries;
        private int _language;
        public LearningModeFourViewModel(LearnViewModel parent, List<VocabularyEntry> tempEntry) {
            _parent = parent;
            _entries = tempEntry;
            _language = _parent.Random.Next(1, 3);
            QuestionText = (_language == 1) ? tempEntry.Select(x => x.English).ToList() : tempEntry.Select(x => x.German).ToList();
            AnswerText = (_language == 1) ? tempEntry.Select(x => x.German).ToList() : tempEntry.Select(x => x.English).ToList();
            for (int i = 0; i < tempEntry.Count; i++) {
                _answers.Add((_language == 1) ? tempEntry[i].German : tempEntry[i].English, tempEntry[i].WordList);
                _questions.Add((_language == 1) ? tempEntry[i].English : tempEntry[i].German, tempEntry[i].WordList);
            }
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand SetFirstQuestionCommand => new RelayCommand(SetFirstQuestion, CanExecuteCommand);
        private void SetFirstQuestion(object obj) {
            _question = QuestionText[0];
            CheckAnswer("question", 0); 
        }
        public ICommand SetSecondQuestionCommand => new RelayCommand(SetSecondQuestion, CanExecuteCommand);
        private void SetSecondQuestion(object obj) {
            _question = QuestionText[1];
            CheckAnswer("question", 1); 
        }
        public ICommand SetThirdQuestionCommand => new RelayCommand(SetThirdQuestion, CanExecuteCommand);
        private void SetThirdQuestion(object obj) {
            _question = QuestionText[2];
            CheckAnswer("question", 2); 
        }
        public ICommand SetFourthQuestionCommand => new RelayCommand(SetFourthQuestion, CanExecuteCommand);
        private void SetFourthQuestion(object obj) {
            _question = QuestionText[3];
            CheckAnswer("question", 3); 
        }
        public ICommand SetFifthQuestionCommand => new RelayCommand(SetFifthQuestion, CanExecuteCommand);
        private void SetFifthQuestion(object obj) {
            _question = QuestionText[4];
            CheckAnswer("question", 4); 
        }

        public ICommand SetFirstAnswerCommand => new RelayCommand(SetFirstAnswer, CanExecuteCommand);
        private void SetFirstAnswer(object obj) {
            _answer = AnswerText[0];
            CheckAnswer("answer", 0); 
        }
        public ICommand SetSecondAnswerCommand => new RelayCommand(SetSecondAnswer, CanExecuteCommand);
        private void SetSecondAnswer(object obj) {
            _answer = AnswerText[1];
            CheckAnswer("answer", 1); 
        }
        public ICommand SetThirdAnswerCommand => new RelayCommand(SetThirdAnswer, CanExecuteCommand);
        private void SetThirdAnswer(object obj) {
            _answer = AnswerText[2];
            CheckAnswer("answer", 2); 
        }
        public ICommand SetFourthAnswerCommand => new RelayCommand(SetFourthAnswer, CanExecuteCommand);
        private void SetFourthAnswer(object obj) {
            _answer = AnswerText[3];
            CheckAnswer("answer", 3); 
        }
        public ICommand SetFifthAnswerCommand => new RelayCommand(SetFifthAnswer, CanExecuteCommand);
        private void SetFifthAnswer(object obj) {
            _answer = AnswerText[4];
            CheckAnswer("answer", 4);
        }
        private void CheckAnswer(string field, int index) {
            if (field == "answer") {
                BackgroundColorsAnswer = new List<Brush>(_standardColors);
                BackgroundColorsAnswer[index] = Brushes.Green;
            } else {
                BackgroundColorsQuestion = new List<Brush>(_standardColors);
                BackgroundColorsQuestion[index] = Brushes.Green;
            }
            if (_question != string.Empty && _answer != string.Empty) {
                VocabularyEntry tempAnswerEntry = new VocabularyEntry() {
                    German = (_language == 1) ? _answer : _question,
                    English = (_language == 1) ? _question : _answer,
                    WordList = _questions[_question]
                };
                int indexEntries = _parent.Entries.IndexOf(tempAnswerEntry);
                if (_parent.Entries.Contains(tempAnswerEntry)) {
                    _parent.Entries[_parent.Entries.IndexOf(_entries[indexEntries])].Seen = true;
                    _parent.Entries[_parent.Entries.IndexOf(_entries[indexEntries])].Repeated += 1;
                    _parent.Entries[_parent.Entries.IndexOf(_entries[indexEntries])].LastTimeWrong = false;
                } else {
                    for (int i = 0; i < _parent.Entries.Count; i++) {
                        if (_language == 1 && _parent.Entries[i].English == _question || _language == 2 && _parent.Entries[i].German == _question) {
                            _parent.Entries[i].Seen = true;
                            _parent.Entries[i].LastTimeWrong = true;
                            _parent.Entries[i].Repeated = 0;
                        }
                    }
                }
                VocabularyEntry tempEntry = new VocabularyEntry() {
                    FilePath = $"{VocabularyEntry.FirstPartFilePath}{_questions[_question]}{VocabularyEntry.SecondPartFilePath}"
                };
                VocabularyEntry.WriteData(tempEntry, _parent.Entries);
                _answer = string.Empty;
                _question = string.Empty;
            }
        }
    }
}
