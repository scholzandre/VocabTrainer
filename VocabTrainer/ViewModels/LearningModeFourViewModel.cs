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
    public class LearningModeFourViewModel : BaseViewModel {
        private List<string> _AnswerText;
        public List<string> AnswerText {
            get => _AnswerText;
            set {
                _AnswerText = value;
                OnPropertyChanged(nameof(AnswerText));
            }
        }
        private List<string> _QuestionText;
        public List<string> QuestionText {
            get => _QuestionText;
            set {
                _QuestionText = value;
                OnPropertyChanged(nameof(QuestionText));
            }
        }
        private string _answer = string.Empty;
        private string _question = string.Empty;
        private readonly LearnViewModel _parent;
        public LearningModeFourViewModel(LearnViewModel parent) {
            _parent = parent;
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand SetFirstQuestionCommand => new RelayCommand(SetFirstQuestion, CanExecuteCommand);
        private void SetFirstQuestion(object obj) {
            _question = QuestionText[0];
        }
        public ICommand SetSecondQuestionCommand => new RelayCommand(SetSecondQuestion, CanExecuteCommand);
        private void SetSecondQuestion(object obj) {
            _question = QuestionText[1];
        }
        public ICommand SetThirdQuestionCommand => new RelayCommand(SetThirdQuestion, CanExecuteCommand);
        private void SetThirdQuestion(object obj) {
            _question = QuestionText[2];
        }
        public ICommand SetFourthQuestionCommand => new RelayCommand(SetFourthQuestion, CanExecuteCommand);
        private void SetFourthQuestion(object obj) {
            _question = QuestionText[3];
        }
        public ICommand SetFifthQuestionCommand => new RelayCommand(SetFifthQuestion, CanExecuteCommand);
        private void SetFifthQuestion(object obj) {
            _question = QuestionText[4];
        }

        public ICommand SetFirstAnswerCommand => new RelayCommand(SetAnswerQuestion, CanExecuteCommand);
        private void SetAnswerQuestion(object obj) {
            _question = AnswerText[0];
        }
        public ICommand SetSecondAnswerCommand => new RelayCommand(SetSecondAnswer, CanExecuteCommand);
        private void SetSecondAnswer(object obj) {
            _question = AnswerText[1];
        }
        public ICommand SetThirdAnswerCommand => new RelayCommand(SetThirdAnswer, CanExecuteCommand);
        private void SetThirdAnswer(object obj) {
            _question = AnswerText[2];
        }
        public ICommand SetFourthAnswerCommand => new RelayCommand(SetFourthAnswer, CanExecuteCommand);
        private void SetFourthAnswer(object obj) {
            _question = AnswerText[3];
        }
        public ICommand SetFifthAnswerCommand => new RelayCommand(SetFifthAnswer, CanExecuteCommand);
        private void SetFifthAnswer(object obj) {
            _question = AnswerText[4];
        }
    }
}
