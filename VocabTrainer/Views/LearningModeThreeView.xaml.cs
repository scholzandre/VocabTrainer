﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace VocabTrainer.Views {
    public partial class LearningModeThreeView : UserControl {
        int Counter { get; set; }
        int Language { get; set; }
        private LearnView _parentLearnView;
        List<int> ints = new List<int>();
        private List<VocabularyEntry> vocabulary;
        public List<VocabularyEntry> Vocabulary { get => vocabulary; set => vocabulary = value; }
        int correctAnswerOrder = 0;
        public LearningModeThreeView(LearnView parentLearnView, int counter) {
            _parentLearnView = parentLearnView;
            Counter = counter;
            vocabulary = parentLearnView.allWordsList;
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
            if (Language == 2 && answer != vocabulary[Counter].German || Language == 1 && answer != vocabulary[Counter].English) {
                senderButton.Foreground = Brushes.Red;
            }
            Grid grid2 = (Grid)grid;
            foreach (var item in grid2.Children)
            {
                if (item is Button button) {
                    button.IsEnabled = false;
                    if (button.Content == vocabulary[Counter].German || button.Content == vocabulary[Counter].English) {
                        button.Foreground = Brushes.Green;
                    }
                }
            };
            await new ExtraFunctions().Wait();
            _parentLearnView.getCounter();
        }

        private void SetStar() {
            VocabularyEntry marked = new VocabularyEntry();
            marked.FilePath = $"./../../{"Marked"}.json";
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
            marked.FilePath = $"./../../{"Marked"}.json";
            marked.German = Vocabulary[Counter].German;
            marked.English = Vocabulary[Counter].English;
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
