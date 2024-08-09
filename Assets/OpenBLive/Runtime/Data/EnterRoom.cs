using System;
using Newtonsoft.Json;

namespace OpenBLive.Runtime.Data
{
    /// <summary>
    /// 礼物数据 https://open-live.bilibili.com/doc/2/2/1
    /// </summary>
    [Serializable]
    public struct EnterRoom
    {
        public string NickName;
        public string FaceUrl;
    }
}