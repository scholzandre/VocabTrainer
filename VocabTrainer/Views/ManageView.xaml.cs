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
        }
    }
}