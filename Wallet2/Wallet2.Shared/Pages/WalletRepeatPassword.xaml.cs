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
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Wallet2.Shared.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WalletRepeatPassword : Page
    {
        private WalletCreateSettings _settings;
        public WalletRepeatPassword()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _settings = e.Parameter as WalletCreateSettings;

            if (!string.IsNullOrEmpty(_settings.restoreKey))
                cmdBar.Content = "Restore your wallet";

            base.OnNavigatedTo(e);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if(_settings.password == this.password.Password)
                Frame.Navigate(typeof(WalletCreating), _settings);
            else
            {
                var messageDialog = new MessageDialog("Password Not Match.");
                _ = messageDialog.ShowAsync();
            }
        }
    }
}
