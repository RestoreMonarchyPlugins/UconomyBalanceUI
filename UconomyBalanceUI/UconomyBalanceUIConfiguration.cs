using Rocket.API;

namespace RestoreMonarchy.UconomyBalanceUI
{
    public class UconomyBalanceUIConfiguration : IRocketPluginConfiguration
    {
        public bool Debug { get; set; }
        public bool ShouldSerializeDebug() => Debug;
        public ushort EffectId { get; set; }
        public string BalanceFormat { get; set; }
        public string EarnMoneyFormat { get; set; }
        public string SpendMoneyFormat { get; set; }

        public void LoadDefaults()
        {
            Debug = false;
            EffectId = 29740;
            BalanceFormat = "[[b]]$ {0}[[/b]]";
            EarnMoneyFormat = "[[color=#3e853d]]+${0}[[/color]]";
            SpendMoneyFormat = "[[color=#ff0000]]-${0}[[/color]]";
        }
    }
}