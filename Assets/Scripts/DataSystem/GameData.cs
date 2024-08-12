using System;
using Newtonsoft.Json;

namespace DataSystem
{
    /// <summary>
    /// 基类所有数值结构都继承自此类。不包含任何属性，仅起到数据结构的作用。
    /// </summary>
    public class GameData : Object
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}