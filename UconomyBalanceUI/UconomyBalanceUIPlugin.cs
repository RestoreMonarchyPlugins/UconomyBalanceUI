using fr34kyn01535.Uconomy;
using HarmonyLib;
using RestoreMonarchy.UconomyBalanceUI.Components;
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

        protected override void Load()
        {
            Instance = this;

            if (Level.isLoaded)
            {
                OnPluginsLoaded();
            } else
            {
                R.Plugins.OnPluginsLoaded += OnPluginsLoaded;
            }

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
                throw new Exception("Uconomy is not loaded!");
            }

            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            Uconomy.Instance.OnBalanceUpdate += OnBalanceUpdate;
        }

        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            UconomyBalanceUIComponent component = player.Player.GetComponent<UconomyBalanceUIComponent>();
            if (component == null)
            {
                return;
            }

            Destroy(component);
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            player.Player.gameObject.AddComponent<UconomyBalanceUIComponent>();
        }

        private void OnBalanceUpdate(UnturnedPlayer player, decimal amt)
        {
            if (Configuration.Instance.Debug)
            {
                Logger.Log($"{player?.DisplayName ?? "unkown"} balance updated by {amt}", ConsoleColor.Yellow);
            }

            UconomyBalanceUIComponent component = player.Player.GetComponent<UconomyBalanceUIComponent>();
            if (component == null)
            {
                return;
            }

            component.AddToBalanceUIAnimation(amt);
            component.UpdateBalanceUI();            
        }
    }
}
