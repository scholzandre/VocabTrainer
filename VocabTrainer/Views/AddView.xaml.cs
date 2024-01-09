using System.Windows.Controls;
using VocabTrainer.ViewModels;

namespace VocabTrainer.Views {
    public partial class AddView : UserControl {
        public AddView() {
            InitializeComponent();
        }
        private void Windows_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Windows_MouseDown(sender, e);
        }

        private void FirstWordTextBox_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            if (sender is TextBox textBox) {
                if (DataContext is AddViewModel viewModel) {
                    viewModel.FirstWordTextBox = textBox;                                               // store current TextBox, to focus it later on 
                }
            }
        }
    }
}
