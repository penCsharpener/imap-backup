using penCsharpener.Mail2DB;
using SqlKata;
using System;

namespace penCsharpener.ImapData {
    public class DataMessage : SqlBase<DataMessage>, IDbClass {

        public uint Id { get; set; }
        public uint EmailAccountId { get; set; }
        [Ignore]
        uint IDbClass.MessageId { get; set; }
        public byte[] MimeMessageBytes { get; set; }
        public uint UId { get; set; }
        public string MailFolder { get; set; }
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string BodyPlainText { get; set; }
        public bool HasAttachments { get; set; }
        public string MessageTextId { get; set; }
        public string InReplyToId { get; set; }
        public DateTime ReceivedAtUTC { get; set; }
        public DateTime ReceivedAtLocal { get; set; }

        public override string ToString() {
            return $"{EmailAccountId} Uid: {UId} folder: {MailFolder} {Subject}";
        }

    }
}
