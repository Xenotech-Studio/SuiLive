using UnityEditor;
using UnityEngine;

namespace DataSystem
{
    public abstract class DataComponent : MonoBehaviour
    {
        public abstract string DataKey { get; }

        public bool IsPrefab = true;

        public void LoadData() => OnDataLoaded(LoadableGameData.Get(DataKey));
        
        public abstract void OnDataLoaded(GameData gameData);
    }
    
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(DataComponent), editorForChildClasses: true)]
    class DataComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            // if this gameobject is a prefab in scene
            

            if (GUILayout.Button("Load Data"))
            {
                DataComponent instance = (DataComponent) target;

                if (string.IsNullOrEmpty(instance.DataKey))
                {
                    EditorUtility.DisplayDialog(
                        "Warning",
                        "你未给此组件配置DataKey，因此无法加载数据。\nDataKey参考格式 1：Monsters[8]\nDataKey参考格式 2：Levels[3].PrizeItems[0]",
                        "好的，我去设置");
                    Debug.Log("Load-data stopped due to no datakey, nothing is changed.");
                    return;
                }
                
                if (PrefabUtility.IsPartOfAnyPrefab(instance.gameObject) && instance.IsPrefab)
                {
                    if (!EditorUtility.DisplayDialog("Warning", "你正在加载数据，但你正处于Scene内，而并没有修改Prefab。这可能导致各个场景内此组件的数值不一致。是否继续?", "我知道自己在做什么，继续", "取消操作"))
                    {
                        Debug.Log("Load-data canceled by user, nothing is changed.");
                        return;
                    }
                }
                
                Undo.RecordObject(target, "DataComponent Load Data");
                instance.LoadData();
                
            }
        }
    }
    #endif
}