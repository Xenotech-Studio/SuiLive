using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTemplate : MonoBehaviour
{
    public int Current;
    
    public GameObject[] Templates;
    
    public GameObject SetActiveTemplate(int index)
    {
        for (int i = 0; i < Templates.Length; i++)
        {
            Templates[i].SetActive(i == index);
        }
        Current = index;
        
        return Templates[Current];
    }
    
    public GameObject GetActiveTemplate()
    {
        return Templates[Current];
    }
}
