using LyraWallet.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Wallet2.Shared.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Wallet2.Shared.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SendToken : Page
    {
        List<string> tokens;
        WalletSendSettings settings;

        public SendToken()
        {
            this.InitializeComponent();

            this.Loaded += SendToken_Loaded;

            settings = new WalletSendSettings();
            DataContext = settings;
            tokens = App.Store.State.wallet.GetLatestBlock()?.Balances?.Keys.ToList();
        }

        private void SendToken_Loaded(object sender, RoutedEventArgs e)
        {
            settings.selectedToken = "LYR";
            settings.CurrentBalance = App.Store.State.wallet.BaseBalance;
            settings.CurrentBalanceInDollar = App.Store.State.wallet.BaseBalance * App.Store.State.LyraPrice;
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            // TODO: add argument check here
            string errMsg = null;
            if(settings.selectedToken == "LYR")
            {
                if (settings.Amount + 1 > App.Store.State.wallet.GetLatestBlock().Balances[settings.selectedToken])
                    errMsg = "Not enough balance.";
            }
            else
            {
                if (1 > App.Store.State.wallet.GetLatestBlock().Balances[settings.selectedToken])
                    errMsg = "Not enough balance.";
            }
            if (string.IsNullOrEmpty(settings.toAddress))
                errMsg = "Invalid to address";

            if(errMsg != null)
            {
                var messageDialog = new MessageDialog(errMsg);
                _ = messageDialog.ShowAsync();
                return;
            }

            var moAct = new WalletSendTokenAction
            {
                DstAddr = settings.toAddress,
                Amount = settings.Amount,
                TokenName = settings.selectedToken,
                wallet = App.Store.State.wallet
            };
            //var mtxt = "Transfering funds...";

            _ = Task.Run(() => { App.Store.Dispatch(moAct); });

            Frame.Navigate(typeof(MainPage));
        }

        private void Scan_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
