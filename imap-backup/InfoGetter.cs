using Newtonsoft.Json;
using penCsharpener.ImapData;
using penCsharpener.Mail2DB;
using System.IO;
using System.Threading.Tasks;

namespace penCsharpener.ImapBackup {
    public static class InfoGetter {

        public const string CredentialFileName = ".dbcredentials";
        public const string ImapCredentials = ".mailCredentials";

        public static async Task<DbCredentials> GetDbCredentials() {
            
            if (!File.Exists(CredentialFileName)) {
                var creds = new DbCredentials("localhost", 3306, "root", "");
                var json = JsonConvert.SerializeObject(creds, Formatting.Indented);
                await File.WriteAllTextAsync(CredentialFileName, json);
                return creds;
            } else {
                var json = await File.ReadAllTextAsync(CredentialFileName);
                return JsonConvert.DeserializeObject<DbCredentials>(json);
            }
        }

        public static async Task<Credentials> GetImapCredentials() {
            if (File.Exists(ImapCredentials)) {
                var text = await File.ReadAllTextAsync(ImapCredentials);
                return JsonConvert.DeserializeObject<Credentials>(text);
            }
            return new Credentials();
        }

        public static async Task WriteCredentials(Credentials credentials) {
            var json = JsonConvert.SerializeObject(credentials, Formatting.Indented);
            await File.WriteAllTextAsync(ImapCredentials, json);
        }
    }
}
