using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace TempGameFlow
{
    public class DevelopmentKeyboardControl : MonoBehaviour
    {
        public KeyCode CoKey = KeyCode.None;
        
        public KeyCode[] KeyCodes;
        public UnityEvent[] OnKeyPressed;

        private void Update()
        {
            for (int i = 0; i < KeyCodes.Length; i++)
            {
                if ((CoKey!=KeyCode.None && Input.GetKey(CoKey)) && Input.GetKeyDown(KeyCodes[i]))
                {
                    OnKeyPressed[i].Invoke();
                }
            }
        }

        public void SimulateKeyPress()
        {
            OnKeyPressed[0].Invoke();
        }
    }
}