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
        readonly private LearnView _parentLearnView;
        public VocabularyEntry Entry = new VocabularyEntry();
        public LearningModeOneView(LearnView parentLearnView) {
            _parentLearnView = parentLearnView;
            Counter = parentLearnView.Counter;
            Vocabulary = parentLearnView.AllWordsList;
            files = parentLearnView.OriginPath;
            Entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{files[Counter]}{VocabularyEntry.SecondPartFilePath}";
            Entry.FirstLanguage = Vocabulary[Counter].FirstLanguage;
            Entry.SecondLanguage = Vocabulary[Counter].SecondLanguage;
            Entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{Vocabulary[Counter].WordList}{VocabularyEntry.SecondPartFilePath}";
            InitializeComponent();
            CheckEmptyLocal();
            CreateGUI();
            SetStar();
        }
        private void CreateGUI() {
            firstLanguage.Text = Vocabulary[Counter].FirstLanguage;
            secondLanguage.Text = Vocabulary[Counter].SecondLanguage;
            germanWord.Text = Vocabulary[Counter].German;
            englishWord.Text = Vocabulary[Counter].English;
        }
        private void NextWord(object sender, RoutedEventArgs e) {
            if (Counter < Vocabulary.Count()) {
                VocabularyEntry entry = new VocabularyEntry() { 
                    FilePath = $"{VocabularyEntry.FirstPartFilePath}{files[Counter]}{VocabularyEntry.SecondPartFilePath}",
                };
                List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
                for (int i = 0; i < entries.Count; i++) {
                    if (entries[i].German == germanWord.Text && entries[i].English == englishWord.Text) {
                        if (files[Counter] != "Marked" &&
                            files[Counter] != "Seen" &&
                            files[Counter] != "NotSeen" &&
                            files[Counter] != "LastTimeWrong") {
                            entries[i].Seen = true;
                            entries[i].Repeated += 1;
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
                        }
                        VocabularyEntry.WriteData(entry, entries);
                    }
                }
                _parentLearnView.GetCounter();
            }
        }
        public bool CheckEmptyLocal() {
            List<string> messages = VocabularyEntry.checkEmpty(Vocabulary);
            if (messages[1] == string.Empty) return false;
            else {
                germanWord.Text = messages[0];
                englishWord.Text = messages[1];
                return true;
            }
        }
        private void SetStar() {
            VocabularyEntry marked = new VocabularyEntry() { 
                FilePath = $"{VocabularyEntry.FirstPartFilePath}{"Marked"}{VocabularyEntry.SecondPartFilePath}",
                German = germanWord.Text,
                English = englishWord.Text,
            };
            List<VocabularyEntry> vocabulary = VocabularyEntry.GetData(marked);
            for (int i = 0; i < vocabulary.Count; i++) {
                if (marked.German == vocabulary[i].German && marked.English == vocabulary[i].English) { 
                    markedButton.Content = "★";
                }
            }
        }
        private void StarWord(object sender, RoutedEventArgs e) {
            VocabularyEntry marked = new VocabularyEntry() { 
                FilePath = $"{VocabularyEntry.FirstPartFilePath}{"Marked"}{VocabularyEntry.SecondPartFilePath}",
                German = germanWord.Text,
                English = englishWord.Text,
                WordList = files[Counter],
                FirstLanguage = Vocabulary[Counter].FirstLanguage,
                SecondLanguage = Vocabulary[Counter].SecondLanguage,
            };
            List<VocabularyEntry> vocabulary = VocabularyEntry.GetData(marked);
            if (markedButton.Content.ToString() == "☆") {
                markedButton.Content = "★";
                vocabulary.Add(marked);
            } else if (markedButton.Content.ToString() == "★") {
                markedButton.Content = "☆";
                for (int i = 0; i < vocabulary.Count; i++) {
                    if (marked.German == vocabulary[i].German && marked.English == vocabulary[i].English) vocabulary.Remove(vocabulary[i]);
                }
            }
            VocabularyEntry.WriteData(marked, vocabulary);
        }
    }
}
