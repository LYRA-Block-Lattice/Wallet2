using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class WalletCreatePassword : Page
    {
        private WalletCreateSettings _settings;
        public WalletCreatePassword()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _settings = e.Parameter as WalletCreateSettings;

            if(!string.IsNullOrEmpty(_settings.restoreKey))
                cmdBar.Content = "Restore your wallet";
            
            base.OnNavigatedTo(e);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(this.password.Password))
            {
                var messageDialog = new MessageDialog("Password can't be empty.");
                _ = messageDialog.ShowAsync();
                return;
            }

            if(this.password.Password.Length < 6)
            {
                var messageDialog = new MessageDialog("Password too short. (At least 6 characters)");
                _ = messageDialog.ShowAsync();
                return;
            }

            _settings.password = this.password.Password;
            Frame.Navigate(typeof(WalletRepeatPassword), _settings);
        }
    }
}
