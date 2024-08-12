using System;
using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SuiLive
{
    public partial class ConfigManager : MonoBehaviour
    {
        public static ConfigManager _instance;
        public static ConfigManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ConfigManager>();
                }
                return _instance;
            }
        }
        
        public static ConfigData Config
        {
            get
            {
                Instance.SaveConfig();
                return SavedConfig;
            }
        }
        
        public static ConfigData SavedConfig => GameProgressData.Instance.Config;

        public void OnEnable()
        {
            InitConfigEditor();
        }

        public void InitConfigEditor()
        {
            DropGuard1Value = SavedConfig.EnterRoomDrop.DropGuard1;
            DropGuard2Value = SavedConfig.EnterRoomDrop.DropGuard2;
            DropGuard3Value = SavedConfig.EnterRoomDrop.DropGuard3;
            DropNormalUserValue = SavedConfig.EnterRoomDrop.DropNormalUser;
        }

        public void SaveConfig()
        {
            SavedConfig.EnterRoomDrop.DropGuard1 = DropGuard1Value;
            SavedConfig.EnterRoomDrop.DropGuard2 = DropGuard2Value;
            SavedConfig.EnterRoomDrop.DropGuard3 = DropGuard3Value;
            SavedConfig.EnterRoomDrop.DropNormalUser = DropNormalUserValue;
        }
    }
}
