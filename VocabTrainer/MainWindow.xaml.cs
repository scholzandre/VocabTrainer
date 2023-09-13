using VocabTrainer.Views;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace VocabTrainer {
    public partial class MainWindow : Window {
        public List<VocabularyEntry> vocabulary;

        public MainWindow() {
            InitializeComponent();
        }

        private void Learn_Clicked(object sender, RoutedEventArgs e) {
            DataContext = new LearnView();
        }

        private void Manage_Clicked(object sender, RoutedEventArgs e) {
            DataContext = new ManageView();
        }

        private void AddWords_Clicked(object sender, RoutedEventArgs e) {
            DataContext = new AddView();
        }
        private void AddWordlists_Clicked(object sender, RoutedEventArgs e) {
            DataContext = new AddWordlistView();
        }
        private void Settings_Clicked(object sender, RoutedEventArgs e) {
            DataContext = new SettingsView(Settings.GetSettings(), WordlistsList.GetWordlists());
        }

        public void Windows_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void minimizeWindow(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void closeWindow(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void ManageWordlists_Clicked(object sender, RoutedEventArgs e) {
            DataContext = new ManageWordlistsView();
        }
    }
}
