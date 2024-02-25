using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace VocabTrainer.Views {
    public class VocabularyEntry {
        public string FirstWord { get; set; }
        public string SecondWord { get; set; }
        public bool Seen { get; set; }
        public bool LastTimeWrong { get; set; }
        public int Repeated { get; set; }
        public string WordList { get; set; }
        public string FirstLanguage { get; set; }
        public string SecondLanguage { get; set; }
        [JsonIgnore]
        public string FilePath { get; set; }
        public static string GeneralFilePath { get => Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().Length - 9); }
        [JsonIgnore]
        public static string FirstPartFilePath { get => GeneralFilePath + "jsons\\"; }
        [JsonIgnore]
        public static string SecondPartFilePath { get => ".json"; }
        [JsonIgnore]
        public static readonly List<string> SpecialWordlistname = new List<string> {
            "Marked",
            "Seen",
            "NotSeen",
            "LastTimeWrong"
        };
        [JsonIgnore]
        public static readonly List<string> FilePathsSpecialLists = new List<string> {
            $"{VocabularyEntry.FirstPartFilePath}{SpecialWordlistname[0]}{VocabularyEntry.SecondPartFilePath}",
            $"{VocabularyEntry.FirstPartFilePath}{SpecialWordlistname[1]}{VocabularyEntry.SecondPartFilePath}",
            $"{VocabularyEntry.FirstPartFilePath}{SpecialWordlistname[2]}{VocabularyEntry.SecondPartFilePath}",
            $"{VocabularyEntry.FirstPartFilePath}{SpecialWordlistname[3]}{VocabularyEntry.SecondPartFilePath}"
        };
        [JsonIgnore]
        public static readonly List<VocabularyEntry> EntrySpecialWordlists = new List<VocabularyEntry> {
            new VocabularyEntry(){ FilePath = FilePathsSpecialLists[0] },
            new VocabularyEntry(){ FilePath = FilePathsSpecialLists[1] },
            new VocabularyEntry(){ FilePath = FilePathsSpecialLists[2] },
            new VocabularyEntry(){ FilePath = FilePathsSpecialLists[3] }
        };
        [JsonIgnore]
        public static List<List<VocabularyEntry>> EntriesSpecialWordlists = new List<List<VocabularyEntry>> {
            GetData(EntrySpecialWordlists[0]),
            GetData(EntrySpecialWordlists[1]),
            GetData(EntrySpecialWordlists[2]),
            GetData(EntrySpecialWordlists[3])
        };

        public VocabularyEntry() { }
        public static List<VocabularyEntry> GetData(VocabularyEntry entry) {
            List<VocabularyEntry> vocabulary = new List<VocabularyEntry>();
            if (File.Exists(entry.FilePath)) {
                string jsonData = File.ReadAllText(entry.FilePath);
                vocabulary = JsonConvert.DeserializeObject<List<VocabularyEntry>>(jsonData);
            }
            return vocabulary;
        }
        public static void WriteData(VocabularyEntry entry, List<VocabularyEntry> vocabulary) {
            string json = JsonConvert.SerializeObject(vocabulary, Formatting.Indented);
            File.WriteAllText(entry.FilePath, json);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            VocabularyEntry otherEntry = (VocabularyEntry)obj;
            return this.WordList == otherEntry.WordList &&
                   this.SecondWord == otherEntry.SecondWord &&
                   this.FirstWord == otherEntry.FirstWord;
        }
        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 31 + WordList.GetHashCode();
            hash = hash * 31 + (SecondWord ?? "").GetHashCode();
            hash = hash * 31 + (FirstWord ?? "").GetHashCode();
            return hash;
        }
        public static void AddEntry(int index, VocabularyEntry entry) {
            UpdateSpecialLists();
            bool alreadyExists = false;
            for (int i = 0; i < EntriesSpecialWordlists[index].Count; i++)
                if (EntriesSpecialWordlists[index][i].FirstWord == entry.FirstWord && EntriesSpecialWordlists[index][i].SecondWord == entry.SecondWord && EntriesSpecialWordlists[index][i].WordList == entry.WordList)
                    alreadyExists = true;
            if (!alreadyExists) {
                EntriesSpecialWordlists[index].Add(entry);
                VocabularyEntry.WriteData(EntrySpecialWordlists[index], EntriesSpecialWordlists[index]);
            }
        }

        public static void RemoveEntry(int index, VocabularyEntry entry) {
            UpdateSpecialLists();
            EntriesSpecialWordlists[index].Remove(entry);
            VocabularyEntry.WriteData(EntrySpecialWordlists[index], EntriesSpecialWordlists[index]);
        }

        public static bool CheckAnswer(VocabularyEntry entry, VocabularyEntry answer) {
            bool isCorrect = false;
            entry.FilePath = $"{FirstPartFilePath}{entry.WordList}_{entry.FirstLanguage}_{entry.SecondLanguage}{SecondPartFilePath}";
            List<VocabularyEntry> entries = GetData(entry);
            RemoveEntry(SpecialWordlistname.IndexOf("NotSeen"), entry);
            AddEntry(SpecialWordlistname.IndexOf("Seen"), entry);
            if (entries.Contains(answer)) {
                int index = entries.IndexOf(answer);
                entries[index].LastTimeWrong = false;
                entries[index].Repeated += 1;
                entries[index].Seen = true;
                RemoveEntry(SpecialWordlistname.IndexOf("LastTimeWrong"), entry);
                isCorrect = true;
            } else if (entries.Contains(entry)) {
                int index = entries.IndexOf(entry);
                entries[index].LastTimeWrong = true;
                entries[index].Repeated = 0;
                entries[index].Seen = true;
                AddEntry(SpecialWordlistname.IndexOf("LastTimeWrong"), entry);
            }
            WriteData(entry, entries);
            return isCorrect;
        }

        public static void UpdateSpecialLists() {
            EntriesSpecialWordlists = new List<List<VocabularyEntry>> {
                GetData(EntrySpecialWordlists[0]),
                GetData(EntrySpecialWordlists[1]),
                GetData(EntrySpecialWordlists[2]),
                GetData(EntrySpecialWordlists[3])
            };
        }
    }
}