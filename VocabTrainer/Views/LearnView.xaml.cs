using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace VocabTrainer.Views {
    public partial class LearnView : UserControl, INotifyPropertyChanged {
        private int _counter;
        public int Counter {
            get { return _counter; }
            set {
                _counter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Counter"));
                UpdateDataContext();
            }
        }
        List<int> alreadyLearned = new List<int>();
        List<int> learningModes = new List<int>();
        List<int> counters = new List<int>();
        List<Settings> settings = Settings.GetSettings();
        public List<VocabularyEntry> allWordsList = new List<VocabularyEntry>();
        bool randomOrder, upCountingOrder = false;
        bool emptyList = true;
        public int allWords = 0;
        Random Random = new Random();

        public LearnView() {
            InitializeComponent();
            getLearningModes();
            GetWordsAndCounter();
            UpdateDataContext();
        }

        private void UpdateDataContext() {
            settings = Settings.GetSettings();
            getLearningModes();
            if (emptyList && Counter == 0 && randomOrder == true) {
                getCounter();
                emptyList = false;
            }
            int random = Random.Next(0, learningModes.Count());
            if (learningModes.Count() > 0 && allWords != 0) {
                switch (learningModes[random]) {
                    case 1:
                        DataContext = new LearningModeOneView(this, Counter);
                        break;
                    case 2:
                        DataContext = new LearningModeTwoView(this, Counter);
                        break;
                    case 3:
                        DataContext = new LearningModeThreeView(this, Counter);
                        break;
                    case 4:
                        setCounters();
                        DataContext = new LearningModeFourView(this, counters);
                        break;
                }
            } else {
                DataContext =  null;
                header.Text = "No learning mode available - not enough words";
            }
        }
        public void getCounter() {
            if (upCountingOrder) {
                Counter = (Counter == allWords - 1) ? Counter = 0 : Counter += 1;
            } else if (randomOrder) {
                while (true) {
                    int index = Random.Next(0, allWords);
                    if (alreadyLearned.Count() == allWords) {
                        alreadyLearned.Clear();
                    } else if (alreadyLearned.Contains(index)) {
                        continue;
                    } else {
                        alreadyLearned.Add(index);
                        Counter = index;
                        break;
                    }
                }
            }
        }
        public void getLearningModes() {
            learningModes.Clear();
            randomOrder = settings[0].IsTrue;
            upCountingOrder = settings[1].IsTrue;
            for (int i = 0; i < settings.Count(); i++) {
                if (settings[i].LearningMode > 0 && settings[i].IsTrue == true)
                    learningModes.Add(settings[i].LearningMode);
            }
        }
        public void setCounters() {
            counters.Clear();
            while (counters.Count() < 5) {
                int index = Random.Next(0, allWords);
                if (alreadyLearned.Count() == allWords) {
                    alreadyLearned.Clear();
                } else if (alreadyLearned.Contains(index) || counters.Contains(index)) {
                    continue;
                } else {
                    alreadyLearned.Add(index);
                    counters.Add(index);
                }
            }
        }
        public void GetWordsAndCounter() {
            WordlistsList wordlistsList = new WordlistsList();
            (int counter, List<VocabularyEntry> words) allWordsAndCounter = wordlistsList.GetAllWords();
            allWords = allWordsAndCounter.counter;
            allWordsList = allWordsAndCounter.words;
        }

        private void Windows_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Windows_MouseDown(sender, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
