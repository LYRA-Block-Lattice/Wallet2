using LyraWallet.States;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Wallet2.Shared.Models;
using Wallet2.Shared.Pages;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZXing;
using ZXing.Mobile;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Wallet2
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		string accountIdShort = string.Empty;
		string mainBalance = string.Empty;
		string networkId = string.Empty;

		MobileBarcodeScanner scanner;

		public ObservableCollection<TxInfo> Items { get; set; }

		public MainPage()
		{
			this.InitializeComponent();

			Loaded += SamplesPage_Loaded;

			//Create a new instance of our scanner
			scanner = new MobileBarcodeScanner();
#if __UWP__
			scanner.RootFrame = this.Frame;
			scanner.Dispatcher = this.Dispatcher;
			scanner.OnCameraError += Scanner_OnCameraError;
			scanner.OnCameraInitialized += Scanner_OnCameraInitialized; ;
#endif

			Items = new ObservableCollection<TxInfo>();
			DataContext = Items;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(WalletBeforeCreate));
		}

#region CollapsibleCommandBar

		private const double CollapsibleCommandBarScrollThreshold = 48;
		private bool _isExpanded = true; // true to force first update UpdateCollapsibleCommandBar(0) in ctor.

		private void SamplesPage_Loaded(object sender, RoutedEventArgs e)
		{
			WalletChanged();

			//Frame?.BackStack?.Clear();
			//SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
			// redux
			_ = App.Store.Select(state => state.IsBusy)
				.Subscribe(async w =>
				{
					await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
					() =>
					{
						InProgressNotification.IsOpen = w;
					});					
				});

			_ = App.Store.Select(state => state.Txs)
				.Subscribe(async w =>
				{
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
						() =>
						{
							if (w == null)
							{
								txDisplay.Visibility = Visibility.Collapsed;
							}
							else
							{
								txDisplay.Visibility = Visibility.Visible;

								Items.Clear();
								foreach (var item in w.Take(3))
								{
									Items.Add(item);
								}

							}
						});
                });

			_ = App.Store.Select(state => state.IsChanged)
				.Subscribe(async w =>
				{
					await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
					() =>
					{
						if(App.Store.State.IsOpening && App.Store.State.wallet != null)
                        {
							accountIdShort = Shorten(App.Store.State.wallet?.AccountId);
							mainBalance = $"{GetMainBalance()}";

							if (App.Store.State.wallet?.NetworkId == "mainnet")
								networkId = "";
							else
								networkId = $"({App.Store.State.wallet?.NetworkId})";

							if(App.Store.State.wallet.BaseBalance != 0 && App.Store.State.LastTransactionName != "Get Transaction History")
                            {
								var oAct = new WalletGetTxHistoryAction
								{
									wallet = App.Store.State.wallet,
									Count = 3
								};
								_ = Task.Run(() => { App.Store.Dispatch(oAct); });
							}

							Bindings.Update();
						}

						WalletChanged();
                    }
					);
				});
		}

		private void WalletChanged()
        {
			if (App.Store.State.wallet != null)
			{
				withoutWallet.Visibility = Visibility.Collapsed;
				withWallet.Visibility = Visibility.Visible;

				// backup notify
				var localSettings = ApplicationData.Current.LocalSettings;
				BackupNotification.IsOpen = !("ture" == localSettings.Values["backup"]?.ToString());
			}
			else
			{
				withoutWallet.Visibility = Visibility.Visible;
				withWallet.Visibility = Visibility.Collapsed;
				BackupNotification.IsOpen = false;
			}
		}

		private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var scrollViewer = sender as ScrollViewer;
			//UpdateCollapsibleCommandBar(scrollViewer.VerticalOffset);
		}

		//private void UpdateCollapsibleCommandBar(bool shouldExpand)
		//{
		//	// We can't simply use Visibility.Collapsed on the CommandBar 
		//	// because it would break the navigation transition on iOS.

		//	//var shouldExpand = verticalOffset > CollapsibleCommandBarScrollThreshold;
		//	//if (shouldExpand == _isExpanded)
		//	//{
		//	//	return;
		//	//}

		//	if (shouldExpand)
		//	{
		//		CollapsibleCommandBar.IsHitTestVisible = true;
		//		CollapsibleCommandBar.Opacity = 1;
		//		CollapsibleCommandBar.Foreground = new SolidColorBrush(Colors.Black);
		//		CollapsibleCommandBar.Background = new SolidColorBrush(Colors.White);
		//	}
		//	else
		//	{
		//		CollapsibleCommandBar.IsHitTestVisible = false;
		//		CollapsibleCommandBar.Opacity = 0;
		//		CollapsibleCommandBar.Foreground = new SolidColorBrush(Colors.Transparent);
		//		CollapsibleCommandBar.Background = new SolidColorBrush(Colors.Transparent);
		//	}

		//	_isExpanded = shouldExpand;
		//}

		private static IEnumerable<DependencyObject> GetChildren(DependencyObject reference)
		{
			var childrenCount = VisualTreeHelper.GetChildrenCount(reference);
			for (int i = 0; i < childrenCount; i++)
			{
				var child = VisualTreeHelper.GetChild(reference, i);
				yield return child;
			}
		}

		private static IEnumerable<DependencyObject> GetDescendants(DependencyObject reference)
		{
			var children = GetChildren(reference);
			return children.Concat(children.SelectMany(GetDescendants));
		}

#endregion

		private void Receive(object sender, RoutedEventArgs e)
		{
			var oAct = new WalletRefreshBalanceAction
			{
				wallet = App.Store.State.wallet
			};
			_ = Task.Run(() => { App.Store.Dispatch(oAct); });
		}

		private void Send(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(SendToken));
		}

		private void GoSettings(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(WalletSettings));
		}

		private void CopyAddr (object sender, RoutedEventArgs e)
		{
			var dataPackage = new DataPackage();
			dataPackage.RequestedOperation = DataPackageOperation.Copy;
			dataPackage.SetText(App.Store.State.wallet.AccountId /*(sender as TextBlock).Text*/);
			Clipboard.SetContent(dataPackage);
		}

		private decimal GetMainBalance()
        {
			return App.Store.State.wallet.BaseBalance;
		}
		private string GetDollarWorth()
        {
			if (App.Store.State.wallet == null)
				return "0.00";
			else
				return (GetMainBalance() * App.Store.State.LyraPrice).ToString("0.##");
        }

		private void Backup(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(BackupWallet));
		}

		private void restore_clicked(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(RestoreWallet));
		}

		private void ShowAddress(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(ShowAddress));
		}

		void Scanner_OnCameraInitialized()
		{
			//handle initialization
		}

		void Scanner_OnCameraError(IEnumerable<string> errors)
		{
			if (errors != null)
			{
				errors.ToList().ForEach(async e => await MessageBox(e));
			}
		}

        private Task MessageBox(string e)
        {
            throw new NotImplementedException();
        }

        void scan(object sender, RoutedEventArgs e)
		{
			//Tell our scanner to use the default overlay
			scanner.UseCustomOverlay = false;
			//We can customize the top and bottom text of our default overlay
			scanner.TopText = "Hold camera up to barcode";
			scanner.BottomText = "Camera will automatically scan barcode\r\n\r\nPress the 'Back' button to Cancel";
			//Start scanning
			scanner.Scan().ContinueWith(t =>
			{
				if (t.Result != null)
					HandleScanResult(t.Result);
			});
		}

        private void HandleScanResult(Result result)
        {
            throw new NotImplementedException();
        }

        public string Shorten(string addr)
		{
			if (string.IsNullOrWhiteSpace(addr) || addr.Length < 10)
				return addr;

			return $"{addr.Substring(0, 6)}...{addr.Substring(addr.Length - 8, 8)}";
		}
	}
}
