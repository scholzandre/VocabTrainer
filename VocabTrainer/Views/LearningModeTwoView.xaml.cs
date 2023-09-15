﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace VocabTrainer.Views {
    public partial class LearningModeTwoView : UserControl {
        List<VocabularyEntry> vocabulary;
        public List<VocabularyEntry> Vocabulary { get => vocabulary; set => vocabulary = value; }
        public int Counter { get; set; }
        private LearnView _parentLearnView;

        public Action<object, PropertyChangedEventArgs> PropertyChanged { get; internal set; }

        public LearningModeTwoView(LearnView parentLearnView, int counter) {
            _parentLearnView = parentLearnView;
            Counter = counter;
            Vocabulary = parentLearnView.allWordsList;
            InitializeComponent();
            CreateQuestion();
            checkEmptyLocal();
            SetStar();
        }
        public async void CheckAnswer(object sender, RoutedEventArgs e) {
            checkButton.IsEnabled = false;
            if (Counter < vocabulary.Count()) {
                List<string> correctAnswer;
                List<string> answers;
                bool isCorrect = false;
                bool isPartyCorrect = false;
                bool atLeastOneCorrect = false;
                bool toManyWords = false;
                if (germanWordBox.IsReadOnly == true) {

                    (bool isCorrect, bool isPartyCorrect, bool atLeastOneCorrect, bool toManyWords) returnedBools = CheckInput(SplitAnswer(vocabulary[Counter].English.ToLower()), SplitAnswer(englishWordBox.Text.ToLower()), isCorrect, isPartyCorrect, atLeastOneCorrect, toManyWords); 

                    if (returnedBools.isPartyCorrect && !returnedBools.isCorrect && returnedBools.atLeastOneCorrect || returnedBools.isCorrect && returnedBools.atLeastOneCorrect && returnedBools.toManyWords) {
                        englishWordBox.Foreground = Brushes.Orange;
                        answer.Text = "yeah";
                    } else if (!returnedBools.isCorrect) {
                        englishWordBox.Text = vocabulary[Counter].English;
                        englishWordBox.Foreground = Brushes.Red;
                        answer.Text = "wrong";
                    } else {
                        englishWordBox.Foreground = Brushes.Green;
                        answer.Text = "correct";
                    }

                } else if (englishWordBox.IsReadOnly == true) {
                    (bool isCorrect, bool isPartyCorrect, bool atLeastOneCorrect, bool toManyWords) returnedBools = CheckInput(SplitAnswer(vocabulary[Counter].German.ToLower()), SplitAnswer(germanWordBox.Text.ToLower()), isCorrect, isPartyCorrect, atLeastOneCorrect, toManyWords);

                    if (returnedBools.isPartyCorrect && !returnedBools.isCorrect && returnedBools.atLeastOneCorrect || returnedBools.isCorrect && returnedBools.atLeastOneCorrect && returnedBools.toManyWords) {
                        germanWordBox.Foreground = Brushes.Orange;
                        answer.Text = "yeah";
                    } else if (!returnedBools.isCorrect) {
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

        public List<string> SplitAnswer(string answerString) {
            string[] answers = answerString.Split(',');
            for (int i = 0; i < answers.Length; i++) {
                answers[i] = answers[i].Trim();
            }
            return new List<string>(answers);
        }

        public (bool, bool, bool, bool) CheckInput(List<string> correctAnswer, List<string> answers, bool isCorrect, bool isPartyCorrect, bool atLeastOneCorrect, bool toManyWords) {
            if (answers.Count > correctAnswer.Count) {
                toManyWords = true;
            }
            for (int i = 0; i < correctAnswer.Count; i++) {
                for (int j = 0; j < answers.Count; j++) {
                    if (answers[j] == correctAnswer[i]) {
                        answers.Remove(answers[j]);
                        isCorrect = true;
                        isPartyCorrect = false;
                        atLeastOneCorrect = true;
                        break;
                    } else {
                        isCorrect = false;
                        isPartyCorrect = true;
                    }
                }
            }
            return (isCorrect, isPartyCorrect, atLeastOneCorrect, toManyWords);
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
