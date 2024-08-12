using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DataSystem
{
    // 是一个abstract class，代表”这个GameData将会对应一个可加载的数据表（一个JSON文件）“。
    // 所有与游戏配表对应的东西都继承此类。包含 static Get() 函数（但因为自己是abstract所以只有到了子类里才可以使用），
    // 可以返回一个GameData（详细的Get故规则见下文），供 IDataLoader 的 LoadData做后续处理。
    // 包含LoadFromJson函数用于从JSON文件加载数据，也包含LoadFromOnlineJson 函数用于从网上获取JSON然后再LoadFromJson。
    // 将会根据自己的Field结构来读取Json，也会根据自己的Field来响应Get函数。
    public abstract class LoadableGameData : GameData
    {
        // Format: mode-dot-counting,
        // mode code 0 for develop, 1 for product,
        // counting number should simply grow, starting from 1
        // save version will be reviewed as same content.
        [JsonProperty(Order = -2)] // Appear first
        public string Version = "0.1";

        public abstract string LocalJsonPath { get; }

        
        #region Json Tool Functions
        
        public virtual string ConvertToJson()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.SerializeObject(GetInstance(this.GetType()), Formatting.Indented, settings);
        }

        // this function is not static because it needs to know its derived type
        public virtual LoadableGameData ConvertFromJson(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.DeserializeObject(json, this.GetType(), settings) as LoadableGameData;
        }

        // 测试磁盘中的文件是否可以读取。
        // 此处基类只进行“文件是否存在”的检测。还可以检测更多信息比如联网校验看看文件是否是最新版
        // 在子类中 Override 此函数以执行更多的检测。
        protected virtual bool ValidateJsonFile()
        {
            bool fileExists = false;
            if (this.LocalJsonPath.Contains("Assets/Resources/"))
            {
                fileExists = Resources.Load<TextAsset>(this.LocalJsonPath.Replace(".json", "").Replace("Assets/Resources/", "")) != null;
            }
            else
            {
                fileExists =File.Exists(Application.persistentDataPath + this.LocalJsonPath);
            }
            
            return fileExists;
        }

        public virtual LoadableGameData CreateInstanceFromJsonFile()
        {
            string json="";
            if (this.LocalJsonPath.Contains("Assets/Resources/"))
            {
                json = Resources.Load<TextAsset>(this.LocalJsonPath.Replace(".json", "").Replace("Assets/Resources/","")).text;
            }
            else
            {
                json =  File.ReadAllText(Application.persistentDataPath +this.LocalJsonPath);
            }

            json = JsonPreprocessor.PreprocessImports(json);
            
            var instance = ConvertFromJson(json);
            SetInstance( instance, this.GetType() );
            return instance; 
            // The `Instance` property is auto updated, so this return value is redundant.
            // You can either take this for further usage, or just ignore it.
        }

        #endregion
        
        #region Querying Tool Functions

        public static GameData Get(string dataKey) => Get<GameData>(dataKey);
        
        public static T Get<T>(string dataKey) where T : class
        {
            int depth = 0;
            T result = null;
            string domain = "";
            
            foreach (string query in dataKey.Split("."))
            {
                // find "xxx[x]" pattern
                string[] split = query.Split("[");
                string key = split[0];
                
                // get LoadableGameData.Instance's field that has a name of key
                if (depth == 0)
                {
                    result = GetInstance(key) as T;
                    domain = key;
                }
                else
                {
                    // field or property
                    var f = result.GetType().GetField(key);
                    var p = result.GetType().GetProperty(key);
                    
                    Action onKeyNotFound = () => Debug.LogError( $"[Error] {domain}.Get(), invalid key,\n Requested key \"{key}\" is not found for query \"{dataKey}\".");
                    Action<int> onIndexOOR = (int index) => Debug.LogError( $"[Error] {domain}.Get(), index out of range,\n Requested index \"{index}\" in {key} out of range for query \"{dataKey}\".");
                    Action<string> onDicKeyNotFound = (string dickey) => Debug.LogError( $"[Error] {domain}.Get(), key not found in dictionary,\n Requested key \"{dickey}\" in {key} not found for query \"{dataKey}\".");
                    Action onListNotInit = () => Debug.LogError( $"[Error] {domain}.Get(), list not initialized,\n List \"{key}\" is not initialized for \"{dataKey}\".");
                    Action onNotIndexable = () => Debug.LogError( $"[Error] {domain}.Get(), requested field can not be indexed,\n Field \"{key}\" can not be indexed in query \"{dataKey}\".");
                    if(f==null && p==null) { onKeyNotFound?.Invoke(); return null; }
                    
                    var v = f!=null? f.GetValue(result) as T : p.GetValue(result) as T;
                    
                    if (split.Length == 1)
                    {
                        // this is a normal field query
                        result = v;
                    }
                    else
                    {
                        // this is an array indexing query, ( target field is a List )
                        object collection = v;
                        
                        // there may be list-in-list, means multiple "[x]" in the query
                        for (int i = 1; i < split.Length; i++)
                        {
                            // Uninitialized check
                            if (collection == null)
                            {
                                onListNotInit?.Invoke();
                                return null;
                            }

                            // Handle List type
                            if (collection is IList)
                            {
                                int index = int.Parse(split[i].Replace("]", ""));

                                // get length perperty
                                var pCount = collection.GetType().GetProperty("Count");
                                if (pCount == null) pCount = collection.GetType().GetProperty("Length");
                                if (pCount == null)
                                {
                                    onNotIndexable?.Invoke();
                                    return null;
                                }

                                // Index test
                                object countObj = pCount.GetValue(collection);
                                int count = countObj is int ? (int)countObj : 0;
                                if (index >= count)
                                {
                                    onIndexOOR?.Invoke(index);
                                    return null;
                                }

                                // Value Get
                                String indexerName = ((DefaultMemberAttribute)(collection.GetType()
                                    .GetCustomAttributes(typeof(DefaultMemberAttribute), true)[0])).MemberName;
                                PropertyInfo pi = collection.GetType().GetProperty(indexerName);
                                object value = pi.GetValue(collection, new object[] { index });
                                
                                collection = value;
                            }
                            
                            // Handle Dictionary type
                            else if (collection is IDictionary)
                            {
                                string dickey = split[1].Replace("]", "");

                                // Key test
                                if (!((IDictionary)collection).Contains(dickey))
                                {
                                    onDicKeyNotFound.Invoke(dickey);
                                    return null;
                                }

                                // Value Get
                                object value = ((IDictionary)collection)[dickey];
                                
                                collection = value;
                            }
                            else
                            {
                                onNotIndexable?.Invoke();
                                return null;
                            }
                        }
                        
                        result = collection as T;
                    }
                }
                depth++;
            }
            return result;
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        // Set
        public static void Set<T>(string dataKey, T newValue)
        {
            int depth = 0;
            System.Object pointer = null;
            string domain = "";
            int maxDepth = dataKey.Split(".").Length;
            
            foreach (string query in dataKey.Split("."))
            {
                // find "xxx[x]" pattern
                string[] split = query.Split("[");
                string key = split[0];
                
                // get LoadableGameData.Instance's field that has a name of key
                if (depth == 0)
                {
                    pointer = GetInstance(key) as System.Object;
                    domain = key;
                }
                else
                {
                    // field or property
                    var f = pointer.GetType().GetField(key);
                    var p = pointer.GetType().GetProperty(key);
                    
                    Action onKeyNotFound = () => Debug.LogError( $"[Error] {domain}.Get(), invalid key,\n Requested key \"{key}\" is not found for query \"{dataKey}\".");
                    Action<int> onIndexOOR = (int index) => Debug.LogError( $"[Error] {domain}.Get(), index out of range,\n Requested index \"{index}\" in {key} out of range for query \"{dataKey}\".");
                    Action<string> onDicKeyNotFound = (string dickey) => Debug.LogError( $"[Error] {domain}.Get(), key not found in dictionary,\n Requested key \"{dickey}\" in {key} not found for query \"{dataKey}\".");
                    Action onListNotInit = () => Debug.LogError( $"[Error] {domain}.Get(), list not initialized,\n List \"{key}\" is not initialized for \"{dataKey}\".");
                    Action onNotIndexable = () => Debug.LogError( $"[Error] {domain}.Get(), requested field can not be indexed,\n Field \"{key}\" can not be indexed in query \"{dataKey}\".");
                    if(f==null && p==null) { onKeyNotFound?.Invoke(); return; }
                    
                    var v = f!=null? f.GetValue(pointer) : p.GetValue(pointer);
                    
                    if (split.Length == 1)
                    {
                        // this is a normal field query
                        
                        if (depth == maxDepth - 1)
                        {
                            if (f != null)
                            {
                                Debug.Log(f.GetValue(pointer).GetType());
                                f.SetValue(pointer, newValue);
                            }
                            else p.SetValue(pointer, newValue);
                        }
                    }
                    else
                    {
                        // this is an array indexing query, ( target field is a List )
                        object collection = v;
                        
                        // there may be list-in-list, means multiple "[x]" in the query
                        for (int i = 1; i < split.Length; i++)
                        {
                            // Uninitialized check
                            if (collection == null)
                            {
                                onListNotInit?.Invoke();
                                return;
                            }

                            // Handle List type
                            if (collection is IList)
                            {
                                int index = int.Parse(split[i].Replace("]", ""));

                                // get length perperty
                                var pCount = collection.GetType().GetProperty("Count");
                                if (pCount == null) pCount = collection.GetType().GetProperty("Length");
                                if (pCount == null)
                                {
                                    onNotIndexable?.Invoke();
                                    return;
                                }

                                // Index test
                                object countObj = pCount.GetValue(collection);
                                int count = countObj is int ? (int)countObj : 0;
                                if (index >= count)
                                {
                                    onIndexOOR?.Invoke(index);
                                    return;
                                }

                                // Value Get
                                String indexerName = ((DefaultMemberAttribute)(collection.GetType()
                                    .GetCustomAttributes(typeof(DefaultMemberAttribute), true)[0])).MemberName;
                                PropertyInfo pi = collection.GetType().GetProperty(indexerName);
                                object value = pi.GetValue(collection, new object[] { index });
                                
                                collection = value;
                            }
                            
                            // Handle Dictionary type
                            else if (collection is IDictionary)
                            {
                                string dickey = split[1].Replace("]", "");

                                // Key test
                                if (!((IDictionary)collection).Contains(dickey))
                                {
                                    onDicKeyNotFound.Invoke(dickey);
                                    return;
                                }

                                // Value Get
                                object value = ((IDictionary)collection)[dickey];
                                
                                // if this is the last depth, set the value
                                if (i == split.Length - 1)
                                {
                                    ((IDictionary)collection)[dickey] = newValue;
                                    return;
                                }
                                
                                collection = value;
                            }
                            else
                            {
                                onNotIndexable?.Invoke();
                                return;
                            }
                        }
                        
                        pointer = collection as System.Object;
                        
                    }
                }
                depth++;
            }
        }
        
        
        
        
        #endregion
        
        #region Child Class Instance Tool Functions
        
        public static LoadableGameData GetInstance(Type type)
        // For example: GetInstance(typeof(GameDesignData))
        {
            var prop = type.GetProperty("Instance");
            if( prop == null ) { Debug.LogError("Child class didn't define `Instance` property"); return null; }
            
            return prop.GetValue(null) as LoadableGameData;
        }

        public static LoadableGameData GetInstance(string c) => GetInstance(Type.GetType("DataSystem." + c));
        // For example: GetInstance("GameDesignData");

        public static void SetInstance(LoadableGameData newInstance)
        {
            Type type = newInstance.GetType();
            SetInstance(newInstance, type);
        }
        
        public static void SetInstance(LoadableGameData newInstance, Type type)
        {
            var prop = type.GetProperty("Instance");
            if( prop == null ) { Debug.LogError("Child class didn't define `Instance` property"); return; }
            prop.SetValue(null, newInstance);
        } 
        
        #if UNITY_EDITOR
        [MenuItem("Versee/GameData/Try Get Instance of `GameDesignData`")]
        #endif
        public static void GetInstanceTest()
        {
            LoadableGameData instance = GetInstance("GameDesignData");
            Debug.Log(instance);
        }

        protected abstract void OnValidationFailed();
        
        #endregion
        
        #region Version Code Tool Functions

        // If versionCodeA is newer than versionCodeB, return "Newer"
        public static VersionComparingResult CompareVersion(string versionCodeA, string versionCodeB)
        {
            // split (by ".") and convert version code strings into List of ints
            List<int> versionA = new List<int>();
            List<int> versionB = new List<int>();
            foreach (string s in versionCodeA.Split('.')) versionA.Add(Convert.ToInt32(s));
            foreach (string s in versionCodeB.Split('.')) versionB.Add(Convert.ToInt32(s));

            if (versionA[0] != versionB[0]) return VersionComparingResult.ModeNotMatch;
            
            for (int i = 0; i < Math.Min(versionA.Count, versionB.Count); i++)
            {
                if (versionA[i] > versionB[i]) return VersionComparingResult.Newer;
                if (versionA[i] < versionB[i]) return VersionComparingResult.Older;
            }
            return VersionComparingResult.Same;
        }

        public static bool IsNewerVersion(string versionCode, LoadableGameData current, Action onModeNotMatch)
        {
            VersionComparingResult comp =  CompareVersion(versionCode, current.Version);
            if (comp == VersionComparingResult.ModeNotMatch)
            {
                onModeNotMatch?.Invoke();
                return false;
            }
            return comp == VersionComparingResult.Newer;
            
        }
        
        public static bool IsNewerVersion(string versionCode, LoadableGameData current)
        {
            return IsNewerVersion(versionCode, current, ()=>Debug.LogError("You are trying to compare"));
        }
        
        public bool IsNewerVersion(string versionCode, Action onModeNotMatch) => IsNewerVersion(versionCode, this);
        
        public bool IsNewerVersion(string versionCode) => IsNewerVersion(versionCode, this);
        
        #endregion
        
        #region Tests
        
        #if UNITY_EDITOR
        [MenuItem("Versee/GameData/Query Tests/Test Get Monster[0]'s data (should show error)")]
        public static void GetDataTest1()
        {
            Debug.Log(GameDesignData.Get("Monster[0]"));
        }
        
        [MenuItem("Versee/GameData/Query Tests/Test Get Monsters[10086]'s data (should show error)")]
        public static void GetDataTest2()
        {
            Debug.Log(GameDesignData.Get("Monsters[10086]"));
        }
        
        [MenuItem("Versee/GameData/Query Tests/Test Get Monsters[0]'s data")]
        public static void GetDataTest3()
        {
            Debug.Log(GameDesignData.Get("Monsters[0]"));
        }
        #endif
        
        #endregion
    }
    
    public enum VersionComparingResult
    {
        Newer,
        Same,
        Older,
        ModeNotMatch,
    }
    
    

}