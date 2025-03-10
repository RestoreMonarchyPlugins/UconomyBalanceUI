using fr34kyn01535.Uconomy;
using RestoreMonarchy.UconomyBalanceUI.Helpers;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using UnityEngine;

namespace RestoreMonarchy.UconomyBalanceUI.Components
{
    public class UconomyBalanceUIComponent : MonoBehaviour
    {
        private UconomyBalanceUIPlugin pluginInstance => UconomyBalanceUIPlugin.Instance;
        private UconomyBalanceUIConfiguration configuration => pluginInstance.Configuration.Instance;

        private const short EffectKey = 27520; 

        public Player Player { get; private set; }

        void Awake()
        {
            Player = GetComponent<Player>();
        }

        void Start()
        {
            Player.movement.onRadiationUpdated += OnRadiationUpdated;
            Player.clothing.onMaskUpdated += OnMaskUpdated;
            Player.life.onLifeUpdated += OnLifeUpdated;
            ShowBalanceUI();
        }

        void OnDestroy()
        {
            Player.movement.onRadiationUpdated -= OnRadiationUpdated;
            Player.clothing.onMaskUpdated -= OnMaskUpdated;
            Player.life.onLifeUpdated -= OnLifeUpdated;
            ClearBalanceUI();
        }

        private bool isHidden = false;

        private void OnRadiationUpdated(bool isRadiated)
        {
            ItemMaskAsset maskAsset = Player.clothing.maskAsset;
            if (isRadiated && maskAsset != null && maskAsset.proofRadiation)
            {
                HideBalanceUI();
            }
            else
            {
                UnhideBalanceUI();
            }
        }

        private void OnMaskUpdated(ushort newMask, byte newMaskQuality, byte[] newMaskState)
        {
            bool isRadiated = Player.movement.isRadiated;
            ItemMaskAsset maskAsset = Player.clothing.maskAsset;
            if (isRadiated && maskAsset != null && maskAsset.proofRadiation)
            {
                HideBalanceUI();
            }
            else
            {
                UnhideBalanceUI();
            }
        }

        private void OnLifeUpdated(bool isDead)
        {
            if (isDead)
            {
                HideBalanceUI();
            }
            else
            {
                UnhideBalanceUI();
            }
        }

        private void ShowBalanceUI()
        {
            ITransportConnection transportConnection = Player.channel.owner.transportConnection;
            EffectManager.sendUIEffect(configuration.EffectId, EffectKey, transportConnection, true);
            UpdateBalanceUI(true);
            isHidden = false;
        }

        private void ClearBalanceUI()
        {
            ITransportConnection transportConnection = Player.channel.owner.transportConnection;
            EffectManager.askEffectClearByID(configuration.EffectId, transportConnection);
            isHidden = true;
        }

        public void HideBalanceUI()
        {
            if (isHidden)
            {
                return;
            }

            ITransportConnection transportConnection = Player.channel.owner.transportConnection;
            EffectManager.sendUIEffectVisibility(EffectKey, transportConnection, true, "Money", false);
            EffectManager.sendUIEffectVisibility(EffectKey, transportConnection, true, "UconomyBalanceUI", false);
            isHidden = true;
        }

        public void UnhideBalanceUI()
        {
            if (!isHidden)
            {
                return;
            }

            ITransportConnection transportConnection = Player.channel.owner.transportConnection;
            EffectManager.sendUIEffectVisibility(EffectKey, transportConnection, true, "UconomyBalanceUI", true);
            isHidden = false;
        }

        public void UpdateBalanceUI(bool makeVisible = false)
        {
            ThreadHelper.RunAsynchronously(() =>
            {
                string steamId = Player.channel.owner.playerID.steamID.ToString();
                decimal balance = Uconomy.Instance.Database.GetBalance(steamId);
                string formattedBalance = FormatAmount(balance);
                ThreadHelper.RunSynchronously(() =>
                {
                    ITransportConnection transportConnection = Player.channel.owner.transportConnection;
                    string text = FormatText(configuration.BalanceFormat, formattedBalance);
                    EffectManager.sendUIEffectText(EffectKey, transportConnection, true, "BalanceText", text);
                    if (makeVisible)
                    {
                        EffectManager.sendUIEffectVisibility(EffectKey, transportConnection, true, "UconomyBalanceUI", true);
                    }                    
                });
            });
        }

        public void AddToBalanceUIAnimation(decimal amount)
        {
            ITransportConnection transportConnection = Player.channel.owner.transportConnection;
            EffectManager.sendUIEffectVisibility(EffectKey, transportConnection, true, "Money", false);

            string formattedAmount = FormatAmount(amount);
            if (amount >= 0)
            {
                string text = FormatText(configuration.EarnMoneyFormat, formattedAmount);
                EffectManager.sendUIEffectText(EffectKey, transportConnection, true, "MoneyText", text);
            }
            else
            {
                string text = FormatText(configuration.SpendMoneyFormat, formattedAmount);
                EffectManager.sendUIEffectText(EffectKey, transportConnection, true, "MoneyText", text);
            }

            EffectManager.sendUIEffectVisibility(EffectKey, transportConnection, true, "Money", true);
        }

        private static string FormatAmount(decimal amount)
        {
            return amount % 1 == 0 ? Math.Abs(amount).ToString("N0") : Math.Abs(amount).ToString("N2");
        }

        private static string FormatText(string text, params object[] placeholder)
        {
            return string.Format(text.Replace("[[", "<").Replace("]]", ">"), placeholder);
        }
    }
}
