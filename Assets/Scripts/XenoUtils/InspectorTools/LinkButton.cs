using UnityEngine;
using System;

namespace Versee.Scripts.Utils
{
    public class LinkButton
    {
        public static void DrawLinkButton(string linkText, float ratio=6.6f, Action OnClick=null)
        {
            GUIStyle linkStyle = new GUIStyle(GUI.skin.label);
            linkStyle.normal.textColor = Color.gray;
            linkStyle.hover.textColor = Color.white;
            linkStyle.alignment = TextAnchor.MiddleCenter;
            linkStyle.richText = true;
            
            Rect linkRect = GUILayoutUtility.GetRect(new GUIContent(linkText), linkStyle);
            GUI.Label(linkRect, linkText, linkStyle);
            
            // 在链接文本下方绘制下划线
            Texture2D underlineTexture = new Texture2D(1, 1);
            underlineTexture.SetPixel(0, 0, Color.gray);
            underlineTexture.Apply();
            float underlineHeight = 0.5f; // 您可以根据需要调整这个值
            float length = linkText.Length * ratio;
            Rect underlineRect = new Rect(linkRect.x + linkRect.width/2 - length/2, linkRect.yMax, length, underlineHeight);
            GUI.DrawTexture(underlineRect, underlineTexture);
            
            // 如果用户点击链接，执行某些操作
            if (linkRect.Contains(UnityEngine.Event.current.mousePosition) && UnityEngine.Event.current.type == EventType.MouseDown)
            {
                OnClick?.Invoke();
            }
        }
    }
}