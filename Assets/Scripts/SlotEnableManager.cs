using System;
using SuiLive;
using UnityEngine;
using VTS.Unity.Examples;

namespace DefaultNamespace
{
    public class SlotEnableManager : MonoBehaviour
    {
        public GameObject ToiletRoot;
        
        public GameObject SlotQuickBar;
        
        public InfoHelperButtonBarHeightHelper InfoHelperButtonBarHeightHelper;

        public void Update()
        {
            ToiletRoot.SetActive(ConfigManager.SavedConfig.SlotConfig.SlotEnabled);
            
            if (SlotQuickBar != null)
            {
                SlotQuickBar.SetActive(ConfigManager.SavedConfig.SlotConfig.SlotEnabled);
            }
            
            if (InfoHelperButtonBarHeightHelper != null)
            {
                InfoHelperButtonBarHeightHelper.HasBottomBar = ConfigManager.SavedConfig.SlotConfig.SlotEnabled;
            }
        }
    }
}