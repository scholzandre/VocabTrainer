using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VocabTrainer.Views {
    public partial class LearningModeTwoView : UserControl {
        List<VocabularyEntry> vocabulary;
        public List<VocabularyEntry> Vocabulary { get => vocabulary; set => vocabulary = value; }
        List<(string firstLanguage, string secondLanguage)> languages = new List<(string, string)>();

        public List<string> files = new List<string>();
        public int Counter { get; set; }
        private LearnView _parentLearnView;

        public Action<object, PropertyChangedEventArgs> PropertyChanged { get; internal set; }

        public LearningModeTwoView(LearnView parentLearnView, int counter) {
            _parentLearnView = parentLearnView;
            Counter = counter;
            Vocabulary = parentLearnView.allWordsList;
            languages = parentLearnView.langues;
            files = parentLearnView.files;
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
                VocabularyEntry entry = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}{_parentLearnView.allWordsList[Counter].WordList}{VocabularyEntry.SecondPartFilePath}" };
                List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
                VocabularyEntry entrySpecial = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}{files[Counter]}{VocabularyEntry.SecondPartFilePath}" };
                List<VocabularyEntry> entrySpecialList = VocabularyEntry.GetData(entrySpecial);


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
                for (int i = 0; i < entries.Count; i++) {
                    if (entries[i].German == vocabulary[Counter].German && entries[i].English == vocabulary[Counter].English) {
                        entries[i].Seen = true;
                        entrySpecialList[Counter].Seen = true;
                        if (answer.Text == "correct") {
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
            firstLanguage.Text = _parentLearnView.allWordsList[Counter].FirstLanguage;
            secondLanguage.Text = _parentLearnView.allWordsList[Counter].SecondLanguage;

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
            marked.FirstLanguage = languages[Counter].firstLanguage;
            marked.SecondLanguage = languages[Counter].secondLanguage;
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

        private void FirstLetter(object sender, RoutedEventArgs e) {
            string word = string.Empty;
            int wordLength = 0;
            if (germanWordBox.IsReadOnly) {
                word = englishWordBox.Text;
                wordLength = word.Length;
                if (wordLength < Vocabulary[Counter].English.Length) {
                    if (word.Substring(0, wordLength) == Vocabulary[Counter].English.Substring(0, wordLength)) {
                        englishWordBox.Text += Vocabulary[Counter].English[wordLength].ToString();
                    } else {
                        englishWordBox.Text = Vocabulary[Counter].English[0].ToString();
                    }
                } else if (wordLength >= Vocabulary[Counter].English.Length && word != Vocabulary[Counter].English) {
                    englishWordBox.Text = Vocabulary[Counter].English[0].ToString();
                } 
            } else {
                word = germanWordBox.Text;
                wordLength = word.Length;
                if (wordLength < Vocabulary[Counter].German.Length) {
                    if (word.Substring(0, wordLength) == Vocabulary[Counter].German.Substring(0, wordLength)) {
                        germanWordBox.Text += Vocabulary[Counter].German[wordLength].ToString();
                    } else {
                        germanWordBox.Text = Vocabulary[Counter].German[0].ToString();
                    }
                } else if (wordLength >= Vocabulary[Counter].German.Length && word != Vocabulary[Counter].German) {
                    germanWordBox.Text = Vocabulary[Counter].German[0].ToString();
                } 
            }
        }
    }
}
