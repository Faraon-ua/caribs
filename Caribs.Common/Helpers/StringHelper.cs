using System;

namespace Caribs.Common.Helpers
{
    public static class StringHelper
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}
