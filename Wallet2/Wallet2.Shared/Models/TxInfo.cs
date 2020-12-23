using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet2.Shared.Models
{
    public class TxInfo
    {
        public long index { get; set; }
        public DateTime timeStamp { get; set; }
        public string hash { get; set; }
        public string type { get; set; }
        public string balance { get; set; }

        public string action { get; set; }
        public string account { get; set; }
        public string diffrence { get; set; }

        public string Color => action == "Send" ? "#f85977" : "#67e5ad";
    }
}
