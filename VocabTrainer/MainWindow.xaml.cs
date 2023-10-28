using VocabTrainer.Views;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using VocabTrainer.ViewModels;
using System.Windows.Media;
using System.Windows.Controls;
using System;

namespace VocabTrainer {
    public partial class MainWindow : Window {
        public List<VocabularyEntry> vocabulary;

        public MainWindow() {
            InitializeComponent();
            SetColors();
            WordlistsList.CheckAvailabilityOfJSONFiles();
            Analysis_Clicked(new object(), new RoutedEventArgs());
        }

        public void SetColors() {
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
        }

        private void Analysis_Clicked(object sender, RoutedEventArgs e) { 
            Type viewType = typeof(AnalysisView);
            UserControl view = (UserControl)Activator.CreateInstance(viewType);
            view.DataContext = new AnalysisViewModel(this);
            viewControl.Content = view;
        }
        private void Learn_Clicked(object sender, RoutedEventArgs e) {
            Type viewType = typeof(LearnView);
            UserControl view = (UserControl)Activator.CreateInstance(viewType);
            view.DataContext = new LearnViewModel();
            viewControl.Content = view;
        }

        private void Manage_Clicked(object sender, RoutedEventArgs e) {
            Type viewType = typeof(ManageView);
            UserControl view = (UserControl)Activator.CreateInstance(viewType);
            view.DataContext = new ManageViewModel();
            viewControl.Content = view;
        }

        private void AddWords_Clicked(object sender, RoutedEventArgs e) {
            Type viewType = typeof(AddView);
            UserControl view = (UserControl)Activator.CreateInstance(viewType);
            view.DataContext = new AddViewModel();
            viewControl.Content = view;
        }
        private void AddWordlists_Clicked(object sender, RoutedEventArgs e) {
            Type viewType = typeof(AddWordlistView);
            UserControl view = (UserControl)Activator.CreateInstance(viewType);
            view.DataContext = new AddWordlistViewModel();
            viewControl.Content = view;
        }
        private void Settings_Clicked(object sender, RoutedEventArgs e) {
            Type viewType = typeof(SettingsView);
            UserControl view = (UserControl)Activator.CreateInstance(viewType); 
            view.DataContext = new SettingsView(this); 
            viewControl.Content = view;
        }

        private void ManageWordlists_Clicked(object sender, RoutedEventArgs e) {
            Type viewType = typeof(ManageWordlistsView);
            UserControl view = (UserControl)Activator.CreateInstance(viewType); 
            view.DataContext = new ManageWordlistsViewModel(); 
            viewControl.Content = view;
        }

        private void Translator_Clicked(object sender, RoutedEventArgs e) {
            Type viewType = typeof(TranslatorView);
            UserControl view = (UserControl)Activator.CreateInstance(viewType); // sets the View
            view.DataContext = new TranslatorViewModel(); // sets the corresponding ViewModel
            viewControl.Content = view;
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
    }
}
