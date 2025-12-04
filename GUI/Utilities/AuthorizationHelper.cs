using System.Collections.Generic;

namespace QLTN_LT.GUI.Utilities
{
    public static class AuthorizationHelper
    {
        public static bool IsAdmin(IEnumerable<string> roles) => Rbac.IsAdmin(roles);
        public static bool IsStaff(IEnumerable<string> roles) => Rbac.IsStaff(roles);
        public static bool CanAccess(string buttonName, IEnumerable<string> roles) => Rbac.CanAccess(buttonName, roles);
    }
}

