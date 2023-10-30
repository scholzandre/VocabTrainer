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
                //stackPanel.Children.Clear();
                //CreateGUI(vocabulary, new VocabularyEntry());
            } else {
                //stackPanel.Children.Clear();
                searchingWord = searchWord.Text.ToLower();
                searchResults = SearchVocab(searchingWord);
                //CreateGUI(searchResults, new VocabularyEntry());
            }
        }
    }
}