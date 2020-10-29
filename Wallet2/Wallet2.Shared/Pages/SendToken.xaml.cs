using LyraWallet.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        string selectedToken;
        string toAddress;
        string amountString;

        public SendToken()
        {
            this.InitializeComponent();

            tokens = App.Store.State.wallet.GetLatestBlock()?.Balances?.Keys.ToList();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            // TODO: add argument check here

            var moAct = new WalletSendTokenAction
            {
                DstAddr = toAddress,
                Amount = decimal.Parse(amountString),
                TokenName = selectedToken,
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
