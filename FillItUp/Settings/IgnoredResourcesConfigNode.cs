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
            ConfigNode internalNode = new ConfigNode();

            //We need the ConfigNode after MM could do some magic
            ConfigNode[] config = GameDatabase.Instance.GetConfigNodes(NODE);

            if (config.Length == 0)
            {
                Debug.Log($"ConfigNode {NODE} does not exist");
            }
            else
            {                
                internalNode = config[0];
            }

            if (!internalNode.HasValue(IGNORED))
            {
                Debug.Log("_node does not have value: " + IGNORED);
                staticIgnoredResources = new string[0];
            }
            else
            staticIgnoredResources = internalNode.GetValues(IGNORED);


            return new IgnoredResourcesConfigNode(internalNode);
        }

        //AFAIK, it is not necessayry anymore
        //private static string GetEnsuredConfigPath()
        //{
        //    string path = "GameData/FillItUp/IgnoredResources.cfg";

        //    return path;
        //}

#endregion
    }
}
