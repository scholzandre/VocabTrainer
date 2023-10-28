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
        public AnalysisView() { 
            InitializeComponent();
            pieChart.DataTooltip = null;
        }
    }
}
