using LyraWallet.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class WalletSettings : Page
    {
        public WalletSettings()
        {
            this.InitializeComponent();

            var localSettings = ApplicationData.Current.LocalSettings;
            bool v = "ture" == localSettings.Values["devmode"]?.ToString();
            chkDev.IsChecked = v;
        }

        private void DevChecked(object sender, RoutedEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["devmode"] = "ture";
        }

        private void DevUnChecked(object sender, RoutedEventArgs e)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["devmode"] = "false";
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void ResetWallet(object sender, RoutedEventArgs e)
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
                _ = Task.Run(() =>
                {
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

                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["backup"] = "false";

                Frame.Navigate(typeof(MainPage));
            }
        }
    }
}
