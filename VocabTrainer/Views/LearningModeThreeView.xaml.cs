using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VocabTrainer.Views {
    public partial class LearningModeThreeView : UserControl {
        int Counter { get; set; }
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        int Language { get; set; }
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        private LearnView _parentLearnView;
        List<int> ints = new List<int>();
        List<VocabularyEntry> vocabulary = new List<VocabularyEntry>();
        int correctAnswerOrder = 0;
        public LearningModeThreeView(LearnView parentLearnView, int counter) {
            _parentLearnView = parentLearnView;
            Counter = counter;
            vocabulary = parentLearnView.allWordsList;
            InitializeComponent();
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
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                    if (button.Content == vocabulary[Counter].German || button.Content == vocabulary[Counter].English) {
                        button.Foreground = Brushes.Green;
                    }
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
                }
            };
            await new ExtraFunctions().Wait();
            _parentLearnView.getCounter();
        }
    }
}
