using System.Collections.Generic;
using System.Linq;

namespace NationsLogic
{
    public static class Nations
    {
        private static IEnumerable<string> EuroNations = new[] { "Italia", "Slovacchia", "Albania", "Francia", "Liechtenstein" };

        public static bool IsEuropean(string nation)
        {
            if (EuroNations.Contains(nation))
                return true;
            return false;
        }
    }
}