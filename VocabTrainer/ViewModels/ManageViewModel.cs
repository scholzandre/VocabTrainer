using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels
{
    public class ManageViewModel : BaseViewModel {
        private ObservableCollection<UserControl> _entries;
        public ObservableCollection<UserControl> Entries {
            get => _entries;
            set {
                _entries = value;
                OnPropertyChanged(nameof(Entries));
            }
        }
        private ObservableCollection<WordlistsList> _comboBoxEntries;
        public ObservableCollection<WordlistsList> ComboBoxEntries {
            get => _comboBoxEntries;
            set {
                _comboBoxEntries = value;
                OnPropertyChanged(nameof(ComboBoxEntries));
            }
        }
        private WordlistsList _selectedItem;
        public WordlistsList SelectedItem {
            get => _selectedItem;
            set {
                if (_selectedItem != value) {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                    FillEntriesCollection();
                }
            }
        }
        public ManageViewModel() {
            ComboBoxEntries = new ObservableCollection<WordlistsList>(WordlistsList.GetWordlistsList());
            SelectedItem = ComboBoxEntries.Count > 0 ? ComboBoxEntries[0] : null;
        }

        private void FillEntriesCollection() {
            Entries = new ObservableCollection<UserControl>();
            VocabularyEntry entry = new VocabularyEntry() { 
                FilePath = VocabularyEntry.FirstPartFilePath + SelectedItem.WordlistName + VocabularyEntry.SecondPartFilePath
            };
            List<VocabularyEntry> entries = VocabularyEntry.GetData(entry);
            foreach (VocabularyEntry tempEntry in entries) { 
                Type viewType = typeof(ManageEntryView);
                UserControl tempControl = (UserControl)Activator.CreateInstance(viewType);
                tempControl.DataContext = new ManageEntryViewModel(tempEntry.German, tempEntry.English, tempEntry.FilePath);
                Entries.Add(tempControl);
            }
        }
    }
}
