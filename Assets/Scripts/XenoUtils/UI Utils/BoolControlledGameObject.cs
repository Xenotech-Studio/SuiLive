using UnityEngine;
using UnityEngine.Events;

namespace Game.UI_Utils
{
    public class BoolControlledGameObject : MonoBehaviour
    {
        public string Key = "";
        
        public UnityEvent OnTrue;
        public UnityEvent OnFalse;

        private bool _state = false;
        
        public void SetState(bool state)
        {
            if (state)
            {
                OnTrue?.Invoke();
            }
            else
            {
                OnFalse?.Invoke();
            }

            _state = state;
        }
        
        public void Toggle()
        {
            SetState(!_state);
        }
    }
}