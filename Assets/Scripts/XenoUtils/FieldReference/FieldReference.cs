using System;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Versee.Scripts.Utils
{

    using UnityEngine;
    using System;
    using System.Reflection;

    [Serializable]
    public class FieldReference
    {
        [SerializeField] private UnityEngine.Object target;
        [SerializeField] private string componentName;
        [SerializeField] private string fieldName;

        public UnityEngine.Object Target
        {
            get => target;
            set
            {
                target = value;
                componentName = string.Empty;
                fieldName = string.Empty;
            }
        }

        public string ComponentName
        {
            get => componentName;
            set => componentName = value;
        }

        public string FieldName
        {
            get => fieldName;
            set => fieldName = value;
        }

        public object GetValue()
        {
            if (target == null || string.IsNullOrEmpty(fieldName))
                return null;

            var targetType = target.GetType();
            UnityEngine.Object tempTarget = target;

            if (target is GameObject gameObject && !string.IsNullOrEmpty(componentName))
            {
                Component component = gameObject.GetComponent(componentName);

                if (component == null)
                    return null;

                targetType = component.GetType();
                tempTarget = component;
            }

            var fieldInfo = targetType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo != null) return fieldInfo.GetValue(tempTarget);
            
            var propertyInfo = targetType.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo != null) return propertyInfo.GetValue(tempTarget);
            
            return null;
        }

        public void SetValue(object value)
        {
            if (target == null || string.IsNullOrEmpty(fieldName))
                return;

            var targetType = target.GetType();
            UnityEngine.Object tempTarget = target;

            if (target is GameObject gameObject && !string.IsNullOrEmpty(componentName))
            {
                Component component = gameObject.GetComponent(componentName);
                if (component == null)
                    return;

                targetType = component.GetType();
                tempTarget = component;
            }

            var fieldInfo = targetType.GetField(fieldName,BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo != null) { fieldInfo.SetValue(tempTarget, value); return; }
            
            var propertyInfo = targetType.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo != null) { propertyInfo.SetValue(tempTarget, value); return; }

            return;
        }
        
        public bool TrySetValue(string value)
        {
            return true;
            /*if (target == null || string.IsNullOrEmpty(fieldName))
                return false;

            var targetType = target.GetType();
            UnityEngine.Object tempTarget = target;

            if (target is GameObject gameObject && !string.IsNullOrEmpty(componentName))
            {
                Component component = gameObject.GetComponent(componentName);
                if (component == null)
                    return false;

                targetType = component.GetType();
                tempTarget = component;
            }

            var fieldInfo = targetType.GetField(fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (fieldInfo == null)
                return false;

            try
            {
                var convertedValue = Convert.ChangeType(value, fieldInfo.FieldType);
                fieldInfo.SetValue(tempTarget, convertedValue);
                return true;
            }
            catch
            {
                return false;
            }*/
        }

        public object Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public bool IsNull()
        {
            return target.IsUnityNull() || string.IsNullOrEmpty(fieldName);
        }

        public static FieldReference Create( UnityEngine.Object obj, string componentName, string fieldName)
        {
            return new FieldReference() { target = obj, componentName = componentName, fieldName = fieldName};
        }
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FieldReference))]
    public class FieldReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            bool flag = property.FindPropertyRelative("target").objectReferenceValue is GameObject;

            var objectRefRect = new Rect(position.x, position.y, position.width / (flag?3:2), position.height);
            var componentRect = new Rect(position.x + position.width / 3, position.y, position.width / 3, position.height);
            var fieldRect = new Rect(position.x + (flag?2:1) * position.width / (flag?3:2), position.y, position.width / (flag?3:2), position.height);

            EditorGUI.PropertyField(objectRefRect, property.FindPropertyRelative("target"), GUIContent.none);

            var objectReference = property.FindPropertyRelative("target").objectReferenceValue as GameObject;
            if (objectReference != null)
            {
                var components = objectReference.GetComponents<Component>();
                var componentNames = new GUIContent[components.Length];
                for (int i = 0; i < components.Length; i++)
                {
                    componentNames[i] = new GUIContent(components[i].GetType().Name);
                }
                
                int selectedIndex = Array.IndexOf(components, objectReference.GetComponent(property.FindPropertyRelative("componentName").stringValue));
                selectedIndex = EditorGUI.Popup(componentRect, selectedIndex, componentNames);
                property.FindPropertyRelative("componentName").stringValue = selectedIndex >= 0 ? components[selectedIndex].GetType().Name : null;

                var componentReference = objectReference.GetComponent(property.FindPropertyRelative("componentName").stringValue);
                
                if (componentReference != null)
                {
                    var fields = componentReference.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                    var properties = componentReference.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    
                    var fieldNames = new GUIContent[fields.Length + properties.Length];
                    for (int i = 0; i < fields.Length; i++)
                    {
                        fieldNames[i] = new GUIContent(fields[i].Name);
                    }
                    for (int i = 0; i < properties.Length; i++)
                    {
                        fieldNames[i + fields.Length] = new GUIContent(properties[i].Name);
                    }

                    selectedIndex = Array.IndexOf(fieldNames, fieldNames.FirstOrDefault(field => field.text == property.FindPropertyRelative("fieldName").stringValue));
                    selectedIndex = EditorGUI.Popup(fieldRect, selectedIndex, fieldNames);
                    
                    property.FindPropertyRelative("fieldName").stringValue = selectedIndex >= 0 ? fieldNames[selectedIndex].text : "";
                }
            }
            else
            {
                
                int selectedIndex = 0;

                var target = property.FindPropertyRelative("target");
                if (target != null)
                {
                    var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                    var properties = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    
                    var fieldNames = new GUIContent[fields.Length + properties.Length];
                    for (int i = 0; i < fields.Length; i++)
                    {
                        fieldNames[i] = new GUIContent(fields[i].Name);
                    }
                    for (int i = 0; i < properties.Length; i++)
                    {
                        fieldNames[i + fields.Length] = new GUIContent(properties[i].Name);
                    }

                    selectedIndex = Array.IndexOf(fieldNames, fieldNames.FirstOrDefault(field => field.text == property.FindPropertyRelative("fieldName").stringValue));
                    selectedIndex = EditorGUI.Popup(fieldRect, selectedIndex, fieldNames);
                    
                    property.FindPropertyRelative("fieldName").stringValue = selectedIndex >= 0 ? fieldNames[selectedIndex].text : "";
                }
            }

            EditorGUI.EndProperty();
        }
    }
    #endif
}

