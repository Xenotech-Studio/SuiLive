using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SuiLive
{
    public partial class ConfigManager
    {
        [Header("Slot")]
        public Toggle SlotEnabledToggle;

        public bool SlotEnabledValue
        {
            get => SlotEnabledToggle.isOn;
            set => SlotEnabledToggle.isOn = value;
        }
    }
}