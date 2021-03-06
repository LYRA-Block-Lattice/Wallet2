﻿using Converto;
using Lyra.Core.API;
using ReduxSimple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ReduxSimple.Reducers;

namespace LyraWallet.States
{
    public static class Reducers
    {
        public static IEnumerable<On<RootState>> CreateReducers()
        {
            return new List<On<RootState>>
                {
                    // input actions
                    On<WalletOpenAction, RootState>(
                        (state, action) => {
                            return state.With(new {
                                IsBusy = true
                            });
                        }
                    ),
                    On<WalletOpenAndSyncAction, RootState>(
                        (state, action) => {
                            return state.With(new {
                                IsBusy = true
                            });
                        }
                    ),
                    On<WalletCreateAction, RootState>(
                        (state, action) => {
                            return state.With(new {
                                IsBusy = true
                            });
                        }
                    ),
                    On<WalletRestoreAction, RootState>(
                        (state, action) => {
                            return state.With(new {
                                IsBusy = true
                            });
                        }
                    ),
                    On<WalletRemoveAction, RootState>(
                        (state, action) => {
                            return state.With(new {
                                IsBusy = true
                            });
                        }
                    ),
                    On<WalletRefreshBalanceAction, RootState>(
                        (state, action) => {
                            return state.With(new {
                                IsBusy = true
                            });
                        }
                    ),
                    On<WalletGetTxHistoryAction, RootState>(
                        (state, action) => {
                            return state.With(new {
                                IsBusy = true
                            });
                        }
                    ),
                    On<WalletSendTokenAction, RootState>(
                        (state, action) => {
                            return state.With(new {
                                IsBusy = true
                            });
                        }
                    ),
                    // result actions
                    On<WalletOpenResultAction, RootState>(
                        (state, action) => {
                            var lb = action.wallet?.GetLatestBlock();
                            return state.With(new {
                                IsBusy = false,
                                IsChanged = Guid.NewGuid().ToString(),
                                wallet = action.wallet,
                                NonFungible = lb?.NonFungibleToken,
                                Balances = lb?.Balances?.ToDictionary(k => k.Key, k => k.Value.ToBalanceDecimal()),
                                IsOpening = action.wallet == null ? false : true,
                                InitRefresh = false,
                                ErrorMessage = action.errorMessage
                            });
                        }
                    ),
                    On<WalletSyncResultAction, RootState>(
                        (state, action) => {
                            var lb = action.wallet?.GetLatestBlock();
                            return state.With(new {
                                IsBusy = false,
                                IsChanged = Guid.NewGuid().ToString(),
                                wallet = action.wallet,
                                NonFungible = lb?.NonFungibleToken,
                                Balances = lb?.Balances?.ToDictionary(k => k.Key, k => k.Value.ToBalanceDecimal()),
                                IsOpening = action.wallet == null ? false : true,
                                InitRefresh = true,
                                ErrorMessage = action.errorMessage
                            });
                        }
                    ),
                    On<WalletTransactionResultAction, RootState>(
                        (state, action) => {
                            var lb = action.wallet?.GetLatestBlock();
                            return state.With(new {
                                IsBusy = false,
                                IsChanged = Guid.NewGuid().ToString(),
                                wallet = action.wallet,
                                NonFungible = lb?.NonFungibleToken,
                                Balances = lb?.Balances?.ToDictionary(k => k.Key, k => k.Value.ToBalanceDecimal()),
                                IsOpening = true,
                                InitRefresh = true,
                                LastTransactionName = action.txName,
                                LyraPrice = action.lyraPrice == 0 ? state.LyraPrice : action.lyraPrice,
                                ErrorMessage = action.txResult.ResultCode == Lyra.Core.Blocks.APIResultCodes.Success ? "" : action.txResult.ResultCode.ToString()
                            });
                        }
                    ),
                    On<WalletNonFungibleTokenResultAction, RootState>(
                        (state, action) => {
                            var lb = action.wallet?.GetLatestBlock();
                            return state.With(new {
                                IsBusy = false,
                                IsChanged = Guid.NewGuid().ToString(),
                                LastTransactionName = "Redemption Code",
                                ErrorMessage = $"{action.name} Discount: {action.denomination.ToString("C")} Redemption Code: {action.redemptionCode}"
                            });
                        }
                    ),
                    On<WalletGetTxHistoryResultAction, RootState>(
                        (state, action) => {
                            var lb = action.wallet?.GetLatestBlock();
                            return state.With(new {
                                IsBusy = false,
                                IsChanged = Guid.NewGuid().ToString(),
                                LastTransactionName = "Get Transaction History",
                                Txs = action.Txs
                            });
                        }
                    ),
                    On<WalletErrorAction, RootState>(
                        (state, action) => 
                        {
                            return state.With(new {
                                IsBusy = false,
                                IsChanged = Guid.NewGuid().ToString(),
                                ErrorMessage = action.Error.Message
                            });
                        }
                    ),
                };

            //var walletReducers = Holding.Reducers.CreateReducers();
            ////var shopReducers = Shop.Reducers.CreateReducers();
            ////var exchangeReducers = Exchange.Reducers.CreateReducers();

            //return ReduxSimple.Reducers.CreateSubReducers(walletReducers.ToArray(), Holding.Selectors.SelectWalletState);
            //    //.Concat(CreateSubReducers(shopReducers.ToArray(), Shop.Selectors.SelectShopState))
            //    //.Concat(CreateSubReducers(exchangeReducers.ToArray(), Exchange.Selectors.SelectExchangeState));
        }
    }
}
