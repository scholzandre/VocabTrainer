using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabTrainer.ViewModels {
    public class LearningModeFourViewModel : BaseViewModel {
        private string _firstAnswerText;
        public string FirstAnswerText {
            get => _firstAnswerText;
            set {
                _firstAnswerText = value;
                OnPropertyChanged(nameof(FirstAnswerText));
            }
        }
        private string _secondAnswerText;
        public string SecondAnswerText {
            get => _secondAnswerText;
            set {
                _secondAnswerText = value;
                OnPropertyChanged(nameof(SecondAnswerText));
            }
        }
        private string _thirdAnswerText;
        public string ThirdAnswerText {
            get => _thirdAnswerText;
            set {
                _thirdAnswerText = value;
                OnPropertyChanged(nameof(ThirdAnswerText));
            }
        }
        private string _fourthAnswerText;
        public string FourthAnswerText {
            get => _fourthAnswerText;
            set {
                _fourthAnswerText = value;
                OnPropertyChanged(nameof(FourthAnswerText));
            }
        }
        private string _fifthAnswerText;
        public string FifthAnswerText {
            get => _fifthAnswerText;
            set {
                _fifthAnswerText = value;
                OnPropertyChanged(nameof(FifthAnswerText));
            }
        }
        private string _firstQuestionText;
        public string FirstQuestionText {
            get => _firstQuestionText;
            set {
                _firstQuestionText = value;
                OnPropertyChanged(nameof(FirstQuestionText));
            }
        }
        private string _secondQuestionText;
        public string SecondQuestionText {
            get => _secondQuestionText;
            set {
                _secondQuestionText = value;
                OnPropertyChanged(nameof(SecondQuestionText));
            }
        }
        private string _thirdQuestionText;
        public string ThirdQuestionText {
            get => _thirdQuestionText;
            set {
                _thirdQuestionText = value;
                OnPropertyChanged(nameof(ThirdQuestionText));
            }
        }
        private string _fourthQuestionText;
        public string FourthQuestionText {
            get => _fourthQuestionText;
            set {
                _fourthQuestionText = value;
                OnPropertyChanged(nameof(FourthQuestionText));
            }
        }
        private string _fifthQuestionText;
        public string FifthQuestionText {
            get => _fifthQuestionText;
            set {
                _fifthQuestionText = value;
                OnPropertyChanged(nameof(FifthQuestionText));
            }
        }
        private LearnViewModel _parent;
        public LearningModeFourViewModel(LearnViewModel parent) {
            _parent = parent;
        }
    }
}
