using DataSystem;
using UnityEngine;

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
            DropCountNormalUserValue = SavedConfig.EnterRoomDrop.DropCountNormalUser;
            DropCountGuard3Value = SavedConfig.EnterRoomDrop.DropCountGuard3;
            DropCountGuard2Value = SavedConfig.EnterRoomDrop.DropCountGuard2;
            DropCountGuard1Value = SavedConfig.EnterRoomDrop.DropCountGuard1;
            
            TopicToolEnabledValue = SavedConfig.TopicHelper.Enabled;
        }

        public void SaveConfig()
        {
            SavedConfig.EnterRoomDrop.DropGuard1 = DropGuard1Value;
            SavedConfig.EnterRoomDrop.DropGuard2 = DropGuard2Value;
            SavedConfig.EnterRoomDrop.DropGuard3 = DropGuard3Value;
            SavedConfig.EnterRoomDrop.DropNormalUser = DropNormalUserValue;
            SavedConfig.EnterRoomDrop.DropCountNormalUser = DropCountNormalUserValue;
            SavedConfig.EnterRoomDrop.DropCountGuard3 = DropCountGuard3Value;
            SavedConfig.EnterRoomDrop.DropCountGuard2 = DropCountGuard2Value;
            SavedConfig.EnterRoomDrop.DropCountGuard1 = DropCountGuard1Value;
            
            SavedConfig.TopicHelper.Enabled = TopicToolEnabledValue;
        }

        private void OnDisable()
        {
            GameProgressData.Save();
        }
    }
}
