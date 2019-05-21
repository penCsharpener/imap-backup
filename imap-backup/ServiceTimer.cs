using System;
using System.Timers;

namespace penCsharpener.ImapBackup {
    public class ServiceTimer {

        private readonly Timer _timer;

        private ModelImapBackup mdl;

        public ServiceTimer() {
            mdl = new ModelImapBackup();
            _timer = new Timer(30000) { AutoReset = true };
            _timer.Elapsed += TimerElapsed;
        }

        private async void TimerElapsed(object sender, ElapsedEventArgs e) {
            if (!mdl.IsWorking) {
                try {
                    await mdl.GetMail();
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void Start() {
            TimerElapsed(_timer, null);
            _timer.Start();
        }

        public void Stop() {
            _timer.Stop();
        }
    }
}
