using System;
using System.Data.SqlTypes;
using Newtonsoft.Json;
using UnityEngine;

namespace OpenBLive.Runtime.Data
{
    /// <summary>
    /// 礼物数据 https://open-live.bilibili.com/doc/2/2/1
    /// </summary>
    [Serializable]
    public struct InteractWord
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        [JsonProperty("uinfo")]
        public UInfo uinfo;
        
        [JsonProperty("fans_medal")]
        public FansMedal fansMedal;

        /// <summary>
        /// 0 表示 
        /// </summary>
        public int msg_type;
        
        public bool IsEnterRoom => msg_type == 1; // 1 表示进入房间, 2 表示关注直播间
        
        public EnterRoom ToEnterRoom()
        {
            Debug.Log("fansMedal is null?: " + (fansMedal == null) + " " + fansMedal?.medal_name);
            return new EnterRoom
            {
                userName = uinfo.baseInfo.name,
                userFace = uinfo.baseInfo.face,
                fansMedalName = fansMedal?.medal_name,
                fansMedalLevel = fansMedal?.medal_level ?? 0,
                daysBeforeGuardExpired = uinfo.guard?.DaysBeforeExpired() ?? 0,
                guardLevel = uinfo.guard?.level ?? 0,
                wealthLevel = uinfo.wealth?.level ?? 0,
            };
        }
    }

    [Serializable]
    public struct UInfo
    {
        [JsonProperty("base")]
        public UInfoBase baseInfo;
        
        [JsonProperty("guard")]
        public UInfoGuard guard;
        
        [JsonProperty("wealth")]
        public UInfoWealth wealth;
        
        [JsonProperty("medal")]
        public UInfoMedal medal;
    }

    [Serializable]
    public class UInfoBase
    {
        public string face;
        public string name;
    }
    
    [Serializable]
    public class FansMedal
    {
        public long anchor_roomid;
        public int guard_level;
        public bool is_lighted;
        public long medal_color;
        public long medal_color_border;
        public long medal_color_end;
        public long medal_color_start;
        public int medal_level;
        public string medal_name;
        public long score;
    }
    
    [Serializable]
    public class UInfoGuard
    {
        public string expired_str; //"2024-09-02 23:59:59"
        public int level;
        
        public int DaysBeforeExpired()
        {
            if (DateTime.TryParse(expired_str, out var expired))
            {
                return (int) (expired - DateTime.Now).TotalDays;
            }
            else
            {
                return 0;
            }
        }
    }
    
    [Serializable]
    public class UInfoWealth
    {
        public int level;
    }
    
    [Serializable]
    public class UInfoMedal
    {
        public int level;
        public string name;
    }
}