using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VocabTrainer.Views {
    public partial class LearningModeFourView : UserControl {
        List<int> Counter { get; set; }
        new int Language { get; set; }
        private LearnView _parentLearnView;
        readonly List<int> ints = new List<int>();
        readonly List<VocabularyEntry> vocabulary = new List<VocabularyEntry>();
        int indexFirstChoice;
        int indexSecondChoice;
        int alreadyConnected; 
        public List<string> files = new List<string>();
        bool firstChoice = false;
        bool secondCoice = false;
        Button senderQuestion = null;
        Button senderAnswer = null;
        int counterVar = 0;
        readonly Grid newGrid;
        public LearningModeFourView(LearnView parentLearnView) {
            _parentLearnView = parentLearnView;
            Counter = parentLearnView.Counters;
            vocabulary = parentLearnView.AllWordsList;
            files = parentLearnView.OriginPath;
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
                } else ints.Add(index);
            }
        }
        private void SetValues() {
            foreach (var element in newGrid.Children) 
                if (element is Button button) {
                    if (button.Name.StartsWith("q")) ApplyContentAndName(Language == 1 ? vocabulary[Counter[counterVar]].German : vocabulary[Counter[counterVar]].English, Counter[counterVar], button);
                    else if (button.Name.StartsWith("a")) ApplyContentAndName(Language == 1 ? vocabulary[Counter[ints[counterVar]]].English : vocabulary[Counter[ints[counterVar]]].German, Counter[ints[counterVar]], button);
                    counterVar = (counterVar == 4) ? 0 : counterVar + 1;
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
            if (secondCoice) CheckAnswer();
        }
        private void SetSecondChoice(object sender, int index) {
            Button answer = (Button)sender;
            ChangeBackground();
            answer.Background = Brushes.LightGreen;
            indexSecondChoice = index;
            secondCoice = true;
            senderAnswer = answer;
            if (firstChoice) CheckAnswer();
        }
        private async void CheckAnswer() {
            VocabularyEntry entry = new VocabularyEntry() {
                FilePath = $"{VocabularyEntry.FirstPartFilePath}{_parentLearnView.AllWordsList[indexFirstChoice].WordList}{VocabularyEntry.SecondPartFilePath}",
            };
            List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
            VocabularyEntry entrySpecial = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}{files[Counter[counterVar]]}{VocabularyEntry.SecondPartFilePath}" };
            List<VocabularyEntry> entrySpecialList = VocabularyEntry.GetData(entrySpecial);

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
            for (int i = 0; i < entries.Count; i++) 
                if (entries[i].German == vocabulary[indexFirstChoice].German && entries[i].English == vocabulary[indexFirstChoice].English) {
                    entries[i].Seen = true;
                    entrySpecialList[indexFirstChoice].Seen = true;
                    if (senderQuestion.Foreground == Brushes.Green) {
                        entries[i].Repeated++;
                        entries[i].LastTimeWrong = false;
                        entrySpecialList[indexFirstChoice].Repeated++;
                        entrySpecialList[indexFirstChoice].LastTimeWrong = false;
                    } else {
                        entries[i].Repeated = 0;
                        entries[i].LastTimeWrong = true;
                        entrySpecialList[indexFirstChoice].Repeated = 0;
                        entrySpecialList[indexFirstChoice].LastTimeWrong = true;
                    }
                }
            VocabularyEntry.WriteData(entry, entries);
            VocabularyEntry.WriteData(entrySpecial, entrySpecialList);
            if (alreadyConnected >= 5) {
                message.Text = "correct";
                await new ExtraFunctions().Wait();
                _parentLearnView.GetCounter();
            } 
        }
        private void ChangeBackground() {
            foreach (var element in newGrid.Children) 
                if (element is Button button) {
                    if (!button.IsEnabled) continue;
                    else if (button.Name.StartsWith("a") || button.Name.StartsWith("q")) {
                        button.Background = Brushes.LightGray;
                        button.Foreground = Brushes.Black;
                    }
                }
        }
        private void NotSet() {
            firstChoice = false;
            secondCoice = false;
        }
    }
}
