using TMPro;
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
        
        public TMP_InputField DroupCountNormalUserInput;
        
        public int DropCountNormalUserValue
        {
            get => int.Parse(DroupCountNormalUserInput.text);
            set => DroupCountNormalUserInput.text = value.ToString();
        }
        
        public TMP_InputField DroupCountGuard3Input;
        
        public int DropCountGuard3Value
        {
            get => int.Parse(DroupCountGuard3Input.text);
            set => DroupCountGuard3Input.text = value.ToString();
        }
        
        public TMP_InputField DroupCountGuard2Input;
        
        public int DropCountGuard2Value
        {
            get => int.Parse(DroupCountGuard2Input.text);
            set => DroupCountGuard2Input.text = value.ToString();
        }
        
        public TMP_InputField DroupCountGuard1Input;
        
        public int DropCountGuard1Value
        {
            get => int.Parse(DroupCountGuard1Input.text);
            set => DroupCountGuard1Input.text = value.ToString();
        }

        public Slider DropSourceXSlider;

        public float DropSourceXValue
        {
            get => DropSourceXSlider.value;
            set => DropSourceXSlider.value = value;
        }

    }
}