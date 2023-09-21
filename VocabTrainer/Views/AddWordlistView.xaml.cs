using System.Windows;
using System.Windows.Controls;

namespace VocabTrainer.Views {
    public partial class AddWordlistView : UserControl {
        public AddWordlistView() {
            InitializeComponent();
        }

        private void AddWordlist(object sender, RoutedEventArgs e) {
            AddWordlist addWordlist = new AddWordlist();
            addingSuccessful.Text = addWordlist.CheckInput($"{wordListN.Text.Trim()}_{firstLan.Text.ToLower().Trim()}_{secondLan.Text.ToLower().Trim()}", firstLan.Text, secondLan.Text);
            if (addingSuccessful.Text == "Adding was successful.") {
                wordListN.Text = "";
                firstLan.Text = "";
                secondLan.Text = "";
            }
        }

        private void Windows_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Windows_MouseDown(sender, e);
        }
    }
}
