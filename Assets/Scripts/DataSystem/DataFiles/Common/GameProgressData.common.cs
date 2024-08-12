using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace DataSystem
{
    public partial class GameProgressData : LoadableGameData
    {
        #region Common Implementations (You can ignore all these)
        // you can simply copy&paste these code into your newly declared classes
        // and don't forget to change all classnames below to your new class's name.
        // CLOSE THIS REGION if your task is not relevant to this.

        // !! NOTICE !!
        // Every child class of LoadableGameData should declare a public property called "Instance"
        // this is not strictly regulated by syntax to there will be no error in code editor,
        // but missing of this property will result in a runtime error. BE AWARE OF THIS.
        [JsonIgnore] private static GameProgressData _instance;

        [JsonIgnore]
        public static GameProgressData Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                else
                {
                    // 如果实例不存在，则从文件读取一个新的
                    GameProgressData newInstance = new GameProgressData();
                    
                    // 如果文件不存在，则创建一个新的文件
                    if (new GameProgressData().ValidateJsonFile()==false) CreateFile(); 
                    
                    newInstance = new GameProgressData().CreateInstanceFromJsonFile() as GameProgressData;
                     _instance = newInstance;
                    return newInstance;
                }
            }
            set => _instance = value as GameProgressData;
        }

        // !! NOTICE !!
        // This code must be overridden in child class, using "new".
        // Calling `ChileClass.Get()` will obtain a correct result which access the correct file.
        // Calling `LoadableGameData.Get()` directly will not return the same result. BE AWARE.
        public new static GameData Get(string dataKey)
        {
            return LoadableGameData.Get(new List<string>(typeof(GameDesignData).ToString().Split(".")).Last() + "." +
                                        dataKey);
        }
        #endregion
        
        protected override bool ValidateJsonFile()
        {
            bool result = true;
            result &= base.ValidateJsonFile();
            // 还可以做一些其他的验证，比如玩家有没有偷偷手动修改存档。这个不着急开发
            return result;
        }
        
        protected override void OnValidationFailed()
        {
            // 存档文件不合规的话就新创建一个就好了。
            // 不过，以后如果要验证存档文件是否被手动偷偷修改的话，可能就不是单纯新建一个了。
            CreateFile();
        }
        
        public static void Save()
        {
            GameProgressData.Instance._Save();
        }
        
        private void _Save() //将内容写入文件
        {
            string json = GameProgressData.Instance.ConvertToJson();//更新写入
            StreamWriter outStream = System.IO.File.CreateText(Application.persistentDataPath + LocalJsonPath);
            outStream.WriteLine(json);
            outStream.Close();
        }

        public static void CreateFile()
        {
            _instance = new GameProgressData();
            string json = _instance.ConvertToJson();
            StreamWriter outStream = System.IO.File.CreateText(Application.persistentDataPath + new GameProgressData().LocalJsonPath);
            outStream.WriteLine(json);
            outStream.Close();
        }
        
        #if UNITY_EDITOR
        [MenuItem("Versee/GameData/Show GameProgressData File In Explorer")]
        public static void ShowGameProgressDataFileInExplorer()
        {
            // Get the absolute path of the folder containing the file
            string filePath = Path.Combine(Application.persistentDataPath + Instance.LocalJsonPath);

            // Open the folder in the file explorer
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                filePath = filePath.Replace("/", "\\");
                
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            }
        }
        
        [MenuItem("Versee/GameData/Open GameProgressData File")]
        public static void OpenGameProgressDataFile()
        {
            EditorUtility.OpenWithDefaultApp(Application.persistentDataPath + Instance.LocalJsonPath);
        }
        #endif
    };
}