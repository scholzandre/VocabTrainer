using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

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

        private string _selectedItem = "All words";
        public string SelectedItem {
            get => _selectedItem;
            set {
                _selectedItem = value;
                if (SelectedItem == "All words") Wordlist = string.Empty;
                else Wordlist = SelectedItem.Substring(0, (SelectedItem.IndexOf('('))).Trim();
                GetPercentages();
            }
        }
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
            List<VocabularyEntry> entries;

            if (Wordlist != "") {
                entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{Wordlist}{VocabularyEntry.SecondPartFilePath}";
                entries = VocabularyEntry.GetData(entry);
                for (int i = 0; i < entries.Count; i++) {
                    entries[i].Seen = false;
                    entries[i].LastTimeWrong = false;
                    entries[i].Repeated = 0;
                }
                VocabularyEntry notSeenVar = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}NotSeen{VocabularyEntry.SecondPartFilePath}" };
                List <VocabularyEntry> notSeen = VocabularyEntry.GetData(notSeenVar);
                for (int i = 0; i < entries.Count; i++) if (!notSeen.Contains(entries[i])) notSeen.Add(entries[i]);
                VocabularyEntry.WriteData(entry, entries);
                VocabularyEntry.WriteData(notSeenVar, notSeen);
            } else {
                List<WordlistsList> wordlists = WordlistsList.GetWordlistsList();
                for (int i = 0; i < wordlists.Count; i++) {
                    entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{wordlists[i].WordlistName}{VocabularyEntry.SecondPartFilePath}";
                    entries = VocabularyEntry.GetData(entry);
                    for (int j = 0; j < entries.Count; j++) {
                        entries[j].Seen = false;
                        entries[j].LastTimeWrong = false;
                        entries[j].Repeated = 0;
                    }
                    if (wordlists[i].WordlistName != "Marked" &&
                        wordlists[i].WordlistName != "Seen" &&
                        wordlists[i].WordlistName != "NotSeen" &&
                        wordlists[i].WordlistName != "LastTimeWrong") {
                        VocabularyEntry notSeenVar = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}NotSeen{VocabularyEntry.SecondPartFilePath}" };
                        List<VocabularyEntry> notSeen = VocabularyEntry.GetData(notSeenVar);
                        for (int j = 0; j < entries.Count; j++) if (!notSeen.Contains(entries[j])) notSeen.Add(entries[j]);
                        VocabularyEntry.WriteData(notSeenVar, notSeen);
                    }
                    VocabularyEntry.WriteData(entry, entries);
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
            List<WordlistsList> wordlistsList = WordlistsList.GetWordlistsList();
            List<VocabularyEntry> words;
            VocabularyEntry entry = new VocabularyEntry();

            if (Wordlist == string.Empty) {
                for (int i = 0; i < wordlistsList.Count; i++) {
                    if (wordlistsList[i].WordlistName != "Marked" &&
                        wordlistsList[i].WordlistName != "Seen" &&
                        wordlistsList[i].WordlistName != "NotSeen" &&
                        wordlistsList[i].WordlistName != "LastTimeWrong") {
                        entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{wordlistsList[i].WordlistName}{VocabularyEntry.SecondPartFilePath}";
                        words = VocabularyEntry.GetData(entry);
                        AddCounters(words);
                    }
                }
            } else {
                entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{Wordlist}{VocabularyEntry.SecondPartFilePath}";
                words = VocabularyEntry.GetData(entry);
                AddCounters(words);
            }
            CreateDiagram();
        }
        public void AddCounters(List<VocabularyEntry> words) {
            foreach (VocabularyEntry word in words) {
                AllWords.Add(word);
                if (word.Repeated > 3) KnownWords++;
                else if (word.LastTimeWrong) LastTimeWrong++;
                else if (word.Seen == true) SeenWords++;
                else NotSeenWords++;
            }
        }
        public void CreateDiagram() {
            SeriesCollection seriesCollection = new SeriesCollection();
            if (AllWords.Count != 0) 
                seriesCollection = new SeriesCollection
                    {
                    new PieSeries {
                        Values = new ChartValues<double> { SeenWords*100/AllWords.Count },
                        Fill = System.Windows.Media.Brushes.LightYellow
                    },
                    new PieSeries {
                        Values = new ChartValues<double> { NotSeenWords*100/AllWords.Count },
                        Fill = System.Windows.Media.Brushes.CadetBlue
                    },
                    new PieSeries {
                        Values = new ChartValues<double> { KnownWords*100/AllWords.Count },
                        Fill = System.Windows.Media.Brushes.Green
                    },
                    new PieSeries {
                        Values = new ChartValues<double> { LastTimeWrong*100/AllWords.Count },
                        Fill = System.Windows.Media.Brushes.Red
                    }
                };
            ParentWindow.DataContext = new AnalysisView(seriesCollection, this, (AllWords.Count, SeenWords, NotSeenWords, KnownWords, LastTimeWrong));
        }
    }
}
