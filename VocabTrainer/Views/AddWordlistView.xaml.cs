using System.Windows;
using System.Windows.Controls;

namespace VocabTrainer.Views {
    public partial class AddWordlistView : UserControl {
        public AddWordlistView() {
            InitializeComponent();
        }
        private void Windows_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Windows_MouseDown(sender, e);
        }
    }
}
