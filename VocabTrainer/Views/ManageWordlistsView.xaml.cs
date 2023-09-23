using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace VocabTrainer.Views {
    public partial class ManageWordlistsView : UserControl {
        List<WordlistsList> wordlists = WordlistsList.GetWordlists();
        string searchingWord = string.Empty;
        List<WordlistsList> searchResults = new List<WordlistsList>();

        public ManageWordlistsView() {
            InitializeComponent();
            checkEmptyLocal();
        }
        private List<WordlistsList> SearchVocab(string searchingWord) {
            searchResults.Clear();
            for (int i = 0; i < wordlists.Count(); i++) {
                if (wordlists[i].FirstLanguage.ToLower().Contains(searchingWord) 
                    || wordlists[i].SecondLanguage.ToLower().Contains(searchingWord)
                    || wordlists[i].WordlistName.ToLower().Contains(searchingWord)) {
                    searchResults.Add(wordlists[i]);
                }
            }
            return searchResults;
        }

        private void CreateGUI(List<WordlistsList> words) {
            for (int i = 0; i < words.Count(); i++) {
                Grid grid = new Grid();

                for (int j = 0; j < 5; j++) {
                    CreateColumn(grid, j);
                }

                TextBox textBoxName = CreateTextBox(words[i].WordlistName, 0, grid);
                TextBox textBoxFirstLanguage = CreateTextBox(words[i].FirstLanguage, 1, grid);
                TextBox textBoxSecondLanguage = CreateTextBox(words[i].SecondLanguage, 2, grid);

                CreateButton(textBoxName, textBoxFirstLanguage, textBoxSecondLanguage, grid, i, "R");
                CreateButton(textBoxName, textBoxFirstLanguage, textBoxSecondLanguage, grid, i, "X");

                stackPanel.Children.Add(grid);
            }
        }
        public void CreateColumn(Grid grid, int columnNumber) {
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = (columnNumber < 3) ? new GridLength(20, GridUnitType.Star) : GridLength.Auto;
            grid.ColumnDefinitions.Add(columnDefinition);
        }

        public TextBox CreateTextBox(string word, int column, Grid grid) {
            TextBox textBox = new TextBox();
            textBox.IsEnabled = false;
            textBox.Text = (word.Contains('_'))? word.Substring(0, word.IndexOf('_')) : word;
            textBox.Background = new SolidColorBrush(Colors.DarkGray);
            textBox.Margin = new Thickness(10, 0, 0, 10);
            Grid.SetColumn(textBox, column);
            grid.Children.Add(textBox);
            return textBox;
        }
        public void CreateButton(TextBox name, TextBox firstLanguage, TextBox secondLanguage, Grid grid, int i, string content) {
            Button button = new Button();
            if (name.Text != "Marked") {
                button.Content = content;
                Dictionary<string, object> buttonTagsR = new Dictionary<string, object>();
                buttonTagsR.Add("NameTextBox", name);
                buttonTagsR.Add("FirstLanguageTextbox", firstLanguage);
                buttonTagsR.Add("SecondLanguageTextbox", secondLanguage);
                button.Tag = buttonTagsR;
                button.Click += (content == "R") ? (RoutedEventHandler)Rename : Remove;
            } else {
                button.Content = "/";
            }
            button.Name = $"B{i}";
            button.Margin = new Thickness(0, 0, 10, 10);
            button.Width = 25;
            Grid.SetColumn(button, (content == "R") ? 3 : 4);
            grid.Children.Add(button);
        }

        private void Rename(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Tag is Dictionary<string, object> buttonTags) {
                if (buttonTags.TryGetValue("NameTextBox", out object nameTextBoxObj) &&
                    buttonTags.TryGetValue("FirstLanguageTextbox", out object firstLanTextBoxObj) &&
                    buttonTags.TryGetValue("SecondLanguageTextbox", out object secondLanTextBoxObj)) {

                    TextBox nameTextBox = nameTextBoxObj as TextBox;
                    TextBox firstLanTextBox = firstLanTextBoxObj as TextBox;
                    TextBox secondLanTextbox = secondLanTextBoxObj as TextBox;
                    int index = (button.Name.Length <= 2) ? Int32.Parse(button.Name.Substring(button.Name.Length - 1)) : Int32.Parse(button.Name.Substring(1, button.Name.Length - 1));

                    if (button.Content.ToString() == "R") {
                        nameTextBox.IsEnabled = true;
                        firstLanTextBox.IsEnabled = true;
                        secondLanTextbox.IsEnabled = true;
                        button.Content = "A";
                    } else {
                        nameTextBox.IsEnabled = false;
                        firstLanTextBox.IsEnabled = false;
                        secondLanTextbox.IsEnabled = false;
                        button.Content = "R";
                        if (wordlists[index].WordlistName != nameTextBox.Text 
                            || wordlists[index].FirstLanguage != firstLanTextBox.Text 
                            || wordlists[index].SecondLanguage != secondLanTextbox.Text) {
                            if (File.Exists($"{VocabularyEntry.FirstPartFilePath}{wordlists[index].WordlistName}{VocabularyEntry.SecondPartFilePath}")) {
                                //if (wordlists[index].WordlistName.Contains('_')) {
                                try {
                                    File.Move($"{VocabularyEntry.FirstPartFilePath}{wordlists[index].WordlistName}{VocabularyEntry.SecondPartFilePath}",
                                              $"{VocabularyEntry.FirstPartFilePath}{nameTextBox.Text.Trim()}_{firstLanTextBox.Text.Trim()}_{secondLanTextbox.Text.Trim()}{VocabularyEntry.SecondPartFilePath}");
                                    wordlists[index].WordlistName = $"{nameTextBox.Text.Trim()}_{firstLanTextBox.Text.Trim()}_{secondLanTextbox.Text.Trim()}";
                                    wordlists[index].FirstLanguage = firstLanTextBox.Text.Trim();
                                    wordlists[index].SecondLanguage = secondLanTextbox.Text.Trim();
                                    WordlistsList.WriteWordlistsList(wordlists);
                                } catch (Exception ex) {
                                    infoTextManage.Text = "Wordlist already exists";
                                    nameTextBox.Text = wordlists[index].WordlistName.Substring(0, wordlists[index].WordlistName.IndexOf('_'));
                                    firstLanTextBox.Text = wordlists[index].FirstLanguage;
                                    secondLanTextbox.Text = wordlists[index].SecondLanguage;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Remove(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Tag is Dictionary<string, object> buttonTags) {
                if (buttonTags.TryGetValue("NameTextBox", out object nameTextBoxObj) &&
                    buttonTags.TryGetValue("FirstLanguageTextbox", out object firstLanTextBoxObj) &&
                    buttonTags.TryGetValue("SecondLanguageTextbox", out object secondLanTextBoxObj)) {

                    TextBox nameTextBox = nameTextBoxObj as TextBox;
                    TextBox firstLanTextBox = firstLanTextBoxObj as TextBox;
                    TextBox secondLanTextBox = secondLanTextBoxObj as TextBox;
                    int index = (button.Name.Length <= 2) ? Int32.Parse(button.Name.Substring(button.Name.Length - 1)) : Int32.Parse(button.Name.Substring(1, button.Name.Length - 1));
                    (bool isTrue, int index, int error, string word) returnedTuple = (false, 0, 0, "");

                    if (nameTextBox.Parent is Grid grid && grid.Parent is StackPanel stackPanel) {
                        stackPanel.Children.Remove(grid);
                    }
                    if (wordlists[index].WordlistName.Contains('_')) {
                        returnedTuple = WordlistsList.alreadyThere($"{wordlists[index].WordlistName}", firstLanTextBox.Text, secondLanTextBox.Text);
                    } else { 
                        returnedTuple = WordlistsList.alreadyThere($"{nameTextBox.Text}", firstLanTextBox.Text, secondLanTextBox.Text);
                    }
                    if (returnedTuple.isTrue) {
                        if (File.Exists($"{VocabularyEntry.FirstPartFilePath}{wordlists[index].WordlistName}{VocabularyEntry.SecondPartFilePath}")) {
                            File.Delete($"{VocabularyEntry.FirstPartFilePath}{wordlists[index].WordlistName}{VocabularyEntry.SecondPartFilePath}");
                        }
                        wordlists.Remove(wordlists[returnedTuple.index]);
                        WordlistsList.WriteWordlistsList(wordlists);
                        checkEmptyLocal();
                    }
                }
            }
        }
        public void checkEmptyLocal() {
            int amount = WordlistsList.GetWordlists().Count();
            if (amount <= 0) {
                infoTextManage.Text = "There are no word lists available";
            }
        }

        private void searchWord_TextChanged(object sender, TextChangedEventArgs e) {
            if (searchWord.Text == "" || searchWord.Text == "Search...") {
                stackPanel.Children.Clear();
                CreateGUI(wordlists);
            } else {
                stackPanel.Children.Clear();
                searchingWord = searchWord.Text.ToLower();
                searchResults = SearchVocab(searchingWord);
                CreateGUI(searchResults);
            }
        }
    }
}
