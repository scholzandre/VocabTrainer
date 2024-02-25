using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabTrainer.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class TranslatorViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _firstLanguageWord = string.Empty;
        public string FirstLanguageWord {
            get => _firstLanguageWord;
            set {
                _firstLanguageWord = value;
                OnPropertyChanged(nameof(FirstLanguageWord));
            }
        }
        private string _infoText = string.Empty;
        public string InfoText {
            get => _infoText;
            set {
                _infoText = value;
                OnPropertyChanged(nameof(InfoText));
            }
        }
        private string _secondLanguageWord = string.Empty;
        public string SecondLanguageWord {
            get => _secondLanguageWord;
            set {
                _secondLanguageWord = value;
                OnPropertyChanged(nameof(SecondLanguageWord));
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
        public List<string> OriginalLanguages { get; set; }
        private List<string> _reducedLanguages;
        public List<string> ReducedLanguages {
            get => _reducedLanguages;
            set {
                _reducedLanguages = value;
                _reducedLanguages.Remove(SelectedItemFirstLanguage);
                OnPropertyChanged(nameof(ReducedLanguages));
            }
        }

        private string _selectedItemFirstLanguage;
        public string SelectedItemFirstLanguage {
            get => _selectedItemFirstLanguage;
            set {
                _selectedItemFirstLanguage = value;
                ReducedLanguages = new List<string>(OriginalLanguages); // creates a copy
                SelectedItemSecondLanguage = ReducedLanguages[0];
                OnPropertyChanged(nameof(SelectedItemFirstLanguage));
            }
        }
        private string _selectedItemSecondLanguage;
        public string SelectedItemSecondLanguage {
            get => _selectedItemSecondLanguage;
            set {
                _selectedItemSecondLanguage = value;
                OnPropertyChanged(nameof(SelectedItemSecondLanguage));
            }
        }
        private readonly Dictionary<string, string> _languageAbbreviation = new Dictionary<string, string>() {
            { "German", "de" },
            { "English", "en" },
            { "French", "fr" },
            { "Japanese", "ja" },
            { "Spanish", "es" },
            { "Italian", "it" },
            { "Russian", "ru" }
        };
        private string _selectedItem;
        public string SelectedItem {
            get => _selectedItem;
            set {
                if (_selectedItem != value) {
                    _selectedItem = value;
                    _parent.TranslatorOpenedWordlist = SelectedItem;
                    OnPropertyChanged(nameof(SelectedItem));
                }
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
        private MainViewModel _parent;
        private string _openedWordlist;
        public TranslatorViewModel(MainViewModel parent, string openedWordlist) {
            _parent = parent;
            _openedWordlist = openedWordlist;
            OriginalLanguages = new List<string>() {
                "German",
                "English",
                "French",
                "Japanese",
                "Spanish",
                "Italian",
                "Russian"
            };
            List<WordlistsList> tempWordlists = WordlistsList.GetWordlistsList();
            foreach (WordlistsList temp in tempWordlists) {
                if (temp.WordlistName != "Marked" &&
                    temp.WordlistName != "Seen" &&
                    temp.WordlistName != "LastTimeWrong" &&
                    temp.WordlistName != "NotSeen") {
                    string tempString = $"{temp.WordlistName} ({temp.FirstLanguage}, {temp.SecondLanguage})";
                    ComboBoxWordlists.Add(tempString, temp);
                    ComboBoxEntries.Add(tempString);
                }
            }
            if (_openedWordlist != "" && ComboBoxEntries.Contains(_openedWordlist))
                SelectedItem = ComboBoxEntries[ComboBoxEntries.IndexOf(_openedWordlist)];
            else
                SelectedItem = (ComboBoxEntries.Count > 0) ? ComboBoxEntries[ComboBoxEntries.Count - 1] : null;
            SelectedItemFirstLanguage = OriginalLanguages[0];
        }
        private bool CanExecuteTranslateCommand(object arg) {
            if (FirstLanguageWord != string.Empty) return true;
            else return false;
        }
        public ICommand TranslateCommand => new RelayCommand(Translate, CanExecuteTranslateCommand);
        private async void Translate(object arg) {
            SecondLanguageWord = await TranslateText("", FirstLanguageWord, _languageAbbreviation[SelectedItemFirstLanguage], _languageAbbreviation[SelectedItemSecondLanguage]);
        }
        private bool CanExecuteAddCommand(object arg) {
            return SelectedItem != null && !string.IsNullOrEmpty(FirstLanguageWord) && !string.IsNullOrEmpty(SecondLanguageWord);
        }

        public ICommand AddWordCommand => new RelayCommand(AddWord, CanExecuteAddCommand);

        private void AddWord(object obj) {
            VocabularyEntry entry = new VocabularyEntry() {
                FirstLanguage = ComboBoxWordlists[SelectedItem].FirstLanguage,
                SecondLanguage = ComboBoxWordlists[SelectedItem].SecondLanguage,
                WordList = ComboBoxWordlists[SelectedItem].WordlistName,
                FilePath = $"{VocabularyEntry.FirstPartFilePath}{ComboBoxWordlists[SelectedItem].WordlistName}_{ComboBoxWordlists[SelectedItem].FirstLanguage}_{ComboBoxWordlists[SelectedItem].SecondLanguage}{VocabularyEntry.SecondPartFilePath}"
            };

            if (SelectedItemFirstLanguage.ToLower() == ComboBoxWordlists[SelectedItem].SecondLanguage.ToLower()) 
                entry.SecondWord = SecondLanguageWord;
            else 
                entry.SecondWord = FirstLanguageWord;
            
            if (SelectedItemSecondLanguage.ToLower() == ComboBoxWordlists[SelectedItem].FirstLanguage.ToLower()) 
                entry.FirstWord = FirstLanguageWord;
            else 
                entry.FirstWord = SecondLanguageWord;
            
            List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
            if (entries.Count > 0) {
                bool alreadyExists = false;
                foreach (VocabularyEntry tempEntry in entries) 
                    if (tempEntry.SecondWord.Contains(FirstLanguageWord) || tempEntry.FirstWord.Contains(SecondLanguageWord)) {
                        alreadyExists = true;
                        break;
                    }
                if (alreadyExists) {
                    InfoText = $"{FirstLanguageWord}, {SecondLanguageWord} already exists.";
                } else {
                    entries.Add(entry);
                    InfoText = $"{FirstLanguageWord}, {SecondLanguageWord} has been added.";
                }
            } else {
                entries.Add(entry);
                InfoText = $"{FirstLanguageWord}, {SecondLanguageWord} has been added.";
            }
            VocabularyEntry.AddEntry(VocabularyEntry.SpecialWordlistname.IndexOf("NotSeen"), entry);
            VocabularyEntry.WriteData(entry, entries);
            _parent.ManageEntriesViewModel = new ManageViewModel(_parent, _parent.ManageEntriesOpenedWordlist);
            _parent.LearnEntriesViewModel = new LearnViewModel(_parent);
        }

        static async Task<string> TranslateText(string apiKey, string text, string originLanguage, string targetLanguage) {
            using (HttpClient client = new HttpClient()) {
                string apiUrl = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(text)}&langpair={originLanguage}|{targetLanguage}&key={apiKey}";
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode) {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<MyMemoryApiResponse>(responseContent);
                    if (result != null && result.ResponseData != null) 
                        return result.ResponseData.TranslatedText;
                    else 
                        return "You're out of tokens";
                } else {
                    return "";
                }
            }
        }
        public class MyMemoryApiResponse {
            public ResponseData ResponseData { get; set; }
        }
        public class ResponseData {
            public string TranslatedText { get; set; }
        }
    }
}
