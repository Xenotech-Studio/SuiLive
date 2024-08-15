using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

namespace DataSystem
{
    public partial class GameProgressData : LoadableGameData
    {
        [JsonIgnore]
        public override string LocalJsonPath { get => "/GameProgress.json"; }

        
        public ConfigData Config = new ConfigData();
        
        public Dictionary<long, Dictionary<long, int>> ContinueAttendance = new Dictionary<long, Dictionary<long, int>>();
        public Dictionary<long, Dictionary<long, string>> LatestAttendanceDate = new Dictionary<long, Dictionary<long, string>>();
    };

    public partial class GameProgressData
    {
        public static Dictionary<long, int> GetContinueAttendance(long roomId)
        {
            if (!Instance.ContinueAttendance.ContainsKey(roomId))
            {
                Instance.ContinueAttendance[roomId] = new Dictionary<long, int>();
            }
            return Instance.ContinueAttendance[roomId];
        }
        
        public static Dictionary<long, string> GetLatestAttendanceDate(long roomId)
        {
            if (!Instance.LatestAttendanceDate.ContainsKey(roomId))
            {
                Instance.LatestAttendanceDate[roomId] = new Dictionary<long, string>();
            }
            return Instance.LatestAttendanceDate[roomId];
        }
    }
}