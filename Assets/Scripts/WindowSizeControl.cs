using System;
using System.Collections;
using System.Collections.Generic;
using SuiLive;
using UnityEngine;

public class WindowSizeControl : MonoBehaviour
{
    private void Update()
    {
        if (ConfigManager.Config.WindowSize.Locked)
        {
            // let user cannot resize the window
            Screen.SetResolution(ConfigManager.SavedConfig.WindowSize.Width, ConfigManager.SavedConfig.WindowSize.Height, FullScreenMode.Windowed);
        }
        else
        {
            // user can draw to resize the window
            ConfigManager.SavedConfig.WindowSize.Width = Screen.width;
            ConfigManager.SavedConfig.WindowSize.Height = Screen.height;
        }
        
        if (ConfigManager.Config.WindowSize.StayOnTop)
        {
            // let the window stay on top
        }
        else
        {
            // let the window not stay on top
        }
    }
}
