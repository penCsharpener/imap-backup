using penCsharpener.Mail2DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace penCsharpener.ImapData {
    public class DataContact : SqlBase<DataContact>, IDbClass {

        public uint Id { get; set; }
        public uint MessageId { get; set; }
        public string ContactName { get; set; }
        public string EmailAddress { get; set; }
        public ContactTypes ContactType { get; set; }

        public override string ToString() {
            return $"{ContactType} {EmailAddress}";
        }
    }
}
