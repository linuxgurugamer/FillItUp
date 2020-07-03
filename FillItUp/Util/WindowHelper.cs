using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FillItUp.Util
{
    public class WindowHelper
    {
        static System.Random random;
        static int ransomSeed = 0;
        private static Dictionary<string, int> _windowDictionary;
        public static int NextWindowId(string windowKey)
        {
            if (_windowDictionary == null)
            {
                int seed = (int)System.DateTime.Now.Ticks;
                random = new System.Random(seed);
                ransomSeed = random.Next();
                _windowDictionary = new Dictionary<string, int>();
            }

            if (_windowDictionary.ContainsKey(windowKey))
            {
                return _windowDictionary[windowKey];
            }

            var newId = ransomSeed + _windowDictionary.Count() + 1;

            _windowDictionary.Add(windowKey, newId);

            return newId;
        }
    }
}
