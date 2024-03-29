﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class LearningModeThreeViewModel : BaseViewModel {
        private string _questionEntry;
        public string QuestionEntry {
            get => _questionEntry;
            set {
                _questionEntry = value;
                OnPropertyChanged(nameof(QuestionEntry));
            }
        }
        private string _firstWord;
        public string FirstWord {
            get => _firstWord;
            set {
                _firstWord = value;
                OnPropertyChanged(nameof(FirstWord));
            }
        }
        private string _firstAnswer;
        public string FirstAnswer {
            get => _firstAnswer;
            set {
                _firstAnswer = value;
                OnPropertyChanged(nameof(FirstAnswer));
            }
        }
        private string _secondAnswer;
        public string SecondAnswer {
            get => _secondAnswer;
            set {
                _secondAnswer = value;
                OnPropertyChanged(nameof(SecondAnswer));
            }
        }
        private string _thirdAnswer;
        public string ThirdAnswer {
            get => _thirdAnswer;
            set {
                _thirdAnswer = value;
                OnPropertyChanged(nameof(ThirdAnswer));
            }
        }
        private string _fourthAnswer;
        public string FourthAnswer {
            get => _fourthAnswer;
            set {
                _fourthAnswer = value;
                OnPropertyChanged(nameof(FourthAnswer));
            }
        }
        private string _fifthAnswer;
        public string FifthAnswer {
            get => _fifthAnswer;
            set {
                _fifthAnswer = value;
                OnPropertyChanged(nameof(FifthAnswer));
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
        private List<Brush> _backgroundColors = new List<Brush>() {
            Brushes.White,
            Brushes.White,
            Brushes.White,
            Brushes.White,
            Brushes.White,
            Brushes.White
        };
        public List<Brush> BackgroundColors {
            get => _backgroundColors;
            set {
                _backgroundColors = value;
                OnPropertyChanged(nameof(BackgroundColors));
            }
        }

        private readonly LearnViewModel _parent;
        private List<VocabularyEntry> _entries;
        private readonly int _language = 0;
        private readonly int _counter;
        static readonly VocabularyEntry _markEntry = new VocabularyEntry() { FilePath = VocabularyEntry.FilePathsSpecialLists[0] };
        private readonly List<VocabularyEntry> _markedEntries = VocabularyEntry.GetData(_markEntry);
        private int _positionCorrectItem;
        private bool _isOver = false;
        MainViewModel _mainViewModel;
        public LearningModeThreeViewModel(LearnViewModel parent, MainViewModel mainViewModel) {
            _mainViewModel = mainViewModel;
            _parent = parent;
            _counter = _parent.Counter;
            _language = _parent.Random.Next(1, 3);
            FillProperties(_language);
            Star = (_markedEntries.Contains(_parent.Entries[_counter])) ? "★" : "☆";
        }
        private void FillProperties(int language) {
            _entries = new List<VocabularyEntry>();
            for (int i = 0; i < 4; i++) {
                int position = _parent.Random.Next(0, _parent.Entries.Count);
                if (_entries.Contains(_parent.Entries[position]) || _parent.Entries[position] == _parent.Entries[_parent.Counter]) {
                    i--;
                    continue;
                } else {
                    _entries.Add(_parent.Entries[position]);
                }
            }
            _positionCorrectItem = _parent.Random.Next(0, 5);
            _entries.Insert(_positionCorrectItem, _parent.Entries[_counter]);
            if (language == 1) {
                QuestionEntry = _parent.Entries[_counter].SecondWord;
                FirstAnswer = _entries[0].FirstWord;
                SecondAnswer = _entries[1].FirstWord;
                ThirdAnswer = _entries[2].FirstWord;
                FourthAnswer = _entries[3].FirstWord;
                FifthAnswer = _entries[4].FirstWord;
            } else {
                QuestionEntry = _parent.Entries[_counter].FirstWord;
                FirstAnswer = _entries[0].SecondWord;
                SecondAnswer = _entries[1].SecondWord;
                ThirdAnswer = _entries[2].SecondWord;
                FourthAnswer = _entries[3].SecondWord;
                FifthAnswer = _entries[4].SecondWord;
            }
            if (QuestionEntry.Contains(",")) {
                string[] tempList = QuestionEntry.Split(',');
                Random random = new Random();
                int randInt = random.Next(tempList.Length);
                FirstWord = tempList[randInt];
            } else {
                FirstWord = QuestionEntry;
            }
        }
        private bool CanExecuteCommand(object arg) {
            return !_isOver;
        }
        public ICommand CheckFirstAnswerCommand => new RelayCommand(CheckFirstAnswer, CanExecuteCommand);
        private async void CheckFirstAnswer(object obj) {
            await CheckInput(_entries[0], 1);
        }
        public ICommand CheckSecondAnswerCommand => new RelayCommand(CheckSecondAnswer, CanExecuteCommand);
        private async void CheckSecondAnswer(object obj) {
            await CheckInput(_entries[1], 2);
        }
        public ICommand CheckThirdAnswerCommand => new RelayCommand(CheckThirdAnswer, CanExecuteCommand);
        private async void CheckThirdAnswer(object obj) {
            await CheckInput(_entries[2], 3);
        }
        public ICommand CheckFourthAnswerCommand => new RelayCommand(CheckFourthAnswer, CanExecuteCommand);
        private async void CheckFourthAnswer(object obj) {
            await CheckInput(_entries[3], 4);
        }
        public ICommand CheckFifthAnswerCommand => new RelayCommand(CheckFifthAnswer, CanExecuteCommand);
        private async void CheckFifthAnswer(object obj) {
            await CheckInput(_entries[4], 5);
        }
        private async Task CheckInput(VocabularyEntry choice, int answer) {
            if (_language == 1) 
                choice.SecondWord = QuestionEntry;
            else 
                choice.FirstWord = QuestionEntry;
            
            List<Brush> tempList = new List<Brush>(BackgroundColors);
            int awaitTime = 1500;
            bool isCorrect = VocabularyEntry.CheckAnswer(_parent.Entries[_counter], choice);
            if (isCorrect) {
                tempList[0] = Brushes.Green;
                tempList[answer] = Brushes.Green;
            } else {
                tempList[0] = Brushes.Red;
                tempList[answer] = Brushes.Red;
                tempList[_positionCorrectItem + 1] = Brushes.Green;
                awaitTime = 2250;
            }
            BackgroundColors = tempList;
            await ExtraFunctions.Wait(awaitTime);
            _parent.ShowLearnMode();
        }

        public ICommand MarkEntryCommand => new RelayCommand(MarkEntry, CanExecuteCommand);
        private void MarkEntry(object obj) {
            if (Star == "☆") {
                Star = "★";
                _markedEntries.Add(_parent.Entries[_counter]);
            } else {
                Star = "☆";
                _markedEntries.Remove(_parent.Entries[_counter]);
            }
            VocabularyEntry.WriteData(_markEntry, _markedEntries);
            _mainViewModel.ManageEntriesViewModel = new ManageViewModel(_mainViewModel, _mainViewModel.ManageEntriesOpenedWordlist);
        }
    }
}