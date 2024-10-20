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
    public struct RawSendGift
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        [JsonProperty("sender_uinfo")]
        public UInfo uinfo;
        
        [JsonProperty("medal")]
        public FansMedal fansMedal;

        [JsonProperty("giftId")] 
        public long giftId;
        
        [JsonProperty("giftName")]
        public string giftName;
        
        [JsonProperty("num")]
        public int num;
        
        [JsonProperty("price")]
        public int price;
        
        public SendGift ToSendGift()
        {
            Debug.Log("fansMedal is null?: " + (fansMedal == null) + " " + fansMedal?.medal_name);
            return new SendGift()
            {
                uid = uinfo.uid,
                userName = uinfo.baseInfo.name,
                userFace = uinfo.baseInfo.face,
                fansMedalName = fansMedal?.medal_name,
                fansMedalLevel = fansMedal?.medal_level ?? 0,
                guardLevel = uinfo.guard?.level ?? 0,
                giftId = giftId,
                giftName = giftName,
                giftNum = num,
                price = price,
            };
        }
    }
}