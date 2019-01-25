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
    public class IgnoredResourcesConfigNode
    {
        const string NODE = "FILLITUP";
        const string IGNORED = "IgnoredResource";
        private ConfigNode _node;

        private static string[] staticIgnoredResources;

        public IgnoredResourcesConfigNode(ConfigNode node)
        {
            _node = node;
        }


        #region Ignored Resource Types

        public string[] IgnoredResources
        {
            get
            {
                for (int i = 0; i < staticIgnoredResources.Count(); i++)
                {
                    Debug.Log("IgnoredResources.staticIgnoredResource[" + i + ": " + staticIgnoredResources[i] + "]");
                }

                return staticIgnoredResources;
            }

        }

#if false
        public void Save()
        {
            string fileFullPath = GetEnsuredConfigPath();

           

            ConfigNode file = new ConfigNode();
            file.AddNode(NODE, _node);
            file.Save(fileFullPath);
        }
#endif

        #endregion


        #region Static

        public static IgnoredResourcesConfigNode LoadOrCreate()
        {
#if false
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

            }
#endif
            ConfigNode internalNode = GameDatabase.Instance.GetConfigNode("FillItUp/" + NODE);

            if (internalNode == null)
            {
                Debug.Log("Missing ConfigNode: " + "FillItUp/" + NODE);
                staticIgnoredResources = new string[0];
            }
            else
                staticIgnoredResources = internalNode.GetValues(IGNORED);

            Debug.Log("staticIgnoredResources.Count: " + staticIgnoredResources.Count());
            for (int i = 0; i < staticIgnoredResources.Count(); i++)
            {
                Debug.Log("IgnoredResourcesConfigNode.staticIgnoredResource[" + i + ": " + staticIgnoredResources[i] + "]");
            }
            return new IgnoredResourcesConfigNode(internalNode);
        }

#if false
        private static string GetEnsuredConfigPath()
        {
            string path = "GameData/FillItUp/IgnoredResources.cfg";

            return path;
        }
#endif
#endregion
    }
}
