using FillItUp.Util;
using KSP.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace FillItUp
{
    public class StageRes
    {
        public const int ALLSTAGES = -99;

        public int stage;
        public string resource;
        public bool locked = false;
        public bool ignored = false;


        public StageRes(int s, string res, bool l = false, bool i = false)
        {
            stage = s;
            resource = res;
            locked = l;
            ignored = i;
        }
        public string Key
        {
            get
            {
                return stage.ToString() + ":" + resource;
            }
        }
        public static string Key2(int s, string res)
        {
            return s.ToString() + ":" + res;
        }
    }

    public class FillItUpConfigNode
    {
        const string NODE = "FILLITUP";

        const string RUNTIMELOCKED = "RuntimeLockedResources";

        StageRes sr;


        private static Dictionary<string, StageRes> runtimeLockedResources;

        private ConfigNode _node;
        public FillItUpConfigNode(ConfigNode node)
        {
            Debug.Log("FillItUpConfigNode Instantiator");
            _node = node;
        }

        public void FIU_Save()
        {
            string fileFullPath = GetEnsuredConfigPath();

            Debug.Log("FillItUpConfigNode.Save");
            if (!_node.HasValue(RUNTIMELOCKED))
            {
                Debug.Log("FillItUpConfigNode.Save, initting RuntimeIgnoredResources");
                RuntimeLockedResources = new Dictionary<string, StageRes>();
            }
            if (!HighLogic.CurrentGame.Parameters.CustomParams<FIU>().saveGlobalLocked)
            {
                _node.RemoveValue(RUNTIMELOCKED);
            }
            ConfigNode file = new ConfigNode();
            file.AddNode(NODE, _node);
            file.Save(fileFullPath);
            LockedToNode(runtimeLockedResources);
        }

#if true
#region Ignored Resource Types

        void LoadRuntimeLockedResources(bool init)
        {
            Debug.Log("FillItUpConfigNode.LoadRuntimeLockedResources");
            string stored = null;
            if (_node.HasValue(RUNTIMELOCKED))
                stored = _node.GetValue(RUNTIMELOCKED);

            if (runtimeLockedResources == null || init)
                runtimeLockedResources =  new Dictionary<string, StageRes>();
            if (stored != null && stored != "")
            {
                Debug.Log("LoadRuntimeLockedResources, stored: " + stored);
                var r = stored.Split(';').ToList();
                foreach (var r1 in r)
                {
                    sr = new StageRes(StageRes.ALLSTAGES, r1);
                    runtimeLockedResources.Add(sr.Key, sr);
                }
            }
        }
        
        void LockedToNode(Dictionary<string, StageRes> value)
        {
            if (value == null || _node == null)
                return;
            string s = "";
            foreach (var s1 in value)
            {
                Debug.Log("stage: " + s1.Value.stage + ", s1.Value.resource: " + s1.Value.resource);
                if (s1.Value.stage == StageRes.ALLSTAGES)
                {
                    if (s != "")
                        s += ";" + s1.Value.resource;
                    else
                        s = s1.Value.resource;
                }
            }
            Debug.Log("RUNTIMELOCKED: " + s);
            _node.SetValue(RUNTIMELOCKED, s, true);
            LoadRuntimeLockedResources(true);
        }

        public Dictionary<string, StageRes> RuntimeLockedResources
        {
            get
            {
                if (runtimeLockedResources == null)
                    LoadRuntimeLockedResources(true);
                return runtimeLockedResources;
            }
            set
            {
                LockedToNode(value);
#if false
                string s = "";
                foreach (var s1 in value)
                {
                    Debug.Log("stage: " + s1.Value.stage + ", s1.Value.resource: " + s1.Value.resource);
                    if (s1.Value.stage == StageRes.ALLSTAGES && HighLogic.CurrentGame.Parameters.CustomParams<FIU>().saveGlobalLocked)
                    {
                        if (s != "")
                            s += ";" + s1.Value.resource;
                        else
                            s = s1.Value.resource;
                    }
                }

                _node.SetValue(IGNORED, s, true);
                LoadRuntimeIgnoredResources();
#endif
            }
        }
       
        public void AddRuntimeLockedResource(int stage, string s)
        {
            Debug.Log("AddRuntimeLockedResource, stage: " + stage + ", res: " + s);
            if (!runtimeLockedResources.TryGetValue(StageRes.Key2(stage, s), out sr))
            {
                sr = new StageRes(stage, s);
                runtimeLockedResources.Add(sr.Key, sr);
                LockedToNode(runtimeLockedResources);
            }
        }
        public void RemoveRuntimeLockedResource(int stage, string s)
        {
            Debug.Log("RemoveRuntimeLockedResource, stage: " + stage + ", res: " + s);

            if (runtimeLockedResources.TryGetValue(StageRes.Key2(stage, s), out sr))
            {
                sr = new StageRes(stage, s);
                runtimeLockedResources.Remove(sr.Key);
                LoadRuntimeLockedResources(false);
                LockedToNode(runtimeLockedResources);
            }
        }


#endregion
#endif
                #region Window Position

        public float WindowX
        {
            get
            {
                string stored = _node.GetValue("WindowX");

                var val = new Parseable<float>(stored);

                return val.Success ? val.Value : 0;
            }
            set
            {
                _node.SetValue("WindowX", value.ToString(), true);
            }
        }

        public float WindowY
        {
            get
            {
                string stored = _node.GetValue("WindowY");

                var val = new Parseable<float>(stored);

                return val.Success ? val.Value : 0;
            }
            set
            {
                _node.SetValue("WindowY", value.ToString(), true);
            }
        }

#endregion

#region Static

        public static FillItUpConfigNode LoadOrCreate()
        {
            Debug.Log("FillItUpConfigNode");
            string fileFullPath = GetEnsuredConfigPath();

            ConfigNode file = ConfigNode.Load(fileFullPath);
            ConfigNode internalNode = null;

            if (file != null)
            {
                if (file.HasNode(NODE))
                    internalNode = file.GetNode(NODE);                    
            }

            if (internalNode == null)
            {
                internalNode = new ConfigNode();

                return new FillItUpConfigNode(internalNode)
                {
                    WindowX = 100,
                    WindowY = 100
                };
            }

            return new FillItUpConfigNode(internalNode);
        }


        private static string GetEnsuredConfigPath()
        {
            string path = "GameData/FillItUp/PluginData/FillItUp.cfg";
            
            return path;
        }

#endregion
    }
}
