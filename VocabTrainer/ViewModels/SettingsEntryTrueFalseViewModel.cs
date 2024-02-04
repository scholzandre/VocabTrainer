using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

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
        private string _text;
        public string Text {
            get => _text;
            set {
                _text = value;
                OnPropertyChanged(nameof(Text));
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
        private List<Settings> _settings;
        private List<WordlistsList> _wordlists;
        private SettingsViewModel _settingsViewModel;
        private MainViewModel _parent;
        public SettingsEntryTrueFalseViewModel(string condition, bool isTrue, List<Settings> settings, SettingsViewModel settingsViewModel, List<WordlistsList> wordlists, string text, MainViewModel parent) {
            Condition = condition;
            Text = text;
            IsTrue = isTrue;
            _wordlists = wordlists;
            _settings = settings;
            _settingsViewModel = settingsViewModel;
            _parent = parent;
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand ChangeSettingCommand => new RelayCommand(ChangeSetting, CanExecuteCommand);
        private void ChangeSetting(object obj) {
            if (Condition == _settings[0].Condition) {
                _settings[0].IsTrue = true;
                _settings[1].IsTrue = false;
            } else if (Condition == _settings[1].Condition) {
                _settings[0].IsTrue = false;
                _settings[1].IsTrue = true;
            } else if (Condition == _settings[2].Condition && EnoughWords(1)) {
                _settings[2].IsTrue = IsTrue;
            } else if (Condition == _settings[3].Condition && EnoughWords(1)) {
                _settings[3].IsTrue = IsTrue;
            } else if (Condition == _settings[4].Condition && EnoughWords(5)) {
                _settings[4].IsTrue = IsTrue;
            } else if (Condition == _settings[5].Condition && EnoughWords(5)) {
                _settings[5].IsTrue = IsTrue;
            }
            if (_settings.Where(x => x.LearningMode != 0 && x.IsTrue).Count() == 0) {
                _settings[2].IsTrue = true;
            }
            Settings.WriteSettings(_settings);
            for (int i = 0; i < _wordlists.Count; i++) {
                if (Condition == _wordlists[i].WordlistName) {
                    _wordlists[i].IsTrue = (_wordlists[i].IsTrue) ? false : true;
                    _parent.ListWordlist[_parent.ListWordlist.IndexOf(_wordlists[i])].IsTrue = _wordlists[i].IsTrue;
                }
            }

            if (!EnoughWords(5)) 
                for (int i = 4; i < 6; i++) _settings[i].IsTrue = false;
            else if (!EnoughWords(1)) 
                for (int i = 0; i < 6; i++) _settings[i].IsTrue = false;
            
            WordlistsList.WriteWordlistsList(_wordlists);
            Settings.WriteSettings(_settings);
            _settingsViewModel.FillCollection();
            _parent.LearnEntriesViewModel = new LearnViewModel(_parent);
        }
        private bool EnoughWords(int minNumber) {
            int counter = 0;
            List<WordlistsList> wordlists = _wordlists;
            for (int i = 0; i < _wordlists.Count; i++) 
                if (_wordlists[i].IsTrue) {
                    VocabularyEntry entry = new VocabularyEntry();
                    if (wordlists[i].WordlistName == "Marked" ||
                        wordlists[i].WordlistName == "Seen" ||
                        wordlists[i].WordlistName == "NotSeen" ||
                        wordlists[i].WordlistName == "LastTimeWrong") {
                        entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{wordlists[i].WordlistName}{VocabularyEntry.SecondPartFilePath}";
                    } else {
                        entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{wordlists[i].WordlistName}_{wordlists[i].FirstLanguage}_{wordlists[i].SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
                    }
                    counter += VocabularyEntry.GetData(entry).Count;
                }
            return (counter >= minNumber);
        }
    }
}
