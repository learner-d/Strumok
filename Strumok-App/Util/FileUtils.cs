using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Linq;

namespace Strumok_App.Util
{
    public static class FileUtils
    {
        [Obsolete]
        public static bool CanBeRead(string path)
        {
            ArgumentUtils.ThrowIfNullOrEmpty(path, "path");

            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
                return false;

            FileSecurity fileSecurity = fileInfo.GetAccessControl();
            AuthorizationRuleCollection fileAccessRules = fileSecurity.GetAccessRules(true, true, typeof(NTAccount));
            //TODO: remove
            //const string userId = "DESKTOP-68NQCNO\\dmytrogergel";
            //AuthorizationRule userRule = fileAccessRules.Cast<AuthorizationRule>().FirstOrDefault(rule => rule.);
            return true;
        }
    }
}
