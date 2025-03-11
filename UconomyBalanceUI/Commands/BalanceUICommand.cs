using RestoreMonarchy.UconomyBalanceUI.Components;
using RestoreMonarchy.UconomyBalanceUI.Models;
using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;

namespace RestoreMonarchy.UconomyBalanceUI.Commands
{
    public class BalanceUICommand : IRocketCommand
    {
        private UconomyBalanceUIPlugin pluginInstance => UconomyBalanceUIPlugin.Instance;
        private UconomyBalanceUIConfiguration configuration => pluginInstance.Configuration.Instance;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            PlayerPreference preference = pluginInstance.PlayerPreferences.FirstOrDefault(p => p.SteamId == player.Id);
            bool isUIDisabled = (preference != null && preference.UIDisabled) || !configuration.ShowUIEffectByDefault;

            if (isUIDisabled)
            {
                if (preference == null)
                {
                    preference = new PlayerPreference
                    {
                        SteamId = player.Id,
                        UIDisabled = false
                    };
                    pluginInstance.PlayerPreferences.Add(preference);
                }
                else
                {
                    preference.UIDisabled = false;
                }

                UconomyBalanceUIComponent component = player.Player.GetComponent<UconomyBalanceUIComponent>();
                if (component != null)
                {
                    UnityEngine.Object.Destroy(component);
                }

                pluginInstance.SendMessageToPlayer(player, "BalanceUIEnabled");
            }
            else
            {
                if (preference == null)
                {
                    preference = new PlayerPreference
                    {
                        SteamId = player.Id,
                        UIDisabled = true
                    };
                    pluginInstance.PlayerPreferences.Add(preference);
                }
                else
                {
                    preference.UIDisabled = true;
                }

                player.Player.gameObject.AddComponent<UconomyBalanceUIComponent>();
                pluginInstance.SendMessageToPlayer(player, "BalanceUIDisabled");
            }
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "balanceui";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new();

        public List<string> Permissions => new();
    }
}
