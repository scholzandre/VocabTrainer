using System.Threading.Tasks;

namespace VocabTrainer {
    public class ExtraFunctions {
        public static async Task Wait(int time = 1000) {
            await Task.Delay(time);
        }
    }
}
