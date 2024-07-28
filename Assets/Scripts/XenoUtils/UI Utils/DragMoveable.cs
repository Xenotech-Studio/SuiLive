using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DragMovable : MonoBehaviour
{
    public RectTransform Boundary; // 指定限制移动的RectTransform
    private Vector3 offset;
    private Camera mainCamera;
    
    public static List<DragMovable> Instances = new List<DragMovable>();
    
    public static DragMovable ActiveInstance = null;

    void Start()
    {
        mainCamera = Camera.main;
        Instances.Add(this);
    }

    void OnMouseDown()
    {
        ActiveInstance = FindBlockedByWho(this);
        
        ActiveInstance.transform.SetAsLastSibling();
        
        offset = ActiveInstance.transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        ActiveInstance = FindBlockedByWho(this);
        
        Vector3 newPosition = GetMouseWorldPos() + offset;
        ActiveInstance.transform.position = ClampToBoundary(newPosition);
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    private Vector3 ClampToBoundary(Vector3 targetPosition)
    {
        // 将目标位置转换为屏幕坐标
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(targetPosition);

        // 限制屏幕坐标在RectTransform范围内
        //screenPosition.x = Mathf.Clamp(screenPosition.x, Boundary.rect.min.x, Boundary.rect.max.x);
        //screenPosition.y = Mathf.Clamp(screenPosition.y, Boundary.rect.min.y, Boundary.rect.max.y);

        // 再次将屏幕坐标转换回世界坐标
        return mainCamera.ScreenToWorldPoint(screenPosition);
    }
    
    void OnDestroy()
    {
        Instances.Remove(this);
    }
    
    private DragMovable FindBlockedByWho(DragMovable refInstance)
    {
        foreach (var instance in Instances)
        {
            if (instance != refInstance && instance.gameObject.activeSelf && instance.transform.parent==refInstance.transform.parent)
            {
                if (instance.transform.GetSiblingIndex() > refInstance.transform.GetSiblingIndex())
                {
                    // check mouse click point is blocked by other instance
                    if (RectTransformUtility.RectangleContainsScreenPoint(instance.GetComponent<RectTransform>(), Input.mousePosition, mainCamera))
                    {
                        //Debug.Log( refInstance.gameObject.name+"is blocked by " + instance.name);
                        return FindBlockedByWho(instance);
                    }
                }
            }
        }

        return refInstance;
    }
}