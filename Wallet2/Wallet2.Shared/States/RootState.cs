using Lyra.Core.Accounts;
using Lyra.Core.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace LyraWallet.States
{
    public class RootState
    {
        public string IsChanged { get; set; }
        public bool IsOpening { get; set; }
        public bool InitRefresh { get; set; }
        public Wallet wallet { get; set; }
        public List<string> txs { get; set; }

        public string LastTransactionName { get; set; }
        public string ErrorMessage { get; set; }

        public NonFungibleToken NonFungible { get; set; }
        public Dictionary<string, decimal> Balances { get; set; }
        public decimal LyraPrice { get; set; }

        public static RootState InitialState =>
            new RootState
            {
                IsChanged = null,
                IsOpening = false,
                wallet = null,
                txs = null,
                ErrorMessage = null,
            };
    }
}
