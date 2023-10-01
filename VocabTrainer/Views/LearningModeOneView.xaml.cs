using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VocabTrainer.Views {
    public partial class LearningModeOneView : UserControl {
        private List<VocabularyEntry> _vocabulary = new List<VocabularyEntry>();
        public List<VocabularyEntry> Vocabulary { get => _vocabulary; set => _vocabulary = value; }
        public List<string> files = new List<string>();
        public int Counter { get; set; }
        private LearnView _parentLearnView;
        public VocabularyEntry Entry = new VocabularyEntry();
        public LearningModeOneView(LearnView parentLearnView) {
            _parentLearnView = parentLearnView;
            Counter = parentLearnView.Counter;
            Vocabulary = parentLearnView.AllWordsList;
            files = parentLearnView.OriginPath;
            InitializeComponent();
            CheckEmptyLocal();

            if (files[Counter] == "Marked" ||
                files[Counter] == "Seen" ||
                files[Counter] == "NotSeen" ||
                files[Counter] == "LastTimeWrong") {
                Entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{files[Counter]}{VocabularyEntry.SecondPartFilePath}";
                List<VocabularyEntry> entries = VocabularyEntry.GetData(Entry);
                Entry.FirstLanguage = Vocabulary[Counter].FirstLanguage;
                firstLanguage.Text = Vocabulary[Counter].FirstLanguage;
                secondLanguage.Text = Vocabulary[Counter].SecondLanguage;
                Entry.SecondLanguage = Vocabulary[Counter].SecondLanguage;
                Entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{Vocabulary[Counter].WordList}{VocabularyEntry.SecondPartFilePath}";
            } else { 
                firstLanguage.Text = Vocabulary[Counter].FirstLanguage;
                secondLanguage.Text = Vocabulary[Counter].SecondLanguage;
            }

            germanWord.Text = Vocabulary[Counter].German;
            englishWord.Text = Vocabulary[Counter].English;
            SetStar();
        }

        public LearningModeOneView() { }
        private void NextWord(object sender, RoutedEventArgs e) {
            if (Counter < Vocabulary.Count()) {
                VocabularyEntry entry = new VocabularyEntry();
                entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{files[Counter]}{VocabularyEntry.SecondPartFilePath}";
                List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);

                for (int i = 0; i < entries.Count; i++) {
                    if (entries[i].German == germanWord.Text && entries[i].English == englishWord.Text) {
                        if (files[Counter] != "Marked" &&
                            files[Counter] != "Seen" &&
                            files[Counter] != "NotSeen" &&
                            files[Counter] != "LastTimeWrong") {
                            entries[i].Seen = true;
                            entries[i].Repeated += 1;
                            VocabularyEntry.WriteData(entry, entries);
                        } else {
                            List<VocabularyEntry> vocabulary = VocabularyEntry.GetData(Entry);
                            for (int j = 0; j < vocabulary.Count; j++) {
                                if (vocabulary[j].English == englishWord.Text && vocabulary[j].German == germanWord.Text) {
                                    vocabulary[j].Seen = true;
                                    vocabulary[j].Repeated += 1;
                                }
                            }
                            VocabularyEntry.WriteData(Entry, vocabulary);

                            entries[i].Seen = true;
                            entries[i].Repeated += 1;
                            VocabularyEntry.WriteData(entry, entries);
                        }
                    }
                }
                _parentLearnView.GetCounter();
            }
        }

        public bool CheckEmptyLocal() {
            List<string> messages = VocabularyEntry.checkEmpty(Vocabulary);
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
            marked.FilePath = $"{VocabularyEntry.FirstPartFilePath}{"Marked"}{VocabularyEntry.SecondPartFilePath}";
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
            marked.FilePath = $"{VocabularyEntry.FirstPartFilePath}{"Marked"}{VocabularyEntry.SecondPartFilePath}";
            marked.German = germanWord.Text;
            marked.English = englishWord.Text;
            marked.WordList = files[Counter];
            marked.FirstLanguage = Vocabulary[Counter].FirstLanguage;
            marked.SecondLanguage = Vocabulary[Counter].SecondLanguage;
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
