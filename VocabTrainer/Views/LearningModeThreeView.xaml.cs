using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VocabTrainer.Views {
    public partial class LearningModeThreeView : UserControl {
        int Counter { get; set; }
        int Language { get; set; }
        private LearnView _parentLearnView;
        List<int> ints = new List<int>();
        private List<VocabularyEntry> vocabulary; 
        public List<string> files = new List<string>();

        public List<VocabularyEntry> Vocabulary { get => vocabulary; set => vocabulary = value; }
        int correctAnswerOrder = 0;
        public LearningModeThreeView(LearnView parentLearnView) {
            _parentLearnView = parentLearnView;
            Counter = parentLearnView.Counter;
            vocabulary = parentLearnView.AllWordsList;
            files = parentLearnView.OriginPath;
            InitializeComponent();
            SetStar();
            CreateQuestion();
        }
        public void CreateQuestion() {
            ints.Clear();
            Random random = new Random();
            for (int i = 0; i < 4; i++) {
                int number = random.Next(0, vocabulary.Count()); 
                if (!(number == Counter) && !(ints.Contains(number))) {
                    ints.Add(number);
                } else {
                    i--;
                }
            }
            correctAnswerOrder = random.Next(0, 5);

            for (int i = 0; i < 5; i++) {
                if (i < ints.Count) {
                    if (i == correctAnswerOrder) {
                        ints.Insert(i, Counter);
                    }
                } else {
                    ints.Add(Counter);
                }
            }

            Language = random.Next(1, 3);
             
            if (Language == 1) {
                question.Content = vocabulary[Counter].German;
                answer1.Content = vocabulary[ints[0]].English;
                answer2.Content = vocabulary[ints[1]].English;
                answer3.Content = vocabulary[ints[2]].English;
                answer4.Content = vocabulary[ints[3]].English;
                answer5.Content = vocabulary[ints[4]].English;
            } else {
                question.Content = vocabulary[Counter].English;
                answer1.Content = vocabulary[ints[0]].German;
                answer2.Content = vocabulary[ints[1]].German;
                answer3.Content = vocabulary[ints[2]].German;
                answer4.Content = vocabulary[ints[3]].German;
                answer5.Content = vocabulary[ints[4]].German;
            }
        }

        private void choice1(object sender, RoutedEventArgs e) {
            checkAnswer(answer1.Content.ToString(), sender);
        }

        private void choice2(object sender, RoutedEventArgs e) {
            checkAnswer(answer2.Content.ToString(), sender);
        }

        private void choice3(object sender, RoutedEventArgs e) {
            checkAnswer(answer3.Content.ToString(), sender);
        }

        private void choice4(object sender, RoutedEventArgs e) {
            checkAnswer(answer4.Content.ToString(), sender);
        }
        private void choice5(object sender, RoutedEventArgs e) {
            checkAnswer(answer5.Content.ToString(), sender);
        }

        private async void checkAnswer(string answer, object sender) {
            Button senderButton = (Button)sender;
            VocabularyEntry entry = new VocabularyEntry();
            entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{_parentLearnView.AllWordsList[Counter].WordList}{VocabularyEntry.SecondPartFilePath}";
            List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
            VocabularyEntry entrySpecial = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}{files[Counter]}{VocabularyEntry.SecondPartFilePath}" };
            List<VocabularyEntry> entrySpecialList = VocabularyEntry.GetData(entrySpecial);

            if (Language == 2 && answer != vocabulary[Counter].German || Language == 1 && answer != vocabulary[Counter].English) {
                senderButton.Foreground = Brushes.Red;
            }
            Grid grid2 = (Grid)grid;
            foreach (var item in grid2.Children)
            {
                if (item is Button button) {
                    if (button.Content.ToString() != "☆" && button.Content.ToString() != "★") button.IsEnabled = false;
                    if (button.Content.ToString() == vocabulary[Counter].German || button.Content.ToString() == vocabulary[Counter].English) {
                        button.Foreground = Brushes.Green;
                    }
                }
            };
            for (int i = 0; i < entries.Count; i++) {
                if (entries[i].German == vocabulary[Counter].German && entries[i].English == vocabulary[Counter].English) {
                    entries[i].Seen = true;
                    entrySpecialList[Counter].Seen = true;
                    if (senderButton.Foreground != Brushes.Red) {
                        entries[i].Repeated++;
                        entries[i].LastTimeWrong = false;
                        entrySpecialList[Counter].Repeated++;
                        entrySpecialList[Counter].LastTimeWrong = false;
                    } else {
                        entries[i].Repeated = 0;
                        entries[i].LastTimeWrong = true;
                        entrySpecialList[Counter].Repeated = 0;
                        entrySpecialList[Counter].LastTimeWrong = true;
                    }
                }
            }
            VocabularyEntry.WriteData(entry, entries);
            VocabularyEntry.WriteData(entrySpecial, entrySpecialList);
            await new ExtraFunctions().Wait();
            _parentLearnView.GetCounter();
        }

        private void SetStar() {
            VocabularyEntry marked = new VocabularyEntry();
            marked.FilePath = $"{VocabularyEntry.FirstPartFilePath}{"Marked"}{VocabularyEntry.SecondPartFilePath}";
            marked.German = Vocabulary[Counter].German;
            marked.English = Vocabulary[Counter].English;
            List<VocabularyEntry> vocabulary = VocabularyEntry.GetData(marked);
            for (int i = 0; i < vocabulary.Count; i++) {
                if (marked.German == vocabulary[i].German && marked.English == vocabulary[i].English) {
                    markedButton.Content = "★";
                }
            }
        }
        private void StarWord(object sender, RoutedEventArgs e) {
            VocabularyEntry marked = new VocabularyEntry();
            marked.FilePath = $"{VocabularyEntry.FirstPartFilePath}{"Marked"}{VocabularyEntry.SecondPartFilePath}";
            marked.German = Vocabulary[Counter].German;
            marked.English = Vocabulary[Counter].English;
            marked.WordList = files[Counter];
            marked.FirstLanguage = _parentLearnView.AllWordsList[Counter].FirstLanguage;
            marked.SecondLanguage = _parentLearnView.AllWordsList[Counter].SecondLanguage;
            List<VocabularyEntry> vocabulary = VocabularyEntry.GetData(marked);

            if (markedButton.Content.ToString() == "☆") {
                markedButton.Content = "★";
                vocabulary.Add(marked);
            } else if (markedButton.Content.ToString() == "★") {
                markedButton.Content = "☆";
                for (int i = 0; i < vocabulary.Count; i++) {
                    if (marked.German == vocabulary[i].German && marked.English == vocabulary[i].English) {
                        vocabulary.Remove(vocabulary[i]);
                    }
                }
            }
            VocabularyEntry.WriteData(marked, vocabulary);
        }
    }
}
