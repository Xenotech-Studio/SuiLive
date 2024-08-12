using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class JsonPreprocessor
{
    
    public static string PreprocessImports(string json)
    {
        string r = PreprocessImports(json, out var A);

        /*
        string report = $"ImportRecord[{A.Count}]:\n";
        foreach (var importRecord in A)
        {
            report+="Imported: " + importRecord+"\n";
        }
        Debug.Log(report);
        */
        
        
        return r;
    }
    
    public static string PreprocessImports(string json, out List<string> importRecords)
    {
        importRecords = new List<string>();
        
        JToken root = JToken.Parse(json);
        JToken preprocessedRoot = ProcessImportsRecursive(root, ref importRecords);
        
        return preprocessedRoot.ToString();
    }

    private static JToken ProcessImportsRecursive(JToken node, ref List<string> importRecords, string hierarchy="")
    {
        
        if (node is JValue value)
        {
            // check whether the value is a string that match the template 
            // if so, replace the value with the imported json, as a JObject
            // if not, do nothing
            if (value.Type == JTokenType.String)
            {
                //Debug.Log(value.ToString());
                
                // 用于匹配 "@import(...)" 语句的正则表达式
                var regex = new System.Text.RegularExpressions.Regex("@import\\(([^\"]+)\\)");
                var match = regex.Match(value.ToString());
                if (match.Success)
                {
                    var path = match.Groups[1].Value;
                    
                    // finalHierarchy is  hierarchy removing the last "." if ends with "."
                    var finalHierarchy = hierarchy.EndsWith(".") ? hierarchy.Substring(0, hierarchy.Length - 1) : hierarchy;
                    var importRecord = finalHierarchy + ": " + path;
                    //Debug.Log(importRecord);
                    importRecords.Add(importRecord);
                    
                    var importedJson = LoadJsonFromFile(path);

                    if (importedJson != null)
                    {
                        // 递归处理导入的 JSON 文件
                        JToken importedJsonRoot = JToken.Parse(importedJson);
                        importedJsonRoot = ProcessImportsRecursive(importedJsonRoot, ref importRecords, hierarchy);
                                
                        // 替换掉原始 JSON 中的 "@import" 部分
                        node = importedJsonRoot;
                    }
                    // else, 引入失败，不做处理, 保持原状
                }
                // else, 不是import指令则保持原状
            }
            // else, 不是import指令则保持原状
        }


        
        else if (node is JObject)
        {
            foreach (var property in ((JObject)node).Properties())
            {
                
                property.Value = ProcessImportsRecursive(property.Value, ref importRecords, hierarchy+property.Name+".");
            }
        }
        
        
        else if (node is JArray array)
        {
            for (int i = 0; i < array.Count; i++)
            {
                var finalHierarchy = hierarchy.EndsWith(".") ? hierarchy.Substring(0, hierarchy.Length - 1) : hierarchy;
                array[i] = ProcessImportsRecursive(array[i], ref importRecords, finalHierarchy+"["+i+"].");
            }
        }

        else
        {
            Debug.Log("日了狗了");
        }

        return node;
    }

    private static string LoadJsonFromFile(string path)
    {
        try
        {
            // 假设 path 是一个有效的资源路径
            string text = null;
            if (path.Contains("Assets/Resources/"))
            {
                text = Resources.Load<TextAsset>(path.Replace(".json", "").Replace("Assets/Resources/","")).text;
            }
            else
            {
                text =  File.ReadAllText(Application.persistentDataPath +path);
            }
            
            return text;
        }
        catch
        {
            // 如果加载失败，返回 null
            return null;
        }
    }


    public static void SaveJson(string path, string json, List<string> imports)
    {
        
        //StreamWriter outStream = System.IO.File.CreateText(instance.LocalJsonPath);
        //outStream.WriteLine(json.ToString());
        //outStream.Close();

        int calcDepth(string hierarchy)
        {
            var hierarchyElements = hierarchy.Split('.');
            int depth = hierarchyElements.Length;
            foreach (var hierarchyElement in hierarchyElements)
            {
                if (hierarchyElement.Contains("[") && hierarchyElement.Contains("]")) depth++;
            }
            return depth;
        }
        
        // sort imports by depth
        imports.Sort((a, b) => calcDepth(b.Split(": ")[0]).CompareTo(calcDepth(a.Split(": ")[0])));
        
        var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);
        
        foreach(string import in imports)
        {
            
            string importPath = import.Split(": ")[1];
            string importHierarchy = import.Split(": ")[0];

            string[] hierarchyElements = importHierarchy.Split('.');
            
            //Debug.Log(importHierarchy);
            JToken node = jsonObject;
            foreach (var hierarchyElement in hierarchyElements)
            {
                if (hierarchyElement.Contains("[") && hierarchyElement.Contains("]"))
                {
                    int index = int.Parse(hierarchyElement.Split('[')[1].Split(']')[0]);
                    string key = hierarchyElement.Split('[')[0];
                    node = (node[key] as JArray)[index];
                }
                else
                {
                    node = node[hierarchyElement];
                }
            }
            
            // save content to file
            StreamWriter outStream = System.IO.File.CreateText(importPath);
            outStream.WriteLine(node.ToString());
            outStream.Close();
            
            // replace node with a string "@import(path)"
            node.Replace("@import("+importPath+")");
        }
        
        string modifiedJson = jsonObject.ToString();
        
        StreamWriter outStream2 = System.IO.File.CreateText(path);
        outStream2.WriteLine(modifiedJson);
        outStream2.Close();
    }
}