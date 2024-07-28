using System;
using UnityEngine;

public class DisappearAfterTime : MonoBehaviour
{

    public float ExistanceTime = 3;

    private float _timer = 0.0f;
    private bool shown = false;
    private Color colorcache;
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        Appear();
        Invoke("Disappear", ExistanceTime);
        colorcache = GetComponent<Renderer>().material.GetColor("_ShadowColor");
    }

    private void OnDisable()
    {
        Recover();
    }

    private void Update()
    {
        if (shown)
        {
            float progress = Math.Max(_timer, 0) / (ExistanceTime + 0.1f);
            GetComponent<Renderer>().material.SetFloat("_DiffuseDarkness", (1 - progress));
            GetComponent<Renderer>().material.SetFloat("_NotCastShadow", progress);

            Color color = colorcache;
            color.a *= (1 - progress / 2);
            GetComponent<Renderer>().material.SetColor("_ShadowColor", color);

            _timer += Time.deltaTime;
            
            // 双重保险
            if(_timer > ExistanceTime + 0.1f) Disappear();
        }
    }

    public void Appear()
    {
        Debug.Log("Appear");
        GetComponent<Renderer>().material.SetFloat("_NotCastShadow", 0.0f);
        GetComponent<Renderer>().material.SetFloat("_DiffuseDarkness", 1.0f);
        shown = true;
        _timer = -2; // 先持一秒
    }

    private void Disappear()
    {
        Debug.Log("Disappear");
        GetComponent<Renderer>().material.SetFloat("_NotCastShadow", 1.0f);
        GetComponent<Renderer>().material.SetFloat("_DiffuseDarkness", 0.0f);
        GetComponent<Renderer>().material.SetColor("_ShadowColor", colorcache);
        shown = false;
    }

    private void Recover()
    {
        GetComponent<Renderer>().material.SetFloat("_NotCastShadow", 0.0f);
        GetComponent<Renderer>().material.SetFloat("_DiffuseDarkness", 1.0f);
        GetComponent<Renderer>().material.SetColor("_ShadowColor", colorcache);
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(DisappearAfterTime))]
public class DisappearAfterTimeEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Appear"))
        {
            ((DisappearAfterTime) target).Appear();
        }
    }
}
#endif
