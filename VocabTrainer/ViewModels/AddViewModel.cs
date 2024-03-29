﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class AddViewModel : BaseViewModel {
        private string _selectedItem;
        public string SelectedItem {
            get => _selectedItem;
            set {
                if (ComboBoxWordlists.Count > 0) { 
                    _selectedItem = value;
                    _parent.AddEntryOpenedWordlist = SelectedItem;
                    FirstLanguage = ComboBoxWordlists[value].FirstLanguage;
                    SecondLanguage = ComboBoxWordlists[value].SecondLanguage;
                    OnPropertyChanged(nameof(SelectedItem));
                }
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
        private TextBox _firstWordTextBox;

        public TextBox FirstWordTextBox {
            get { return _firstWordTextBox; }
            set {
                _firstWordTextBox = value;
                OnPropertyChanged(nameof(FirstWordTextBox));
            }
        }
        private string _firstWord = string.Empty;
        public string FirstWord {
            get => _firstWord;
            set {
                _firstWord = value;
                OnPropertyChanged(nameof(FirstWord));
            }
        }
        private string _secondWord = string.Empty;
        public string SecondWord {
            get => _secondWord;
            set {
                _secondWord = value;
                OnPropertyChanged(nameof(SecondWord));
            }
        }
        private string _firstLanguage = string.Empty;
        public string FirstLanguage {
            get => _firstLanguage;
            set {
                _firstLanguage = value;
                OnPropertyChanged(nameof(FirstLanguage));
            }
        }
        private string _secondLanguage = string.Empty;
        public string SecondLanguage {
            get => _secondLanguage;
            set {
                _secondLanguage = value;
                OnPropertyChanged(nameof(SecondLanguage));
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
        private MainViewModel _parent;
        private string _openedWordlist;
        public AddViewModel(MainViewModel parent, string OpenedWordlist) {
            _parent = parent;
            _openedWordlist = OpenedWordlist;
            List<WordlistsList> tempWordlists = WordlistsList.GetWordlistsList();
            foreach (WordlistsList temp in tempWordlists) 
                if (!VocabularyEntry.SpecialWordlistname.Contains(temp.WordlistName)) {
                    string tempString = $"{temp.WordlistName} ({temp.FirstLanguage}, {temp.SecondLanguage})";
                    ComboBoxWordlists.Add(tempString, temp);
                    ComboBoxEntries.Add(tempString);
                }
            if (_openedWordlist != "" && ComboBoxEntries.Contains(_openedWordlist))
                SelectedItem = ComboBoxEntries[ComboBoxEntries.IndexOf(_openedWordlist)];
            else
                SelectedItem = (ComboBoxEntries.Count > 0) ? ComboBoxEntries[ComboBoxEntries.Count - 1] : null;
        }
        private bool CanExecuteAddCommand(object arg) {
            return (FirstWord.Trim() != string.Empty && SecondWord.Trim() != string.Empty && ComboBoxEntries.Count > 0);
        }

        public ICommand AddEntryCommand => new RelayCommand(AddEntry, CanExecuteAddCommand);
        private void AddEntry(object obj) {
            VocabularyEntry entry = new VocabularyEntry() {
                SecondWord = FirstWord.Trim(),
                FirstWord = SecondWord.Trim(),
                FirstLanguage = ComboBoxWordlists[SelectedItem].FirstLanguage,
                SecondLanguage = ComboBoxWordlists[SelectedItem].SecondLanguage,
                WordList = ComboBoxWordlists[SelectedItem].WordlistName,
                FilePath = $"{VocabularyEntry.FirstPartFilePath}{ComboBoxWordlists[SelectedItem].WordlistName}_{ComboBoxWordlists[SelectedItem].FirstLanguage}_{ComboBoxWordlists[SelectedItem].SecondLanguage}{VocabularyEntry.SecondPartFilePath}"
            };
            List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
            bool contains = false;
            foreach (VocabularyEntry tempEntry in entries) 
                if (tempEntry.SecondWord == entry.SecondWord || tempEntry.FirstWord == entry.FirstWord) {
                    contains = true;
                    break;
                }
            if (!contains) {
                entries.Add(entry);
                VocabularyEntry.WriteData(entry, entries);
                InfoText = $"{FirstWord.Trim()} and {SecondWord.Trim()} have been successfully added!";
                FirstWord = string.Empty;
                SecondWord = string.Empty;

                VocabularyEntry.AddEntry(VocabularyEntry.SpecialWordlistname.IndexOf("NotSeen"), entry);
            } else 
                InfoText = "This entry already exists.";
            FirstWordTextBox.Focus();
            _parent.ManageEntriesViewModel = new ManageViewModel(_parent, _parent.ManageEntriesOpenedWordlist);
            _parent.LearnEntriesViewModel = new LearnViewModel(_parent);
            _parent.SettingsViewModel = new SettingsViewModel(_parent);
        }
    }
}
