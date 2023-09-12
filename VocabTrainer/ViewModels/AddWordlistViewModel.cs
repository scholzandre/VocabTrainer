using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabTrainer.ViewModels {
    internal class AddWordlistViewModel {
    }
    public class WordList {
        public string wordLangOne { get; set; }
        public string wordLangTwo { get; set; }
    }
}
