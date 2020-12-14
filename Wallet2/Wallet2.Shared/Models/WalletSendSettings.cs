using Lyra.Core.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Wallet2.Shared.Models
{
    [Windows.UI.Xaml.Data.Bindable]
    public class WalletSendSettings : BaseViewModel
    {
        string _selectedToken = string.Empty;
        public string selectedToken
        {
            get { return _selectedToken; }
            set { SetProperty(ref _selectedToken, value); Update(); }
        }

        decimal _selectedTokenInDollar;
        public decimal SelectedTokenInDollar
        {
            get { return _selectedTokenInDollar; }
            set { SetProperty(ref _selectedTokenInDollar, value); }
        }

        string _totalStr = string.Empty;
        public string TotalStr
        {
            get { return _totalStr; }
            set { SetProperty(ref _totalStr, value); }
        }

        string _toAddress = string.Empty;
        public string toAddress
        {
            get { return _toAddress; }
            set { SetProperty(ref _toAddress, value); }
        }

        decimal _amount;
        public decimal Amount
        {
            get { return _amount; }
            set {
                SetProperty(ref _amount, value);
                Update();
            }
        }

        decimal _currentBalance;
        public decimal CurrentBalance
        {
            get { return _currentBalance; }
            set { SetProperty(ref _currentBalance, value); }
        }

        decimal _currentBalanceInDollar;
        public decimal CurrentBalanceInDollar
        {
            get { return _currentBalanceInDollar; }
            set { SetProperty(ref _currentBalanceInDollar, value); }
        }

        private void Update()
        {
            if (selectedToken == "LYR")
            {
                SelectedTokenInDollar = App.Store.State.LyraPrice * _amount;
                TotalStr = $"Total: {_amount + 1} LYR";

                CurrentBalance = App.Store.State.wallet.BaseBalance;
                CurrentBalanceInDollar = App.Store.State.wallet.BaseBalance * App.Store.State.LyraPrice;
            }
            else if(!string.IsNullOrWhiteSpace(selectedToken))
            {
                SelectedTokenInDollar = 0;
                TotalStr = $"Total: 1 LYR + {Amount} {selectedToken}";

                var blk = App.Store.State.wallet.GetLatestBlock();
                if(blk != null && blk.Balances.ContainsKey(selectedToken))
                {
                    CurrentBalance = blk.Balances[selectedToken] / LyraGlobal.TOKENSTORAGERITO;
                }
                else
                {
                    CurrentBalance = 0;
                }
                CurrentBalanceInDollar = 0;
            }
        }
    }
}
