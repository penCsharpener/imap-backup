using SqlKata;

namespace penCsharpener.ImapData {
    public class DataAccount : SqlBase<DataAccount>, IDbClass {
        public uint Id { get; set; }
        [Ignore]
        uint IDbClass.MessageId { get; set; }
        public string EmailAddress { get; set; }
        public string PasswordEncrypted { get; set; }
        public ushort ImapPort { get; set; }
        public string ImapServer { get; set; }

        public override string ToString() {
            return $"{EmailAddress} {ImapServer} {ImapPort}"; 
        }
    }
}
