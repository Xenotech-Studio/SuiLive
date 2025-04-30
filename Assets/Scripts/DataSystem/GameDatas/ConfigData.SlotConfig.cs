using System.Collections.Generic;

namespace DataSystem
{
    public class SlotConfigData : GameData
    {
        public bool SlotEnabled;
        public List<float> Weights = new List<float>();
    }
}