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
        
        private Type _learnEntryViewType = typeof(LearnView);

        private LearnViewModel _learnEntryViewModel;
        public LearnViewModel LearnEntriesViewModel {
            get => _learnEntryViewModel;
            set {
                _learnEntryViewModel = value;
                OnPropertyChanged(nameof(LearnEntriesViewModel)); 
            }
        }

        private Type _analysisViewType = typeof(AnalysisView);
        public string AnalysisOpenedWordlist = "";

        private Type _addEntryViewType = typeof(AddView);
        private AddViewModel _addEntryViewModel;
        public AddViewModel AddEntryViewModel {
            get => _addEntryViewModel;
            set {
                _addEntryViewModel = value;
                OnPropertyChanged(nameof(AddEntryViewModel));
            }
        }
        public string AddEntryOpenedWordlist = "";

        private Type _manageEntryViewType = typeof(ManageView);
        private ManageViewModel _manageEntryViewModel;
        public ManageViewModel ManageEntriesViewModel {
            get => _manageEntryViewModel;
            set {
                _manageEntryViewModel = value;
                OnPropertyChanged(nameof(ManageEntriesViewModel));
            }
        }
        public string ManageEntriesOpenedWordlist = "";

        private Type _addWordlistViewType = typeof(AddWordlistView);
        private AddWordlistViewModel _addWordlistViewModel;
        public AddWordlistViewModel AddWordlistViewModel {
            get => _addWordlistViewModel;
            set {
                _addWordlistViewModel = value;
                OnPropertyChanged(nameof(AddWordlistViewModel));
            }
        }

        private Type _manageWordlistViewType = typeof(ManageWordlistsView);
        private ManageWordlistsViewModel _manageWordlistViewModel;
        public ManageWordlistsViewModel ManageWordlistViewModel {
            get => _manageWordlistViewModel;
            set {
                _manageWordlistViewModel = value;
                OnPropertyChanged(nameof(ManageWordlistViewModel));
            }
        }

        private Type _translatorViewType = typeof(TranslatorView);
        private TranslatorViewModel _translatorViewModel;
        public TranslatorViewModel TranslatorViewModel {
            get => _translatorViewModel;
            set {
                _translatorViewModel = value;
                OnPropertyChanged(nameof(TranslatorViewModel));
            }
        }
        public string TranslatorOpenedWordlist = "";

        private Type _settingsViewType = typeof(SettingsView);
        private SettingsViewModel _settingsViewModel;
        public SettingsViewModel SettingsViewModel {
            get => _settingsViewModel;
            set {
                _settingsViewModel = value;
                OnPropertyChanged(nameof(SettingsViewModel));
            }
        }

        public MainViewModel() {
            WordlistsList.CheckJsonFolder();
            Settings.CheckSettingsFile();
            WordlistsList.CheckSpecialWordlists();
            WordlistsList.CheckAvailabilityOfJSONFiles();
            SetColors();
            LearnEntriesViewModel = new LearnViewModel(this);
            AddEntryViewModel = new AddViewModel(this, AddEntryOpenedWordlist);
            ManageEntriesViewModel = new ManageViewModel(this, ManageEntriesOpenedWordlist);
            AddWordlistViewModel = new AddWordlistViewModel(this);
            ManageWordlistViewModel = new ManageWordlistsViewModel(this);
            TranslatorViewModel = new TranslatorViewModel(this, TranslatorOpenedWordlist);
            SettingsViewModel = new SettingsViewModel(this);
            OpenAnalysisViewCommand.Execute(this);
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
            UserControl = (UserControl)Activator.CreateInstance(_learnEntryViewType);
            UserControl.DataContext = LearnEntriesViewModel;
        }
        public ICommand OpenAddWordlistViewCommand => new RelayCommand(OpenAddWordlistView, CanExecuteCommand);
        private void OpenAddWordlistView(object obj) {
            SetEnabled(1);
            UserControl = (UserControl)Activator.CreateInstance(_addWordlistViewType);
            UserControl.DataContext = AddWordlistViewModel;
        }
        public ICommand OpenManageWordlistsViewCommand => new RelayCommand(OpenManageWordlistsView, CanExecuteCommand);
        private void OpenManageWordlistsView(object obj) {
            SetEnabled(2);
            UserControl = (UserControl)Activator.CreateInstance(_manageWordlistViewType);
            UserControl.DataContext = ManageWordlistViewModel;
        }
        public ICommand OpenAddWordsViewCommand => new RelayCommand(OpenAddWordsView, CanExecuteCommand);
        private void OpenAddWordsView(object obj) {
            SetEnabled(3);
            UserControl = (UserControl)Activator.CreateInstance(_addEntryViewType);
            UserControl.DataContext = AddEntryViewModel;
        }
        public ICommand OpenManageWordsViewCommand => new RelayCommand(OpenManageWordsView, CanExecuteCommand);
        private void OpenManageWordsView(object obj) {
            SetEnabled(4);
            UserControl = (UserControl)Activator.CreateInstance(_manageEntryViewType);
            UserControl.DataContext = ManageEntriesViewModel;
        }
        public ICommand OpenAnalysisViewCommand => new RelayCommand(OpenAnalysisView, CanExecuteCommand);
        private void OpenAnalysisView(object obj) {
            SetEnabled(5);
            UserControl = (UserControl)Activator.CreateInstance(_analysisViewType);
            UserControl.DataContext = new AnalysisViewModel(this, AnalysisOpenedWordlist);
        }
        public ICommand OpenTranslatorViewCommand => new RelayCommand(OpenTranslatorView, CanExecuteCommand);
        private void OpenTranslatorView(object obj) {
            SetEnabled(6);
            UserControl = (UserControl)Activator.CreateInstance(_translatorViewType);
            UserControl.DataContext = TranslatorViewModel;
        }
        public ICommand OpenSettingsViewCommand => new RelayCommand(OpenSettingsView, CanExecuteCommand);
        private void OpenSettingsView(object obj) {
            SetEnabled(7);
            UserControl = (UserControl)Activator.CreateInstance(_settingsViewType);
            UserControl.DataContext = SettingsViewModel;
        }
        private void SetEnabled(int index) {
            List<bool> tempEnabled = new List<bool>(_defaultEnabled) {
                [index] = false
            };
            Enabled = tempEnabled;
        }
    }
}
