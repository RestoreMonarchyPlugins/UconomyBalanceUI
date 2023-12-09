using fr34kyn01535.Uconomy;
using HarmonyLib;
using RestoreMonarchy.UconomyBalanceUI.Helpers;
using Rocket.API;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using System;

namespace RestoreMonarchy.UconomyBalanceUI
{
    public class UconomyBalanceUIPlugin : RocketPlugin<UconomyBalanceUIConfiguration>
    {
        public static UconomyBalanceUIPlugin Instance { get; private set; }

        private short EffectKey = 29740;

        private const string HarmonyId = "com.restoremonarchy.uconomybalanceui";
        private Harmony harmony;

        protected override void Load()
        {
            Instance = this;

            harmony = new(HarmonyId);            
            harmony.PatchAll(Assembly);

            R.Plugins.OnPluginsLoaded += OnPluginsLoaded;

            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()
        {
            R.Plugins.OnPluginsLoaded -= OnPluginsLoaded;

            U.Events.OnPlayerConnected -= OnPlayerConnected;
            if (Uconomy.Instance != null)
            {
                Uconomy.Instance.OnBalanceUpdate -= OnBalanceUpdate;
            }            

            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }

        private void OnPluginsLoaded()
        {
            if (Uconomy.Instance == null)
            {
                UnloadPlugin(PluginState.Failure);
                return;
            }

            U.Events.OnPlayerConnected += OnPlayerConnected;
            Uconomy.Instance.OnBalanceUpdate += OnBalanceUpdate;
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            ShowBalanceUI(player);
        }

        private void OnBalanceUpdate(UnturnedPlayer player, decimal amt)
        {
            if (Configuration.Instance.Debug)
            {
                Logger.Log($"{player.DisplayName} balance updated by {amt}", ConsoleColor.Yellow);
            }            
            AddToBalanceUIAnimation(player, amt);
            UpdateBalanceUI(player);
        }

        private void ShowBalanceUI(UnturnedPlayer player)
        {
            if (player.Player == null)
            {   
                return;
            }

            ITransportConnection transportConnection = player.Player.channel.owner.transportConnection;
            EffectManager.sendUIEffect(Configuration.Instance.EffectId, EffectKey, transportConnection, true);
            EffectManager.sendUIEffectVisibility(EffectKey, transportConnection, true, "Money", false);

            UpdateBalanceUI(player);

            EffectManager.sendUIEffectVisibility(EffectKey, transportConnection, true, "UconomyBalanceUI", true);
        }

        internal void HideBalanceUI(UnturnedPlayer player)
        {
            if (player.Player == null)
            {
                return;
            }

            ITransportConnection transportConnection = player.Player.channel.owner.transportConnection;
            EffectManager.sendUIEffectVisibility(EffectKey, transportConnection, true, "Money", false);
            EffectManager.sendUIEffectVisibility(EffectKey, transportConnection, true, "UconomyBalanceUI", false);
        }

        internal void ShowExistingBalanceUI(UnturnedPlayer player)
        {
            if (player.Player == null)
            {
                return;
            }

            ITransportConnection transportConnection = player.Player.channel.owner.transportConnection;
            EffectManager.sendUIEffectVisibility(EffectKey, transportConnection, true, "UconomyBalanceUI", true);
        }

        private void UpdateBalanceUI(UnturnedPlayer player)
        {
            if (player.Player == null)
            {
                return;
            }

            ThreadHelper.RunAsynchronously(() =>
            {
                decimal balance = Uconomy.Instance.Database.GetBalance(player.CSteamID.ToString());
                string formattedBalance = FormatAmount(balance);
                ThreadHelper.RunSynchronously(() =>
                {
                    ITransportConnection transportConnection = player.Player.channel.owner.transportConnection;
                    string text = FormatText(Configuration.Instance.BalanceFormat, formattedBalance);
                    EffectManager.sendUIEffectText(EffectKey, transportConnection, true, "BalanceText", text);
                });
            });            
        }

        private void AddToBalanceUIAnimation(UnturnedPlayer player, decimal amount)
        {
            if (player.Player == null)
            {
                return;
            }

            ThreadHelper.RunSynchronously(() =>
            {
                ITransportConnection transportConnection = player.Player.channel.owner.transportConnection;
                EffectManager.sendUIEffectVisibility(EffectKey, transportConnection, true, "Money", false);

                string formattedAmount = FormatAmount(amount);
                if (amount >= 0)
                {
                    string text = FormatText(Configuration.Instance.EarnMoneyFormat, formattedAmount);
                    EffectManager.sendUIEffectText(EffectKey, transportConnection, true, "MoneyText", text);
                }
                else
                {
                    string text = FormatText(Configuration.Instance.SpendMoneyFormat, formattedAmount);
                    EffectManager.sendUIEffectText(EffectKey, transportConnection, true, "MoneyText", text);
                }

                EffectManager.sendUIEffectVisibility(EffectKey, transportConnection, true, "Money", true);
            }); 
        }

        private string FormatAmount(decimal amount) 
        { 
            return amount % 1 == 0 ? Math.Abs(amount).ToString("N0") : Math.Abs(amount).ToString("N2");
        }

        private string FormatText(string text, params object[] placeholder)
        {
            return string.Format(text.Replace("[[", "<").Replace("]]", ">"), placeholder);
        }
    }
}
