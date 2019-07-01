using System.Linq;

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
                    Log.Info("IgnoredResources.staticIgnoredResource[" + i + ": " + staticIgnoredResources[i] + "]");
                }

                return staticIgnoredResources;
            }

        }

        #endregion


        #region Static

        public static IgnoredResourcesConfigNode LoadOrCreate()
        {

            ConfigNode internalNode = GameDatabase.Instance.GetConfigNode("FillItUp/" + NODE);

            if (internalNode == null)
            {
                Log.Error("Missing ConfigNode: " + "FillItUp/" + NODE);
                staticIgnoredResources = new string[0];
            }
            else
                staticIgnoredResources = internalNode.GetValues(IGNORED);
            
            for (int i = 0; i < staticIgnoredResources.Count(); i++)
            {
                Log.Info("IgnoredResourcesConfigNode.staticIgnoredResource[" + i + ": " + staticIgnoredResources[i] + "]");
            }
            return new IgnoredResourcesConfigNode(internalNode);
        }

#endregion
    }
}
