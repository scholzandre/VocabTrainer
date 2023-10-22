using VocabTrainer.Views;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using VocabTrainer.ViewModels;
using System.Windows.Media;
using System.Windows.Controls;

namespace VocabTrainer {
    public partial class MainWindow : Window {
        public List<VocabularyEntry> vocabulary;

        public MainWindow() {
            InitializeComponent();
            List<Settings> settings = Settings.GetSettings();
            outerBorder.BorderBrush = (Brush)new BrushConverter().ConvertFrom(settings[8].BorderBrush);
            outerBorder.Background = (Brush)new BrushConverter().ConvertFrom(settings[9].BorderBackground);
            closeButton.Background = (Brush)new BrushConverter().ConvertFrom(settings[7].Buttons_Background);
            minimizeButton.Background = (Brush)new BrushConverter().ConvertFrom(settings[7].Buttons_Background);
            navBar.Background = (Brush)new BrushConverter().ConvertFrom(settings[10].NavBarBackground);
            foreach (object child in mainGrid.Children) {
                if (child is Button temp) { 
                    temp.Foreground = (Brush)new BrushConverter().ConvertFrom(settings[6].Buttons_Foreground);
                    temp.Background = (Brush)new BrushConverter().ConvertFrom(settings[6].Buttons_Background);
                }
            }
            WordlistsList.CheckAvailabilityOfJSONFiles();
            new AnalysisViewModel(this);
        }
        private void Analysis_Clicked(object sender, RoutedEventArgs e) { 
            new AnalysisViewModel(this);
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
            DataContext = new SettingsView();
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

        private void Translator_Clicked(object sender, RoutedEventArgs e) {
            new TranslatorViewModel(this);
        }
    }
}
