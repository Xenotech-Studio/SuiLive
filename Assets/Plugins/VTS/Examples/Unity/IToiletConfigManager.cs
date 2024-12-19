using UnityEngine;

namespace VTS.Unity.Examples
{
    public abstract class ToiletConfigManagerBase : MonoBehaviour
    {
        public abstract float GetToiletPositionX();

        public abstract float GetToiletPositionY();
        
        public abstract float GetModelPositionX();
        
        public abstract float GetModelPositionY();

        public abstract float GetToiletSize();

        public abstract float GetModelSize();

        public abstract float GetFlushAnimTime();
        
        public abstract float GetAfterFlushTime();
        
    }
}