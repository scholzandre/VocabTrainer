using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VocabTrainer.Views {
    public partial class LearningModeTwoView : UserControl {
        List<VocabularyEntry> vocabulary = new List<VocabularyEntry>();
        public int Counter { get; set; }
        private LearnView _parentLearnView;

        public Action<object, PropertyChangedEventArgs> PropertyChanged { get; internal set; }

        public LearningModeTwoView(LearnView parentLearnView, int counter) {
            _parentLearnView = parentLearnView;
            Counter = counter;
            vocabulary = parentLearnView.allWordsList;
            InitializeComponent();
            CreateQuestion();
            checkEmptyLocal();
        }
        public async void CheckAnswer(object sender, RoutedEventArgs e) {
            checkButton.IsEnabled = false;
            if (Counter < vocabulary.Count()) {
                if (germanWordBox.IsReadOnly == true) { 
                    if (englishWordBox.Text.ToLower() != vocabulary[Counter].English.ToLower()) {
                        englishWordBox.Text = vocabulary[Counter].English;
                        englishWordBox.Foreground = Brushes.Red;
                        answer.Text = "wrong";
                    } else {
                        englishWordBox.Foreground = Brushes.Green;
                        answer.Text = "correct";
                    }

                } else if (englishWordBox.IsReadOnly == true) { 
                    if (germanWordBox.Text.ToLower() != vocabulary[Counter].German.ToLower()) {
                        germanWordBox.Text = vocabulary[Counter].German;
                        germanWordBox.Foreground = Brushes.Red;
                        answer.Text = "wrong";
                    } else {
                        germanWordBox.Foreground = Brushes.Green;
                        answer.Text = "correct";
                    }
                }  

                await new ExtraFunctions().Wait();
                _parentLearnView.getCounter();
            }
        }

        public bool checkEmptyLocal() {
            List<string> messages = VocabularyEntry.checkEmpty(vocabulary);
            if (messages[1] == string.Empty) {
                return false;
            } else {
                germanWordBox.Text = messages[0];
                englishWordBox.Text = messages[1];
                return true;
            }
        }
        public void CreateQuestion() {
            answer.Text = string.Empty;
            checkButton.IsEnabled = true;
            germanWordBox.Foreground = Brushes.Black;
            englishWordBox.Foreground = Brushes.Black;
            Random random = new Random();
            int randomNumber = random.Next(1, 3);
            if (randomNumber == 1) {
                germanWordBox.Text = vocabulary[Counter].German;
                germanWordBox.IsReadOnly = true;
                englishWordBox.Text = string.Empty;
                englishWordBox.IsReadOnly = false;
            } else {
                englishWordBox.Text = vocabulary[Counter].English;
                englishWordBox.IsReadOnly = true;
                germanWordBox.Text = string.Empty;
                germanWordBox.IsReadOnly = false;
            }
        }
    }
}
