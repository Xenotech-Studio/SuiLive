using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataSystem
{
    public class LevelResultData
    {
        // 第几天。根据此日期可以对应到当日的关卡配置
        public int LevelId; // 第一天为0
        [JsonIgnore] public int Date => LevelId + 1;
        
        // Sample Usage:
        // public int FinalCoin;
    }
}