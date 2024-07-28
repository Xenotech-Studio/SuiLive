#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Versee.UI
{
    public class VXR_DialogueWindow : VXR_FlowItem
    {
        
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(VXR_DialogueWindow))]
    public class VXR_DialogueWindowEditor : VXR_FlowItemEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
    #endif
}