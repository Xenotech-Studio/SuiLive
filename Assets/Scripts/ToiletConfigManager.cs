using System;
using SuiLive;
using UnityEngine;
using VTS.Unity.Examples;

namespace DefaultNamespace
{
    public class ToiletConfigManager : ToiletConfigManagerBase
    {
        public GameObject ToiletRoot;
        
        public override float GetToiletPositionX() => ConfigManager.SavedConfig.ToiletConfig.ToiletPositionX;
        public override float GetToiletPositionY() => ConfigManager.SavedConfig.ToiletConfig.ToiletPositionY;
        public override float GetModelPositionX() => ConfigManager.SavedConfig.ToiletConfig.ModelPositionX;
        public override float GetModelPositionY() => ConfigManager.SavedConfig.ToiletConfig.ModelPositionY;
        public override float GetToiletSize() => ConfigManager.SavedConfig.ToiletConfig.ToiletSize;
        public override float GetModelSize() => ConfigManager.SavedConfig.ToiletConfig.ModelSize;
        public override float GetFlushAnimTime() => ConfigManager.SavedConfig.ToiletConfig.FlushAnimTime;
        public override float GetAfterFlushTime() => ConfigManager.SavedConfig.ToiletConfig.AfterFlushTime;

        public void Update()
        {
            ToiletRoot.SetActive(ConfigManager.SavedConfig.ToiletConfig.ToiletEnabled);
        }
    }
}