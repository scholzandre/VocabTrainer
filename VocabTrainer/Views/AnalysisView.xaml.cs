using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using VocabTrainer.ViewModels;

namespace VocabTrainer.Views {
    /// <summary>
    /// Interaction logic for AnalysisView.xaml
    /// </summary>
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
            for (int i = 0; i < wordlists.Count; i++) {
                if (wordlists[i].WordlistName != "Seen" &&
                    wordlists[i].WordlistName != "NotSeen" &&
                    wordlists[i].WordlistName != "LastTimeWrong") {
                    comboWordlists.Items.Add($"{wordlists[i].WordlistName} ({wordlists[i].FirstLanguage}, {wordlists[i].SecondLanguage})");
                    if (AnalysisViewModel.Wordlist == "") {
                        comboWordlists.SelectedIndex = 0;
                    } else if (wordlists[i].WordlistName == AnalysisViewModel.Wordlist) {
                        comboWordlists.SelectedIndex = i - 2;
                    }
                }
            }
        }

        private void Closed(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(comboWordlists.Text)) { 
                if (comboWordlists.Text == "All words") {
                    AnalysisViewModel.Wordlist = string.Empty;
                } else {
                    AnalysisViewModel.Wordlist = comboWordlists.Text.Substring(0, (comboWordlists.Text.IndexOf('('))).Trim();
                }
                AnalysisViewModel.GetPercentages();
            }
        }
    }
}
