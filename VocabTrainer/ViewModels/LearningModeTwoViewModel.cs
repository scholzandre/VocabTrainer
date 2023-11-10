﻿using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class LearningModeTwoViewModel : BaseViewModel {
        private string _firstWord;  
        private string _secondWord;
        private string _firstWordAnswer = string.Empty;
        public string FirstWordAnswer {
            get => _firstWordAnswer;
            set {
                _firstWordAnswer = value;
                OnPropertyChanged(nameof(FirstWordAnswer));
            }
        }
        private string _secondWordAnswer = string.Empty;
        public string SecondWordAnswer {
            get => _secondWordAnswer;
            set {
                _secondWordAnswer = value;
                OnPropertyChanged(nameof(SecondWordAnswer));
            }
        }
        private string _firstLanguage;
        public string FirstLanguage {
            get => _firstLanguage;
            set {
                _firstLanguage = value;
                OnPropertyChanged(nameof(FirstLanguage));
            }
        }
        private string _secondLanguage;
        public string SecondLanguage {
            get => _secondLanguage;
            set {
                _secondLanguage = value;
                OnPropertyChanged(nameof(SecondLanguage));
            }
        }
        private string _star;
        public string Star {
            get => _star;
            set {
                _star = value;
                OnPropertyChanged(nameof(Star));
            }
        }
        private string _infoText;
        public string InfoText {
            get => _infoText;
            set {
                _infoText = value;
                OnPropertyChanged(nameof(InfoText));
            }
        }
        private string _buttonText;
        public string ButtonText {
            get => _buttonText;
            set {
                _buttonText = value;
                OnPropertyChanged(nameof(ButtonText));
            }
        }
        private bool _firstWordWritable = false;
        public bool FirstWordWritable { 
            get => _firstWordWritable;
            set {
                _firstWordWritable = value;
                OnPropertyChanged(nameof(FirstWordWritable));
            }
        }
        private bool _secondWordWritable = false;
        public bool SecondWordWritable {
            get => _secondWordWritable;
            set {
                _secondWordWritable = value;
                OnPropertyChanged(nameof(SecondWordWritable));
            }
        }
        private SolidColorBrush _firstWordForeground = Brushes.Black;
        public SolidColorBrush FirstWordForeground {
            get => _firstWordForeground;
            set {
                _firstWordForeground = value;
                OnPropertyChanged(nameof(FirstWordForeground));
            }
        }
        private SolidColorBrush _secondWordForeground = Brushes.Black;
        public SolidColorBrush SecondWordForeground {
            get => _secondWordForeground;
            set {
                _secondWordForeground = value;
                OnPropertyChanged(nameof(SecondWordForeground));
            }
        }
        public bool IsMarked { get; set; }
        private readonly LearnViewModel _parent;
        static VocabularyEntry markEntry = new VocabularyEntry() { FilePath = $"{VocabularyEntry.FirstPartFilePath}Marked{VocabularyEntry.SecondPartFilePath}" };
        private List<VocabularyEntry> markedEntries = VocabularyEntry.GetData(markEntry);
        private int _counter;
        private string _checkText = "check answer";
        private string _nextText = "next";
        private int _hints = 0;
        public LearningModeTwoViewModel(LearnViewModel parent) {
            _parent = parent;
            _counter = _parent.Counter;
            _firstWord = _parent.Entries[_counter].German;
            _secondWord = _parent.Entries[_counter].English;
            FirstLanguage = _parent.Entries[_counter].FirstLanguage;
            SecondLanguage = _parent.Entries[_counter].SecondLanguage;
            Star = (markedEntries.Contains(_parent.Entries[_counter])) ? "★" : "☆";
            ButtonText = _checkText;
            if (_parent.Random.Next(2) == 1) {
                FirstWordWritable = true;
                SecondWordAnswer = _secondWord;
            } else { 
                SecondWordWritable = true;
                FirstWordAnswer = _firstWord;
            }
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand CheckAnswerCommand => new RelayCommand(CheckAnswer, CanExecuteCommand);
        private void CheckAnswer(object obj) {
            if (ButtonText == _checkText) {
                if (FirstWordAnswer == _parent.Entries[_counter].German && SecondWordAnswer == _parent.Entries[_counter].English) {
                    _parent.Entries[_counter].Seen = true;
                    if (_hints < 3) _parent.Entries[_counter].Repeated += 1;
                    _parent.Entries[_counter].LastTimeWrong = false;
                    if (FirstWordWritable) {
                        FirstWordForeground = Brushes.Green;
                    } else {
                        SecondWordForeground = Brushes.Green;
                    }
                } else {
                    _parent.Entries[_counter].Seen = true;
                    _parent.Entries[_counter].Repeated = 0;
                    _parent.Entries[_counter].LastTimeWrong = true;
                    if (FirstWordWritable) {
                        FirstWordForeground = Brushes.Red;
                    } else { 
                        SecondWordForeground = Brushes.Red;
                    }
                    InfoText = $"The correct answer would have been\n{FirstLanguage}\n{_firstWord}\n\n{SecondLanguage}\n{_secondWord}";
                }
                VocabularyEntry tempEntry = new VocabularyEntry() {
                    FilePath = $"{VocabularyEntry.FirstPartFilePath}{_parent.Entries[_counter].WordList}{VocabularyEntry.SecondPartFilePath}",
                    FirstLanguage = _parent.Entries[_parent.Counter].FirstLanguage,
                    SecondLanguage = _parent.Entries[_parent.Counter].SecondLanguage,
                };
                VocabularyEntry.WriteData(tempEntry, _parent.Entries);
                ButtonText = _nextText;
            } else { 
                _parent.ShowLearnMode();
            }
        }

        public ICommand MarkEntryCommand => new RelayCommand(MarkEntry, CanExecuteCommand);
        private void MarkEntry(object obj) {
            if (Star == "☆") {
                Star = "★";
                markedEntries.Add(_parent.Entries[_counter]);
            } else {
                Star = "☆";
                markedEntries.Remove(_parent.Entries[_counter]);
            }
            VocabularyEntry.WriteData(markEntry, markedEntries);
        }

        public ICommand HintCommand => new RelayCommand(Hint, CanExecuteCommand);
        private void Hint(object obj) {
            _hints++;
            if (FirstWordWritable && FirstWordAnswer.Length < _firstWord.Length)
                FirstWordAnswer = _firstWord.Substring(0, FirstWordAnswer.Length+1);
            else if (SecondWordWritable && SecondWordAnswer.Length < _secondWord.Length)
                SecondWordAnswer = _secondWord.Substring(0, SecondWordAnswer.Length+1);
        }
    }
}
