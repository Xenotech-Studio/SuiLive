using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataSystem
{
    public class RoundData
    {
        public List<LevelResultData> Levels = new List<LevelResultData>();
        
        public int Ending = -1; // 触发了怎样的结局
        
        public string ArchivedTime = "";
        
        [JsonIgnore]
        public bool Finished => Ending != -1;
    }
    
    public enum Ending
    {
        NotEnded = -1,
        
        // Sample Usage
        // Died = 0,
        // Win = 1,
    }
}