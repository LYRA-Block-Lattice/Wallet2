using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Wallet2.Shared.Models;
using System.Threading;
using LyraWallet.States;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Wallet2.Shared.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WalletCreating : Page
    {
        private WalletCreateSettings _settings;
        private CancellationTokenSource _cancel;

        public WalletCreating()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _settings = e.Parameter as WalletCreateSettings;

            _cancel = new CancellationTokenSource();
            // redux
            App.Store.Select(state => state)
                .Subscribe(w =>
                {
                    //Device.BeginInvokeOnMainThread(async () =>
                    //{
                    //    UserDialogs.Instance.HideLoading();

                    if (w.IsOpening)
                    {
                        _cancel.Cancel();
                        App.WalletSubscribeCancellation = new CancellationTokenSource();
                        //App.Current.MainPage = new AppShell();
                    }
                    else if (w.ErrorMessage != null)
                    {


                    }
                    //await DisplayAlert("Warnning", "Wallet creation or restore failed.\n\n" + w.ErrorMessage, "Confirm");
                    //});
                }, _cancel.Token);

            // create or restore then goto appshell
            string path = null;
#if !__WASM__
            path = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#endif

            if(string.IsNullOrWhiteSpace(_settings.restoreKey))
            {
                var oAct = new WalletCreateAction
                {
                    network = _settings.network,
                    name = "default",
                    password = _settings.password,
                    path = path
                };

                _ = Task.Run(() => { App.Store.Dispatch(oAct); });
            }
            else
            {
                // create or restore then goto appshell
                var oAct = new WalletRestoreAction
                {
                    privateKey = _settings.restoreKey,
                    network = _settings.network,
                    name = "default",
                    password = _settings.password,
                    path = path
                };

                _ = Task.Run(() => { App.Store.Dispatch(oAct); });
            }

            base.OnNavigatedTo(e);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
