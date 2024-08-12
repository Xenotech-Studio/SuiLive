using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OverlayCameraScaleAdjust : MonoBehaviour
{
    public float Size = 240;

    public float BottomOffset = 1.04f;

    public float Width = 4.8f;

    private Camera _cam;

    public Camera Cam
    {
        get
        {
            if (_cam == null)
            {
                _cam = GetComponent<Camera>();
            }
            return _cam;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Cam.orthographicSize = Size / Cam.aspect;

        float y = - (Width/2) * (1 - 1/Cam.aspect) + BottomOffset;
        Cam.transform.localPosition = new Vector3(0, y, 0);
    }
}
