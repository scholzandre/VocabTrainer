using LiveCharts;
using System.Collections.Generic;
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
            if (Values.seen == 0 &&
                Values.repeated == 0 &&
                Values.lastTimeWrong == 0) resetButton.IsEnabled = false;
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
            for (int i = 0; i <= AnalysisViewModel.SearchingWords.Count; i++) {
                RowDefinition newRow = new RowDefinition();
                wordsTable.RowDefinitions.Add(newRow);
            }
            for (int i = 0; i < AnalysisViewModel.SearchingWords.Count; i++) {
                CreateTextBox(AnalysisViewModel.SearchingWords[i].German, 0, i + 1);
                CreateTextBox(AnalysisViewModel.SearchingWords[i].English, 1, i + 1);
                CreateTextBox(AnalysisViewModel.SearchingWords[i].Repeated.ToString(), 2, i + 1);
                CreateTextBox(AnalysisViewModel.SearchingWords[i].Seen.ToString(), 3, i + 1);
                CreateTextBox(AnalysisViewModel.SearchingWords[i].LastTimeWrong.ToString(), 4, i + 1);
            }
        }

        private void CreateTextBox(string input, int column, int row) {
            TextBlock textBox = new TextBlock() { Text = input, Foreground = Brushes.White };
            Grid.SetColumn(textBox, column);
            Grid.SetRow(textBox, row);
            wordsTable.Children.Add(textBox);
        }

        private void Loaded(object sender, System.Windows.RoutedEventArgs e) { // automatically jumps to the end of the TextBox
            SearchingBox.CaretIndex = SearchingBox.Text.Length;
            SearchingBox.Focus();
        }
    }
}
