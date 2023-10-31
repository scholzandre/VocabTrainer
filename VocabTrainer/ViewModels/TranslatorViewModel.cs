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
        private string _secondLanguageWord = string.Empty;
        public string SecondLanguageWord {
            get => _secondLanguageWord;
            set {
                _secondLanguageWord = value;
                OnPropertyChanged(nameof(SecondLanguageWord));
            }
        }
        private ObservableCollection<WordlistsList> _comboBoxEntries;
        public ObservableCollection<WordlistsList> ComboBoxEntries {
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
        private WordlistsList _selectedItem;
        public WordlistsList SelectedItem {
            get => _selectedItem;
            set {
                if (_selectedItem != value) {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
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
            ComboBoxEntries = new ObservableCollection<WordlistsList>(WordlistsList.GetWordlistsList().Where(x => x.WordlistName != "Marked" &&
                                                                                                                  x.WordlistName != "Seen" &&
                                                                                                                  x.WordlistName != "NotSeen" &&
                                                                                                                  x.WordlistName != "LastTimeWrong"));
            SelectedItem = (ComboBoxEntries.Count > 0) ? ComboBoxEntries[0] : null;
            SelectedItemFirstLanguage = OriginalLanguages[0];
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand TranslateCommand => new RelayCommand(Translate, CanExecuteCommand);
        private async void Translate(object arg) {
            if (FirstLanguageWord != string.Empty) SecondLanguageWord = await TranslateText("", FirstLanguageWord, _languageAbbreviation[SelectedItemFirstLanguage], _languageAbbreviation[SelectedItemSecondLanguage]);
        }

        public ICommand AddWordCommand => new RelayCommand(AddWord, CanExecuteCommand);

        private void AddWord(object obj) {
            Debug.WriteLine("Hello World");
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
