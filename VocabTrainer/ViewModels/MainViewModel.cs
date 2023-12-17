using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class MainViewModel : BaseViewModel {
        private UserControl _userControl;
        public UserControl UserControl {
            get => _userControl;
            set {
                _userControl = value;
                OnPropertyChanged(nameof(UserControl)); 
            }
        }

        private Brush _buttonForeground;
        public Brush ButtonForeground {
            get => _buttonForeground;
            set {
                _buttonForeground = value;
                OnPropertyChanged(nameof(ButtonForeground));
            }
        }
        private Brush _buttonBackground;
        public Brush ButtonBackground {
            get => _buttonBackground;
            set {
                _buttonBackground = value;
                OnPropertyChanged(nameof(ButtonBackground));
            }
        }

        private Brush _outerBorderBrush;
        public Brush OuterBorderBrush {
            get => _outerBorderBrush;
            set {
                _outerBorderBrush = value;
                OnPropertyChanged(nameof(OuterBorderBrush));
            }
        }
        private Brush _outerBorderBackground;
        public Brush OuterBorderBackground {
            get => _outerBorderBackground;
            set {
                _outerBorderBackground = value;
                OnPropertyChanged(nameof(OuterBorderBackground));
            }
        }
        private Brush _navBarBackground;
        public Brush NavBarBackground {
            get => _navBarBackground;
            set {
                _navBarBackground = value;
                OnPropertyChanged(nameof(NavBarBackground));
            }
        }
        private List<Settings> _settingsList;
        public List<Settings> SettingsList { 
            get => _settingsList;
            set {
                _settingsList = value;
                OnPropertyChanged(nameof(SettingsList));
            } 
        }
        private List<WordlistsList> _listWordlist;
        public List<WordlistsList> ListWordlist {
            get => _listWordlist;
            set {
                _listWordlist = value;
                OnPropertyChanged(nameof(ListWordlist));
            }
        }
        private string _iconFilePath = VocabularyEntry.GeneralFilePath + "Pictures\\icon.png";
        public string IconFilePath {
            get => _iconFilePath;
            set {
                _iconFilePath = value;
                OnPropertyChanged(nameof(IconFilePath));    
            }
        }
        private readonly static List<bool> _defaultEnabled = new List<bool> {
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            true
        };
        private List<bool> _enabled = new List<bool>(_defaultEnabled);
        public List<bool> Enabled {
            get => _enabled;
            set {
                _enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }

        public MainViewModel() {
            if (File.Exists(IconFilePath)) Console.WriteLine("yes");
            WordlistsList.CheckAvailabilityOfJSONFiles();
            SetColors();
            OpenAnalysisView(new object());
        }

        public void SetColors() {
            SettingsList = Settings.GetSettings();
            ListWordlist = WordlistsList.GetWordlistsList();
            ButtonForeground = (Brush)new BrushConverter().ConvertFrom(SettingsList[6].ButtonsForeground);
            ButtonBackground = (Brush)new BrushConverter().ConvertFrom(SettingsList[7].ButtonsBackground);
            OuterBorderBrush = (Brush)new BrushConverter().ConvertFrom(SettingsList[8].BorderBrush);
            OuterBorderBackground = (Brush)new BrushConverter().ConvertFrom(SettingsList[9].BorderBackground);
            NavBarBackground = (Brush)new BrushConverter().ConvertFrom(SettingsList[10].NavBarBackground);
        }

        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand OpenLearnViewCommand => new RelayCommand(OpenLearnView, CanExecuteCommand);
        private void OpenLearnView(object obj) {
            SetEnabled(0);
            Type viewType = typeof(LearnView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new LearnViewModel();
        }
        public ICommand OpenAddWordlistViewCommand => new RelayCommand(OpenAddWordlistView, CanExecuteCommand);
        private void OpenAddWordlistView(object obj) {
            SetEnabled(1);
            Type viewType = typeof(AddWordlistView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new AddWordlistViewModel(this);
        }
        public ICommand OpenManageWordlistsViewCommand => new RelayCommand(OpenManageWordlistsView, CanExecuteCommand);
        private void OpenManageWordlistsView(object obj) {
            SetEnabled(2);
            Type viewType = typeof(ManageWordlistsView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new ManageWordlistsViewModel();
        }
        public ICommand OpenAddWordsViewCommand => new RelayCommand(OpenAddWordsView, CanExecuteCommand);
        private void OpenAddWordsView(object obj) {
            SetEnabled(3);
            Type viewType = typeof(AddView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new AddViewModel();
        }
        public ICommand OpenManageWordsViewCommand => new RelayCommand(OpenManageWordsView, CanExecuteCommand);
        private void OpenManageWordsView(object obj) {
            SetEnabled(4);
            Type viewType = typeof(ManageView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new ManageViewModel();
        }
        public ICommand OpenAnalysisViewCommand => new RelayCommand(OpenAnalysisView, CanExecuteCommand);
        private void OpenAnalysisView(object obj) {
            SetEnabled(5);
            Type viewType = typeof(AnalysisView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new AnalysisViewModel();
        }
        public ICommand OpenTranslatorViewCommand => new RelayCommand(OpenTranslatorView, CanExecuteCommand);
        private void OpenTranslatorView(object obj) {
            SetEnabled(6);
            Type viewType = typeof(TranslatorView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new TranslatorViewModel();
        }
        public ICommand OpenSettingsViewCommand => new RelayCommand(OpenSettingsView, CanExecuteCommand);
        private void OpenSettingsView(object obj) {
            SetEnabled(7);
            Type viewType = typeof(SettingsView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new SettingsViewModel(this);
        }
        private void SetEnabled(int index) {
            List<bool> tempEnabled = new List<bool>(_defaultEnabled) {
                [index] = false
            };
            Enabled = tempEnabled;
        }
    }
}
