using HarmonyLib;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.UconomyBalanceUI.Patches
{

    [HarmonyPatch(typeof(Player))]
    class PlayerPatches
    {
        [HarmonyPatch("setAllPluginWidgetFlags")]
        [HarmonyPostfix]
        static void setAllPluginWidgetFlagsPostfix(Player __instance, EPluginWidgetFlags newFlags)
        {
            UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromPlayer(__instance);
            if (newFlags.HasFlag(EPluginWidgetFlags.ShowHealth))
            {
                UconomyBalanceUIPlugin.Instance.ShowExistingBalanceUI(unturnedPlayer);
            }
            else
            {
                UconomyBalanceUIPlugin.Instance.HideBalanceUI(unturnedPlayer);
            }
        }
    }
}
