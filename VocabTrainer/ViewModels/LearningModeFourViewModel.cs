using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class LearningModeFourViewModel : BaseViewModel {
        private List<string> _answerString = new List<string>();
        public List<string> AnswerString {
            get => _answerString;
            set {
                _answerString = value;
                OnPropertyChanged(nameof(AnswerString));
            }
        }
        private List<string> _answerText = new List<string>();
        public List<string> AnswerText {
            get => _answerText;
            set {
                _answerText = value;
                OnPropertyChanged(nameof(AnswerText));
            }
        }
        private List<string> _questionString = new List<string>();
        public List<string > QuestionString {
            get => _questionString;
            set {
                _questionString = value;
                OnPropertyChanged(nameof(QuestionString));
            }
        }
        private List<string> _questionText = new List<string>();
        public List<string> QuestionText {
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

        private List<Brush> _backgroundColorsQuestionNG = new List<Brush>(_standardColors);
        private List<Brush> _backgroundColorsQuestion = new List<Brush>(_standardColors);
        public List<Brush> BackgroundColorsQuestion {
            get => _backgroundColorsQuestion;
            set {
                _backgroundColorsQuestion = value;
                OnPropertyChanged(nameof(BackgroundColorsQuestion));
            }
        }
        private List<Brush> _backgroundColorsAnswerNG = new List<Brush>(_standardColors);
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
        private Brush _clickColor = Brushes.Gray;
        private string _questionIdentifier = "question";
        private string _answerIdentifier = "answer";
        public LearningModeFourViewModel(LearnViewModel parent, List<VocabularyEntry> tempEntry) {
            _parent = parent;
            _entries = tempEntry;
            _language = _parent.Random.Next(1, 3);
            QuestionString = (_language == 1) ? tempEntry.Select(x => x.FirstWord).ToList() : tempEntry.Select(x => x.SecondWord).ToList();
            List<VocabularyEntry> tempEntry2 = new List<VocabularyEntry>(tempEntry);
            for (int i = 0; i < tempEntry.Count; i++) {
                _questions.Add((_language == 1) ? tempEntry[i].FirstWord : tempEntry[i].SecondWord, $"{tempEntry[i].WordList}_{tempEntry[i].FirstLanguage}_{tempEntry[i].SecondLanguage}");
                int random = _parent.Random.Next(0, tempEntry2.Count);
                _answers.Add((_language == 1) ? tempEntry2[random].SecondWord : tempEntry2[random].FirstWord, tempEntry2[random].WordList);
                AnswerString.Add((_language == 1) ? tempEntry2[random].SecondWord : tempEntry2[random].FirstWord);
                tempEntry2.Remove(tempEntry2[random]);
            }
            for (int i = 0; i < AnswerString.Count; i++) {
                if (AnswerString[i].Contains(",")) {
                    string[] tempList = AnswerString[i].Split(',');
                    Random random = new Random();
                    int randInt = random.Next(tempList.Length);
                    AnswerText.Add(tempList[randInt].Trim());
                } else {
                    AnswerText.Add(AnswerString[i]);
                }
                if (QuestionString[i].Contains(",")) {
                    string[] tempList = QuestionString[i].Split(',');
                    Random random = new Random();
                    int randInt = random.Next(tempList.Length);
                    QuestionText.Add(tempList[randInt].Trim());
                } else {
                    QuestionText.Add(QuestionString[i]);
                }
            }
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand SetFirstQuestionCommand => new RelayCommand(SetFirstQuestion, CanExecuteCommand);
        private void SetFirstQuestion(object obj) {
            _question = (QuestionString[0], 0);
            CheckAnswer(_questionIdentifier); 
        }
        public ICommand SetSecondQuestionCommand => new RelayCommand(SetSecondQuestion, CanExecuteCommand);
        private void SetSecondQuestion(object obj) {
            _question = (QuestionString[1], 1);
            CheckAnswer(_questionIdentifier); 
        }
        public ICommand SetThirdQuestionCommand => new RelayCommand(SetThirdQuestion, CanExecuteCommand);
        private void SetThirdQuestion(object obj) {
            _question = (QuestionString[2], 2);
            CheckAnswer(_questionIdentifier); 
        }
        public ICommand SetFourthQuestionCommand => new RelayCommand(SetFourthQuestion, CanExecuteCommand);
        private void SetFourthQuestion(object obj) {
            _question = (QuestionString[3], 3);
            CheckAnswer(_questionIdentifier); 
        }
        public ICommand SetFifthQuestionCommand => new RelayCommand(SetFifthQuestion, CanExecuteCommand);
        private void SetFifthQuestion(object obj) {
            _question = (QuestionString[4], 4);
            CheckAnswer(_questionIdentifier); 
        }
        public ICommand SetFirstAnswerCommand => new RelayCommand(SetFirstAnswer, CanExecuteCommand);
        private void SetFirstAnswer(object obj) {
            _answer = (AnswerString[0], 0);
            CheckAnswer(_answerIdentifier); 
        }
        public ICommand SetSecondAnswerCommand => new RelayCommand(SetSecondAnswer, CanExecuteCommand);
        private void SetSecondAnswer(object obj) {
            _answer = (AnswerString[1], 1);
            CheckAnswer(_answerIdentifier); 
        }
        public ICommand SetThirdAnswerCommand => new RelayCommand(SetThirdAnswer, CanExecuteCommand);
        private void SetThirdAnswer(object obj) {
            _answer = (AnswerString[2], 2);
            CheckAnswer(_answerIdentifier); 
        }
        public ICommand SetFourthAnswerCommand => new RelayCommand(SetFourthAnswer, CanExecuteCommand);
        private void SetFourthAnswer(object obj) {
            _answer = (AnswerString[3], 3);
            CheckAnswer(_answerIdentifier); 
        }
        public ICommand SetFifthAnswerCommand => new RelayCommand(SetFifthAnswer, CanExecuteCommand);
        private void SetFifthAnswer(object obj) {
            _answer = (AnswerString[4], 4);
            CheckAnswer(_answerIdentifier);
        }
        private async void CheckAnswer(string field) {
            if (_question.Item1 != string.Empty && _answer.Item1 != string.Empty) {
                VocabularyEntry tempAnswerEntry = new VocabularyEntry() {
                    SecondWord = (_language == 1) ? _answer.Item1 : _question.Item1,
                    FirstWord = (_language == 1) ? _question.Item1 : _answer.Item1,
                    WordList = _answers[_answer.Item1]
                };
                VocabularyEntry tempQuestionEntry = new VocabularyEntry() {
                    SecondWord = (_language == 1) ? _question.Item1 : "",
                    FirstWord = (_language == 1) ? "" : _question.Item1,
                    FilePath = $"{VocabularyEntry.FirstPartFilePath}{_questions[_question.Item1]}{VocabularyEntry.SecondPartFilePath}"
                };
                List<VocabularyEntry> tempEntries = VocabularyEntry.GetData(tempQuestionEntry);
                int index = 0;
                for (int i = 0; i < tempEntries.Count; i++) {
                    if (tempEntries[i].SecondWord == tempQuestionEntry.SecondWord || tempEntries[i].FirstWord == tempQuestionEntry.FirstWord) { 
                        index = i;
                        break;
                    }
                }
                bool isCorrect = VocabularyEntry.CheckAnswer(tempEntries[index], tempAnswerEntry);

                List<bool> tempListAnswersClickable = new List<bool>(AnswersClickable);
                List<bool> tempListQuestionsClickable = new List<bool>(QuestionsClickable);

                if (isCorrect) {
                    _counter++;
                    tempListAnswersClickable[_answer.Item2] = false;
                    AnswersClickable = tempListAnswersClickable;
                    tempListQuestionsClickable[_question.Item2] = false;
                    QuestionsClickable = tempListQuestionsClickable;
                } else {
                    SetAnswerBackground(Brushes.Red);
                    SetQuestionBackground(Brushes.Red);
                }
                _answer = (string.Empty, 0);
                _question = (string.Empty, 0);
                if (_counter > 5) {
                    await ExtraFunctions.Wait();
                    _parent.ShowLearnMode();
                }
            } else {
                if (field == _answerIdentifier) 
                    SetAnswerBackground(_clickColor);
                else 
                    SetQuestionBackground(_clickColor);
            }
        }

        private void SetAnswerBackground(Brush color) {
            BackgroundColorsAnswer = new List<Brush>(_backgroundColorsAnswerNG);
            if (color == Brushes.Gray) 
                _backgroundColorsAnswerNG = new List<Brush>(BackgroundColorsAnswer);
            
            List<Brush> tempListAnswer = new List<Brush>(BackgroundColorsAnswer);
            tempListAnswer[_answer.Item2] = color;
            BackgroundColorsAnswer = tempListAnswer;

            if (color == Brushes.Red)
                _backgroundColorsAnswerNG = new List<Brush>(BackgroundColorsAnswer);
        }
        private void SetQuestionBackground(Brush color) {
            BackgroundColorsQuestion = new List<Brush>(_backgroundColorsQuestionNG);
            if (color == Brushes.Gray)
                _backgroundColorsQuestionNG = new List<Brush>(BackgroundColorsQuestion);
            
            List<Brush> tempListQuestion = new List<Brush>(BackgroundColorsQuestion);
            tempListQuestion[_question.Item2] = color;
            BackgroundColorsQuestion = tempListQuestion;
            
            if (color == Brushes.Red)
                _backgroundColorsQuestionNG = new List<Brush>(BackgroundColorsQuestion);
        }
    }
}
