﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VocabTrainer.Views {
    public partial class LearningModeFourView : UserControl {
        List<int> Counter { get; set; }
        new int Language { get; set; }
        private LearnView _parentLearnView;
        List<int> ints = new List<int>();
        List<VocabularyEntry> vocabulary = new List<VocabularyEntry>();
        int indexFirstChoice;
        int indexSecondChoice;
        int alreadyConnected;
        bool firstChoice = false;
        bool secondCoice = false;
        Button senderQuestion = null;
        Button senderAnswer = null;
        int counterVar = 0;
        Grid newGrid;
        public LearningModeFourView(LearnView parentLearnView, List<int> counter) {
            _parentLearnView = parentLearnView;
            Counter = counter;
            vocabulary = parentLearnView.allWordsList;
            InitializeComponent();
            newGrid = (Grid)grid;
            CreateQuestion();
        }
        public void CreateQuestion() {
            CreateRandomOrder();
            SetValues();
        }

        private void CreateRandomOrder() {
            Random random = new Random();
            Language = random.Next(1, 3);
            ints.Clear();

            for (int i = 0; i < 5; i++) {
                int index = random.Next(0, 5);
                if (ints.Contains(index)) {
                    i--;
                    continue;
                } else {
                    ints.Add(index);
                }
            }
        }
        private void SetValues() {
            foreach (var element in newGrid.Children) {
                if (element is Button button) {
                    if (button.Name.StartsWith("q")) {
                        ApplyContentAndName(Language == 1 ? vocabulary[Counter[counterVar]].German : vocabulary[Counter[counterVar]].English, Counter[counterVar], button);
                    } else if (button.Name.StartsWith("a")) {
                        ApplyContentAndName(Language == 1 ? vocabulary[Counter[ints[counterVar]]].English : vocabulary[Counter[ints[counterVar]]].German, Counter[ints[counterVar]], button);
                    }
                    counterVar = (counterVar == 4) ? 0 : counterVar + 1;
                }
            }
        }
        void ApplyContentAndName(string content, int counterValue, object button) {
            Button newButton = (Button)button;
            newButton.Content = content;
            newButton.Name += counterValue.ToString();
        }
        private void AnswerButton(object sender, RoutedEventArgs e) {
            Button button = (Button)sender;
            int index = Int32.Parse(button.Name.Substring(7));
            SetSecondChoice(sender, index);
        }

        private void QuestionButton(object sender, RoutedEventArgs e) {
            Button button = (Button) sender;
            int index = Int32.Parse(button.Name.Substring(9));
            SetFirstChoice(sender, index);
        }

        private void SetFirstChoice(object sender, int index) {
            Button question = (Button)sender;
            ChangeBackground();
            question.Background = Brushes.LightGreen;
            indexFirstChoice = index;
            firstChoice = true;
            senderQuestion = question;
            if (secondCoice) {
                CheckAnswer();
            }
        }
        private void SetSecondChoice(object sender, int index) {
            Button answer = (Button)sender;
            ChangeBackground();
            answer.Background = Brushes.LightGreen;
            indexSecondChoice = index;
            secondCoice = true;
            senderAnswer = answer;
            if (firstChoice) {
                CheckAnswer();
            }
        }

        private async void CheckAnswer() {
            if (vocabulary[indexFirstChoice].German == vocabulary[indexSecondChoice].German) {
                senderQuestion.Foreground = Brushes.Green;
                senderQuestion.IsEnabled = false;
                senderAnswer.Foreground = Brushes.Green;
                senderAnswer.IsEnabled = false;
                NotSet();
                alreadyConnected++;
            } else if (vocabulary[indexFirstChoice].German != vocabulary[indexSecondChoice].German) { 
                ChangeBackground();
                NotSet();
                senderQuestion.Foreground = Brushes.Red;
                senderAnswer.Foreground = Brushes.Red;
                await new ExtraFunctions().Wait();
                ChangeBackground();
            }
            if (alreadyConnected >= 5) {
                message.Text = "correct";
                await new ExtraFunctions().Wait();
                _parentLearnView.getCounter();
            } 
        }

        private void ChangeBackground() {
            foreach (var element in newGrid.Children) {
                if (element is Button button) {
                    if (!button.IsEnabled) {
                        continue;
                    } else if (button.Name.StartsWith("a") || button.Name.StartsWith("q")) {
                        button.Background = Brushes.LightGray;
                        button.Foreground = Brushes.Black;
                    }
                }
            }
        }
        private void NotSet() {
            firstChoice = false;
            secondCoice = false;
        }
    }
}
