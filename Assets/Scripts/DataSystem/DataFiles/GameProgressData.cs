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

        // 玩家全部的游戏进度数据，最末尾的即表示當前正在進行的一局遊戲
        // 列表中的其他項目也許並不會被玩家看見
        public List<RoundData> Rounds = new List<RoundData>();
        
        public List<RoundData> Archives = new List<RoundData>();
        
        [JsonIgnore]
        public bool HaveUnfinishedRound => Rounds.Count > 0 && !Rounds[^1].Finished;
    };
    
    public partial class GameProgressData
    {
        public static void ArchiveRound(RoundData roundData)
        {
            Instance.Archives.Add(roundData);
            Instance.Archives[^1].ArchivedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Save();
        }
    }
}