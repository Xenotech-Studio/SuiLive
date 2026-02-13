using System;
using SuiLive;
using TMPro;
using UnityEngine;
using VTS.Unity.Examples;

namespace DefaultNamespace
{
    public class SlotEnableManager : MonoBehaviour
    {
        public SlotMachine SlotRoot;
        
        public GameObject SlotQuickBar;
        
        public InfoHelperButtonBarHeightHelper InfoHelperButtonBarHeightHelper;
        
        public TMP_InputField GiftKeysDisplay;

        public void Update()
        {
            SlotRoot.gameObject.SetActive(ConfigManager.SavedConfig.SlotConfig.SlotEnabled);
            
            if (SlotQuickBar != null)
            {
                SlotQuickBar.SetActive(ConfigManager.SavedConfig.SlotConfig.SlotEnabled);
            }
            
            if (InfoHelperButtonBarHeightHelper != null)
            {
                InfoHelperButtonBarHeightHelper.HasBottomBar = ConfigManager.SavedConfig.SlotConfig.SlotEnabled;
            }
            
            if (GiftKeysDisplay != null)
            {
                GiftKeysDisplay.text = string.Join("\n", SlotRoot.SlotItems);
            }
            
            if (ConfigManager.Config.SlotConfig.Weights.Count < SlotRoot.SlotItems.Count)
            {
                for (int i = ConfigManager.SavedConfig.SlotConfig.Weights.Count; i < SlotRoot.SlotItems.Count; i++)
                {
                    ConfigManager.SavedConfig.SlotConfig.Weights.Add(1);
                }
                ConfigManager.Save();
            }
            
            if (ConfigManager.Config.SlotConfig.Weights.Count > SlotRoot.SlotItems.Count)
            {
                for (int i = ConfigManager.SavedConfig.SlotConfig.Weights.Count; i > SlotRoot.SlotItems.Count; i--)
                {
                    ConfigManager.SavedConfig.SlotConfig.Weights.RemoveAt(i - 1);
                }
                ConfigManager.Save();
            }
        }
    }
}