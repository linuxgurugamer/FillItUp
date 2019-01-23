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
    public class FillItUpConfigNode
    {
        const string NODE = "FILLITUP";
        const string IGNORED = "IgnoredResources";
        private ConfigNode _node;
        public FillItUpConfigNode(ConfigNode node)
        {
            _node = node;
        }

        public void Save()
        {
            string fileFullPath = GetEnsuredConfigPath();
            if (!_node.HasValue(IGNORED))
                IgnoredResources = new string[]{ ""};
            ConfigNode file = new ConfigNode();
            file.AddNode(NODE, _node);
            file.Save(fileFullPath);
        }

        #region Ignored Resource Types

        public string[] IgnoredResources
        {
            get
            {
                string stored = _node.GetValue(IGNORED);

                if (String.IsNullOrEmpty(stored)) return new string[0];

                return stored.Split(';');
            }
            set
            {
                _node.SetValue(IGNORED, String.Join(";", value), true);
            }
        }

        #endregion

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
