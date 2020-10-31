using dotnetstandard_bip39;
using Lyra.Data.Crypto;
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
    public sealed partial class RestoreWallet : Page
    {
        private List<TextBox> tboxes;
        public RestoreWallet()
        {
            this.InitializeComponent();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tboxes = new List<TextBox>();
            // create grid
            for (var i = 0; i < 24; i++)
            {
                var tblock = new TextBlock();

                var run1 = new Windows.UI.Xaml.Documents.Run { Text = $"{i + 1}  " };
                run1.FontSize = 12;
                tblock.Inlines.Add(run1);

                tblock.Padding = new Thickness(5);
                wordsGrid.RowDefinitions.Add(new RowDefinition());

                wordsGrid.Children.Add(tblock);
                Grid.SetRow(tblock, i % 12);
                Grid.SetColumn(tblock, (i / 12) * 2);

                var tbox = new TextBox { Width = 120 };
                tbox.Padding = new Thickness(5);

                wordsGrid.Children.Add(tbox);
                Grid.SetRow(tbox, i % 12);
                Grid.SetColumn(tbox, (i / 12) * 2 + 1);

                tboxes.Add(tbox);
            }
        }

        private void wroten(object sender, RoutedEventArgs e)
        {
            if (tboxes.Any(a => string.IsNullOrWhiteSpace(a.Text)))
                return;

            var words = string.Join(" ", tboxes.Select(a => a.Text));

            var bip39 = new BIP39();
            var recovered = bip39.MnemonicToEntropy(words, BIP39Wordlist.English);

            var privateKey = StringToByteArray(recovered);

            var lyraKey = Base58Encoding.EncodePrivateKey(privateKey);

            // create or restore then goto appshell
            var oAct = new WalletRestoreAction
            {
                privateKey = lyraKey,
                network = "testnet",
                name = "default",
                password = "111111",
                path = Windows.Storage.ApplicationData.Current.LocalFolder.Path
            };

            _ = Task.Run(() => { App.Store.Dispatch(oAct); });

            Frame.Navigate(typeof(MainPage));
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
