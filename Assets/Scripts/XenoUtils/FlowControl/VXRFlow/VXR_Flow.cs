using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Versee.UI
{

    public class VXR_Flow : MonoBehaviour, VXR_FlowItemInterface
    {
        public int _current = 0;

        public bool ResetOnStart = true;

        public void Reset()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                VXR_Flow flow;
                VXR_FlowItem item;
                if (child.TryGetComponent<VXR_Flow>(out flow))
                {
                    flow.Reset();
                    child.gameObject.SetActive(false);
                }
                if (i == 0)
                {
                    child.gameObject.SetActive(true);
                }
                else 
                { 
                    child.gameObject.SetActive(false);
                }

            }

            _current = 0;
        }

        public void GoTo(int target)
        {
            GameObject theOne = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
                if (i == target)
                {
                    theOne = transform.GetChild(i).gameObject;
                }
            }
            
            _current = target;
            theOne.SetActive(true);
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                foreach (ExecuteOnEnable e in theOne.GetComponentsInChildren<ExecuteOnEnable>())
                {
                    e.OnEnable();
                }
            }
            #endif
        }

        public void GoToID(string ID)
        {
            // Debug.Log(gameObject.name + " GoToID: " + ID + " is called.");

            GameObject theOne = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
                string id = "";
                try { id = transform.GetChild(i).GetComponent<VXR_FlowItem>().ID; } catch { }
                if (id==ID)
                {
                    _current = i;
                    theOne = transform.GetChild(i).gameObject;
                }
            }
            if(theOne==null) Debug.Log("Requested ID not found.");
            else
            {
                theOne.SetActive(true);
                #if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    foreach (ExecuteOnEnable e in theOne.GetComponents<ExecuteOnEnable>())
                    {
                        e.OnEnable();
                    }
                }
                #endif
            }
        }

        public int GetIndex()
        {
            return _current;
        }

        public void Next(bool recursive=false)
        {
            Debug.Log(gameObject.name + "Next is called.");
            
            _current += 1;
            if (_current >= transform.childCount)
            {
                if (recursive) _current = 0;
                else _current = _current-1;
            }
            GoTo(_current);
        }
        
        public bool HierarchyNext(bool recursive=false)
        {
            bool moved = false;
            VXR_Flow subFlow;
            bool subEnded = false;
            if (transform.GetChild(_current).TryGetComponent<VXR_Flow>(out subFlow))
            {
                moved = subFlow.HierarchyNext();
            }

            if (!moved)
            {
                _current++;
                if (_current >= transform.childCount)
                {
                    _current--;
                }
                else
                {
                    GoTo(_current);
                    moved = true;
                }
            }
            return moved;
        }

        public void Previous(bool recursive=false)
        {
            _current -= 1;
            if (_current >= transform.childCount)
            {
                if (recursive) _current = 0;
                else _current = _current+1;
            }
            GoTo(_current);
        }

        public void DoublePre(bool recursive = false)
        {
            Previous(recursive);
            Previous(recursive);
        }

        private void Start()
        {
            if (ResetOnStart) Reset();
        }

        public string ID { get; }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(VXR_Flow))]
    public class VXR_FlowEditor : Editor
    {
        public VXR_Flow group;

        private void Awake()
        {
            group = (VXR_Flow)target;
        }

        // Start is called before the first frame update
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawControlButtons();
        }

        public void DrawControlButtons()
        {
            if (GUILayout.Button("Reset"))
            {
                group.Reset();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Prev"))
            {
                group.Previous();
            }
            if (GUILayout.Button("Next"))
            {
                group.Next();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Hierarchy Next"))
            {
                group.HierarchyNext();
            }
        }
    }
    #endif
}

