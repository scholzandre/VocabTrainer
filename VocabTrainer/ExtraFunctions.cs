using System.Threading.Tasks;

namespace VocabTrainer {
    public class ExtraFunctions {
        public static async Task Wait() {
            await Task.Delay(1000);
        }
    }
}
