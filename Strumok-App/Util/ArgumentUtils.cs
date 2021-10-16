using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strumok_App.Util
{
    public static class ArgumentUtils
    {
        public static void ThrowIfNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName ?? "");
            }
        }

        internal static void ThrowIfNullOrEmpty(string argument, string argumentName)
        {
            if (argument == null || argument.Length < 1)
            {
                throw new ArgumentNullException(argumentName ?? "");
            }
        }
    }
}
