using System.Collections;
using System.Collections.Generic;
using EditorUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Versee.Scripts.Utils;

public class ImageFieldIndexing : MonoBehaviour
{    
    [HideInInspector]
    public SerializableDictionary<string, RawImage> ImageFields = new SerializableDictionary<string, RawImage>();
    // Start is called before the first frame update

}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(ImageFieldIndexing))]
public class ImageFieldIndexingEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var imageFieldIndexing = (ImageFieldIndexing) target;
        DictionaryUtils.RenderStringToObjectDic("Image Fields", ref imageFieldIndexing.ImageFields);
        
    }
}
#endif



