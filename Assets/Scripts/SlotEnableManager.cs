using System;
using SuiLive;
using UnityEngine;
using VTS.Unity.Examples;

namespace DefaultNamespace
{
    public class SlotEnableManager : MonoBehaviour
    {
        public GameObject ToiletRoot;

        public void Update()
        {
            ToiletRoot.SetActive(ConfigManager.SavedConfig.SlotConfig.SlotEnabled);
        }
    }
}