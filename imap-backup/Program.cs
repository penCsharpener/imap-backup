using penCsharpener.ImapBackup;
using System;
using System.Linq;
using Topshelf;

namespace penCsharpener.ImapBackup {
    static class Program {
        static void Main(string[] args) {
            var exitCode = HostFactory.Run(x => {
                x.Service<ServiceTimer>(s => {
                    s.ConstructUsing(timer => new ServiceTimer());
                    s.WhenStarted(timer => timer.Start());
                    s.WhenStopped(timer => timer.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("ImapBackupService");
                x.SetDisplayName("IMAP Backup Service");
                x.SetDescription("This service backs up your Imap accounts into an SQL database server");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;

        }
    }

    public static class SomeExtensions {
        public static string[] GetPropertyNames(this Type type) {
            return type.GetProperties()?.Select(x => x.Name).ToArray();
        }
    }
}
