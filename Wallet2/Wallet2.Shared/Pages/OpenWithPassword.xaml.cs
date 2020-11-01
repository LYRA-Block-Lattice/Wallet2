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
using Windows.UI.Core;
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
    public sealed partial class OpenWithPassword : Page
    {
        private WalletCreateSettings _settings;

        public OpenWithPassword()
        {
            this.InitializeComponent();

            _settings = new WalletCreateSettings();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            // redux
            _ = App.Store.Select(state => state.wallet)
                .Subscribe(async w =>
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        if (w != null)
                        {
                            Frame.Navigate(typeof(MainPage));
                        }
                        else
                        {
                            //notification.Show("Wrong Password", 3000);
                            //ExampleVSCodeInAppNotification.Show();
                        }
                    }
                    );
                });

            var pwd = password.Password;
            _ = Task.Run(() =>
            {
                string path = null;
#if !__WASM__
                path = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#endif
                var oAct = new WalletOpenAction
                {
                    path = path,
                    name = "default",
                    password = pwd ?? ""  // android simulator never get null
                };
                App.Store.Dispatch(oAct);
            });
        }

        private async void ForgotPassword(object sender, RoutedEventArgs e)
        {
            var dialog1 = new ResetWalletDialog();
            var result = await dialog1.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var dialog = new MessageDialog("Don't reset if you didn't make a backup, as there will be no way to restore your account after that.", "Warning: You can lose your account and funds forever");
                dialog.Commands.Add(new UICommand("Yes, reset wallet", null));
                dialog.Commands.Add(new UICommand("No", null));
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;
                var cmd = await dialog.ShowAsync();

                if (cmd.Label == "Yes, reset wallet")
                {
                    var dataPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
                    _ = Task.Run(() => {
                        App.Store.Dispatch(new WalletRemoveAction
                        {
                            path = dataPath,
                            name = "default"
                        });
                    });

                    var fn = $"{dataPath}/default.lyrawallet";
                    while (File.Exists(fn))
                    {
                        await Task.Delay(100);
                    }

                    Frame.Navigate(typeof(MainPage));
                }
            }
        }
    }
}
