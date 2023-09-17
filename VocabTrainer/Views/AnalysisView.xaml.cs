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

namespace VocabTrainer.Views {
    /// <summary>
    /// Interaction logic for AnalysisView.xaml
    /// </summary>
    public partial class AnalysisView : UserControl {
        private SeriesCollection _seriesCollectionData;
        public SeriesCollection SeriesCollectionData { get => _seriesCollectionData; set => _seriesCollectionData = value; }
        public AnalysisView(SeriesCollection seriesCollection) {
            InitializeComponent();
            SeriesCollectionData = seriesCollection;
            ShowDiagram(SeriesCollectionData);
        }
        public void ShowDiagram(SeriesCollection seriesCollection) {
            pieChart.Series = seriesCollection;
        }
    }
}
