using System.Windows;
using System.Windows.Input;

namespace VocabTrainer {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        public void Windows_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
    }
}
