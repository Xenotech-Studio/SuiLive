using System;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace OpenBLive.Runtime.Data
{
    /// <summary>
    /// 礼物数据 https://open-live.bilibili.com/doc/2/2/1
    /// </summary>
    [Serializable]
    public struct EnterRoom
    {
        public string userName;
        public string userFace;
        public string fansMedalName;
        public int fansMedalLevel;
        public int guardLevel;
        public int daysBeforeGuardExpired;
        public int wealthLevel;
    }
}