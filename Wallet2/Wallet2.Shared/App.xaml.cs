﻿using LyraWallet.States;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ReduxSimple;
using System.Threading;
using Windows.Storage;
using Windows.UI.Core;
using Wallet2.Shared.Pages;
using Microsoft.Extensions.DependencyInjection;
using Wallet2.Shared.Models;
using System.Threading.Tasks;

namespace Wallet2
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private Frame _rootFrame;

        static ILogger _log;

        //public static ILogger Log => _log;

        public static IServiceProvider ServiceProvider { get; set; }

        public static readonly ReduxStore<RootState> Store =
            new ReduxStore<RootState>(LyraWallet.States.Reducers.CreateReducers(), RootState.InitialState, true);

        public static CancellationTokenSource WalletSubscribeCancellation = new CancellationTokenSource();


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            //ConfigureFilters(global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);
            this.UnhandledException += App_UnhandledException;

            this.InitializeComponent();
            this.Suspending += OnSuspending;

            Store.RegisterEffects(
                LyraWallet.States.Effects.CreateWalletEffect,
                LyraWallet.States.Effects.OpenWalletEffect,
                LyraWallet.States.Effects.OpenWalletAndSyncEffect,
                LyraWallet.States.Effects.RestoreWalletEffect,
                LyraWallet.States.Effects.RemoveWalletEffect,
                LyraWallet.States.Effects.ChangeVoteWalletEffect,
                LyraWallet.States.Effects.RefreshWalletEffect,
                LyraWallet.States.Effects.GetTxHistoryEffect,
                LyraWallet.States.Effects.SendTokenWalletEffect,
                LyraWallet.States.Effects.CreateTokenWalletEffect,
                LyraWallet.States.Effects.ImportWalletEffect,
                LyraWallet.States.Effects.RedeemWalletEffect,
                LyraWallet.States.Effects.NonFungibleTokenWalletEffect
                );
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            _log?.LogCritical($"App_UnhandledException: {e.Message}\nInternal Exception: {e.Exception}");
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            //var tsk = Task.Run(() => {
            //    // Create IOC container and add logging feature to it.
            //    IServiceCollection services = new ServiceCollection();
            //    services.AddLogging();

            //    // Build provider to access the logging service.
            //    IServiceProvider provider = services.BuildServiceProvider();

            //    // UWP is very restrictive of where you can save files on the disk.
            //    // The preferred place to do that is app's local folder.
            //    StorageFolder folder = ApplicationData.Current.LocalFolder;
            //    string fullPath = $"{folder.Path}/App.log";

            //    // Tell the logging service to use Serilog.File extension.
            //    var logFac = provider.GetService<ILoggerFactory>();
            //    logFac.AddFile(fullPath);

            //    _log = logFac.CreateLogger("App");
            //});
            //Task.WaitAll(tsk);

#if __IOS__
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
#endif
#if __MACOS__
            ZXing.Net.Mobile.Forms.MacOS.Platform.Init();
#endif
#if NETFX_CORE
            ZXing.Net.Mobile.Forms.WindowsUniversal.Platform.Init();
#endif


            // Set a default palette to make sure all colors used by MaterialResources exist
            this.Resources.MergedDictionaries.Add(new Uno.Material.MaterialColorPalette());

            // Overlap the default colors with the application's colors palette. 
            // TODO: Replace ms-appx:///Views/ColorPaletteOverride.xaml with your resourceDictionary.
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("ms-appx:///Views/ColorPaletteOverride.xaml") });

            // Add all the material resources. Those resources depend on the colors above, which is why this one must be added last.
            this.Resources.MergedDictionaries.Add(new Uno.Material.MaterialResources());

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
			{
				// this.DebugSettings.EnableFrameRateCounter = true;
			}
#endif
            _rootFrame = Windows.UI.Xaml.Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (_rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                _rootFrame = new Frame();

                _rootFrame.NavigationFailed += OnNavigationFailed;
                _rootFrame.Navigated += RootFrame_Navigated;
                SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Windows.UI.Xaml.Window.Current.Content = _rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (_rootFrame.Content == null)
                {
                    var dataPath = ApplicationData.Current.LocalFolder.Path;
                    var fn = $"{dataPath}/default.lyrawallet";

                    Type typEntry = null;

                    if (!File.Exists(fn) || (App.Store.State?.IsOpening ?? false))
                        typEntry = typeof(MainPage);
                    else
                        typEntry = typeof(OpenWithPassword);

                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    _rootFrame.Navigate(typEntry, e.Arguments);
                }
                // Ensure the current window is active
                Windows.UI.Xaml.Window.Current.Activate();
            }
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (_rootFrame.CanGoBack)
            {
                e.Handled = true;
                _rootFrame.GoBack();
            }
        }

        private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var view = SystemNavigationManager.GetForCurrentView();
            if (_rootFrame.CurrentSourcePageType?.Name == "MainPage")
                view.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            else
            {
                view.AppViewBackButtonVisibility = _rootFrame.BackStack.Any()
                  ? AppViewBackButtonVisibility.Visible
                  : AppViewBackButtonVisibility.Collapsed;
            }

            //Analytics.ReportPageView(e);
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }


/*        /// <summary>
        /// Configures global logging
        /// </summary>
        /// <param name="factory"></param>
        static void ConfigureFilters(ILoggerFactory factory)
        {
            factory
                .WithFilter(new FilterLoggerSettings
                    {
                        { "Uno", LogLevel.Warning },
                        { "Windows", LogLevel.Warning },

						// Debug JS interop
						// { "Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug },

						// Generic Xaml events
						// { "Windows.UI.Xaml", LogLevel.Debug },
						// { "Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug },
						// { "Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.UIElement", LogLevel.Debug },

						// Layouter specific messages
						// { "Windows.UI.Xaml.Controls", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Panel", LogLevel.Debug },
						// { "Windows.Storage", LogLevel.Debug },

						// Binding related messages
						// { "Windows.UI.Xaml.Data", LogLevel.Debug },

						// DependencyObject memory references tracking
						// { "ReferenceHolder", LogLevel.Debug },

						// ListView-related messages
						// { "Windows.UI.Xaml.Controls.ListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.GridView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelLayout", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.NativeListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListViewBaseSource", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.ListViewBaseInternalContainer", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.NativeListViewBaseAdapter", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.BufferViewCache", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelGenerator", LogLevel.Debug }, //WASM
					}
                )
#if DEBUG
				.AddConsole(LogLevel.Debug);
#else
                .AddConsole(LogLevel.Information);
#endif
        }*/
    }
}

// nuget pkg fail. remove if fixed in future.
#if NETFX_CORE
namespace ZXing.Net.Mobile.Forms.WindowsUniversal
{
    public static class Platform
    {
        public static void Init()
        {
            ZXing.Net.Mobile.Forms.WindowsUniversal.ZXingScannerViewRenderer.Init();
            ZXing.Net.Mobile.Forms.WindowsUniversal.ZXingBarcodeImageViewRenderer.Init();
        }
    }
}
#endif