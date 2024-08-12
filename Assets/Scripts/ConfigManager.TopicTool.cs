using UnityEngine;
using UnityEngine.UI;

namespace SuiLive
{
    public partial class ConfigManager
    {
        [Header("Topic Tool")] 
        
        
        public Toggle TopicToolEnabledToggle;

        public bool TopicToolEnabledValue
        {
            get => TopicToolEnabledToggle.isOn;
            set => TopicToolEnabledToggle.isOn = value;
        }

    }
}