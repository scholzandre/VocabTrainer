using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
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
        private MainViewModel _parent;
        public SettingsViewModel(MainViewModel parent) {
            _parent = parent;
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
                    tempControl.DataContext = new SettingsEntryTrueFalseViewModel(_settings[i].Condition, _settings[i].IsTrue, _settings, this, _wordlists);
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
                tempControl.DataContext = new SettingsEntryTrueFalseViewModel(_wordlists[i].WordlistName, _wordlists[i].IsTrue, _settings, this, _wordlists);
                ListWordlists.Add(tempControl);
            }
        }
    }
}
