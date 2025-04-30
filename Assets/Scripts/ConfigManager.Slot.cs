using System.Collections.Generic;
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
        
        public TMP_InputField WeightsInputField;
        
        public List<float> WeightsValue
        {
            get
            {
                List<float> result = new List<float>();
                
                if (WeightsInputField.text == "")
                {
                    return result;
                }
                
                string[] weights = WeightsInputField.text.Split('\n');
                foreach (string weight in weights)
                {
                    // if there is an empty line, put zero
                    // if the line failed to parse, put zero
                    if (string.IsNullOrEmpty(weight))
                    {
                        result.Add(0);
                        continue;
                    }
                    if (float.TryParse(weight, out float parsedWeight))
                    {
                        result.Add(parsedWeight);
                    }
                    else
                    {
                        result.Add(0);
                    }
                }
                return result;
            }
            set
            {
                string result = "";
                for(int i = 0; i < value.Count; i++)
                {
                    result += value[i].ToString();
                    if (i != value.Count - 1)
                    {
                        result += "\n";
                    }
                }
                WeightsInputField.text = result;
            }
        }
    }
}