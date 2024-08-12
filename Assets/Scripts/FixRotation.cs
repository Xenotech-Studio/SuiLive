using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FaceCamera : MonoBehaviour
{
    public Vector3 forward = new Vector3(0, 0, 1);
    
    // Update is called once per frame
    public void FixedUpdate()
    {
        transform.forward = forward;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FaceCamera))]
public class FaceCameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("FixedUpdate Now"))
        {
            (target as FaceCamera).FixedUpdate();
        }
    }
}
#endif

