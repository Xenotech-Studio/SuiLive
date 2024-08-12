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
    };
}