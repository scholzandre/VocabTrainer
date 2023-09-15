using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VocabTrainer.Views {
    public partial class LearningModeOneView : UserControl {
        List<VocabularyEntry> vocabulary = new List<VocabularyEntry>();
        public int Counter { get; set; }
        private LearnView _parentLearnView;
        public LearningModeOneView(LearnView parentLearnView, int counter) {
            _parentLearnView = parentLearnView;
            Counter = counter;
            vocabulary = parentLearnView.allWordsList;
            InitializeComponent();
            CheckEmptyLocal();
            germanWord.Text = vocabulary[Counter].German;
            englishWord.Text = vocabulary[Counter].English;
            SetStar();
        }

        public LearningModeOneView() { }
        private void NextWord(object sender, RoutedEventArgs e) {
            if (Counter < vocabulary.Count()) {
                germanWord.Text = vocabulary[Counter].German;
                englishWord.Text = vocabulary[Counter].English;

                _parentLearnView.getCounter();
            }
        }

        public bool CheckEmptyLocal() {
            List<string> messages = VocabularyEntry.checkEmpty(vocabulary);
            if (messages[1] == string.Empty) {
                return false;
            } else {
                germanWord.Text = messages[0];
                englishWord.Text = messages[1];
                return true;
            }
        }
        private void SetStar() {
            VocabularyEntry marked = new VocabularyEntry();
            marked.FilePath = $"./../../{"Marked"}.json";
            marked.German = germanWord.Text;
            marked.English = englishWord.Text;
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
            marked.German = germanWord.Text;
            marked.English = englishWord.Text;
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
