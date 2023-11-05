using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabTrainer.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class TranslatorViewModel : INotifyPropertyChanged {
        public event EventHandler CanExecuteChange;
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
        public TranslatorViewModel() {
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
            ComboBoxEntries.Add("All words");
            SelectedItem = (ComboBoxEntries.Count > 0) ? ComboBoxEntries[0] : null;
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
                German = FirstLanguageWord,
                English = SecondLanguageWord,
                FirstLanguage = ComboBoxWordlists[SelectedItem].FirstLanguage,
                SecondLanguage = ComboBoxWordlists[SelectedItem].SecondLanguage,
                FilePath = $"{VocabularyEntry.FirstPartFilePath}{ComboBoxWordlists[SelectedItem].WordlistName}_{ComboBoxWordlists[SelectedItem].FirstLanguage}_{ComboBoxWordlists[SelectedItem].SecondLanguage}{VocabularyEntry.SecondPartFilePath}"
            };
            List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
            if (entries.Count > 0) {
                bool alreadyExists = false;
                foreach (VocabularyEntry tempEntry in entries) {
                    if (tempEntry.German.Contains(FirstLanguageWord) || tempEntry.English.Contains(SecondLanguageWord)) {
                        alreadyExists = true;
                        break;
                    }
                }
                if (alreadyExists) {
                    InfoText = "Entry already exists.";
                } else {
                    entries.Add(entry);
                    InfoText = "Entry has been added.";
                }
            } else {
                entries.Add(entry);
                InfoText = "Entry has been added.";
            }

            VocabularyEntry.WriteData(entry, entries);
        }

        static async Task<string> TranslateText(string apiKey, string text, string originLanguage, string targetLanguage) {
            using (HttpClient client = new HttpClient()) {
                string apiUrl = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(text)}&langpair={originLanguage}|{targetLanguage}&key={apiKey}";
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode) {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<MyMemoryApiResponse>(responseContent);
                    if (result != null && result.ResponseData != null) {
                        return result.ResponseData.TranslatedText;
                    } else {
                        return "You're out of tokens";
                    }
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
