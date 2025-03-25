using fr34kyn01535.Uconomy;
using RestoreMonarchy.UconomyBalanceUI.Components;
using RestoreMonarchy.UconomyBalanceUI.Helpers;
using RestoreMonarchy.UconomyBalanceUI.Models;
using RestoreMonarchy.UconomyBalanceUI.Storages;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestoreMonarchy.UconomyBalanceUI
{
    public class UconomyBalanceUIPlugin : RocketPlugin<UconomyBalanceUIConfiguration>
    {
        public static UconomyBalanceUIPlugin Instance { get; private set; }
        public UnityEngine.Color MessageColor { get; set; }

        public DataStorage<List<PlayerPreference>> DataStorage { get; set; }
        public List<PlayerPreference> PlayerPreferences { get; set; }

        protected override void Load()
        {
            Instance = this;
            MessageColor = UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, UnityEngine.Color.green);

            string path = Configuration.Instance.JsonFilePath.Replace("{rocket_directory}", System.IO.Directory.GetCurrentDirectory());
            DataStorage = new(path);

            PlayerPreferences = DataStorage.Read();
            if (PlayerPreferences == null)
            {
                PlayerPreferences = [];
            }

            if (Level.isLoaded)
            {
                if (Uconomy.Instance == null)
                {
                    Logger.Log("Uconomy is not loaded!", ConsoleColor.Yellow);
                    UnloadPlugin(PluginState.Cancelled);
                    return;
                }

                OnPluginsLoaded();
            } else
            {
                R.Plugins.OnPluginsLoaded += OnPluginsLoaded;
            }

            Logger.Log($"{Name} {Assembly.GetName().Version.ToString(3)} has been loaded!", ConsoleColor.Yellow);
            Logger.Log("Check out more Unturned plugins at restoremonarchy.com");
        }

        protected override void Unload()
        {
            R.Plugins.OnPluginsLoaded -= OnPluginsLoaded;

            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            SaveManager.onPostSave -= SaveDisabledUIPlayers;
            if (Uconomy.Instance != null)
            {
                Uconomy.Instance.OnBalanceUpdate -= OnBalanceUpdate;
            }

            SaveDisabledUIPlayers();

            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }

        public override TranslationList DefaultTranslations => new()
        {
            { "BalanceUIDisabled", "Balance UI has been disabled" },
            { "BalanceUIEnabled", "Balance UI has been enabled" }
        };

        private void OnPluginsLoaded()
        {
            if (Uconomy.Instance == null)
            {
                Logger.Log($"Uconomy is not loaded!", ConsoleColor.Yellow);
                UnloadPlugin(PluginState.Cancelled);
                return;
            }

            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            Uconomy.Instance.OnBalanceUpdate += OnBalanceUpdate;
            SaveManager.onPostSave += SaveDisabledUIPlayers;
        }

        private void SaveDisabledUIPlayers()
        {
            DataStorage.Save(PlayerPreferences);
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
            PlayerPreference playerPreference = PlayerPreferences.FirstOrDefault(p => p.SteamId == player.Id);
            
            if (Configuration.Instance.ShowUIEffectByDefault && playerPreference != null && playerPreference.UIDisabled) 
            {
                return;
            }

            if (!Configuration.Instance.ShowUIEffectByDefault)
            {
                if (playerPreference == null || playerPreference.UIDisabled)
                {
                    return;
                }
            }

            player.Player.gameObject.AddComponent<UconomyBalanceUIComponent>();
        }

        private void OnBalanceUpdate(UnturnedPlayer player, decimal amt)
        {
            ThreadHelper.RunSynchronously(() =>
            {
                if (player == null || player.Player == null)
                {
                    return;
                }

                if (Configuration.Instance.Debug)
                {
                    Logger.Log($"{player.DisplayName} balance updated by {amt}", ConsoleColor.Yellow);
                }

                UconomyBalanceUIComponent component = player.Player.GetComponent<UconomyBalanceUIComponent>();
                if (component == null)
                {
                    return;
                }

                component.AddToBalanceUIAnimation(amt);
                component.UpdateBalanceUI();
            });
        }

        internal void SendMessageToPlayer(IRocketPlayer player, string translationKey, params object[] placeholder)
        {
            if (player == null)
            {
                return;
            }

            string msg = Translate(translationKey, placeholder);
            msg = msg.Replace("[[", "<").Replace("]]", ">");
            if (player is ConsolePlayer)
            {
                Logger.Log(msg);
                return;
            }

            UnturnedPlayer unturnedPlayer = (UnturnedPlayer)player;
            if (unturnedPlayer != null)
            {
                ChatManager.serverSendMessage(msg, MessageColor, null, unturnedPlayer.SteamPlayer(), EChatMode.SAY, Configuration.Instance.MessageIconUrl, true);
            }
        }
    }
}
