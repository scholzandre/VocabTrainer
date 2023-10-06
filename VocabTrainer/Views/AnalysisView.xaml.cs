using LiveCharts;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VocabTrainer.ViewModels;

namespace VocabTrainer.Views {
    public partial class AnalysisView : UserControl {
        private SeriesCollection _seriesCollectionData;
        public SeriesCollection SeriesCollectionData { get => _seriesCollectionData; set => _seriesCollectionData = value; }
        private AnalysisViewModel _analysisViewModel;
        public AnalysisViewModel AnalysisViewModel { get => _analysisViewModel; set => _analysisViewModel = value; }
        private (int allWords, int seen, int notSeen, int repeated, int lastTimeWrong) _values;
        public (int allWords, int seen, int notSeen, int repeated, int lastTimeWrong) Values { get => _values; set => _values = value; }
        public AnalysisView(SeriesCollection seriesCollection, AnalysisViewModel viewModel, (int, int, int, int, int) values) {
            InitializeComponent();
            DataContext = viewModel;
            SeriesCollectionData = seriesCollection;
            AnalysisViewModel = viewModel;
            Values = values;
            FillComboBox();
            FillWordTable();
            ShowDiagram(SeriesCollectionData);
        }
        public void ShowDiagram(SeriesCollection seriesCollection) {
            pieChart.Series = seriesCollection;
            name.Text = AnalysisViewModel.Wordlist;
            allWords.Text = "words: \t\t\t" + Values.allWords.ToString();
            seen.Text = "seen: \t\t\t" + Values.seen.ToString();
            notSeen.Text = "not seen: \t\t" + Values.notSeen.ToString();
            repeated.Text = "known words: \t\t" + Values.repeated.ToString();
            lastTimeWrong.Text = "unknown words: \t\t" + Values.lastTimeWrong.ToString();
            pieChart.DataTooltip = null; // disables further information
        }
        public void FillComboBox() {
            comboWordlists.Items.Add($"All words");
            List<WordlistsList> wordlists = WordlistsList.GetWordlistsList();
            for (int i = 0; i < wordlists.Count; i++) 
                if (wordlists[i].WordlistName != "Seen" &&
                    wordlists[i].WordlistName != "NotSeen" &&
                    wordlists[i].WordlistName != "LastTimeWrong") {
                    comboWordlists.Items.Add($"{wordlists[i].WordlistName} ({wordlists[i].FirstLanguage}, {wordlists[i].SecondLanguage})");
                    if (AnalysisViewModel.Wordlist == "") comboWordlists.SelectedIndex = 0;
                    else if (wordlists[i].WordlistName == AnalysisViewModel.Wordlist) comboWordlists.SelectedIndex = i - 2;
                }
        }
        public void FillWordTable() {
            for (int i = 0; i < AnalysisViewModel.AllWords.Count; i++) { 
                RowDefinition newRow = new RowDefinition();
                wordsTable.RowDefinitions.Add(newRow);
                TextBlock firstWord = new TextBlock() { Text = AnalysisViewModel.AllWords[i].German, Foreground = Brushes.White };
                TextBlock secondWord = new TextBlock() { Text = AnalysisViewModel.AllWords[i].English, Foreground = Brushes.White };
                TextBlock repeated = new TextBlock() { Text = AnalysisViewModel.AllWords[i].Repeated.ToString(), Foreground = Brushes.White };
                TextBlock seen = new TextBlock() { Text = AnalysisViewModel.AllWords[i].Seen.ToString(), Foreground = Brushes.White };
                TextBlock lastTimeWrong = new TextBlock() { Text = AnalysisViewModel.AllWords[i].LastTimeWrong.ToString(), Foreground = Brushes.White };

                Grid.SetColumn(firstWord, 0); 
                Grid.SetColumn(secondWord, 1);   
                Grid.SetColumn(repeated, 2); 
                Grid.SetColumn(seen, 3);
                Grid.SetColumn(lastTimeWrong, 4);

                Grid.SetRow(firstWord, i+1);
                Grid.SetRow(secondWord, i+1);
                Grid.SetRow(repeated, i+1);
                Grid.SetRow(seen, i+1);
                Grid.SetRow(lastTimeWrong, i+1);

                wordsTable.Children.Add(firstWord);
                wordsTable.Children.Add(secondWord);
                wordsTable.Children.Add(repeated);
                wordsTable.Children.Add(seen);
                wordsTable.Children.Add(lastTimeWrong);
            }
        }
        private void Closed(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(comboWordlists.Text)) { 
                if (comboWordlists.Text == "All words") AnalysisViewModel.Wordlist = string.Empty;
                else AnalysisViewModel.Wordlist = comboWordlists.Text.Substring(0, (comboWordlists.Text.IndexOf('('))).Trim();
                AnalysisViewModel.GetPercentages();
            }
        }
    }
}
