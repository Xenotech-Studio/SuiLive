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
    public struct EntranceEffect
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        [JsonProperty("uinfo")]
        public UInfo uinfo;
        
        public EnterRoom ToEnterRoom()
        {
            // Debug.Log("fansMedal is null?: " + (fansMedal == null) + " " + fansMedal?.medal_name);
            return new EnterRoom
            {
                userName = uinfo.baseInfo.name,
                userFace = uinfo.baseInfo.face,
                fansMedalName = uinfo.medal?.name,
                fansMedalLevel = uinfo.medal?.level ?? 0,
                daysBeforeGuardExpired = uinfo.guard?.DaysBeforeExpired() ?? 0,
                guardLevel = uinfo.guard?.level ?? 0,
                wealthLevel = uinfo.wealth?.level ?? 0,
            };
        }
    }
}