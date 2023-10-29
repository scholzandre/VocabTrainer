using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class SettingsViewModel {
        private readonly List<Settings> _settings;
        public ObservableCollection<UserControl> ListEntries { get; set; }
        private MainViewModel _parent;
        public SettingsViewModel(MainViewModel parent) {
            _parent = parent;
            _settings = _parent.SettingsList;
            ListEntries = new ObservableCollection<UserControl>();
            FillCollection();
        }

        private void FillCollection() {
            foreach (Settings setting in _settings) {
                if (setting.LearningMode > 0 ||
                    setting.Condition == "random order" ||
                    setting.Condition == "list order") {
                    Type viewType = typeof(SettingsEntryTrueFalseView);
                    UserControl tempControl = (UserControl)Activator.CreateInstance(viewType);
                    tempControl.DataContext = new SettingsEntryTrueFalseViewModel(setting.Condition, setting.IsTrue);
                    ListEntries.Add(tempControl);
                } else {
                    Type viewType = typeof(SettingsEntryTextView);
                    UserControl tempControl = (UserControl)Activator.CreateInstance(viewType);
                    if (setting.APIKey != null) { 
                        tempControl.DataContext = new SettingsEntryTextViewModel(setting.Condition, setting.APIKey, _settings, _parent);
                    } else if (setting.BorderBackground != null) {
                        tempControl.DataContext = new SettingsEntryTextViewModel(setting.Condition, setting.BorderBackground, _settings, _parent);
                    } else if (setting.BorderBrush != null) {
                        tempControl.DataContext = new SettingsEntryTextViewModel(setting.Condition, setting.BorderBrush, _settings, _parent);
                    } else if (setting.ButtonsBackground != null) {
                        tempControl.DataContext = new SettingsEntryTextViewModel(setting.Condition, setting.ButtonsBackground, _settings, _parent);
                    } else if (setting.ButtonsForeground != null) {
                        tempControl.DataContext = new SettingsEntryTextViewModel(setting.Condition, setting.ButtonsForeground, _settings, _parent);
                    } else if (setting.NavBarBackground != null) {
                        tempControl.DataContext = new SettingsEntryTextViewModel(setting.Condition, setting.NavBarBackground, _settings, _parent);
                    }
                    ListEntries.Add(tempControl);
                }
            }
        }
    }
}
