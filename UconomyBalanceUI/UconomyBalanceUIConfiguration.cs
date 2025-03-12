using Rocket.API;

namespace RestoreMonarchy.UconomyBalanceUI
{
    public class UconomyBalanceUIConfiguration : IRocketPluginConfiguration
    {
        public bool Debug { get; set; }
        public bool ShouldSerializeDebug() => Debug;
        public string MessageColor { get; set; }
        public string MessageIconUrl { get; set; }
        public ushort EffectId { get; set; }
        public string BalanceFormat { get; set; }
        public string EarnMoneyFormat { get; set; }
        public string SpendMoneyFormat { get; set; }
        public bool ShowUIEffectByDefault { get; set; }
        public string JsonFilePath { get; set; }

        public void LoadDefaults()
        {
            Debug = false;
            MessageColor = "yellow";
            MessageIconUrl = "https://i.imgur.com/XF9jnxo.png";
            EffectId = 27520;
            BalanceFormat = "[[b]]$ {0}[[/b]]";
            EarnMoneyFormat = "[[color=#3e9c35]]+${0}[[/color]]";
            SpendMoneyFormat = "[[color=#ff0000]]-${0}[[/color]]";
            ShowUIEffectByDefault = true;
            JsonFilePath = "{rocket_directory}/Plugins/UconomyBalanceUI/PlayerPreferences.json";
        }
    }
}