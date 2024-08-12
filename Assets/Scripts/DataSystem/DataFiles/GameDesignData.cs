using System;
using System.Collections.Generic;
using System.IO;
using Game;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Versee.Scripts.Utils;

namespace DataSystem
{
    public partial class GameDesignData : LoadableGameData
    {
        [JsonIgnore]
        public static string LocalJsonPathValue = "Assets/Resources/GameDesign.json";
        
        [JsonIgnore]
        public override string LocalJsonPath { get=>LocalJsonPathValue; }
        
        
        
    }
};