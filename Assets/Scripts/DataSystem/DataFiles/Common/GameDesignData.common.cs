using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DataSystem
{
    public partial class GameDesignData : LoadableGameData
    {
        #region Common Implementations (You can ignore all these)
        // you can simply copy&paste these code into your newly declared classes
        // and don't forget to change all classnames below to your new class's name.
        // CLOSE THIS REGION if your task is not relevant to this.

        // !! NOTICE !!
        // Every child class of LoadableGameData should declare a public property called "Instance"
        // this is not strictly regulated by syntax to there will be no error in code editor,
        // but missing of this property will result in a runtime error. BE AWARE OF THIS.
        [JsonIgnore]
        private static GameDesignData _instance;
        
        [JsonIgnore]
        public static GameDesignData Instance { 
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                else
                {
                    // 如果实例不存在，则从文件读取一个新的
                    GameDesignData newInstance = new GameDesignData();
                    
                    // 如果文件不存在，则创建一个新的文件
                    if (new GameDesignData().ValidateJsonFile()==false) new GameDesignData().OnValidationFailed(); 
                    
                    newInstance = new GameDesignData().CreateInstanceFromJsonFile() as GameDesignData;
                    _instance = newInstance;
                    return newInstance;
                }
            }
            set => _instance = value as GameDesignData;  
        }

        // !! NOTICE !!
        // This code must be overridden in child class, using "new".
        // Calling `ChileClass.Get()` will obtain a correct result which access the correct file.
        // Calling `LoadableGameData.Get()` directly will not return the same result. BE AWARE.
        public new static GameData Get(string dataKey)
        {
            return LoadableGameData.Get(new List<string>(typeof(GameDesignData).ToString().Split(".")).Last() + "." + dataKey);
        }

        

        #endregion

        public static void Reload()
        {
            Instance = new GameDesignData().CreateInstanceFromJsonFile() as GameDesignData;
            
            //Debug.Log(Instance.Dialogues.Count);
        }
        
        protected override bool ValidateJsonFile()
        {
            bool result = true;
            result &= base.ValidateJsonFile();
            
            // 验证JSON格式正确可读

            result &= ValidateJsonFormat();
            
            // TODO 联网验证设计版本是否为最新。

            return result;
        }


        public static bool ValidateJsonFormat() => ValidateJsonFormat(out var imports);
        public static bool ValidateJsonFormat(out List<string> imports)
        {
            bool jsonFormatValid = true;
            try
            {
                var json = File.ReadAllText(GameDesignData.LocalJsonPathValue);
                json = JsonPreprocessor.PreprocessImports(json, out imports);
                
                //Debug.Log(json);
                
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                JsonConvert.DeserializeObject<GameDesignData>(json, settings);
            }
            catch (Exception e)
            {
                jsonFormatValid = false;
                Debug.LogError("JSON format invalid: " + e.Message);
                imports = new List<string>();
            }
            return jsonFormatValid;
        }
        
        #if UNITY_EDITOR
        [MenuItem("Versee/GameData/Validate GameDesignData JSON Format")]
        [MenuItem("Versee/Validate GameDesignData JSON Format &v")]
        public static void ValidateJsonFormatMenuItem()
        {
            if (ValidateJsonFormat(out var imports))
            {
                // popup
                EditorUtility.DisplayDialog("OK!", "JSON format is valid.  OK !", "OK");
            }
            else 
            {
                // popup
                EditorUtility.DisplayDialog("ERROR!", "JSON format is invalid.  ERROR !", "OK");
            }

            var instance = new GameDesignData().CreateInstanceFromJsonFile() as GameDesignData;
            string json = instance.ConvertToJson();
            
            //StreamWriter outStream = System.IO.File.CreateText(instance.LocalJsonPath);
            //outStream.WriteLine(json.ToString());
            //outStream.Close();
            JsonPreprocessor.SaveJson(instance.LocalJsonPath, json, imports);
        }
        #endif
        
        
        public static void SaveBackToFiles()
        {
            var _ = File.ReadAllText(GameDesignData.LocalJsonPathValue);
            _ = JsonPreprocessor.PreprocessImports(_, out List<string> imports);
            
            string json = Instance.ConvertToJson();
            JsonPreprocessor.SaveJson(Instance.LocalJsonPath, json, imports);
        }
        
        protected override void OnValidationFailed()
        {
            // TODO 如果发现版本不是最新，则从服务器下载最新版本。
            #if UNITY_EDITOR
            Init(); // 单纯创建一个空设计文件。以后不这样做。
            #endif
        }
        
        #region Tests

        #if UNITY_EDITOR
        [MenuItem("Versee/GameData/Init GameDesignData")]
        public static void Init()
        {
            _instance = new GameDesignData();
            string json = _instance.ConvertToJson();
            
            // if file exists, ask for overwrite
            if (System.IO.File.Exists(_instance.LocalJsonPath))
            {
                
                if (!EditorUtility.DisplayDialog("Warning", "File already exists, overwrite?", "Yes", "No (Suggested)"))
                {
                    Debug.Log("init canceled");
                    return;
                }
            }
            
            StreamWriter outStream = System.IO.File.CreateText(_instance.LocalJsonPath);
            outStream.WriteLine(json.ToString());
            outStream.Close();
            
            Debug.Log("init success");
        }
        #endif
        
        #if UNITY_EDITOR
        [MenuItem("Versee/GameData/Load GameDesignData")]
        #endif
        public static void MenuItemLoad()
        {
            Instance.CreateInstanceFromJsonFile();
            Debug.Log("load success, version code = " + _instance.Version);
        }

        #endregion
    };
}