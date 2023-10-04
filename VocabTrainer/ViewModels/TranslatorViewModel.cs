using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    internal class TranslatorViewModel {

        public MainWindow ParentWindow { get; }
        public TranslatorViewModel(MainWindow parentWindow) {
            ParentWindow = parentWindow;
            ParentWindow.DataContext = new TranslatorView();
        }
    }
}
