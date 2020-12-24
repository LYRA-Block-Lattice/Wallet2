using LyraWallet.States;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Wallet2.Shared.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZXing;
using ZXing.Mobile;

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

        MobileBarcodeScanner scanner;

        public SendToken()
        {
            this.InitializeComponent();

            scanner = new MobileBarcodeScanner();

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

        private void Amount_Typed(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtAmount.Text, out decimal result))
                settings.Amount = result;

            settings.toAddress = txtAddr.Text;      // android bug. not binding properly.
        }
        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            // TODO: add argument check here
            string errMsg = null;

            var lb = App.Store.State.wallet?.GetLatestBlock();
            if (!App.Store.State.IsOpening || lb == null || !lb.Balances.ContainsKey("LYR"))
            {
                errMsg = "Wallet not open or empty";
            }                
            else
            {
                if (settings.selectedToken == "LYR")
                {
                    if (settings.Amount + 1 > lb.Balances[settings.selectedToken])
                        errMsg = "Not enough balance.";
                }
                else
                {
                    if (1 > lb.Balances[settings.selectedToken])
                        errMsg = "Not enough balance.";
                }
                if (string.IsNullOrEmpty(settings.toAddress))
                    errMsg = "Invalid to address";
                else if (settings.Amount <= 0)
                    errMsg = "Amount must > 0";
            }

            if(errMsg != null)
            {
                var messageDialog = new MessageDialog(errMsg);
                _ = messageDialog.ShowAsync();
                return;
            }

            var localSettings = ApplicationData.Current.LocalSettings;
            var bioEnabled = "ture" == localSettings.Values["biometric"]?.ToString();
            if (bioEnabled)
            {
                var fpService = CrossFingerprint.Current;// Mvx.Resolve<IFingerprint>(); // or use dependency injection and inject IFingerprint

                var request = new AuthenticationRequestConfiguration("Prove you have mvx fingers!", "Because without it you can't have access");
                var result = await fpService.AuthenticateAsync(request);
                if (result.Authenticated)
                {

                }
                else
                {
                    var messageDialog = new MessageDialog("Biometric authentication failed.");
                    _ = messageDialog.ShowAsync();
                    return;
                }
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
            //Tell our scanner to use the default overlay
            scanner.UseCustomOverlay = false;
            //We can customize the top and bottom text of our default overlay
            scanner.TopText = "Hold camera up to QR-Code";
            scanner.BottomText = "Camera will automatically scan QR-Code\r\n\r\nPress the 'Back' button to Cancel";
            //Start scanning
            scanner.Scan().ContinueWith(t =>
            {
                if (t.Result != null)
                    HandleScanResult(t.Result);
            });
        }

        private void HandleScanResult(Result result)
        {
            var t = Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                if (!string.IsNullOrWhiteSpace(result.Text) && result.Text.StartsWith("lyra:"))
                    txtAddr.Text = result.Text.Substring(5);
            });
            Task.WaitAll(t.AsTask());
        }
    }
}
