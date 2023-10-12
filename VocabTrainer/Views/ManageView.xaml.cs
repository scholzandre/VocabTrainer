using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VocabTrainer.Views {
    public partial class ManageView : UserControl {

        List<VocabularyEntry> vocabulary = new List<VocabularyEntry>();
        string searchingWord = string.Empty;
        List<VocabularyEntry> searchResults = new List<VocabularyEntry>();

        public ManageView() {
            InitializeComponent();
            CheckEmptyLocal();
            FillComboBox();

        }
        public void FillComboBox() {
            List<WordlistsList> wordlists = WordlistsList.GetWordlistsList();
            for (int i = 0; i < wordlists.Count; i++) {
                comboWordlists.Items.Add($"{wordlists[i].WordlistName} ({wordlists[i].FirstLanguage}, {wordlists[i].SecondLanguage})");
            }
        }
        private List<VocabularyEntry> SearchVocab(string searchingWord) {
            searchResults.Clear();
            for (int i = 0; i < vocabulary.Count(); i++) {
                if (vocabulary[i].German.ToLower().Contains(searchingWord) || vocabulary[i].English.ToLower().Contains(searchingWord)) {
                    searchResults.Add(vocabulary[i]);
                }
            }
            return searchResults;
        }

        private void CreateGUI(List<VocabularyEntry> words, VocabularyEntry entry) {
            for (int i = 0; i < words.Count(); i++) {
                Grid grid = new Grid();

                for (int j = 0; j < 4; j++) {
                    CreateColumn(grid, j);
                }

                TextBox textBoxGerman = CreateTextBox(words[i].German, 0, grid);
                TextBox textBoxEnglish = CreateTextBox(words[i].English, 1, grid);

                if (comboWordlists.Text != "Marked (-, -)" &&
                    comboWordlists.Text != "LastTimeWrong (-, -)" &&
                    comboWordlists.Text != "Seen (-, -)" &&
                    comboWordlists.Text != "NotSeen (-, -)") CreateButton(textBoxGerman, textBoxEnglish, grid, i, "R", entry);

                if (comboWordlists.Text != "LastTimeWrong (-, -)" &&
                    comboWordlists.Text != "Seen (-, -)" &&
                    comboWordlists.Text != "NotSeen (-, -)") CreateButton(textBoxGerman, textBoxEnglish, grid, i, "X", entry);

                stackPanel.Children.Add(grid);
            }
        }
        public void CreateColumn(Grid grid, int columnNumber) {
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = (columnNumber < 2) ? new GridLength(25, GridUnitType.Star) : GridLength.Auto;
            grid.ColumnDefinitions.Add(columnDefinition);
        }

        public TextBox CreateTextBox(string word, int column, Grid grid) {
            TextBox textBox = new TextBox();
            textBox.IsEnabled = false;
            textBox.Text = word;
            textBox.Background = new SolidColorBrush(Colors.DarkGray);
            textBox.Margin = new Thickness(10, 0, 0, 10);
            Grid.SetColumn(textBox, column);
            grid.Children.Add(textBox);
            return textBox;
        }
        public void CreateButton(TextBox textBoxGerman, TextBox textBoxEnglish, Grid grid, int i, string content, VocabularyEntry entry) {
            Button button = new Button();
            button.Content = content;
            Dictionary<string, object> buttonTagsR = new Dictionary<string, object>();
            buttonTagsR.Add("GermanTextBox", textBoxGerman);
            buttonTagsR.Add("EnglishTextBox", textBoxEnglish);
            buttonTagsR.Add("EObject", entry);
            button.Tag = buttonTagsR;
            button.Click += (content == "R") ? (RoutedEventHandler)Rename : Remove;
            button.Name = $"B{i}";
            button.Margin = new Thickness(0, 0, 10, 10);
            button.Width = 25;
            Grid.SetColumn(button, (content == "R") ? 2 : 3);
            grid.Children.Add(button);
        }

        private void Rename(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Tag is Dictionary<string, object> buttonTags) {
                if (buttonTags.TryGetValue("GermanTextBox", out object germanTextBoxObj) &&
                    buttonTags.TryGetValue("EnglishTextBox", out object englishTextBoxObj) &&
                    buttonTags.TryGetValue("EObject", out object eobj)) {

                    TextBox germanTextBox = germanTextBoxObj as TextBox;
                    TextBox englishTextBox = englishTextBoxObj as TextBox;
                    VocabularyEntry entry = eobj as VocabularyEntry;
                    int index = (button.Name.Length <= 2) ? Int32.Parse(button.Name.Substring(button.Name.Length - 1)) : Int32.Parse(button.Name.Substring(1, button.Name.Length - 1));

                    vocabulary = VocabularyEntry.GetData(entry);
                    if (button.Content.ToString() == "R") {
                        germanTextBox.IsEnabled = true;
                        englishTextBox.IsEnabled = true;
                        button.Content = "A";
                    } else {
                        germanTextBox.IsEnabled = false;
                        englishTextBox.IsEnabled = false;
                        button.Content = "R";
                        if (vocabulary[index].German != germanTextBox.Text || vocabulary[index].English != englishTextBox.Text) {
                            vocabulary[index].German = germanTextBox.Text.Trim();
                            vocabulary[index].English = englishTextBox.Text.Trim();
                            VocabularyEntry.WriteData(entry, vocabulary);
                        }
                    }

                }
            }
        }

        private void Remove(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Tag is Dictionary<string, object> buttonTags) {
                if (buttonTags.TryGetValue("GermanTextBox", out object germanTextBoxObj) &&
                    buttonTags.TryGetValue("EnglishTextBox", out object englishTextBoxObj) &&
                    buttonTags.TryGetValue("EObject", out object eobj)) {

                    TextBox germanTextBox = germanTextBoxObj as TextBox;
                    TextBox englishTextBox = englishTextBoxObj as TextBox;
                    VocabularyEntry entry = eobj as VocabularyEntry;
                    vocabulary = VocabularyEntry.GetData(entry);

                    if (germanTextBox.Parent is Grid grid && grid.Parent is StackPanel stackPanel) {
                        stackPanel.Children.Remove(grid);
                    }
                    (bool isTrue, int index, int error, string word) returnedTuple = VocabularyEntry.alreadyThere(vocabulary, germanTextBox.Text, englishTextBox.Text);
                    if (returnedTuple.isTrue) {
                        vocabulary.Remove(vocabulary[returnedTuple.index]);
                        VocabularyEntry.WriteData(entry, vocabulary);
                    }
                }
            }
        }
        public void CheckEmptyLocal() {
            List<string> messages = VocabularyEntry.CheckEmpty(vocabulary);
            if (messages[1] != string.Empty) {
                infoTextManage.Text = messages[1];
            }
        }

        private void SearchWord_TextChanged(object sender, TextChangedEventArgs e) {
            if (searchWord.Text == "" || searchWord.Text == "Search...") {
                stackPanel.Children.Clear();
                CreateGUI(vocabulary, new VocabularyEntry());
            } else {
                stackPanel.Children.Clear();
                searchingWord = searchWord.Text.ToLower();
                searchResults = SearchVocab(searchingWord);
                CreateGUI(searchResults, new VocabularyEntry());
            }
        }


        private void ComboBox_DropDownClosed(object sender, EventArgs e) {
            if (comboWordlists.Text != "") {
                stackPanel.Children.Clear();
                ComboBox comboBox = sender as ComboBox;
                string input = comboBox.Text;
                int index = input.IndexOf("(");
                string wordListName = input.Substring(0, index - 1);
                VocabularyEntry entry = new VocabularyEntry();
                entry.FilePath = $"{VocabularyEntry.FirstPartFilePath}{wordListName}{VocabularyEntry.SecondPartFilePath}";
                List<VocabularyEntry> vocabularyTemp = VocabularyEntry.GetData(entry);
                vocabulary = vocabularyTemp;
                if (vocabularyTemp.Count > 0) infoTextManage.Text = "Manage words";
                else infoTextManage.Text = "There are no words available";
                CreateGUI(vocabularyTemp, entry);
            }
        }
    }
}