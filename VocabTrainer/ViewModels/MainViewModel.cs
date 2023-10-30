using System;
using System.Collections.Generic;
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
        public MainViewModel() {
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
        public ICommand OpenAnalysisViewCommand => new RelayCommand(OpenAnalysisView, CanExecuteCommand);
        private void OpenAnalysisView(object obj) {
            Type viewType = typeof(AnalysisView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new AnalysisViewModel();
        }
        public ICommand OpenLearnViewCommand => new RelayCommand(OpenLearnView, CanExecuteCommand);
        private void OpenLearnView(object obj) {
            Type viewType = typeof(LearnView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            //UserControl.DataContext = new LearnViewModel(); because code behind
        }
        public ICommand OpenSettingsViewCommand => new RelayCommand(OpenSettingsView, CanExecuteCommand);
        private void OpenSettingsView(object obj) {
            Type viewType = typeof(SettingsView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new SettingsViewModel(this);
        }
        public ICommand OpenTranslatorViewCommand => new RelayCommand(OpenTranslatorView, CanExecuteCommand);
        private void OpenTranslatorView(object obj) {
            Type viewType = typeof(TranslatorView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new TranslatorViewModel();
        }
        public ICommand OpenManageWordsViewCommand => new RelayCommand(OpenManageWordsView, CanExecuteCommand);
        private void OpenManageWordsView(object obj) {
            Type viewType = typeof(ManageView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new ManageViewModel();
        }
        public ICommand OpenManageWordlistsViewCommand => new RelayCommand(OpenManageWordlistsView, CanExecuteCommand);
        private void OpenManageWordlistsView(object obj) {
            Type viewType = typeof(ManageWordlistsView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            UserControl.DataContext = new ManageWordlistsViewModel();
        }
        public ICommand OpenAddWordsViewCommand => new RelayCommand(OpenAddWordsView, CanExecuteCommand);
        private void OpenAddWordsView(object obj) {
            Type viewType = typeof(AddView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            //UserControl.DataContext = new AddViewModel(); code behind
        }
        public ICommand OpenAddWordlistViewCommand => new RelayCommand(OpenAddWordlistView, CanExecuteCommand);
        private void OpenAddWordlistView(object obj) {
            Type viewType = typeof(AddWordlistView);
            UserControl = (UserControl)Activator.CreateInstance(viewType);
            //UserControl.DataContext = new AddWordlistViewModel(); code behind
        }
    }
}
