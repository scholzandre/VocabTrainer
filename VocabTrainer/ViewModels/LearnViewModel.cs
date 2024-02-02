using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class LearnViewModel : BaseViewModel {
        private UserControl _userControl;
        public UserControl UserControl {
            get => _userControl;
            set {
                _userControl = value;
                OnPropertyChanged(nameof(UserControl));
            }
        }
        private string _infoText = "Learn words";
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
        private List<VocabularyEntry> _entries = new List<VocabularyEntry>();
        public List<VocabularyEntry> Entries { 
            get => _entries;
            set { 
                _entries = value;
                OnPropertyChanged(nameof(Entries));
            }
        }
        public int Counter { get; set; } = 0;
        public Random Random = new Random();
        private MainViewModel _parent;
        public LearnViewModel(MainViewModel parent) {
            _parent = parent;
            GetSettings();
            GetEntries();
            ShowLearnMode();
        }

        public void ShowLearnMode() {
            int learningMode = Random.Next(_learningModes.Count);
            InfoText = "Learn words";
            if (_learningModes.Count >= 1 && _learningModes[learningMode].LearningMode == 1 && Entries.Count >= 1 && Entries.Count > Counter) {
                Type viewType = typeof(LearningModeOneView);
                UserControl = (UserControl)Activator.CreateInstance(viewType);
                UserControl.DataContext = new LearningModeOneViewModel(this, _parent);
            } else if (_learningModes.Count >= 1 && _learningModes[learningMode].LearningMode == 2 && Entries.Count >= 1 && Entries.Count > Counter) {
                Type viewType = typeof(LearningModeTwoView);
                UserControl = (UserControl)Activator.CreateInstance(viewType);
                UserControl.DataContext = new LearningModeTwoViewModel(this, _parent);
            } else if (_learningModes.Count >= 1 && _learningModes[learningMode].LearningMode == 3 && Entries.Count >= 5 && Entries.Count > Counter) {
                Type viewType = typeof(LearningModeThreeView);
                UserControl = (UserControl)Activator.CreateInstance(viewType);
                UserControl.DataContext = new LearningModeThreeViewModel(this, _parent);
            } else if (_learningModes.Count >= 1 && _learningModes[learningMode].LearningMode == 4 && Entries.Count >= 5 && Entries.Count > Counter) {
                List<VocabularyEntry> tempEntries = new List<VocabularyEntry>();
                for (int i = 0; i < 5; i++) {
                    if (Counter < Entries.Count() && tempEntries.Contains(Entries[Counter])) {
                        i--;
                        Counter++;
                    } else if (Counter >= Entries.Count()) {
                        Counter = 0;
                        GetEntries();
                        tempEntries.Add(Entries[Counter]);
                    } else {
                        tempEntries.Add(Entries[Counter]);
                        Counter++;
                    }
                }
                Type viewType = typeof(LearningModeFourView);
                UserControl = (UserControl)Activator.CreateInstance(viewType);
                UserControl.DataContext = new LearningModeFourViewModel(this, tempEntries);
            } else
                InfoText = "Not enough word available";

            if (Counter < Entries.Count) {
                Counter++;
            } else if (Entries.Count > 0) {
                Counter = 0;
                GetEntries();
                ShowLearnMode();
            }
        }

        private void GetSettings() {
            _settings = Settings.GetSettings();
            for (int i = 2; i < 6; i++) 
                if (_settings[i].LearningMode > 0 && _settings[i].IsTrue) 
                    _learningModes.Add(_settings[i]);
        }

        private void GetEntries() {
            Entries = new List<VocabularyEntry>();
            List<WordlistsList> tempWordlist = WordlistsList.GetWordlistsList();
            List<VocabularyEntry> tempEntries = new List<VocabularyEntry>();
            for (int i = 0; i < tempWordlist.Count; i++) 
                if (tempWordlist[i].IsTrue) {
                    VocabularyEntry tempEntry = new VocabularyEntry();
                    if (tempWordlist[i].WordlistName != "Marked" &&
                        tempWordlist[i].WordlistName != "Seen" &&
                        tempWordlist[i].WordlistName != "NotSeen" &&
                        tempWordlist[i].WordlistName != "LastTimeWrong") {
                        tempEntry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{tempWordlist[i].WordlistName}_{tempWordlist[i].FirstLanguage}_{tempWordlist[i].SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
                    } else {
                        tempEntry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{tempWordlist[i].WordlistName}{VocabularyEntry.SecondPartFilePath}";
                    }
                    _wordlists.Add(tempWordlist[i]);
                    tempEntries.AddRange(
                        VocabularyEntry.GetData(tempEntry)
                    );
                }
            if (_settings[0].IsTrue) {
                while (tempEntries.Count > 0) { 
                    int index = Random.Next(tempEntries.Count);
                    Entries.Add(tempEntries[index]);
                    tempEntries.Remove(tempEntries[index]);
                }
            } else {
                List<VocabularyEntry> seen = new List<VocabularyEntry>();
                List<VocabularyEntry> notSeen = new List<VocabularyEntry>();
                List<VocabularyEntry> known = new List<VocabularyEntry>();
                List<VocabularyEntry> lastTimeWrong = new List<VocabularyEntry>();
                int counterSeen = 0;
                int counterNotSeen = 0;
                int counterKnown = 0;
                int counterLastTimeWrong = 0;
                foreach(VocabularyEntry tempEntry in tempEntries) { 
                    if (tempEntry.LastTimeWrong) lastTimeWrong.Add(tempEntry);
                    else if (!tempEntry.Seen) notSeen.Add(tempEntry);
                    else if (tempEntry.Seen) seen.Add(tempEntry);
                    else if (tempEntry.Repeated > 5) known.Add(tempEntry);
                }
                for (int i = 0; i < tempEntries.Count; i++) {
                    int percentage = Random.Next(1, 101);
                    if (percentage > 60 && counterLastTimeWrong < lastTimeWrong.Count) {
                        Entries.Add(lastTimeWrong[counterLastTimeWrong]);
                        counterLastTimeWrong++;
                    } else if (percentage > 30 && percentage <= 60 && counterNotSeen < notSeen.Count) {
                        Entries.Add(notSeen[counterNotSeen]);
                        counterNotSeen++;
                    } else if (percentage > 10 && percentage <= 30 && counterSeen < seen.Count) {
                        Entries.Add(seen[counterSeen]);
                        counterSeen++;
                    } else if (percentage > 0 && percentage <= 10 && counterKnown < known.Count) {
                        Entries.Add(known[counterKnown]);
                        counterKnown++;
                    } else {
                        i--;
                    }
                }
            }
        }
    }
}
