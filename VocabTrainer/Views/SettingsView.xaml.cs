using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VocabTrainer.Views {
    public partial class SettingsView : UserControl, INotifyPropertyChanged {

        #region Properties

        private List<Settings> _settingsList;
        public List<Settings> SettingsList {
            get { return _settingsList; }
            set {
                _settingsList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("settingsList"));
                CreateGUI();
            }
        }

        private List<WordlistsList> _wordlistsSettings;
        public List<WordlistsList> WordlistsSettings {
            get { return _wordlistsSettings; }
            set {
                _wordlistsSettings = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("wordlistsSettings"));
                CreateGUIWordlists();
            }
        }
        private bool updatingSettings = false;

        #endregion
        public SettingsView() {
            InitializeComponent();
            SettingsList = Settings.GetSettings();
            WordlistsSettings = WordlistsList.GetWordlistsList();
        }
        public void CreateGUI() {
            Grid mainGrid = new Grid();

            ColumnDefinition colDef0 = new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) };
            ColumnDefinition colDef1 = new ColumnDefinition() { Width = new GridLength(75, GridUnitType.Star) };
            ColumnDefinition colDef2 = new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Star) };
            
            mainGrid.ColumnDefinitions.Add(colDef0);
            mainGrid.ColumnDefinitions.Add(colDef1);
            mainGrid.ColumnDefinitions.Add(colDef2);

            for (int i = 0; i < SettingsList.Count(); i++) {
                RowDefinition rowDef = new RowDefinition() { Height = new GridLength(0.1, GridUnitType.Star) };

                TextBlock textBlock = new TextBlock() {
                    Text = SettingsList[i].Condition,
                    Foreground = Brushes.White
                };
                Grid.SetColumn(textBlock, 1);
                Grid.SetRow(textBlock, i);

                mainGrid.RowDefinitions.Add(rowDef);

                if (SettingsList[i].BorderBackground == null &&
                    SettingsList[i].BorderBrush == null &&
                    SettingsList[i].Buttons_Foreground == null &&
                    SettingsList[i].Buttons_Background == null &&
                    SettingsList[i].NavBarBackground == null) { 

                    CheckBox checkBox = new CheckBox() { Name = $"CB{i}" };
                    checkBox.Checked += (sender, e) => CheckBox_Checked_Settings(sender, e, checkBox.Name);
                    checkBox.Unchecked += (sender, e) => CheckBox_UnChecked_Settings(sender, e, checkBox.Name);
                    if (SettingsList[i].IsTrue == true) {
                        checkBox.IsChecked = true;
                    }
                    Grid.SetColumn(checkBox, 2);
                    Grid.SetRow(checkBox, i);
                    mainGrid.Children.Add(checkBox);
                }
                mainGrid.Children.Add(textBlock);
            }
            Grid.SetRow(mainGrid, 1);
            stackPanelGeneralSettings.Children.Clear();
            stackPanelGeneralSettings.Children.Add(mainGrid);
        }
        private void CheckBox_Checked_Settings(object sender, RoutedEventArgs e, string name) {
            if (!updatingSettings) {
                int number = getIndex(name);
                if (number == 1) {  
                    SettingsList[0].IsTrue = false;
                } else if (number == 0) {
                    SettingsList[1].IsTrue = false;
                }
                SettingsList[number].IsTrue = true;
                UpdateSettingsList();
            }
        }
        private void UpdateSettingsList() {
            updatingSettings = true;
            Settings.WriteSettings(SettingsList);
            SettingsList = Settings.GetSettings();
            updatingSettings = false;
        }
        private void CheckBox_UnChecked_Settings(object sender, RoutedEventArgs e, string name) {
            int number = getIndex(name);
            SettingsList[number].IsTrue = false;
            Settings.WriteSettings(SettingsList);
            SettingsList = Settings.GetSettings();
        }

        public void CreateGUIWordlists() {
            Grid mainGrid = new Grid();

            ColumnDefinition colDef0 = new ColumnDefinition();
            colDef0.Width = new GridLength(5, GridUnitType.Star);
            ColumnDefinition colDef1 = new ColumnDefinition();
            colDef1.Width = new GridLength(35, GridUnitType.Star);
            ColumnDefinition colDef2 = new ColumnDefinition();
            colDef2.Width = new GridLength(20, GridUnitType.Star);
            ColumnDefinition colDef3 = new ColumnDefinition();
            colDef3.Width = new GridLength(20, GridUnitType.Star);
            ColumnDefinition colDef4 = new ColumnDefinition();
            colDef4.Width = new GridLength(20, GridUnitType.Star);

            mainGrid.ColumnDefinitions.Add(colDef0);
            mainGrid.ColumnDefinitions.Add(colDef1);
            mainGrid.ColumnDefinitions.Add(colDef2);
            mainGrid.ColumnDefinitions.Add(colDef3);
            mainGrid.ColumnDefinitions.Add(colDef4);

            for (int i = 0; i < WordlistsSettings.Count(); i++) {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(0.1, GridUnitType.Star);
                mainGrid.RowDefinitions.Add(rowDef);

                TextBlock textBlock1 = new TextBlock();
                TextBlock textBlock2 = new TextBlock();
                TextBlock textBlock3 = new TextBlock();
                textBlock1.Text = WordlistsSettings[i].WordlistName;
                textBlock1.Foreground = Brushes.White;
                Grid.SetColumn(textBlock1, 1);
                Grid.SetRow(textBlock1, i);
                mainGrid.Children.Add(textBlock1);

                textBlock2.Text = WordlistsSettings[i].FirstLanguage;
                textBlock2.Foreground = Brushes.White;
                Grid.SetColumn(textBlock2, 2);
                Grid.SetRow(textBlock2, i);
                mainGrid.Children.Add(textBlock2);

                textBlock3.Text = WordlistsSettings[i].SecondLanguage;
                textBlock3.Foreground = Brushes.White;
                Grid.SetColumn(textBlock3, 3);
                Grid.SetRow(textBlock3, i);
                mainGrid.Children.Add(textBlock3);

                CheckBox checkBox = new CheckBox();
                checkBox.Name = $"CB{i}";
                checkBox.Checked += (sender, e) => CheckBox_Checked_Wordlists(sender, e, checkBox.Name);
                checkBox.Unchecked += (sender, e) => CheckBox_UnChecked_Wordlists(sender, e, checkBox.Name);
                if (WordlistsSettings[i].IsTrue == true) {
                    checkBox.IsChecked = true;
                }
                Grid.SetColumn(checkBox, 4);
                Grid.SetRow(checkBox, i);
                mainGrid.Children.Add(checkBox);
            }
            RowDefinition lastRow = new RowDefinition();
            mainGrid.RowDefinitions.Add(lastRow);
            Grid.SetRow(mainGrid, 1);
            stackPanelWordlistsSettings.Children.Add(mainGrid);
        }

        private void CheckBox_Checked_Wordlists(object sender, RoutedEventArgs e, string name) {
            int number = getIndex(name);
            WordlistsSettings[number].IsTrue = true;
            WordlistsList.WriteWordlistsList(WordlistsSettings);
            if (!updatingSettings) {
                UpdateSettingsList();
            }
        }
        private void CheckBox_UnChecked_Wordlists(object sender, RoutedEventArgs e, string name) {
            int number = getIndex(name);
            WordlistsSettings[number].IsTrue = false;
            WordlistsList.WriteWordlistsList(WordlistsSettings);
            if (!updatingSettings) {
                UpdateSettingsList();
            }
        }

        private int getIndex(string name) {
            int length = name.Length - 1;
            int number;
            if (length == 2) {
                number = Int32.Parse(name.Substring(length));
            } else {
                number = Int32.Parse(name.Substring(2, length));
            }
            return number;
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
