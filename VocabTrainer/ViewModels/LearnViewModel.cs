using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels
{
    public class LearnViewModel : BaseViewModel {
        private UserControl _userControl;
        public UserControl UserControl {
            get => _userControl;
            set {
                _userControl = value;
                OnPropertyChanged(nameof(UserControl));
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
        private List<Settings> _settings;
        private List<Settings> _learningModes = new List<Settings>();
        private List<WordlistsList> _wordlists = new List<WordlistsList>();
        private List<VocabularyEntry> entries = new List<VocabularyEntry>();
        public LearnViewModel() {
            GetSettings();
            Type viewType = typeof(LearningModeOneView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new LearningModeOneViewModel();
        }
        private void GetSettings() { 
            _settings = Settings.GetSettings();
            List<WordlistsList> tempWordlist = WordlistsList.GetWordlistsList();
            for(int i = 2; i < 6; i++) {
                if (_settings[i].LearningMode > 0) {
                    _learningModes.Add(_settings[i]);
                }
            }
            for (int i = 0; i < tempWordlist.Count; i++) { 
                if (tempWordlist[i].IsTrue) _wordlists.Add(tempWordlist[i]);
            }
            for (int i = 0; i < _wordlists.Count; i++) {
                entries.AddRange(
                    VocabularyEntry.GetData(
                        new VocabularyEntry() { 
                            FilePath = $"{VocabularyEntry.FirstPartFilePath}{_wordlists[i].WordlistName}_{_wordlists[i].FirstLanguage}_{_wordlists[i].SecondLanguage}{VocabularyEntry.SecondPartFilePath}"
                        }
                    )
                );
            }
        }
    }
}
