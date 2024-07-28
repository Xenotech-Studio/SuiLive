using System;
using System.Collections.Generic;
using UnityEngine;

namespace Temp
{
    public class ExclusiveAppear : MonoBehaviour
    {
        public string ExclusiveGroup;
        private List<ExclusiveAppear> _whatIsHidden = new List<ExclusiveAppear>();
        private ExclusiveAppear _hiddenBy = null; 
        
        private bool _isTemporarilyHidden = false;

        private void OnEnable()
        {
            if (_hiddenBy == null)
            {
                // when open, hide all other objects in the same group
                var others = FindObjectsOfType<ExclusiveAppear>();
                foreach (var other in others)
                {
                    if (other.ExclusiveGroup == ExclusiveGroup && other != this)
                    {
                        other._isTemporarilyHidden = true;
                        other._hiddenBy = this;
                        other.gameObject.SetActive(false);
                        _whatIsHidden.Add(other);
                    }
                }
            }
            else
            {
                // Means this is a recovering call, do nothing
                _hiddenBy = null;
            }
        }

        private void OnDisable()
        {
            if (!_isTemporarilyHidden)
            {
                // when close, recover all hidden objects
                foreach (var other in _whatIsHidden)
                {
                    other.gameObject.SetActive(true);
                }
            }
            else
            {
                // Means this is a temp-hiding call, do nothing
                _isTemporarilyHidden = false;
            }
        }
    }
}