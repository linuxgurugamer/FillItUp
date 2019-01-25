using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FillItUp
{
    public class FuelTypes
    {
        //static internal FillItUpConfigNode config = null;
        static internal IgnoredResourcesConfigNode resources = null;

        public class StageResDef
        {
            public List<Part> parts;
            internal  List<Tuple<string, string>> resources;
            public bool stageExpanded;

            public StageResDef()
            {
                parts = new List<Part>();
                resources = new List<Tuple<string, string>>();
                stageExpanded = false;
            }
        }

        public static void Discover(ShipConstruct ship, ref StageResDef allResources, ref SortedDictionary<int, StageResDef> allPartsResourcesByStage,
             out SortedDictionary<int, StageResDef>allPartsResourcesShip)
        {
            if (FillItUp.Instance._config == null)
                FillItUp.Instance._config = FillItUpConfigNode.LoadOrCreate();
            if (resources == null)
                resources = IgnoredResourcesConfigNode.LoadOrCreate();

            int maxStage = -1;
            SortedDictionary<int, StageResDef> oldPartResByStage = allPartsResourcesByStage;
            allPartsResourcesByStage = new SortedDictionary<int, StageResDef>();
            allPartsResourcesShip = new SortedDictionary<int, StageResDef>();
            StageResDef srd;

            // Get all parts in ship into individual lists
            // Also get max stages
            for (int i = ship.Parts.Count - 1; i >= 0; i--)
            {
                var partValidResources = ship.Parts[i].Resources.Distinct().Where(r => !resources.IgnoredResources.Contains(r.resourceName)).ToList();
                if (partValidResources.Count > 0)
                {

                    if (!allPartsResourcesByStage.ContainsKey(ship.Parts[i].inverseStage))
                    {
                        srd = new StageResDef();
                        if (oldPartResByStage != null & oldPartResByStage.ContainsKey(ship.Parts[i].inverseStage))
                            srd.stageExpanded = oldPartResByStage[ship.Parts[i].inverseStage].stageExpanded;
                        allPartsResourcesByStage.Add(ship.Parts[i].inverseStage, srd);
                    }

                    if (!allPartsResourcesShip.ContainsKey(StageRes.ALLSTAGES))
                    {
                        srd = new StageResDef();
                        allPartsResourcesShip.Add(StageRes.ALLSTAGES, srd);
                    }


                    srd = allPartsResourcesByStage[ship.Parts[i].inverseStage];
                    srd.parts.Add(ship.Parts[i]);
                    foreach (var r in partValidResources)
                    {
                        Tuple<string, string> key = new Tuple<string, string>(r.info.displayName, r.resourceName);
                        if (!srd.resources.Contains(key))
                            srd.resources.Add(key);
                    }
                    maxStage = Math.Max(maxStage, ship.Parts[i].inverseStage);

                }
            }


            allResources = new StageResDef();

            var r1 = ship.Parts.SelectMany(p => p.Resources.Select(r => r)).ToList();
            foreach (var r2 in r1)
            {
                var key = new Tuple<string, string>(r2.info.displayName, r2.resourceName);
                if (!resources.IgnoredResources.Contains(r2.resourceName) && !allResources.resources.Contains(key))
                {
                    allResources.resources.Add(key);
                }
            }

            allResources.parts = ship.Parts;


            //allPartsResourcesByStage = stages;

        }
    }
}
