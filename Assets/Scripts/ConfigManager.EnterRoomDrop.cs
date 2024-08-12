using UnityEngine;
using UnityEngine.UI;

namespace SuiLive
{
    public partial class ConfigManager
    {
        [Header("Enter Room Drop")] public Toggle DropGuard1Toggle;

        public bool DropGuard1Value
        {
            get => DropGuard1Toggle.isOn;
            set => DropGuard1Toggle.isOn = value;
        }

        public Toggle DropGuard2Toggle;

        public bool DropGuard2Value
        {
            get => DropGuard2Toggle.isOn;
            set => DropGuard2Toggle.isOn = value;
        }

        public Toggle DropGuard3Toggle;

        public bool DropGuard3Value
        {
            get => DropGuard3Toggle.isOn;
            set => DropGuard3Toggle.isOn = value;
        }

        public Toggle DropNormalUserToggle;

        public bool DropNormalUserValue
        {
            get => DropNormalUserToggle.isOn;
            set => DropNormalUserToggle.isOn = value;
        }

    }
}