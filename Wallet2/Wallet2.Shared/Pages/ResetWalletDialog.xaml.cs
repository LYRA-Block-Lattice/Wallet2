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

namespace Wallet2.Shared.Pages
{
    public sealed partial class ResetWalletDialog : ContentDialog
    {
        public ResetWalletDialog()
        {
            this.InitializeComponent();
            IsPrimaryButtonEnabled = false;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        private void RiskChecked(object sender, RoutedEventArgs e)
        {
            IsPrimaryButtonEnabled = true;
        }

        private void RiskUnChecked(object sender, RoutedEventArgs e)
        {
            IsPrimaryButtonEnabled = false;
        }
    }
}
