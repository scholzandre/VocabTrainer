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
                    addingSuccessful.Text = returnedTuple.returnText;
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
    }
}
