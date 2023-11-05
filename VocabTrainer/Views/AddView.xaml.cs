using System.Windows.Controls;

namespace VocabTrainer.Views {
    public partial class AddView : UserControl {
        public AddView() {
            InitializeComponent();
        }
        private void Windows_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Windows_MouseDown(sender, e);
        }
    }
}
