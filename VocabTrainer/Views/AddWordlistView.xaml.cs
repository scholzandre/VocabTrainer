using System.Windows;
using System.Windows.Controls;

namespace VocabTrainer.Views {
    public partial class AddWordlistView : UserControl {
        public AddWordlistView() {
            InitializeComponent();
        }

        private void AddWordlist(object sender, RoutedEventArgs e) {
            AddWordlist addWordlist = new AddWordlist();
            addingSuccessful.Text = addWordlist.CheckInput(wordListN.Text, firstLan.Text, secondLan.Text);
            if (addingSuccessful.Text == "Adding was successful.") {
                wordListN.Text = "";
                firstLan.Text = "";
                secondLan.Text = "";
            }
        }
    }
}
