using LiveCharts.Wpf;
using LiveCharts;
using System.Collections.Generic;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;
using System.Collections.ObjectModel;

namespace VocabTrainer.ViewModels {
    public class AnalysisViewModel : BaseViewModel {
        private List<VocabularyEntry> _allWords = new List<VocabularyEntry>();
        public List<VocabularyEntry> AllWords { 
            get => _allWords;
            set { 
                _allWords = value;
                SearchingWords = new List<VocabularyEntry>(AllWords);
                OnPropertyChanged(nameof(AllWords));
            } 
        }
        private int _seenWords;
        public int SeenWords { 
            get => _seenWords;
            set => _seenWords = value;
             
        }
        private int _lastTimeWrong;
        public int LastTimeWrong { 
            get => _lastTimeWrong;
            set => _lastTimeWrong = value;
        }
        private int _knownWords;
        public int KnownWords { 
            get => _knownWords;
            set => _knownWords = value;
        }
        private int _notSeenWords;
        public int NotSeenWords { 
            get => _notSeenWords; 
            set => _notSeenWords = value;
        }
        private string _wordlist = string.Empty;
        public string Wordlist { get => _wordlist; set => _wordlist = value; }
        private string _allWordsString = string.Empty;
        public string AllWordsString { 
            get => _allWordsString;
            set {
                _allWordsString = value;
                OnPropertyChanged(nameof(AllWordsString));
            }
        }
        private string _seenWordsString = string.Empty;
        public string SeenWordsString { 
            get => _seenWordsString;
            set {
                _seenWordsString = value;
                OnPropertyChanged(nameof(SeenWordsString));
            }
        }
        private string _notSeenWordsString = string.Empty;
        public string NotSeenWordsString { 
            get => _notSeenWordsString;
            set {
                _notSeenWordsString = value;
                OnPropertyChanged(nameof(NotSeenWordsString));
            }
        }
        private string _lastTimeWrongString = string.Empty;
        public string LastTimeWrongString { 
            get => _lastTimeWrongString;
            set {
                _lastTimeWrongString = value;
                OnPropertyChanged(nameof(LastTimeWrongString));
            }
        }
        private string _knownWordsString = string.Empty;
        public string KnownWordsString { 
            get => _knownWordsString;
            set {
                _knownWordsString = value;
                OnPropertyChanged(nameof(KnownWordsString));
            }
        }
        private SeriesCollection _seriesCollection;
        public SeriesCollection SeriesCollection { 
            get => _seriesCollection;
            set {
                _seriesCollection = value;
                OnPropertyChanged(nameof(SeriesCollection));
            } 
        } 

        private string _selectedItem;
        public string SelectedItem {
            get => _selectedItem;
            set {
                _selectedItem = value;
                if (SelectedItem == _standardChoice) Wordlist = string.Empty;
                else Wordlist = SelectedItem;
                _parent.AnalysisOpenedWordlist = SelectedItem;
                GetPercentages();
                CreateDiagram();
                SetStrings();
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        private void SetStrings() {
            AllWordsString = $"words:\t\t\t{AllWords.Count}";
            SeenWordsString = $"seen:\t\t\t{SeenWords}";
            NotSeenWordsString = $"not seen:\t\t{NotSeenWords}";
            KnownWordsString = $"known words:\t\t{KnownWords}";
            LastTimeWrongString = $"unknown words:\t\t{LastTimeWrong}";
        }

        private List<VocabularyEntry> _searchingWords = new List<VocabularyEntry>();
        public List<VocabularyEntry> SearchingWords {
            get => _searchingWords;
            set {
                _searchingWords = value;
                OnPropertyChanged(nameof(SearchingWords));
            } 
        }
        private string _searchingWord = "Searching...";
        public string SearchingWord { 
            get => _searchingWord;
            set {
                _searchingWord = value;
                OnPropertyChanged(nameof(SearchingWord));
            }
        }
        private ObservableCollection<string> _comboBoxEntries = new ObservableCollection<string>();
        public ObservableCollection<string> ComboBoxEntries {
            get => _comboBoxEntries;
            set {
                _comboBoxEntries = value;
                OnPropertyChanged(nameof(ComboBoxEntries));
            }
        }
        private Dictionary<string, WordlistsList> _comboBoxWordlists = new Dictionary<string, WordlistsList>();
        public Dictionary<string, WordlistsList> ComboBoxWordlists {
            get => _comboBoxWordlists;
            set {
                _comboBoxWordlists = value;
                OnPropertyChanged(nameof(ComboBoxWordlists));
            }
        }
        private string _standardChoice = "All words";
        private MainViewModel _parent;
        private string _openedWordlist;
        public AnalysisViewModel(MainViewModel parent, string openedWordlist) {
            _parent = parent;
            _openedWordlist = openedWordlist;
            List<WordlistsList> tempWordlists = WordlistsList.GetWordlistsList();
            foreach (WordlistsList temp in tempWordlists) {
                if (!VocabularyEntry.SpecialWordlistname.Contains(temp.WordlistName)) {
                    string tempString = $"{temp.WordlistName} ({temp.FirstLanguage}, {temp.SecondLanguage})";
                    ComboBoxWordlists.Add(tempString, temp);
                    ComboBoxEntries.Add(tempString);
                }
            }
            ComboBoxEntries.Add(_standardChoice);
            if (_openedWordlist != "" && ComboBoxEntries.Contains(_openedWordlist))  
                SelectedItem = ComboBoxEntries[ComboBoxEntries.IndexOf(_openedWordlist)];
            else
                SelectedItem = (ComboBoxEntries.Count > 0) ? ComboBoxEntries[ComboBoxEntries.Count-1] : null;
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand SearchCommand => new RelayCommand(Search, CanExecuteCommand);
        private void Search(object obj) {
            SearchingWords = new List<VocabularyEntry>(AllWords);
            if (SearchingWord != "" && SearchingWord != "Searching...")
                for (int i = 0; i < SearchingWords.Count; i++)
                    if (!SearchingWords[i].SecondWord.ToLower().Contains(SearchingWord.ToLower()) && !SearchingWords[i].FirstWord.ToLower().Contains(SearchingWord.ToLower())) {
                        SearchingWords.Remove(SearchingWords[i]);
                        i--;
                    }
        }
        private bool CanExecuteResetCommand(object arg) {
            if ((SeenWords != 0 || SeenWords != 0 || KnownWords != 0 || LastTimeWrong != 0) && AllWords.Count != 0) return true;
            else return false;
        }
        public ICommand ResetCommand => new RelayCommand(Reset, CanExecuteResetCommand);

        private void Reset(object obj) {
            VocabularyEntry.UpdateSpecialLists();
            int notSeenIndex = VocabularyEntry.SpecialWordlistname.IndexOf("NotSeen");
            int seenIndex = VocabularyEntry.SpecialWordlistname.IndexOf("NotSeen");
            int ltwIndex = VocabularyEntry.SpecialWordlistname.IndexOf("LastTimeWrong");
            VocabularyEntry entry = new VocabularyEntry();
            List<VocabularyEntry> entries;
            if (Wordlist != "") {
                entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{ComboBoxWordlists[SelectedItem].WordlistName}_{ComboBoxWordlists[SelectedItem].FirstLanguage}_{ComboBoxWordlists[SelectedItem].SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
                entries = VocabularyEntry.GetData(entry);
                for (int i = 0; i < entries.Count; i++) {
                    entries[i].Seen = false;
                    entries[i].LastTimeWrong = false;
                    entries[i].Repeated = 0;
                }
                for (int i = 0; i < entries.Count; i++)
                    if (!VocabularyEntry.EntriesSpecialWordlists[notSeenIndex].Contains(entries[i])) {
                        VocabularyEntry.EntriesSpecialWordlists[notSeenIndex].Add(entries[i]);
                        VocabularyEntry.EntriesSpecialWordlists[seenIndex].Remove(entries[i]);
                    }
                VocabularyEntry.WriteData(entry, entries);
                VocabularyEntry.WriteData(VocabularyEntry.EntrySpecialWordlists[notSeenIndex], VocabularyEntry.EntriesSpecialWordlists[notSeenIndex]);
                VocabularyEntry.WriteData(VocabularyEntry.EntrySpecialWordlists[seenIndex], VocabularyEntry.EntriesSpecialWordlists[seenIndex]);
            } else {
                List<WordlistsList> wordlists = WordlistsList.GetWordlistsList();
                for (int i = 0; i < wordlists.Count; i++) {
                    if (!VocabularyEntry.SpecialWordlistname.Contains(wordlists[i].WordlistName)) {
                        entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{wordlists[i].WordlistName}_{wordlists[i].FirstLanguage}_{wordlists[i].SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
                        entries = VocabularyEntry.GetData(entry);
                        for (int j = 0; j < entries.Count; j++) {
                            entries[j].Seen = false;
                            entries[j].LastTimeWrong = false;
                            entries[j].Repeated = 0;
                        }
                        for (int j = 0; j < entries.Count; j++) { 
                            if (!VocabularyEntry.EntriesSpecialWordlists[notSeenIndex].Contains(entries[j])) VocabularyEntry.EntriesSpecialWordlists[notSeenIndex].Add(entries[j]);
                            if (VocabularyEntry.EntriesSpecialWordlists[seenIndex].Contains(entries[j])) VocabularyEntry.EntriesSpecialWordlists[seenIndex].Remove(entries[j]);
                            if (VocabularyEntry.EntriesSpecialWordlists[ltwIndex].Contains(entries[j])) VocabularyEntry.EntriesSpecialWordlists[ltwIndex].Remove(entries[j]);
                        }
                        VocabularyEntry.WriteData(VocabularyEntry.EntrySpecialWordlists[notSeenIndex], VocabularyEntry.EntriesSpecialWordlists[notSeenIndex]);
                        VocabularyEntry.WriteData(VocabularyEntry.EntrySpecialWordlists[seenIndex], VocabularyEntry.EntriesSpecialWordlists[seenIndex]);
                        VocabularyEntry.WriteData(VocabularyEntry.EntrySpecialWordlists[ltwIndex], VocabularyEntry.EntriesSpecialWordlists[ltwIndex]);
                        VocabularyEntry.WriteData(entry, entries);
                    }
                }
            }
            GetPercentages();
            CreateDiagram();
            SetStrings();
            SearchCommand.Execute(this);
        }
        public void GetPercentages() {
            KnownWords = 0;
            NotSeenWords = 0;
            SeenWords = 0;
            LastTimeWrong = 0;
            AllWords.Clear();
            List<WordlistsList> wordlistsList = WordlistsList.GetWordlistsList();
            List<VocabularyEntry> words;
            VocabularyEntry entry = new VocabularyEntry();

            if (Wordlist == string.Empty) {
                for (int i = 0; i < wordlistsList.Count; i++) 
                    if (!VocabularyEntry.SpecialWordlistname.Contains(wordlistsList[i].WordlistName)) {
                        entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{wordlistsList[i].WordlistName}_{wordlistsList[i].FirstLanguage}_{wordlistsList[i].SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
                        words = VocabularyEntry.GetData(entry);
                        AddCounters(words);
                    }
            } else {
                entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{ComboBoxWordlists[SelectedItem].WordlistName}_{ComboBoxWordlists[SelectedItem].FirstLanguage}_{ComboBoxWordlists[SelectedItem].SecondLanguage}{VocabularyEntry.SecondPartFilePath}";
                words = VocabularyEntry.GetData(entry);
                AddCounters(words);
            }
            SearchingWords = new List<VocabularyEntry>(AllWords);
        }
        public void AddCounters(List<VocabularyEntry> words) {
            foreach (VocabularyEntry word in words) {
                SearchingWords.Add(word);
                AllWords.Add(word);
                if (word.Repeated > 3) KnownWords++;
                else if (word.LastTimeWrong) LastTimeWrong++;
                else if (word.Seen == true) SeenWords++;
                else NotSeenWords++;
            }
        }
        public void CreateDiagram() {
            if (AllWords.Count != 0) 
                SeriesCollection = new SeriesCollection
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
        }
    }
}
