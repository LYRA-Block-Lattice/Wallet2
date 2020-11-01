using dotnetstandard_bip39;
using Lyra.Data.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
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
    public sealed partial class BackupWallet : Page
    {
        public BackupWallet()
        {
            this.InitializeComponent();

            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BIP39 bip39 = new BIP39();
            var privateKey = App.Store.State.wallet.PrivateKey;
            var pvkBin = Base58Encoding.DecodePrivateKey(privateKey);
            var pkv = ByteArrayToString(pvkBin);
            var mnu = bip39.EntropyToMnemonic(pkv, BIP39Wordlist.English);

            var words = mnu.Trim().Split(' ');
            // create grid
            for(var i = 0; i < words.Length; i++)
            {
                var tblock = new TextBlock();

                var run1 = new Windows.UI.Xaml.Documents.Run { Text = $"{i + 1}  " };
                run1.FontSize = 12;
                tblock.Inlines.Add(run1);

                var run2 = new Windows.UI.Xaml.Documents.Run { Text = $"{words[i]}" };
                run2.FontSize = 24;
                run2.FontWeight = FontWeights.Bold;
                tblock.Inlines.Add(run2);

                tblock.Padding = new Thickness(5);

                if(i < 12)
                {
                    var rd = new RowDefinition { Height = new GridLength((double)42) };
                    wordsGrid.RowDefinitions.Add(rd);
                }

                wordsGrid.Children.Add(tblock);
                Grid.SetRow(tblock, i % 12);
                Grid.SetColumn(tblock, i / 12);
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private void wroten(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
