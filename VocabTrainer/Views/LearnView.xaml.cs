using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace VocabTrainer.Views {
    public partial class LearnView : UserControl, INotifyPropertyChanged {
        #region Properties
        private int _counter;
        public int Counter {
            get => _counter;
            set {
                _counter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Counter"));
                UpdateDataContext();
            }
        }
        private List<int> _alreadyLearned = new List<int>();
        public List<int> AlreadyLearned { get => _alreadyLearned; set => _alreadyLearned = value; }
        private List<int> _learningModes = new List<int>();
        public List<int> LearningModes { get => _learningModes; set => _learningModes = value; }
        private List<int> _counters = new List<int>();
        public List<int> Counters { get => _counters; set => _counters = value; }

        private List<VocabularyEntry> _allWordsList = new List<VocabularyEntry>();
        public List<VocabularyEntry> AllWordsList { get => _allWordsList; set => _allWordsList = value; }
        private List<string> _originPath = new List<string>();
        public List<string> OriginPath { get => _originPath; set => _originPath = value; }

        private List<Settings> settings = Settings.GetSettings();
        bool randomOrder, intelligentOrder = false;
        bool emptyList = true;
        private readonly Random _random = new Random();

        #endregion
        public LearnView() {
            InitializeComponent();
            GetLearningModes();
            GetWordsAndCounter();
            UpdateDataContext();
        }

        private void UpdateDataContext() {
            settings = Settings.GetSettings();
            GetLearningModes();
            if (emptyList && Counter == 0 && randomOrder == true) {
                GetCounter();
                emptyList = false;
            }
            int random = _random.Next(0, LearningModes.Count());
            //if (LearningModes.Count() > 0 && AllWordsList.Count != 0) {
            //    switch (LearningModes[random]) {
            //        case 1:
            //            DataContext = new LearningModeOneView(this);
            //            break;
            //        case 2:
            //            DataContext = new LearningModeTwoView(this);
            //            break;
            //        case 3:
            //            DataContext = new LearningModeThreeView(this);
            //            break;
            //        case 4:
            //            SetCounters();
            //            DataContext = new LearningModeFourView(this);
            //            break;
            //    }
            //} else {
            //    DataContext =  null;
            //    header.Text = "No learning mode available - not enough words";
            //}
        }
        public void GetCounter() {

            if (intelligentOrder) {
                Counter = (Counter == AllWordsList.Count - 1) ? Counter = 0 : Counter += 1;
            } else if (randomOrder) {
                while (true) {
                    int index = _random.Next(0, AllWordsList.Count);
                    if (AlreadyLearned.Count() == AllWordsList.Count) AlreadyLearned.Clear();
                    else if (AlreadyLearned.Contains(index)) continue;
                    else {
                        AlreadyLearned.Add(index);
                        Counter = index;
                        break;
                    }
                }
            }
        }
        public void GetLearningModes() {
            LearningModes.Clear();
            randomOrder = settings[0].IsTrue;
            intelligentOrder = settings[1].IsTrue;
            for (int i = 0; i < settings.Count(); i++) {
                if (settings[i].LearningMode > 0 && settings[i].IsTrue == true) LearningModes.Add(settings[i].LearningMode);
            }
        }
        public void SetCounters() {
            Counters.Clear();
            while (Counters.Count() < 5) {
                int index = _random.Next(0, AllWordsList.Count);
                if (AlreadyLearned.Count() == AllWordsList.Count) AlreadyLearned.Clear();
                else if (AlreadyLearned.Contains(index) || Counters.Contains(index)) continue;
                else {
                    AlreadyLearned.Add(index);
                    Counters.Add(index);
                }
            }
        }
        public void GetWordsAndCounter() {
            WordlistsList wordlistsList = new WordlistsList();
            List<(VocabularyEntry entry, string firstLanguage, string secondLanguage, string originPath)> words = wordlistsList.GetAllWords();
            AllWordsList = words.Select(x => x.entry).ToList();
            OriginPath = words.Select(x => x.originPath).ToList();
        }
        private void Windows_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Windows_MouseDown(sender, e);
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
