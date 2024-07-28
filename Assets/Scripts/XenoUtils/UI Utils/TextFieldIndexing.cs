using EditorUtils;
using TMPro;
using UnityEngine;
using Versee.Scripts.Utils;

public class TextFieldIndexing : MonoBehaviour
{
    [HideInInspector]
    public SerializableDictionary<string, TMP_Text> TextFields = new SerializableDictionary<string, TMP_Text>();
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(TextFieldIndexing))]
public class TextFieldIndexingEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var textFieldIndexing = (TextFieldIndexing) target;
        DictionaryUtils.RenderStringToObjectDic("Text Fields", ref textFieldIndexing.TextFields);
        
    }
}
#endif