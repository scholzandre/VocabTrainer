using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VocabTrainer.Views {
    public partial class SettingsView {
        public SettingsView() {
            InitializeComponent();
        }

        private void HandlePreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e) {
            if (e.Handled) {
                return;
            }
            e.Handled = true;
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) {
                RoutedEvent = UIElement.MouseWheelEvent
            };
            eventArg.Source = sender;
            var parent = ((Control)sender).Parent as UIElement;
            parent.RaiseEvent(eventArg); // gives parent element this mouse wheel event
        }
    }
}
