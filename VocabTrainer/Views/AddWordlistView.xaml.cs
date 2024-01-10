using System.Windows;
using System.Windows.Controls;
using VocabTrainer.ViewModels;

namespace VocabTrainer.Views {
    public partial class AddWordlistView : UserControl {
        public AddWordlistView() {
            InitializeComponent();
        }
        private void Windows_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Windows_MouseDown(sender, e);
        }

        private void WordlistTextbox_Loaded(object sender, RoutedEventArgs e) {
            if (sender is TextBox textBox) {
                if (DataContext is AddWordlistViewModel viewModel) {
                    viewModel.WordlistTextBox = textBox;                                               // store current TextBox, to focus it later on 
                }
            }
        }
    }
}
