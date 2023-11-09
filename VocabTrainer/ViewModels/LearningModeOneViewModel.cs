using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        public bool IsMarked { get; set; }
        LearnViewModel _parent;
        public LearningModeOneViewModel(LearnViewModel parent) { 
            _parent = parent;
            FirstWord = _parent.Entries[_parent.Counter].German;
            SecondWord = _parent.Entries[_parent.Counter].English;
            FirstLanguage = _parent.Entries[_parent.Counter].FirstLanguage;
            SecondLanguage = _parent.Entries[_parent.Counter].SecondLanguage;
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand NextWordCommand => new RelayCommand(NextWord, CanExecuteCommand);
        private void NextWord(object obj) {
            _parent.ShowLearnMode();
        }
    }
}
