using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class SettingsViewModel : BaseViewModel {
        private readonly List<Settings> _settings;
        private readonly List<WordlistsList> _wordlists;
        public ObservableCollection<UserControl> _listGeneralSettings;
        public ObservableCollection<UserControl> ListGeneralSettings {
            get => _listGeneralSettings;
            set {
                _listGeneralSettings = value;
                OnPropertyChanged(nameof(ListGeneralSettings));
            }
        }
        public ObservableCollection<UserControl> _listWordlists;
        public ObservableCollection<UserControl> ListWordlists {
            get => _listWordlists;
            set {
                _listWordlists = value;
                OnPropertyChanged(nameof(ListWordlists));
            }
        }
        public string _importInformation;
        public string ImportInformation {
            get => _importInformation;
            set {
                _importInformation = value;
                OnPropertyChanged(nameof(ImportInformation));
            }
        }
        private MainViewModel _parent;
        public SettingsViewModel(MainViewModel parent) {
            _parent = parent;
            _parent.ListWordlist = WordlistsList.GetWordlistsList();
            _settings = _parent.SettingsList;
            _wordlists = _parent.ListWordlist;
            FillCollection();
        }

        public void FillCollection() {
            ListGeneralSettings = new ObservableCollection<UserControl>();
            ListWordlists = new ObservableCollection<UserControl>();
            for (int i = 0; i < _settings.Count; i++) {
                if (_settings[i].LearningMode > 0 ||
                    _settings[i].Condition == "random order" ||
                    _settings[i].Condition == "intelligent order") {
                    Type viewType = typeof(SettingsEntryTrueFalseView);
                    UserControl tempControl = (UserControl)Activator.CreateInstance(viewType);
                    tempControl.DataContext = new SettingsEntryTrueFalseViewModel(_settings[i].Condition, _settings[i].IsTrue, _settings, this, _wordlists, _settings[i].Condition, _parent);
                    ListGeneralSettings.Add(tempControl);
                } else {
                    Type viewType = typeof(SettingsEntryTextView);
                    UserControl tempControl = (UserControl)Activator.CreateInstance(viewType);
                    if (_settings[i].BorderBackground != null) {
                        tempControl.DataContext = new SettingsEntryTextViewModel(_settings[i].Condition, _settings[i].BorderBackground, _settings, _parent);
                    } else if (_settings[i].BorderBrush != null) {
                        tempControl.DataContext = new SettingsEntryTextViewModel(_settings[i].Condition, _settings[i].BorderBrush, _settings, _parent);
                    } else if (_settings[i].ButtonsBackground != null) {
                        tempControl.DataContext = new SettingsEntryTextViewModel(_settings[i].Condition, _settings[i].ButtonsBackground, _settings, _parent);
                    } else if (_settings[i].ButtonsForeground != null) {
                        tempControl.DataContext = new SettingsEntryTextViewModel(_settings[i].Condition, _settings[i].ButtonsForeground, _settings, _parent);
                    } else if (_settings[i].NavBarBackground != null) {
                        tempControl.DataContext = new SettingsEntryTextViewModel(_settings[i].Condition, _settings[i].NavBarBackground, _settings, _parent);
                    }
                    ListGeneralSettings.Add(tempControl);
                }
            }
            for (int i = 0; i < _wordlists.Count; i++) {
                Type viewType = typeof(SettingsEntryTrueFalseView);
                UserControl tempControl = (UserControl)Activator.CreateInstance(viewType);
                tempControl.DataContext = new SettingsEntryTrueFalseViewModel(_wordlists[i].WordlistName, _wordlists[i].IsTrue, _settings, this, _wordlists, $"{_wordlists[i].WordlistName} ({_wordlists[i].FirstLanguage}, {_wordlists[i].SecondLanguage})", _parent);
                ListWordlists.Add(tempControl);
            }
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand ImportListCommand => new RelayCommand(ImportList, CanExecuteCommand);
        private void ImportList(object obj) {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document";
            dialog.DefaultExt = ".json"; 
            dialog.Filter = "Text documents (.json)|*.json"; 
            bool? result = dialog.ShowDialog();
            if (result == true) {
                List<WordlistsList> list = WordlistsList.GetWordlistsList();
                ImportInformation = string.Empty;
                string wholeWordlist = dialog.FileName.Substring(dialog.FileName.LastIndexOf(@"\")+1);
                if (wholeWordlist.Count(x => x == '_') == 2) {
                    string wordlist = wholeWordlist.Substring(0, wholeWordlist.IndexOf("_"));
                    string firstlanguage = wholeWordlist.Substring(wordlist.Length+1, wholeWordlist.Substring(wordlist.Length+1).IndexOf("_"));
                    string secondLanguage = wholeWordlist.Substring(wholeWordlist.LastIndexOf("_")+1, wholeWordlist.Length - wholeWordlist.LastIndexOf("_") - 1 - VocabularyEntry.SecondPartFilePath.Length);
                    WordlistsList wordlistsList = new WordlistsList() { 
                        WordlistName = wordlist, 
                        FirstLanguage = firstlanguage,
                        SecondLanguage = secondLanguage
                    };
                    if (!list.Contains(wordlistsList)) {
                        list.Add(wordlistsList);
                        WordlistsList.WriteWordlistsList(list);
                    } else {
                        ImportInformation = "List already exists.";
                    }
                    ImportInformation = "Import successfully";
                } else {
                    ImportInformation = "File name not valid";
                }
            }
        }
    }
}
