using AssemblyFuelUtility.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyFuelUtility
{
    public class FuelTypes
    {
        public static string[] Discover(ShipConstruct ship)
        {
            var config = AssemblyFuelConfigNode.LoadOrCreate();

            var resources = ship.Parts.SelectMany(p => p.Resources.list.Select(r => r.resourceName));

            return resources.Distinct().Where(r => !config.IgnoredResources.Contains(r)).ToArray();
        }
    }
}
