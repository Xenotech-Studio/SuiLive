using EditorUtils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Versee.Scripts.Utils;

public class TextInputIndexing : MonoBehaviour
{
    [FormerlySerializedAs("TextFields")] [HideInInspector]
    public SerializableDictionary<string, TMP_InputField> InputFields = new SerializableDictionary<string, TMP_InputField>();
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(TextInputIndexing))]
public class TextInputIndexingEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var textFieldIndexing = (TextInputIndexing) target;
        DictionaryUtils.RenderStringToObjectDic("Text Fields", ref textFieldIndexing.InputFields);
        
    }
}
#endif