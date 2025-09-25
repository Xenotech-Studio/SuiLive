using UnityEngine.UI;
using TMPro;
using UnityEngine;

namespace SuiLive
{
    public partial class ConfigManager
    {
        [Header("Timer Countdown")]
        public Toggle CountdownEnabledToggle;
        public bool CountdownEnabledValue
        {
            get => CountdownEnabledToggle != null && CountdownEnabledToggle.isOn;
            set
            {
                if (CountdownEnabledToggle != null) CountdownEnabledToggle.isOn = value;
            }
        }

        public Slider CountdownPositionXSlider;
        public float CountdownPositionXValue
        {
            get => CountdownPositionXSlider != null ? CountdownPositionXSlider.value : 0f;
            set { if (CountdownPositionXSlider != null) CountdownPositionXSlider.value = value; }
        }

        public Slider CountdownPositionYSlider;
        public float CountdownPositionYValue
        {
            get => CountdownPositionYSlider != null ? CountdownPositionYSlider.value : 0f;
            set { if (CountdownPositionYSlider != null) CountdownPositionYSlider.value = value; }
        }

        public Slider CountdownSizeSlider;
        public float CountdownSizeValue
        {
            get => CountdownSizeSlider != null ? CountdownSizeSlider.value : 0f;
            set { if (CountdownSizeSlider != null) CountdownSizeSlider.value = value; }
        }

        public TMP_InputField CountdownSecondsInput;
        public float CountdownSecondsValue
        {
            get
            {
                if (CountdownSecondsInput == null) return 0f;
                return float.TryParse(CountdownSecondsInput.text, out var v) ? v : 0f;
            }
            set
            {
                if (CountdownSecondsInput != null) CountdownSecondsInput.text = value.ToString();
            }
        }
    }
}

