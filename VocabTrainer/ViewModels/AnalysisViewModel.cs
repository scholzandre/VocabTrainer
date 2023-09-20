using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using VocabTrainer.Models;
using VocabTrainer.Views;
using System.Runtime.CompilerServices;

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
        public int KnownWords { get => _knownWords; set => _knownWords = value; }
        private int _notSeenWords;
        public int NotSeenWords { get => _notSeenWords; set => _notSeenWords = value; }
        private string _wordlist = string.Empty;
        public string Wordlist { get => _wordlist; set => _wordlist = value; }
        public event EventHandler CanExecuteChange;
        public AnalysisViewModel(MainWindow parentWindow) {
            ParentWindow = parentWindow;
            GetPercentages();
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }

        public ICommand ResetCommand => new RelayCommand(Reset, CanExecuteCommand);

        private void Reset(object obj) {
            VocabularyEntry entry = new VocabularyEntry();
            if (Wordlist != "") {
                entry.FilePath = $"./../../{Wordlist}.json";
                List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);

                for (int i = 0; i < entries.Count; i++) {
                    entries[i].Seen = false;
                    entries[i].LastTimeWrong = false;
                    entries[i].Repeated = 0;
                }
                VocabularyEntry.WriteData(entry, entries);
            } else {
                List<WordlistsList> wordlists = WordlistsList.GetWordlists();
                for (int i = 0; i < wordlists.Count; i++) {
                    entry.FilePath = $"./../../{wordlists[i].WordlistName}.json";
                    List<VocabularyEntry> vocabulary = VocabularyEntry.GetData(entry);
                    for (int j = 0; j < vocabulary.Count; j++) {
                        vocabulary[j].Seen = false;
                        vocabulary[j].LastTimeWrong = false;
                        vocabulary[j].Repeated = 0;
                        VocabularyEntry.WriteData(entry, vocabulary);
                    }
                }
            }
            GetPercentages();
        }


        public void GetPercentages() {
            KnownWords = 0;
            NotSeenWords = 0;
            SeenWords = 0;
            LastTimeWrong = 0;
            AllWords = new List<VocabularyEntry>();
            List<WordlistsList> wordlistsList = WordlistsList.GetWordlists();

            if (Wordlist == string.Empty) {
                for (int i = 0; i < wordlistsList.Count; i++) {
                    if (wordlistsList[i].WordlistName != "Marked") {
                        VocabularyEntry entry = new VocabularyEntry();
                        entry.FilePath = $"./../../{wordlistsList[i].WordlistName}.json";
                        List<VocabularyEntry> words = VocabularyEntry.GetData(entry);
                        foreach (VocabularyEntry word in words) {
                            AllWords.Add(word);
                            if (word.Repeated > 3) {
                                KnownWords++;
                            } else if (word.LastTimeWrong) {
                                LastTimeWrong++;
                            } else if (word.Seen == true) {
                                SeenWords++;
                            } else {
                                NotSeenWords++;
                            }
                        }
                    }
                }
            } else {
                VocabularyEntry entry = new VocabularyEntry();
                entry.FilePath = $"./../../{Wordlist}.json";
                List<VocabularyEntry> words = VocabularyEntry.GetData(entry);
                foreach (VocabularyEntry word in words) {
                    AllWords.Add(word);
                    if (word.Repeated > 3) {
                        KnownWords++;
                    } else if (word.LastTimeWrong) {
                        LastTimeWrong++;
                    } else if (word.Seen == true) {
                        SeenWords++;
                    } else {
                        NotSeenWords++;
                    }
                }
            }
            CreateDiagram();
        }

        public void CreateDiagram() {
            SeriesCollection seriesCollection = new SeriesCollection();
            if (AllWords.Count != 0) {
                seriesCollection = new SeriesCollection
                    {
                    new PieSeries
                    {
                        Title = "Seen",
                        Values = new ChartValues<double> { SeenWords*100/AllWords.Count },
                        Fill = System.Windows.Media.Brushes.LightYellow
                    },
                    new PieSeries
                    {
                        Title = "Not seen",
                        Values = new ChartValues<double> { NotSeenWords*100/AllWords.Count },
                        Fill = System.Windows.Media.Brushes.CadetBlue
                    },
                    new PieSeries
                    {
                        Title = "Known",
                        Values = new ChartValues<double> { KnownWords*100/AllWords.Count },
                        Fill = System.Windows.Media.Brushes.Green
                    },
                    new PieSeries
                    {
                        Title = "Last time wrong",
                        Values = new ChartValues<double> { LastTimeWrong*100/AllWords.Count },
                        Fill = System.Windows.Media.Brushes.Red
                    }
                };
            }
            ParentWindow.DataContext = new AnalysisView(seriesCollection, this, (AllWords.Count, SeenWords, NotSeenWords, KnownWords, LastTimeWrong));
        }
    }
}
