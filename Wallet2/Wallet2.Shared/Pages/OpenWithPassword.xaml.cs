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
            this.DataContext = _settings;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            // redux
            _ = App.Store.Select(state => state.IsChanged)
                .Subscribe(async w =>
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        if (App.Store.State.wallet != null)
                        {
                            Frame.Navigate(typeof(MainPage));
                        }
                        //else
                        //{
                        //    var messageDialog = new MessageDialog(App.Store.State?.ErrorMessage?? "Wrong password", "Failed to open wallet");
                        //    _ = messageDialog.ShowAsync();
                        //}
                    }
                    );
                });

            var pwd = password.Password;
            _ = Task.Run(() =>
            {
                
                var oAct = new WalletOpenAction
                {
                    path = Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                    name = "default",
                    password = pwd ?? ""  // android simulator never get null
                };
                App.Store.Dispatch(oAct);
            });
        }
    }
}
