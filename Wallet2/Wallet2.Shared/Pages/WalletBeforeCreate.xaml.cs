using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wallet2.Shared.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class WalletBeforeCreate : Page
    {
        public WalletBeforeCreate()
        {
            this.InitializeComponent();

            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values["devmode"]?.ToString() == "ture")
                btnTestnet.Visibility = Visibility.Visible;
            else
                btnTestnet.Visibility = Visibility.Collapsed;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(WalletCreatePassword), new WalletCreateSettings { network = "mainnet" });
        }

        private void CreateTestnet_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(WalletCreatePassword), new WalletCreateSettings { network = "testnet" });
        }
    }
}
