using EditorUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Versee.Scripts.Utils;

public class SliderFieldIndexing : MonoBehaviour
{
    [HideInInspector]
    public SerializableDictionary<string, Slider> SliderFields = new SerializableDictionary<string, Slider>();
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(SliderFieldIndexing))]
public class SliderFieldIndexingEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var textFieldIndexing = (SliderFieldIndexing) target;
        DictionaryUtils.RenderStringToObjectDic("Slider Fields", ref textFieldIndexing.SliderFields);
    }
}
#endif