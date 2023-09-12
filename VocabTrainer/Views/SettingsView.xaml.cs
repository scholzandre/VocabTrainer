using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VocabTrainer.Views {
    public partial class SettingsView : UserControl, INotifyPropertyChanged {
        private List<Settings> settingsList;
        public List<Settings> SettingsList {
            get { return settingsList; }
            set {
                settingsList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("settingsList"));
            }
        }

        private List<WordlistsList> wordlistsSettings;
        public List<WordlistsList> WordlistsSettings {
            get { return wordlistsSettings; }
            set {
                wordlistsSettings = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("wordlistsSettings"));
            }
        }
        public SettingsView(List<Settings> setting, List<WordlistsList> wordlists) {
            SettingsList = setting;
            WordlistsSettings = wordlists;
            InitializeComponent();
            CreateGUI();
            CreateGUIWordlists();
        }
        public void CreateGUI() {
            Grid mainGrid = new Grid();

            ColumnDefinition colDef1 = new ColumnDefinition();
            colDef1.Width = new GridLength(0.25, GridUnitType.Star);
            ColumnDefinition colDef2 = new ColumnDefinition();
            ColumnDefinition colDef3 = new ColumnDefinition();

            mainGrid.ColumnDefinitions.Add(colDef1);
            mainGrid.ColumnDefinitions.Add(colDef2);
            mainGrid.ColumnDefinitions.Add(colDef3);

            for (int i = 0; i < SettingsList.Count(); i++) {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(0.1, GridUnitType.Star);
                mainGrid.RowDefinitions.Add(rowDef);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = SettingsList[i].Condition;
                textBlock.Foreground = Brushes.White;
                Grid.SetColumn(textBlock, 1);
                Grid.SetRow(textBlock, i);
                mainGrid.Children.Add(textBlock);

                CheckBox checkBox = new CheckBox();
                checkBox.Name = $"CB{i}";
                checkBox.Checked += (sender, e) => CheckBox_Checked_Settings(sender, e, checkBox.Name);
                checkBox.Unchecked += (sender, e) => CheckBox_UnChecked_Settings(sender, e, checkBox.Name);
                if (SettingsList[i].IsTrue == true) {
                    checkBox.IsChecked = true;
                }
                Grid.SetColumn(checkBox, 2);
                Grid.SetRow(checkBox, i);
                mainGrid.Children.Add(checkBox);
            }
            RowDefinition lastRow = new RowDefinition();
            mainGrid.RowDefinitions.Add(lastRow);
            Grid.SetRow(mainGrid, 1);
            stackPanelGeneralSettings.Children.Add(mainGrid);
        }

        public void CreateGUIWordlists() {
            Grid mainGrid = new Grid();

            ColumnDefinition colDef1 = new ColumnDefinition();
            colDef1.Width = new GridLength(0.25, GridUnitType.Star);
            ColumnDefinition colDef2 = new ColumnDefinition();
            ColumnDefinition colDef3 = new ColumnDefinition();
            ColumnDefinition colDef4 = new ColumnDefinition();
            ColumnDefinition colDef5 = new ColumnDefinition();

            mainGrid.ColumnDefinitions.Add(colDef1);
            mainGrid.ColumnDefinitions.Add(colDef2);
            mainGrid.ColumnDefinitions.Add(colDef3);
            mainGrid.ColumnDefinitions.Add(colDef4);
            mainGrid.ColumnDefinitions.Add(colDef5);

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

        private void CheckBox_Checked_Settings(object sender, RoutedEventArgs e, string name) {
            int number = getIndex(name);
            SettingsList[number].IsTrue = true;
            Settings.WriteSettings(SettingsList);
        }
        private void CheckBox_UnChecked_Settings(object sender, RoutedEventArgs e, string name) {
            int number = getIndex(name);
            SettingsList[number].IsTrue = false;
            Settings.WriteSettings(SettingsList);
            SettingsList = Settings.GetSettings();
            stackPanelGeneralSettings.Children.Clear();
            CreateGUI();
        }

        private void CheckBox_Checked_Wordlists(object sender, RoutedEventArgs e, string name) {
            int number = getIndex(name);
            WordlistsSettings[number].IsTrue = true;
            WordlistsList.WriteWordlistsList(WordlistsSettings);
        }
        private void CheckBox_UnChecked_Wordlists(object sender, RoutedEventArgs e, string name) {
            int number = getIndex(name);
            WordlistsSettings[number].IsTrue = false;
            WordlistsList.WriteWordlistsList(WordlistsSettings);
            WordlistsSettings = WordlistsList.GetWordlists();
            stackPanelWordlistsSettings.Children.Clear();
            CreateGUIWordlists();
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
