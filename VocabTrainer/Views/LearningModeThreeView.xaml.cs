using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VocabTrainer.Views {
    public partial class LearningModeThreeView : UserControl {
        int Counter { get; set; }
        new public int Language { get; set; }
        private LearnView _parentLearnView;
        private List<int> _ints = new List<int>();
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
            Random random = new Random();
            for (int i = 0; i < 4; i++) {
                int number = random.Next(0, vocabulary.Count()); 
                if (!(number == Counter) && !(_ints.Contains(number))) _ints.Add(number);
                else i--;
            }
            correctAnswerOrder = random.Next(0, 5);
            for (int i = 0; i < 5; i++) {
                if (i < _ints.Count) 
                    if (i == correctAnswerOrder) _ints.Insert(i, Counter);
                else _ints.Add(Counter);
            }

            Language = random.Next(1, 3);
            if (Language == 1) SetText(vocabulary[Counter].German, vocabulary[_ints[0]].English, vocabulary[_ints[1]].English, vocabulary[_ints[2]].English, vocabulary[_ints[3]].English, vocabulary[_ints[4]].English);
            else SetText(vocabulary[Counter].English, vocabulary[_ints[0]].German, vocabulary[_ints[1]].German, vocabulary[_ints[2]].German, vocabulary[_ints[3]].German, vocabulary[_ints[4]].German);
        }
        private void SetText(string questionText, string firstAnswer, string secondAnswer, string thirdAnswer, string fourthAnswer, string fifthAnwser) {
            question.Content = questionText;
            answer1.Content = firstAnswer;
            answer2.Content = secondAnswer;
            answer3.Content = thirdAnswer;
            answer4.Content = fourthAnswer;
            answer5.Content = fifthAnwser;
        }
        private void Choice1(object sender, RoutedEventArgs e) {
            CheckAnswer(answer1.Content.ToString(), sender);
        }
        private void Choice2(object sender, RoutedEventArgs e) {
            CheckAnswer(answer2.Content.ToString(), sender);
        }
        private void Choice3(object sender, RoutedEventArgs e) {
            CheckAnswer(answer3.Content.ToString(), sender);
        }
        private void Choice4(object sender, RoutedEventArgs e) {
            CheckAnswer(answer4.Content.ToString(), sender);
        }
        private void Choice5(object sender, RoutedEventArgs e) {
            CheckAnswer(answer5.Content.ToString(), sender);
        }
        private async void CheckAnswer(string answer, object sender) {
            Button senderButton = (Button)sender;
            VocabularyEntry entry = new VocabularyEntry() { 
                FilePath = $"{VocabularyEntry.FirstPartFilePath}{_parentLearnView.AllWordsList[Counter].WordList}{VocabularyEntry.SecondPartFilePath}"
            };
            List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
            VocabularyEntry entrySpecial = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}{files[Counter]}{VocabularyEntry.SecondPartFilePath}" };
            List<VocabularyEntry> entrySpecialList = VocabularyEntry.GetData(entrySpecial);

            VocabularyEntry lastTimeWrongEntry = vocabulary[Counter];
            lastTimeWrongEntry.FilePath = $"{VocabularyEntry.FirstPartFilePath}LastTimeWrong{VocabularyEntry.SecondPartFilePath}";
            List<VocabularyEntry> list = VocabularyEntry.GetData(lastTimeWrongEntry);

            if (Language == 2 && answer != vocabulary[Counter].German || Language == 1 && answer != vocabulary[Counter].English) senderButton.Foreground = Brushes.Red;
            Grid grid2 = (Grid)grid;
            foreach (var item in grid2.Children) {
                if (item is Button button) {
                    if (button.Content.ToString() != "☆" && button.Content.ToString() != "★") button.IsEnabled = false;
                    if (button.Content.ToString() == vocabulary[Counter].German || button.Content.ToString() == vocabulary[Counter].English) button.Foreground = Brushes.Green;
                }
            };
            for (int i = 0; i < entries.Count; i++) {
                if (entries[i].German == vocabulary[Counter].German && entries[i].English == vocabulary[Counter].English) {
                    entries[i].Seen = true;
                    if (entrySpecial.FilePath == $"{VocabularyEntry.FirstPartFilePath}Marked{VocabularyEntry.SecondPartFilePath}" ||
                            entrySpecial.FilePath == "Seen" ||
                            entrySpecial.FilePath == "NotSeen" ||
                            entrySpecial.FilePath == "LastTimeWrong")
                        entrySpecialList[Counter].Seen = true;
                    if (senderButton.Foreground != Brushes.Red) {
                        entries[i].Repeated++;
                        entries[i].LastTimeWrong = false;
                        if (list.Contains(lastTimeWrongEntry)) list.Remove(lastTimeWrongEntry);
                        if (entrySpecial.FilePath == $"{VocabularyEntry.FirstPartFilePath}Marked{VocabularyEntry.SecondPartFilePath}" ||
                            entrySpecial.FilePath == "Seen" ||
                            entrySpecial.FilePath == "NotSeen" ||
                            entrySpecial.FilePath == "LastTimeWrong") {
                            entrySpecialList[Counter].Repeated++;
                            entrySpecialList[Counter].LastTimeWrong = false;
                        }
                    } else {
                        if (!vocabulary[Counter].LastTimeWrong) {
                            entries[i].Repeated = 0;
                            entries[i].LastTimeWrong = true;
                            if (!list.Contains(lastTimeWrongEntry)) list.Add(lastTimeWrongEntry);
                        }
                        if (entrySpecial.FilePath == $"{VocabularyEntry.FirstPartFilePath}Marked{VocabularyEntry.SecondPartFilePath}" ||
                            entrySpecial.FilePath == "Seen" ||
                            entrySpecial.FilePath == "NotSeen" ||
                            entrySpecial.FilePath == "LastTimeWrong") {
                            entrySpecialList[Counter].Repeated = 0;
                            entrySpecialList[Counter].LastTimeWrong = true;
                        }
                    }
                }
            }
            VocabularyEntry.WriteData(lastTimeWrongEntry, list);
            VocabularyEntry.WriteData(entry, entries);
            if (entrySpecial.FilePath == $"{VocabularyEntry.FirstPartFilePath}Marked{VocabularyEntry.SecondPartFilePath}" ||
                entrySpecial.FilePath == "Seen" ||
                entrySpecial.FilePath == "NotSeen" ||
                entrySpecial.FilePath == "LastTimeWrong")
                VocabularyEntry.WriteData(entrySpecial, entrySpecialList);
            await new ExtraFunctions().Wait();
            _parentLearnView.GetCounter();
        }
        private void SetStar() {
            VocabularyEntry marked = new VocabularyEntry() { 
                FilePath = $"{VocabularyEntry.FirstPartFilePath}{"Marked"}{VocabularyEntry.SecondPartFilePath}",
                German = Vocabulary[Counter].German,
                English = Vocabulary[Counter].English,
            };
            List<VocabularyEntry> vocabulary = VocabularyEntry.GetData(marked);
            for (int i = 0; i < vocabulary.Count; i++) {
                if (marked.German == vocabulary[i].German && marked.English == vocabulary[i].English) markedButton.Content = "★";
            }
        }
        private void StarWord(object sender, RoutedEventArgs e) {
            VocabularyEntry marked = new VocabularyEntry() { 
                FilePath = $"{VocabularyEntry.FirstPartFilePath}{"Marked"}{VocabularyEntry.SecondPartFilePath}",
                German = Vocabulary[Counter].German,
                English = Vocabulary[Counter].English,
                WordList = files[Counter],
                FirstLanguage = _parentLearnView.AllWordsList[Counter].FirstLanguage,
                SecondLanguage = _parentLearnView.AllWordsList[Counter].SecondLanguage,
            };
            markedButton.Content = marked.ChangeMarkedList(marked, markedButton.Content.ToString(), files, Counter, _parentLearnView, Vocabulary);
        }
    }
}
