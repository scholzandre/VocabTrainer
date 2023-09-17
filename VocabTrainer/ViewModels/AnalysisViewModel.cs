using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class AnalysisViewModel {
        private MainWindow _parentWindow;
        public MainWindow ParentWindow { get => _parentWindow; set => _parentWindow = value; }
        private List<VocabularyEntry> _allWords = new List<VocabularyEntry>();
        public List<VocabularyEntry> AllWords { get => _allWords; set => _allWords = value; }
        private int _seenWords;
        public int SeenWords { get => _seenWords; set => _seenWords = value; }
        private int _lastTimeWrong;
        public int LastTimeWrong { get => _lastTimeWrong; set => _lastTimeWrong = value; }
        private int _knownWords;
        public int KnownWords{ get => _knownWords; set => _knownWords = value; }
        private int _notSeenWords;
        public int NotSeenWords { get => _notSeenWords; set => _notSeenWords = value; }
        public AnalysisViewModel(MainWindow parentWindow) {
            ParentWindow = parentWindow;
            GetPercentages();
            CreateDiagram();
        }

        private void GetPercentages() {
            List<WordlistsList> wordlistsList = WordlistsList.GetWordlists();

            for (int i = 0; i < wordlistsList.Count; i++) {
                if (wordlistsList[i].WordlistName != "Marked") { 
                    VocabularyEntry entry = new VocabularyEntry();
                    entry.FilePath = $"./../../{wordlistsList[i].WordlistName}.json";
                    List<VocabularyEntry> words = VocabularyEntry.GetData(entry);
                    foreach (VocabularyEntry word in words) { 
                        AllWords.Add(word);
                        if (word.Seen == true) { SeenWords++; } else { NotSeenWords++; }
                        if (word.LastTimeWrong) {
                            if (word.LastTimeWrong == true) { LastTimeWrong++; }
                        } else { 
                            if (word.Repeated > 3) KnownWords++;
                        }
                    }
                }
            }
        }

        public void CreateDiagram() {
            SeriesCollection seriesCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Seen",
                    Values = new ChartValues<double> { SeenWords*100/AllWords.Count },
                },
                new PieSeries
                {
                    Title = "Not seen",
                    Values = new ChartValues<double> { NotSeenWords*100/AllWords.Count },
                },
                new PieSeries
                {
                    Title = "Known",
                    Values = new ChartValues<double> { KnownWords*100/AllWords.Count },
                },
                new PieSeries
                {
                    Title = "Last time wrong",
                    Values = new ChartValues<double> { LastTimeWrong*100/AllWords.Count },
                }
            };
            ParentWindow.DataContext = new AnalysisView(seriesCollection);
        }
    }
}
