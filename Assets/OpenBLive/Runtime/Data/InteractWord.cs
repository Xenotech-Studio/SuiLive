using System;
using Newtonsoft.Json;

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
        public UInfo uinfo;

        /// <summary>
        /// 0 表示 
        /// </summary>
        public int msg_type;
        
        public string NickName => uinfo.baseInfo.name;
        public string FaceUrl => uinfo.baseInfo.face;
        public bool IsEnterRoom => msg_type == 1; // 1 表示进入房间, 2 表示关注直播间
        
        public EnterRoom ToEnterRoom()
        {
            return new EnterRoom
            {
                NickName = NickName,
                FaceUrl = FaceUrl
            };
        }
    }

    [Serializable]
    public struct UInfo
    {
        [JsonProperty("base")]
        public UInfoBase baseInfo;
    }

    [Serializable]
    public struct UInfoBase
    {
        public string face;
        public string name;
    }
}