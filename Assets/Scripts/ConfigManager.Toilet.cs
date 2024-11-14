using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SuiLive
{
    public partial class ConfigManager
    {
        [Header("Toilet")]
        public Toggle ToiletEnabledToggle;

        public bool ToiletEnabledValue
        {
            get => ToiletEnabledToggle.isOn;
            set => ToiletEnabledToggle.isOn = value;
        }

        public Slider ToiletPositionXSlider;

        public float ToiletPositionXValue
        {
            get => ToiletPositionXSlider.value;
            set => ToiletPositionXSlider.value = value;
        }
        
        public Slider ToiletPositionYSlider;

        public float ToiletPositionYValue
        {
            get => ToiletPositionYSlider.value;
            set => ToiletPositionYSlider.value = value;
        }
        
        
            
        public Slider ToiletSizeSlider;

        public float ToiletSizeValue
        {
            get => ToiletSizeSlider.value;
            set => ToiletSizeSlider.value = value;
        }
        
        public Slider ModelSizeSlider;

        public float ModelSizeValue
        {
            get => ModelSizeSlider.value;
            set => ModelSizeSlider.value = value;
        }
        
        public TMP_InputField AfterFlushTimeInput;
        
        public int AfterFlushTimeValue
        {
            get => int.Parse(AfterFlushTimeInput.text);
            set => AfterFlushTimeInput.text = value.ToString();
        }

    }
}