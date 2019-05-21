using System;
using System.Collections.Generic;
using System.Text;

namespace penCsharpener.ImapData {
    public interface IDbClass {
        uint Id { get; set; }
        uint MessageId { get; set; }
    }
}
