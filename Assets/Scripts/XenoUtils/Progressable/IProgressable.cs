using System;
using UnityEngine;


namespace Versee.GameFramework
{
    
    [ExecuteInEditMode]
    public interface IProgressable
    { 
        public abstract float Progress { get; set; }
    }
}