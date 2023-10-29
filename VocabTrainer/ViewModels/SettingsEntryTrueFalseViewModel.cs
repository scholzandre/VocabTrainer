using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabTrainer.ViewModels {
    internal class SettingsEntryTrueFalseViewModel : BaseViewModel {
        private string _condition;
        public string Condition {
            get => _condition;
            set {
                _condition = value;
                OnPropertyChanged(nameof(Condition));
            }
        }
        private bool _isTrue;
        public bool IsTrue {
            get => _isTrue;
            set {
                _isTrue = value;
                OnPropertyChanged(nameof(IsTrue));
            }
        }
        public SettingsEntryTrueFalseViewModel(string condition, bool isTrue) {
            Condition = condition;
            IsTrue = isTrue;
        }
    }
}
