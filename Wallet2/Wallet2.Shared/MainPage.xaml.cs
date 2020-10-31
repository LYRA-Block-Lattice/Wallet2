﻿using LyraWallet.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Wallet2.Shared.Pages;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Wallet2
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		string accountId = string.Empty;
		string mainBalance = string.Empty;

		public MainPage()
		{
			this.InitializeComponent();

			Loaded += SamplesPage_Loaded;
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
			//Frame?.BackStack?.Clear();
			//SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
			// redux
			_ = App.Store.Select(state => state.IsChanged)
				.Subscribe(async w =>
				{
					await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
					() =>
					{
						if(App.Store.State.IsOpening)
                        {
							accountId = App.Store.State.wallet?.AccountId;
							mainBalance = $"{GetMainBalance()}";

							Bindings.Update();
						}

                        if (App.Store.State.wallet != null)
                        {
                            withoutWallet.Visibility = Visibility.Collapsed;
                            withWallet.Visibility = Visibility.Visible;
                        }
                        else
                        {
							withoutWallet.Visibility = Visibility.Visible;
							withWallet.Visibility = Visibility.Collapsed;
                        }
                    }
					);
				});
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
			dataPackage.SetText((sender as TextBlock).Text);
			Clipboard.SetContent(dataPackage);
		}

		private double GetMainBalance()
        {
			return (double)App.Store.State.wallet.MainBalance / 100000000;
		}
		private string GetDollarWorth()
        {
			if (App.Store.State.wallet == null)
				return "0.00";
			else
				return $"{GetMainBalance() * 0.0026}";
        }

		private void Backup(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(BackupWallet));
		}

		private void restore_clicked(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(RestoreWallet));
		}
	}
}
