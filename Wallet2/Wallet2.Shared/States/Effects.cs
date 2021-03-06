﻿using CoinGecko.Clients;
using CoinGecko.Interfaces;
using Lyra.Core.Accounts;
using Lyra.Core.API;
using Lyra.Core.Blocks;
using ReduxSimple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet2;
using Wallet2.Shared.Models;

namespace LyraWallet.States
{
    /// <summary>
    /// Effects can not have exception. if exception it will not fired any more, its a feature of observer model
    /// </summary>
    public static class Effects
    {
        private static IAccountDatabase GetStorage(string path)
        {
            IAccountDatabase store;
            if (path == null)
                store = new AccountInMemoryStorage();
            else
                store = new SecuredWalletStore(path);

            return store;
        }
        public static Effect<RootState> CreateWalletEffect = ReduxSimple.Effects.CreateEffect<RootState>
            (
                () => App.Store.ObserveAction<WalletCreateAction>()
                    .Select(action => 
                    {
                        try
                        {
                            IAccountDatabase store = GetStorage(action.path);
                            Wallet.Create(store, action.name, action.password, action.network);

                            var wallet = Wallet.Open(store, action.name, action.password);

                            return Observable.Return((wallet, ""));
                        }
                        catch (Exception ex)
                        {
                            return Observable.Return<(Wallet, string errMsg)>((null, ex.Message));
                        }
                    })
                    .Switch()
                    .Select(result =>
                    {
                        return new WalletOpenResultAction
                        {
                            wallet = result.Item1,
                            errorMessage = result.Item2
                        };
                    })
                    .Catch<object, Exception>(e =>
                    {
                        return Observable.Return(new WalletErrorAction
                        {
                            Error = e
                        });
                    }),
                true
            );

        public static Effect<RootState> OpenWalletEffect = ReduxSimple.Effects.CreateEffect<RootState>
            (
                () => App.Store.ObserveAction<WalletOpenAction>()
                    .Select(action =>
                    {
                        try
                        {
                            IAccountDatabase store = GetStorage(action.path);
                            var wallet = Wallet.Open(store, action.name, action.password);

                            return Observable.Return((wallet, ""));
                        }
                        catch(Exception ex)
                        {
                            return Observable.Return<(Wallet, string errMsg)>((null, ex.Message));
                        }
                    })
                    .Switch()
                    .Select(result =>
                    {
                        return new WalletOpenResultAction
                        {
                            wallet = result.Item1,
                            errorMessage = result.Item2
                        };
                    })
                    .Catch<object, Exception>(e =>
                    {
                        return Observable.Return(new WalletErrorAction
                        {
                            Error = e
                        });
                    }),
                true
            );

        public static Effect<RootState> OpenWalletAndSyncEffect = ReduxSimple.Effects.CreateEffect<RootState>
            (
                () => App.Store.ObserveAction<WalletOpenAndSyncAction>()
                    .Select(action =>
                    {
                        return Observable.StartAsync(async () =>
                        {
                            try
                            {
                                IAccountDatabase store = GetStorage(action.path);
                                var wallet = Wallet.Open(store, action.name, action.password);

                                var client = LyraRestClient.Create(wallet.NetworkId, Environment.OSVersion.ToString(), "Mobile Wallet", "1.0");
                                await wallet.Sync(client);

                                return (wallet, "");
                            }
                            catch (Exception ex)
                            {
                                return (null, ex.Message);
                            }
                        });
                    })
                    .Switch()
                    .Select(result =>
                    {
                        return new WalletSyncResultAction
                        {
                            wallet = result.wallet,
                            errorMessage = result.Item2
                        };
                    })
                    .Catch<object, Exception>(e =>
                    {
                        return Observable.Return(new WalletErrorAction
                        {
                            Error = e
                        });
                    }),
                true
            );

        public static Effect<RootState> RestoreWalletEffect = ReduxSimple.Effects.CreateEffect<RootState>
            (
                () => App.Store.ObserveAction<WalletRestoreAction>()
                    .Select(action =>
                    {
                        return Observable.StartAsync(async () => {
                            try
                            {
                                IAccountDatabase store = GetStorage(action.path);
                                Wallet.Create(store, action.name, action.password, action.network, action.privateKey);

                                var wallet = Wallet.Open(store, action.name, action.password);
                                var client = LyraRestClient.Create(action.network, Environment.OSVersion.ToString(), "Mobile Wallet", "1.0");
                                await wallet.Sync(client);

                                return (wallet, "");
                            }
                            catch (Exception ex)
                            {
                                return (null, ex.Message);
                            }
                        });
                    })
                    .Switch()
                    .Select(result =>
                    {
                        return new WalletSyncResultAction
                        {
                            wallet = result.wallet,
                            errorMessage = result.Item2
                        };
                    })
                    .Catch<object, Exception>(e =>
                    {
                        return Observable.Return(new WalletErrorAction
                        {
                            Error = e
                        });
                    }),
                true
            );

        public static Effect<RootState> RemoveWalletEffect = ReduxSimple.Effects.CreateEffect<RootState>
            (
                () => App.Store.ObserveAction<WalletRemoveAction>()
                    .Select(action =>
                    {
                        IAccountDatabase store = GetStorage(action.path);
                        store.Delete(action.name);
                        return Observable.Return<Wallet>(null);
                     })
                    .Switch()
                    .Select(result =>
                    {
                        return new WalletOpenResultAction
                        {
                            wallet = null
                        };
                    })
                    .Catch<object, Exception>(e =>
                    {
                        return Observable.Return(new WalletErrorAction
                        {
                            Error = e
                        });
                    }),
                true
            );


        public static Effect<RootState> ChangeVoteWalletEffect = ReduxSimple.Effects.CreateEffect<RootState>
            (
                () => App.Store.ObserveAction<WalletChangeVoteAction>()
                    .Select(action =>
                    {
                        action.wallet.VoteFor = action.VoteFor;
                        return Observable.Return(action.wallet);
                    })
                    .Switch()
                    .Select(result =>
                    {
                        return new WalletTransactionResultAction
                        {
                            wallet = result,
                            txName = "SetVote",
                            txResult = new APIResult { ResultCode = APIResultCodes.Success }
                        };
                    })
                    .Catch<object, Exception>(e =>
                    {
                        return Observable.Return(new WalletErrorAction
                        {
                            Error = e
                        });
                    }),
                true
            );

        public static Effect<RootState> RefreshWalletEffect = ReduxSimple.Effects.CreateEffect<RootState>
            (
                () => App.Store.ObserveAction<WalletRefreshBalanceAction>()
                    .Select(action =>
                    {
                        return Observable.StartAsync(async () => {
                            try
                            {
                                var client = LyraRestClient.Create(action.wallet.NetworkId, Environment.OSVersion.ToString(), "Mobile Wallet", "1.0");
                                var lyraTask = action.wallet.Sync(client);

                                ICoinGeckoClient _client;
                                _client = CoinGeckoClient.Instance;
                                const string vsCurrencies = "usd";
                                var priceTask = _client.SimpleClient.GetSimplePrice(new[] { "bitcoin", "lyra" }, new[] { vsCurrencies });

                                await Task.WhenAll(priceTask, lyraTask);

                                decimal lyraPrice = 0;
                                if(!priceTask.IsFaulted)
                                {
                                    lyraPrice = (decimal)priceTask.Result["lyra"]["usd"];
                                }

                                return (action.wallet, new APIResult { ResultCode = lyraTask.Result }, lyraPrice);
                            }
                            catch (Exception ex)
                            {
                                return (action.wallet, new APIResult { ResultCode = APIResultCodes.FailedToSyncAccount, ResultMessage = ex.Message }, 0);
                            }
                        });
                    })
                    .Switch()
                    .Select(result =>
                    {
                        return new WalletTransactionResultAction
                        {
                            wallet = result.wallet,
                            txName = "Refresh Balance",
                            txResult = result.Item2,
                            lyraPrice = result.Item3
                        };
                    })
                    .Catch<object, Exception>(e =>
                    {
                        return Observable.Return(new WalletErrorAction
                        {
                            Error = e
                        });
                    }),
                true
            );

        public static Effect<RootState> GetTxHistoryEffect = ReduxSimple.Effects.CreateEffect<RootState>
            (
                () => App.Store.ObserveAction<WalletGetTxHistoryAction>()
                    .Select(action =>
                    {
                        return Observable.StartAsync(async () => {
                            try
                            {
                                var txs = await GetBlocks(action.Count);

                                return (action.wallet, txs);
                            }
                            catch (Exception ex)
                            {
                                return (action.wallet, null);
                            }
                        });
                    })
                    .Switch()
                    .Select(result =>
                    {
                        return new WalletGetTxHistoryResultAction
                        {
                            wallet = result.wallet,
                            Txs = result.txs
                        };
                    })
                    .Catch<object, Exception>(e =>
                    {
                        return Observable.Return(new WalletErrorAction
                        {
                            Error = e
                        });
                    }),
                true
            );

        public static Effect<RootState> SendTokenWalletEffect = ReduxSimple.Effects.CreateEffect<RootState>
            (
                () => App.Store.ObserveAction<WalletSendTokenAction>()
                    .Select(action =>
                    {
                        return Observable.StartAsync(async () => {
                            try
                            {
                                await action.wallet.Sync(null);

                                var result = await action.wallet.Send(action.Amount, action.DstAddr, action.TokenName);

                                return (action.wallet, result);
                            }
                            catch (Exception ex)
                            {
                                return (action.wallet, new APIResult { ResultCode = APIResultCodes.ExceptionInSendTransfer, ResultMessage = ex.Message });
                            }
                        });
                    })
                    .Switch()
                    .Select(result =>
                    {
                        return new WalletTransactionResultAction
                        {
                            wallet = result.wallet,
                            txName = "Send Token",
                            txResult = result.Item2
                        };
                    })
                    .Catch<object, Exception>(e =>
                    {
                        return Observable.Return(new WalletErrorAction
                        {
                            Error = e
                        });
                    }),
                true
            );

        public static Effect<RootState> CreateTokenWalletEffect = ReduxSimple.Effects.CreateEffect<RootState>
            (
                () => App.Store.ObserveAction<WalletCreateTokenAction>()
                    .Select(action =>
                    {
                        return Observable.StartAsync(async () => {
                            try
                            {
                                await action.wallet.Sync(null);

                                var result = await action.wallet.CreateToken(action.tokenName, action.tokenDomain ?? "", action.description ?? "", Convert.ToSByte(action.precision), action.totalSupply,
                                            true, action.ownerName ?? "", action.ownerAddress ?? "", null, ContractTypes.Default, null);

                                return (action.wallet, result);
                            }
                            catch (Exception ex)
                            {
                                return (action.wallet, new APIResult { ResultCode = APIResultCodes.ExceptionInCreateToken, ResultMessage = ex.Message });
                            }
                        });
                    })
                    .Switch()
                    .Select(result =>
                    {
                        return new WalletTransactionResultAction
                        {
                            wallet = result.wallet,
                            txName = "Create Token",
                            txResult = result.Item2
                        };
                    })
                    .Catch<object, Exception>(e =>
                    {
                        return Observable.Return(new WalletErrorAction
                        {
                            Error = e
                        });
                    }),
                true
            );

        public static Effect<RootState> ImportWalletEffect = ReduxSimple.Effects.CreateEffect<RootState>
            (
                () => App.Store.ObserveAction<WalletImportAction>()
                    .Select(action =>
                    {
                        return Observable.StartAsync(async () => {
                            try
                            {
                                await action.wallet.Sync(null);

                                var result = await action.wallet.ImportAccount(action.targetPrivateKey);

                                return (action.wallet, result);
                            }
                            catch (Exception ex)
                            {
                                return (action.wallet, new APIResult { ResultCode = APIResultCodes.FailedToSyncAccount, ResultMessage = ex.Message });
                            }
                        });
                    })
                    .Switch()
                    .Select(result =>
                    {
                        return new WalletTransactionResultAction
                        {
                            wallet = result.wallet,
                            txName = "Import",
                            txResult = result.Item2
                        };
                    })
                    .Catch<object, Exception>(e =>
                    {
                        return Observable.Return(new WalletErrorAction
                        {
                            Error = e
                        });
                    }),
                true
            );


        public static Effect<RootState> RedeemWalletEffect = ReduxSimple.Effects.CreateEffect<RootState>
            (
                () => App.Store.ObserveAction<WalletRedeemAction>()
                    .Select(action =>
                    {
                        return Observable.StartAsync(async () => {
                            try
                            {
                                await action.wallet.Sync(null);

                                var result = await action.wallet.RedeemRewards(action.tokenToRedeem, action.countToRedeem);

                                return (action.wallet, result);
                            }
                            catch (Exception ex)
                            {
                                return (action.wallet, new APIResult { ResultCode = APIResultCodes.FailedToSyncAccount, ResultMessage = ex.Message });
                            }
                        });
                    })
                    .Switch()
                    .Select(result =>
                    {
                        return new WalletTransactionResultAction
                        {
                            wallet = result.wallet,
                            txName = "Redeem",
                            txResult = result.Item2
                        };
                    })
                    .Catch<object, Exception>(e =>
                    {
                        return Observable.Return(new WalletErrorAction
                        {
                            Error = e
                        });
                    }),
                true
            );

        public static Effect<RootState> NonFungibleTokenWalletEffect = ReduxSimple.Effects.CreateEffect<RootState>
            (
                () => App.Store.ObserveAction<WalletNonFungibleTokenAction>()
                    .Select(action =>
                    {
                        return Observable.StartAsync(async () => {
                            var result = await action.wallet.NonFungToStringAsync(action.nfToken);

                            return (action.wallet, result);
                        }, NewThreadScheduler.Default)
                        .ObserveOn(Scheduler.Default);
                    })
                    .Switch()
                    .Select(result =>
                    {
                        return new WalletNonFungibleTokenResultAction
                        {
                            wallet = result.wallet,
                            name = result.result.name,
                            denomination = result.result.Denomination,
                            redemptionCode = result.result.Redemption
                        };
                    })
                    .Catch<object, Exception>(e =>
                    {
                        return Observable.Return(new WalletErrorAction
                        {
                            Error = e
                        });
                    }),
                true
            );

        private static async Task<List<TxInfo>> GetBlocks(int count)
        {
            var blocks = new List<TxInfo>();
            var height = App.Store.State.wallet.GetLocalAccountHeight();

            Dictionary<string, long> oldBalance = null;
            for (long i = height - count - 1; i <= height; i++)
            {
                var blockResult = await App.Store.State.wallet.GetBlockByIndex(i);
                var block = blockResult;
                if (block == null)
                { }
                else
                {
                    string action = "", account = "";
                    if (block is SendTransferBlock sb)
                    {
                        action = $"Send";
                        account = sb.DestinationAccountId;
                    }
                    else if (block is ReceiveTransferBlock rb)
                    {
                        if (rb.SourceHash == null)
                        {
                            action = $"Genesis";
                            account = "";
                        }
                        else
                        {
                            var srcBlockResult = await App.Store.State.wallet.GetBlockByHash(rb.SourceHash);
                            var srcBlock = srcBlockResult;
                            action = $"Receive";
                            account = srcBlock.AccountID;
                        }
                    }

                    blocks.Add(new TxInfo()
                    {
                        index = block.Height,
                        timeStamp = block.TimeStamp,
                        hash = block.Hash,
                        type = block.BlockType.ToString(),
                        balance = block.Balances.Aggregate(new StringBuilder(),
                          (sbd, kvp) => sbd.AppendFormat("{0}{1} = {2}",
                                       sbd.Length > 0 ? ", " : "", kvp.Key, kvp.Value.ToBalanceDecimal()),
                          sbd => sbd.ToString()),

                        action = action,
                        account = account,
                        diffrence = BalanceDifference(oldBalance, block.Balances)
                    });

                    oldBalance = block.Balances;
                }
            }

            blocks.Reverse();
            return blocks;
        }

        private static string BalanceDifference(Dictionary<string, long> oldBalance, Dictionary<string, long> newBalance)
        {
            if (oldBalance == null)
            {
                return string.Join(", ", newBalance.Select(m => $"{m.Key} {m.Value.ToBalanceDecimal()}"));
            }
            else
            {
                var diff = newBalance.Where(x => (x.Value - (oldBalance.ContainsKey(x.Key) ? oldBalance[x.Key] : 0)) != 0)
                    .ToDictionary(p => p.Key, p => p.Value);

                return string.Join(", ", diff.Select(m => $"{m.Key} {(decimal)(m.Value - (oldBalance.ContainsKey(m.Key) ? oldBalance[m.Key] : 0)) / LyraGlobal.TOKENSTORAGERITO}"));
            }
        }
    }
}
