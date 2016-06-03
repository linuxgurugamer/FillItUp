using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyFuelUtility
{
    public enum FuelType
    {
        LiquidFuel = 0,
        Oxidizer = 1,
        SolidFuel = 2,
        MonoPropellant = 3,
        XenonGas = 4,
        Unknown = 5
    }

    public class FuelTypes
    {
        public static FuelType FromString(string type)
        {
            try
            {
                return (FuelType)Enum.Parse(typeof(FuelType), type);
            }
            catch
            {
                return FuelType.Unknown;
            }
        }

        public static string[] AllNames()
        {
            return Enum.GetNames(typeof(FuelType));
        }

        public static FuelType[] All()
        {
            return (FuelType[])Enum.GetValues(typeof(FuelType));
        }
    }
}
