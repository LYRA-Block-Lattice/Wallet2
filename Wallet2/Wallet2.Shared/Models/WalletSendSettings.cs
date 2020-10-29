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
            set { SetProperty(ref _selectedToken, value); }
        }

        string _toAddress = string.Empty;
        public string toAddress
        {
            get { return _toAddress; }
            set { SetProperty(ref _toAddress, value); }
        }

        string _amountString = string.Empty;
        public string amountString
        {
            get { return _amountString; }
            set { SetProperty(ref _amountString, value); }
        }
    }
}
