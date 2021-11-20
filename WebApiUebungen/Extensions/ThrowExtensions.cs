using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ThrowExtensions
    {
        public static void ThrowIfNull(this object obj, string paramName)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(paramName));
        }

        public static void ThrowIfNullOrEmpty(this string obj, string paramName)
        {
            obj.ThrowIfNull(paramName);

            if (string.IsNullOrEmpty(obj))
                throw new ArgumentException(nameof(paramName));
        }
    }
}
