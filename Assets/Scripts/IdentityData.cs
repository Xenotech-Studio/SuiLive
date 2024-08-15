using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OpenBLive._Demo.LoginSample.Scripts
{
    [Serializable]
    public class IdentityData
    {
        public string IdCode = "";
        public string AccessKeyId = "";
        public string AccessKeySecret = "";
        public string AppId = "";
        public int RoomId = 0;
    }
    
    // custom property drawer
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(IdentityData))]
    public class IdentityDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var idCode = property.FindPropertyRelative("IdCode");
            var accessKeyId = property.FindPropertyRelative("AccessKeyId");
            var accessKeySecret = property.FindPropertyRelative("AccessKeySecret");
            var gameId = property.FindPropertyRelative("AppId");
            var roomId = property.FindPropertyRelative("RoomId");
            var height = EditorGUIUtility.singleLineHeight;
            var rect = new Rect(position.x, position.y, position.width, height);
            EditorGUI.PropertyField(rect, idCode);
            rect.y += height;
            EditorGUI.PropertyField(rect, accessKeyId);
            rect.y += height;
            EditorGUI.PropertyField(rect, accessKeySecret);
            rect.y += height;
            EditorGUI.PropertyField(rect, gameId);
            rect.y += height;
            EditorGUI.PropertyField(rect, roomId);
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 5 + 5;
        }
    }
    #endif
    
}