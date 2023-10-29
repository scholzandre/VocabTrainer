using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    internal class SettingsEntryTextViewModel : BaseViewModel {
        private string _condition;
        public string Condition {
            get => _condition;
            set {
                _condition = value;
                OnPropertyChanged(nameof(Condition));
            }
        }
        private string _value;
        public string Value {
            get => _value;
            set {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        private List<Settings> _settings;
        private MainViewModel _parent;
        public SettingsEntryTextViewModel(string condition, string value, List<Settings> settings, MainViewModel parent) { 
            Condition = condition;
            Value = value;
            _settings = settings;
            _parent = parent;
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand SaveValueCommand => new RelayCommand(SaveValue, CanExecuteCommand);
        private void SaveValue(object obj) {
            if (Value.Length == 7) {
                for(int i = 0; i < _settings.Count; i++)  {
                    if (_settings[i].Condition == Condition) {
                        if (_settings[i].APIKey != null) {
                            _settings[i].APIKey = Value;
                        } else if (_settings[i].BorderBackground != null) {
                            _settings[i].BorderBackground = Value;
                        } else if (_settings[i].BorderBrush != null) {
                            _settings[i].BorderBrush = Value;
                        } else if (_settings[i].ButtonsBackground != null) {
                            _settings[i].ButtonsBackground = Value;
                        } else if (_settings[i].ButtonsForeground != null) {
                            _settings[i].ButtonsForeground = Value;
                        } else if (_settings[i].NavBarBackground != null) {
                            _settings[i].NavBarBackground = Value;
                        }
                    }
                }
                Settings.WriteSettings(_settings);
                _parent.SetColors();
            }
        }
    }
}
