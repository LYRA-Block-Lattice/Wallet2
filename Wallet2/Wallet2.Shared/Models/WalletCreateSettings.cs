using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Wallet2.Shared.Models
{
    public class WalletCreateSettings : BaseViewModel
    {
        string _network = string.Empty;
        public string network
        {
            get { return _network; }
            set { SetProperty(ref _network, value); }
        }

        string _password = string.Empty;
        public string password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        string _repeatPassword = string.Empty;
        public string repeatPassword
        {
            get { return _repeatPassword; }
            set { SetProperty(ref _repeatPassword, value); }
        }
    }
}
