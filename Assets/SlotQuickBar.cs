using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlotQuickBar : MonoBehaviour
{
    public TMP_InputField NameInputField;

    public TMP_Text PrizeText;
    
    public void SetPrizeText(string text)
    {
        PrizeText.text = text;
    }
    
    public void CopyToClipboard()
    {
        string result = "";

        result += NameInputField.text + " 抽到了 ";
        result += PrizeText.text;

        // current time
        result += $" ({System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})";
        
        // Copy the text to clipboard
        GUIUtility.systemCopyBuffer = result;
    }
}
