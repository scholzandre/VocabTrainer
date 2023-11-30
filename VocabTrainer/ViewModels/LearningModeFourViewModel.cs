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

        private static readonly List<bool> _writable = new List<bool>() {
            true,
            true,
            true,
            true,
            true
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

        private List<bool> _answersClickable = new List<bool>(_writable);
        public List<bool> AnswersClickable {
            get => _answersClickable;
            set {
                _answersClickable = value;
                OnPropertyChanged(nameof(AnswersClickable));
            }
        }

        private List<bool> _questionsClickable = new List<bool>(_writable);
        public List<bool> QuestionsClickable {
            get => _questionsClickable;
            set {
                _questionsClickable = value;
                OnPropertyChanged(nameof(QuestionsClickable));
            }
        }
        private Dictionary<string, string> _answers = new Dictionary<string, string>();
        private Dictionary<string, string> _questions = new Dictionary<string, string>();
        private (string, int) _question = (string.Empty, 0);
        private (string, int) _answer = (string.Empty,0);
        private readonly LearnViewModel _parent;
        private List<VocabularyEntry> _entries;
        private int _language;
        private int _counter = 1;
        public LearningModeFourViewModel(LearnViewModel parent, List<VocabularyEntry> tempEntry) {
            _parent = parent;
            _entries = tempEntry;
            _language = _parent.Random.Next(1, 3);
            QuestionText = (_language == 1) ? tempEntry.Select(x => x.English).ToList() : tempEntry.Select(x => x.German).ToList();
            List<VocabularyEntry> tempEntry2 = new List<VocabularyEntry>(tempEntry);
            for (int i = 0; i < tempEntry.Count; i++) {
                _questions.Add((_language == 1) ? tempEntry[i].English : tempEntry[i].German, tempEntry[i].WordList);
                int random = _parent.Random.Next(0, tempEntry2.Count);
                _answers.Add((_language == 1) ? tempEntry2[random].German : tempEntry2[random].English, tempEntry2[random].WordList);
                AnswerText.Add((_language == 1) ? tempEntry2[random].German : tempEntry2[random].English);
                tempEntry2.Remove(tempEntry2[random]);
            }
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand SetFirstQuestionCommand => new RelayCommand(SetFirstQuestion, CanExecuteCommand);
        private void SetFirstQuestion(object obj) {
            _question = (QuestionText[0], 0);
            CheckAnswer("question"); 
        }
        public ICommand SetSecondQuestionCommand => new RelayCommand(SetSecondQuestion, CanExecuteCommand);
        private void SetSecondQuestion(object obj) {
            _question = (QuestionText[1], 1);
            CheckAnswer("question"); 
        }
        public ICommand SetThirdQuestionCommand => new RelayCommand(SetThirdQuestion, CanExecuteCommand);
        private void SetThirdQuestion(object obj) {
            _question = (QuestionText[2], 2);
            CheckAnswer("question"); 
        }
        public ICommand SetFourthQuestionCommand => new RelayCommand(SetFourthQuestion, CanExecuteCommand);
        private void SetFourthQuestion(object obj) {
            _question = (QuestionText[3], 3);
            CheckAnswer("question"); 
        }
        public ICommand SetFifthQuestionCommand => new RelayCommand(SetFifthQuestion, CanExecuteCommand);
        private void SetFifthQuestion(object obj) {
            _question = (QuestionText[4], 4);
            CheckAnswer("question"); 
        }
        public ICommand SetFirstAnswerCommand => new RelayCommand(SetFirstAnswer, CanExecuteCommand);
        private void SetFirstAnswer(object obj) {
            _answer = (AnswerText[0], 0);
            CheckAnswer("answer"); 
        }
        public ICommand SetSecondAnswerCommand => new RelayCommand(SetSecondAnswer, CanExecuteCommand);
        private void SetSecondAnswer(object obj) {
            _answer = (AnswerText[1], 1);
            CheckAnswer("answer"); 
        }
        public ICommand SetThirdAnswerCommand => new RelayCommand(SetThirdAnswer, CanExecuteCommand);
        private void SetThirdAnswer(object obj) {
            _answer = (AnswerText[2], 2);
            CheckAnswer("answer"); 
        }
        public ICommand SetFourthAnswerCommand => new RelayCommand(SetFourthAnswer, CanExecuteCommand);
        private void SetFourthAnswer(object obj) {
            _answer = (AnswerText[3], 3);
            CheckAnswer("answer"); 
        }
        public ICommand SetFifthAnswerCommand => new RelayCommand(SetFifthAnswer, CanExecuteCommand);
        private void SetFifthAnswer(object obj) {
            _answer = (AnswerText[4], 4);
            CheckAnswer("answer");
        }
        private async void CheckAnswer(string field) {
            if (_question.Item1 != string.Empty && _answer.Item1 != string.Empty) {
                _counter++;
                bool isCorrect = false;
                VocabularyEntry tempAnswerEntry = new VocabularyEntry() {
                    German = (_language == 1) ? _answer.Item1 : _question.Item1,
                    English = (_language == 1) ? _question.Item1 : _answer.Item1,
                    WordList = _questions[_question.Item1]
                };
                int indexEntries = _entries.IndexOf(tempAnswerEntry);
                if (_parent.Entries.Contains(tempAnswerEntry)) {
                    isCorrect = true;
                    _parent.Entries[_parent.Entries.IndexOf(_entries[indexEntries])].Seen = true;
                    _parent.Entries[_parent.Entries.IndexOf(_entries[indexEntries])].Repeated += 1;
                    _parent.Entries[_parent.Entries.IndexOf(_entries[indexEntries])].LastTimeWrong = false;
                } else {
                    for (int i = 0; i < _parent.Entries.Count; i++) {
                        if (_language == 1 && _parent.Entries[i].English == _question.Item1 || _language == 2 && _parent.Entries[i].German == _question.Item1) {
                            _parent.Entries[i].Seen = true;
                            _parent.Entries[i].LastTimeWrong = true;
                            _parent.Entries[i].Repeated = 0;
                        }
                    }
                }
                List<Brush> tempListAnswer = new List<Brush>(BackgroundColorsAnswer);
                List<Brush> tempListQuestion = new List<Brush>(BackgroundColorsQuestion);
                List<bool> tempListAnswersClickable = new List<bool>(AnswersClickable);
                List<bool> tempListQuestionsClickable = new List<bool>(QuestionsClickable);

                tempListAnswersClickable[_answer.Item2] = false;
                AnswersClickable = tempListAnswersClickable;

                tempListQuestionsClickable[_question.Item2] = false;
                QuestionsClickable = tempListQuestionsClickable;

                tempListAnswer[_answer.Item2] = (isCorrect)? Brushes.Green : Brushes.Red;
                BackgroundColorsAnswer = tempListAnswer;

                tempListQuestion[_question.Item2] = (isCorrect) ? Brushes.Green : Brushes.Red;
                BackgroundColorsQuestion = tempListQuestion;

                VocabularyEntry tempEntry = new VocabularyEntry() {
                    FilePath = $"{VocabularyEntry.FirstPartFilePath}{_questions[_question.Item1]}{VocabularyEntry.SecondPartFilePath}"
                };
                VocabularyEntry.WriteData(tempEntry, _parent.Entries);
                _answer = (string.Empty, 0);
                _question = (string.Empty, 0);
                if (_counter == 5) {
                    await ExtraFunctions.Wait();
                    _parent.ShowLearnMode();
                }
            }
        }
    }
}
