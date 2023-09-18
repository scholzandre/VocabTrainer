using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace VocabTrainer.Views {
    public partial class AddView : UserControl {
        public AddView() {
            InitializeComponent();
            FillComboBox();
        }

        public void FillComboBox() {
            List<WordlistsList> wordlists = WordlistsList.GetWordlists();
            for (int i = 0; i < wordlists.Count; i++) {
                if (wordlists[i].WordlistName != "Marked") { 
                    comboWordlists.Items.Add($"{wordlists[i].WordlistName} ({wordlists[i].FirstLanguage}, {wordlists[i].SecondLanguage})");
                }
            }
        }

        public void AddWord(object sender, RoutedEventArgs e) {
            if (comboWordlists.Text != "") {
                (bool isTrue, string returnText) returnedTuple = VocabularyEntry.CheckInput(comboWordlists.Text, germanWord.Text, englishWord.Text);
                if (returnedTuple.isTrue) {
                    addingSuccessful.Text = $"{germanWord.Text} ({firstLanguage.Text}) and {englishWord.Text} ({secondLanguage.Text}) have been successfully added";
                    germanWord.Text = String.Empty;
                    englishWord.Text = String.Empty;
                } else {
                    addingSuccessful.Text = returnedTuple.returnText;
                }
            }
        }

        private void Windows_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Windows_MouseDown(sender, e);
        }

        private void closed(object sender, EventArgs e) {
            int indexKomma = comboWordlists.Text.IndexOf(',');
            int indexBracketsOpen = comboWordlists.Text.IndexOf('(');
            int indexBracketsClose = comboWordlists.Text.IndexOf(')');
            firstLanguage.Text = comboWordlists.Text.Substring(indexBracketsOpen+1, indexKomma-1-indexBracketsOpen).Trim();
            secondLanguage.Text = comboWordlists.Text.Substring(indexKomma+1, indexBracketsClose-1-indexKomma).Trim();
        }
    }
}
