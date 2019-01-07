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
        private ConfigNode _node;
        public FillItUpConfigNode(ConfigNode node)
        {
            _node = node;
        }

        public void Save()
        {
            string fileFullPath = GetEnsuredConfigPath();

            _node.Save(fileFullPath);
        }

        #region Ignored Resource Types

        public string[] IgnoredResources
        {
            get
            {
                string stored = _node.GetValue("IgnoredResources");

                if (String.IsNullOrEmpty(stored)) return new string[0];

                return stored.Split(';');
            }
            set
            {
                _node.SetValue("IgnoredResources", String.Join(";", value), true);
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

            var internalNode = ConfigNode.Load(fileFullPath);

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
            string path = "GameData/FillItUp/PluginData/afu_settings.cfg";
            
            return path;
        }

        #endregion
    }
}
