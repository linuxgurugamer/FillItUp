using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyFuelUtility
{
    public class FuelTypes
    {
        public const string SolidFuel = "SolidFuel";
        public const string LiquidFuel = "LiquidFuel";
        public const string Oxidizer = "Oxidizer";
        public const string Monopropellant = "MonoPropellant";

        public static string[] All = new string[] { LiquidFuel, Oxidizer, SolidFuel, Monopropellant };
    }
}
