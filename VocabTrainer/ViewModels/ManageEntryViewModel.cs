using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using VocabTrainer.Models;
using VocabTrainer.Views;

namespace VocabTrainer.ViewModels {
    public class ManageEntryViewModel : BaseViewModel {
        public string FirstWord { get; set; }
        public string SecondWord { get; set; }
        public string FilePath { get; set; }
        public bool FirstWordWritable { get; set; } = false;
        public bool SecondWordWritable { get; set; } = false;
        private string _editButtonText;
        public string EditButtonText {
            get => _editButtonText;
            set {
                _editButtonText = value;
                OnPropertyChanged(nameof(EditButtonText));
            }
        }
        private string _deleteButtonText;
        public string DeleteButtonText {
            get => _deleteButtonText;
            set {
                _deleteButtonText = value;
                OnPropertyChanged(nameof(DeleteButtonText));
            }
        }
        public ManageEntryViewModel(string firstWord, string secondWord, string filePath) { 
            FirstWord = firstWord;
            SecondWord = secondWord;
            EditButtonText = "🖉";
            DeleteButtonText = "🗑";
            FilePath = filePath;
        }
        private bool CanExecuteCommand(object arg) {
            return true;
        }
        public ICommand ChangeTextCommand => new RelayCommand(ChangeText, CanExecuteCommand);
        private void ChangeText(object obj) {
            if (EditButtonText == "🖉") {
                EditButtonText = "💾";
            } else { 
                EditButtonText = "🖉";
            }
        }
        public ICommand DeleteEntryCommand => new RelayCommand(DeleteEntry, CanExecuteCommand);
        private void DeleteEntry(object obj) {
            if (DeleteButtonText == "🗑") {
                DeleteButtonText = "✓";
            } else {
                DeleteButtonText = "🗑";
            }
        }
    }
}