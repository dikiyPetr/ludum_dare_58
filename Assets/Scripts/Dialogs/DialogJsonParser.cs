using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogs
{
    /// <summary>
    /// Парсер JSON для диалогов с поддержкой полиморфных условий
    /// </summary>
    public static class DialogJsonParser
    {
        /// <summary>
        /// Парсить JSON с диалогами
        /// </summary>
        public static List<Dialog> ParseDialogs(string json)
        {
            var dialogs = new List<Dialog>();
            
            try
            {
                var rootObj = JSONNode.Parse(json);
                var dialogsArray = rootObj["dialogs"];
                
                if (dialogsArray == null || !dialogsArray.IsArray)
                {
                    Debug.LogError("JSON не содержит массив dialogs");
                    return dialogs;
                }
                
                foreach (var dialogNode in dialogsArray.Children)
                {
                    var dialog = ParseDialog(dialogNode);
                    if (dialog != null)
                    {
                        dialogs.Add(dialog);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка парсинга JSON: {e.Message}\n{e.StackTrace}");
            }
            
            return dialogs;
        }
        
        private static Dialog ParseDialog(JSONNode node)
        {
            var dialog = new Dialog
            {
                id = node["id"],
                startNodeId = node["startNodeId"],
                speaker = node["speaker"],
                nodes = new List<DialogNode>()
            };
            
            var nodesArray = node["nodes"];
            if (nodesArray != null && nodesArray.IsArray)
            {
                foreach (var nodeJson in nodesArray.Children)
                {
                    var dialogNode = ParseDialogNode(nodeJson);
                    if (dialogNode != null)
                    {
                        dialog.nodes.Add(dialogNode);
                    }
                }
            }
            
            return dialog;
        }
        
        private static DialogNode ParseDialogNode(JSONNode node)
        {
            var dialogNode = new DialogNode
            {
                id = node["id"],
                text = node["text"],
                options = new List<DialogOption>(),
                notebookEntries = new List<NotebookEntry>(),
                highlights = new List<DialogHighlight>()
            };
            
            // Парсинг options
            var optionsArray = node["options"];
            if (optionsArray != null && optionsArray.IsArray)
            {
                foreach (var optionJson in optionsArray.Children)
                {
                    var option = ParseDialogOption(optionJson);
                    if (option != null)
                    {
                        dialogNode.options.Add(option);
                    }
                }
            }
            
            // Парсинг notebookEntries
            var notebookArray = node["notebookEntries"];
            if (notebookArray != null && notebookArray.IsArray)
            {
                foreach (var entryJson in notebookArray.Children)
                {
                    dialogNode.notebookEntries.Add(new NotebookEntry
                    {
                        clueId = entryJson["clueId"],
                        description = entryJson["description"]
                    });
                }
            }
            
            // Парсинг highlights
            var highlightsArray = node["highlights"];
            if (highlightsArray != null && highlightsArray.IsArray)
            {
                foreach (var highlightJson in highlightsArray.Children)
                {
                    var highlight = ParseDialogHighlight(highlightJson);
                    if (highlight != null)
                    {
                        dialogNode.highlights.Add(highlight);
                    }
                }
            }
            
            return dialogNode;
        }
        
        private static DialogOption ParseDialogOption(JSONNode node)
        {
            var option = new DialogOption
            {
                text = node["text"],
                nextNodeId = node["nextNodeId"].Value == "null" ? null : node["nextNodeId"],
                condition = ParseCondition(node["condition"])
            };
            
            return option;
        }
        
        private static DialogHighlight ParseDialogHighlight(JSONNode node)
        {
            var highlight = new DialogHighlight
            {
                word = node["word"],
                tooltips = new List<Tooltip>()
            };
            
            var tooltipsArray = node["tooltips"];
            if (tooltipsArray != null && tooltipsArray.IsArray)
            {
                foreach (var tooltipJson in tooltipsArray.Children)
                {
                    var tooltip = new Tooltip
                    {
                        condition = ParseCondition(tooltipJson["condition"]),
                        text = tooltipJson["text"]
                    };
                    highlight.tooltips.Add(tooltip);
                }
            }
            
            return highlight;
        }
        
        /// <summary>
        /// Парсить условие с поддержкой полиморфизма
        /// </summary>
        private static Condition ParseCondition(JSONNode node)
        {
            if (node == null || node.IsNull) return null;
            
            string type = node["type"];
            
            if (string.IsNullOrEmpty(type))
            {
                return null;
            }
            
            switch (type)
            {
                case "HasEvidence":
                    return new HasEvidence
                    {
                        id = node["id"]
                    };
                    
                case "HasConnection":
                    return new HasConnection
                    {
                        id = node["id"]
                    };
                    
                case "MultiCondition":
                    var multiCondition = new MultiCondition
                    {
                        logicType = node["logicType"] == "OR" 
                            ? MultiCondition.LogicType.OR 
                            : MultiCondition.LogicType.AND,
                        conditions = new List<Condition>()
                    };
                    
                    var conditionsArray = node["conditions"];
                    if (conditionsArray != null && conditionsArray.IsArray)
                    {
                        foreach (var conditionJson in conditionsArray.Children)
                        {
                            var condition = ParseCondition(conditionJson);
                            if (condition != null)
                            {
                                multiCondition.conditions.Add(condition);
                            }
                        }
                    }
                    
                    return multiCondition;
                    
                case "NotCondition":
                    return new NotCondition
                    {
                        condition = ParseCondition(node["condition"])
                    };

                case "SuspectAliveAndFree":
                    return new SuspectAliveAndFree
                    {
                        suspectId = node["suspectId"]
                    };

                default:
                    Debug.LogWarning($"Неизвестный тип условия: {type}");
                    return null;
            }
        }
    }
    
    #region SimpleJSON
    
    // Упрощенный JSON парсер (SimpleJSON by Bunny83)
    // Лицензия: MIT
    public abstract class JSONNode
    {
        public virtual JSONNode this[int index] { get { return null; } set { } }
        public virtual JSONNode this[string key] { get { return null; } set { } }
        public virtual string Value { get { return ""; } set { } }
        public virtual bool IsArray { get { return false; } }
        public virtual bool IsNull { get { return false; } }
        
        public virtual IEnumerable<JSONNode> Children
        {
            get { yield break; }
        }
        
        public static implicit operator string(JSONNode node)
        {
            return (node == null) ? null : node.Value;
        }
        
        public static JSONNode Parse(string json)
        {
            return JSONParser.Parse(json);
        }
    }
    
    public class JSONArray : JSONNode
    {
        private List<JSONNode> list = new List<JSONNode>();
        
        public override JSONNode this[int index]
        {
            get { return (index >= 0 && index < list.Count) ? list[index] : new JSONNull(); }
            set { if (index >= 0 && index < list.Count) list[index] = value; }
        }
        
        public override bool IsArray { get { return true; } }
        
        public override IEnumerable<JSONNode> Children
        {
            get { foreach (var node in list) yield return node; }
        }
        
        public void Add(JSONNode node)
        {
            list.Add(node);
        }
    }
    
    public class JSONObject : JSONNode
    {
        private Dictionary<string, JSONNode> dict = new Dictionary<string, JSONNode>();
        
        public override JSONNode this[string key]
        {
            get { return dict.ContainsKey(key) ? dict[key] : new JSONNull(); }
            set { dict[key] = value; }
        }
        
        public override IEnumerable<JSONNode> Children
        {
            get { foreach (var kvp in dict) yield return kvp.Value; }
        }
        
        public void Add(string key, JSONNode node)
        {
            dict[key] = node;
        }
    }
    
    public class JSONString : JSONNode
    {
        private string value;
        
        public JSONString(string val) { value = val; }
        
        public override string Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
    
    public class JSONNull : JSONNode
    {
        public override bool IsNull { get { return true; } }
        public override string Value { get { return "null"; } set { } }
    }
    
    public static class JSONParser
    {
        private static int index;
        private static string json;
        
        public static JSONNode Parse(string jsonString)
        {
            json = jsonString;
            index = 0;
            return ParseValue();
        }
        
        private static JSONNode ParseValue()
        {
            SkipWhitespace();
            if (index >= json.Length) return new JSONNull();
            
            char c = json[index];
            
            if (c == '{')
            {
                return ParseObject();
            }
            else if (c == '[')
            {
                return ParseArray();
            }
            else if (c == '"')
            {
                return ParseString();
            }
            else if (c == 'n' && json.Substring(index, 4) == "null")
            {
                index += 4;
                return new JSONNull();
            }
            else if (c == 't' && json.Substring(index, 4) == "true")
            {
                index += 4;
                return new JSONString("true");
            }
            else if (c == 'f' && json.Substring(index, 5) == "false")
            {
                index += 5;
                return new JSONString("false");
            }
            else if (char.IsDigit(c) || c == '-')
            {
                return ParseNumber();
            }
            
            return new JSONNull();
        }
        
        private static JSONObject ParseObject()
        {
            var obj = new JSONObject();
            index++; // skip '{'
            
            while (true)
            {
                SkipWhitespace();
                if (index >= json.Length) break;
                if (json[index] == '}')
                {
                    index++;
                    break;
                }
                
                // Parse key
                var key = ParseString();
                SkipWhitespace();
                
                if (index >= json.Length || json[index] != ':') break;
                index++; // skip ':'
                
                // Parse value
                var value = ParseValue();
                obj.Add(key.Value, value);
                
                SkipWhitespace();
                if (index >= json.Length) break;
                
                if (json[index] == ',')
                {
                    index++;
                }
                else if (json[index] == '}')
                {
                    index++;
                    break;
                }
            }
            
            return obj;
        }
        
        private static JSONArray ParseArray()
        {
            var array = new JSONArray();
            index++; // skip '['
            
            while (true)
            {
                SkipWhitespace();
                if (index >= json.Length) break;
                if (json[index] == ']')
                {
                    index++;
                    break;
                }
                
                var value = ParseValue();
                array.Add(value);
                
                SkipWhitespace();
                if (index >= json.Length) break;
                
                if (json[index] == ',')
                {
                    index++;
                }
                else if (json[index] == ']')
                {
                    index++;
                    break;
                }
            }
            
            return array;
        }
        
        private static JSONString ParseString()
        {
            index++; // skip opening quote
            int startIndex = index;
            
            while (index < json.Length)
            {
                if (json[index] == '"' && (index == 0 || json[index - 1] != '\\'))
                {
                    string value = json.Substring(startIndex, index - startIndex);
                    value = value.Replace("\\\"", "\"").Replace("\\\\", "\\").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t");
                    index++;
                    return new JSONString(value);
                }
                index++;
            }
            
            return new JSONString("");
        }
        
        private static JSONString ParseNumber()
        {
            int startIndex = index;
            
            if (json[index] == '-') index++;
            
            while (index < json.Length && (char.IsDigit(json[index]) || json[index] == '.'))
            {
                index++;
            }
            
            string value = json.Substring(startIndex, index - startIndex);
            return new JSONString(value);
        }
        
        private static void SkipWhitespace()
        {
            while (index < json.Length && char.IsWhiteSpace(json[index]))
            {
                index++;
            }
        }
    }
    
    #endregion
}

