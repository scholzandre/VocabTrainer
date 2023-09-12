using System.Threading.Tasks;

namespace VocabTrainer {
    public class ExtraFunctions {
        public async Task Wait() {
            await Task.Delay(1000);
        }
    }
}
