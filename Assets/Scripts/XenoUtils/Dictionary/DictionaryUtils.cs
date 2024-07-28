using System;
using UnityEditor;
using UnityEngine;

namespace EditorUtils
{
    public abstract class DictionaryUtils
    {
        #if UNITY_EDITOR
        public static bool RenderEnumToStringDic<T1>(string label, ref Versee.Scripts.Utils.SerializableDictionary<T1, string> dic)
            where T1 : System.Enum
        {
            bool changed = false;
            UnityEditor.EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(label);
            if (dic.Keys.Count == 0)
            {
                EditorGUILayout.HelpBox("Dictionary is empty.", MessageType.Info);
            }

            foreach (T1 key in dic.Keys)
            {
                EditorGUILayout.BeginHorizontal();
                T1 newKey = (T1)EditorGUILayout.EnumPopup(key);
                string newValue = EditorGUILayout.TextField(dic[key]);
                if (GUILayout.Button("Delete"))
                {
                    dic.Remove(key);
                    changed = true;
                    break;
                }

                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    string value = dic[key];
                    if (!newKey.Equals(key))
                    {

                        dic.Remove(key);
                        dic.Add(newKey, value);
                        changed = true;
                        break;
                    }

                    if (!newValue.Equals(value))
                    {
                        dic[newKey] = newValue;
                        changed = true;
                        break;
                    }
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Key-Value"))
            {
                dic.Add((T1)(object)0, "value");
            }

            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();
            return changed;
        }
        
        public static bool RenderEnumToIntDic<T1>(string label, ref Versee.Scripts.Utils.SerializableDictionary<T1, int> dic)
            where T1 : System.Enum
        {
            bool changed = false;
            UnityEditor.EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(label);
            if (dic.Keys.Count == 0)
            {
                EditorGUILayout.HelpBox("Dictionary is empty.", MessageType.Info);
            }

            foreach (T1 key in dic.Keys)
            {
                EditorGUILayout.BeginHorizontal();
                T1 newKey = (T1)EditorGUILayout.EnumPopup(key);
                int newValue = EditorGUILayout.IntField(dic[key]);
                if (GUILayout.Button("Delete"))
                {
                    dic.Remove(key);
                    changed = true;
                    break;
                }

                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    int value = dic[key];
                    if (!newKey.Equals(key))
                    {

                        dic.Remove(key);
                        dic.Add(newKey, value);
                        changed = true;
                        break;
                    }

                    if (!newValue.Equals(value))
                    {
                        dic[newKey] = newValue;
                        changed = true;
                        break;
                    }
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Key-Value"))
            {
                dic.Add((T1)(object)0, 0);
            }

            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();
            return changed;
        }
        
        public static bool RenderStringToStringDic(string label, ref Versee.Scripts.Utils.SerializableDictionary<string, string> dic)
        {
            bool changed = false;
            UnityEditor.EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(label);
            if (dic.Keys.Count == 0)
            {
                EditorGUILayout.HelpBox("Dictionary is empty.", MessageType.Info);
            }

            foreach (string key in dic.Keys)
            {
                EditorGUILayout.BeginHorizontal();
                string newKey = EditorGUILayout.TextField(key);
                string newValue = EditorGUILayout.TextField(dic[key]);
                if (GUILayout.Button("Delete"))
                {
                    dic.Remove(key);
                    changed = true;
                    break;
                }

                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    string value = dic[key];
                    if (!newKey.Equals(key))
                    {

                        dic.Remove(key);
                        dic.Add(newKey, value);
                        changed = true;
                        break;
                    }

                    if (!newValue.Equals(value))
                    {
                        dic[newKey] = newValue;
                        changed = true;
                        break;
                    }
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Key-Value"))
            {
                dic.Add("key", "value");
            }

            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();
            return changed;
        }

        public static bool RenderEnumToEnumDic<T1, T2>(string label, ref Versee.Scripts.Utils.SerializableDictionary<T1, T2> dic)
            where T1 : System.Enum where T2 : System.Enum
        {
            bool changed = false;
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(label);
            if (dic.Keys.Count == 0)
            {
                EditorGUILayout.HelpBox("Dictionary is empty.", MessageType.Info);
            }

            foreach (T1 key in dic.Keys)
            {
                EditorGUILayout.BeginHorizontal();
                T1 newKey = (T1)EditorGUILayout.EnumPopup(key);
                T2 newValue = (T2)EditorGUILayout.EnumPopup(dic[key]);
                if (GUILayout.Button("Delete"))
                {
                    dic.Remove(key);
                    changed = true;
                    break;
                }
                
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    T2 value = dic[key];
                    if (!newKey.Equals(key))
                    {
                        dic.Remove(key);
                        dic.Add(newKey, value);
                        changed = true;
                        break;
                    }
                    if (!newValue.Equals(value))
                    {
                        dic[newKey] = newValue;
                        changed = true;
                        break;
                    }
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Key-Value"))
            {
                dic.Add((T1)(object)0, (T2)(object)0);
            }
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();
            return changed;
        }

        public static bool RenderEnumToObjectDic<T1, T2>(string label, ref Versee.Scripts.Utils.SerializableDictionary<T1, T2> dic)
            where T1 : System.Enum
            where T2 : UnityEngine.Object
        {
            bool changed = false;
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(label);
            if (dic.Keys.Count == 0)
            {
                EditorGUILayout.HelpBox("Dictionary is empty.", MessageType.Info);
            }

            foreach (T1 key in dic.Keys)
            {
                EditorGUILayout.BeginHorizontal();
                T1 newKey = (T1)EditorGUILayout.EnumPopup(key);
                T2 newValue = (T2)EditorGUILayout.ObjectField(dic[key], typeof(T2), true);
                if (GUILayout.Button("Delete"))
                {
                    dic.Remove(key);
                    changed = true;
                    break;
                }

                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    T2 value = dic[key];
                    if (!newKey.Equals(key))
                    {
                        dic.Remove(key);
                        dic.Add(newKey, value);
                        changed = true;
                        break;
                    }

                    if (newValue != value)
                    {
                        dic[newKey] = newValue;
                        changed = true;
                        break;
                    }
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Key-Value"))
            {
                dic.Add((T1)(object)0, null);
            }

            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();
            return changed;
        }
        
        public static bool RenderStringToBoolDic(string label, ref Versee.Scripts.Utils.SerializableDictionary<string, bool> dic)
        {
            bool changed = false;
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(label);
            if (dic.Keys.Count == 0)
            {
                EditorGUILayout.HelpBox("Dictionary is empty.", MessageType.Info);
            }

            foreach (string key in dic.Keys)
            {
                EditorGUILayout.BeginHorizontal();
                string newKey = EditorGUILayout.TextField(key);
                bool newValue = EditorGUILayout.Toggle(dic[key]);
                if (GUILayout.Button("Delete"))
                {
                    dic.Remove(key);
                    changed = true;
                    break;
                }

                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    bool value = dic[key];
                    if (!newKey.Equals(key))
                    {
                        dic.Remove(key);
                        dic.Add(newKey, value);
                        changed = true;
                        break;
                    }

                    if (newValue != value)
                    {
                        dic[newKey] = newValue;
                        changed = true;
                        break;
                    }
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Key-Value"))
            {
                dic.Add("", false);
            }

            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();
            return changed;
        }
        
        public static bool RenderStringToObjectDic<T>(string label, ref Versee.Scripts.Utils.SerializableDictionary<string, T> dic)
            where T : UnityEngine.Object
        {
            bool changed = false;
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(label);
            if (dic.Keys.Count == 0)
            {
                EditorGUILayout.HelpBox("Dictionary is empty.", MessageType.Info);
            }

            foreach (string key in dic.Keys)
            {
                EditorGUILayout.BeginHorizontal();
                string newKey = EditorGUILayout.TextField(key);
                T newValue = (T)EditorGUILayout.ObjectField(dic[key], typeof(T), true);
                if (GUILayout.Button("Delete"))
                {
                    dic.Remove(key);
                    changed = true;
                    break;
                }

                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    T value = dic[key];
                    if (!newKey.Equals(key))
                    {
                        dic.Remove(key);
                        dic.Add(newKey, value);
                        changed = true;
                        break;
                    }

                    if (newValue != value)
                    {
                        dic[newKey] = newValue;
                        changed = true;
                        break;
                    }
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Key-Value"))
            {
                dic.Add("", null);
            }

            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();
            return changed;
        }
        
        public static void RenderDic(string label, SerializedProperty dic, bool oneLine=false, Action<int, SerializedProperty> KeyAddition=null)
        {
            bool changed = false;
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(label);

            SerializedProperty keysProperty = dic.FindPropertyRelative("keys");
            SerializedProperty valuesProperty = dic.FindPropertyRelative("values");
            
            
            if (valuesProperty == null)
            {
                EditorGUILayout.HelpBox("Dictionary structure broken.", MessageType.Info);
                return;
            }
            
            int size = dic.FindPropertyRelative("keys").arraySize;
            if (size == 0)
            {
                EditorGUILayout.HelpBox("Dictionary is empty.", MessageType.Info);
            }

            
            
            for (int i=0; i<size; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(keysProperty.GetArrayElementAtIndex(i),new GUIContent(""), true);
                
                KeyAddition?.Invoke(i, valuesProperty.GetArrayElementAtIndex(i));

                if (!oneLine)
                {
                    if (GUILayout.Button("Delete", GUILayout.Width(50)))
                    {
                        keysProperty.DeleteArrayElementAtIndex(i);
                        valuesProperty.DeleteArrayElementAtIndex(i);
                        return; // The array size has changed; we should exit the loop.
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (!oneLine)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(12);
                }

                EditorGUILayout.PropertyField(valuesProperty.GetArrayElementAtIndex(i), new GUIContent(""), true);
                
                if (!oneLine)
                {
                    GUILayout.Space(50);
                    EditorGUILayout.EndHorizontal();
                }
                
                if (oneLine)
                {
                    if (GUILayout.Button("Delete", GUILayout.Width(50)))
                    {
                        keysProperty.DeleteArrayElementAtIndex(i);
                        valuesProperty.DeleteArrayElementAtIndex(i);
                        return; // The array size has changed; we should exit the loop.
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Key-Value"))
            {
                size++;
                keysProperty.arraySize = size;
                try { keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1).stringValue = "new"; } catch (Exception e) { }
                try { keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1).intValue = 0; } catch (Exception e) { }
                valuesProperty.arraySize = size;
            }
            GUILayout.Space(50f);
            EditorGUILayout.EndHorizontal();
            
            // You need to manually save modified property.
        }


#endif
    }
    
}