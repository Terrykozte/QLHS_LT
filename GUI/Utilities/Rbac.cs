using System.Collections.Generic;
using System.Linq;

namespace QLTN_LT.GUI.Utilities
{
    public static class Rbac
    {
        public static bool IsAdmin(IEnumerable<string> roles)
        {
            if (roles == null) return false;
            return roles.Any(r => string.Equals(r?.Trim(), Roles.Admin, System.StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsStaff(IEnumerable<string> roles)
        {
            if (roles == null) return false;
            return roles.Any(r => string.Equals(r?.Trim(), Roles.Staff, System.StringComparison.OrdinalIgnoreCase));
        }

        public static bool CanAccess(string buttonName, IEnumerable<string> roles)
        {
            if (IsAdmin(roles)) return true; // Admin full access
            if (IsStaff(roles))
            {
                switch (buttonName)
                {
                    case "btnDashboard":
                    case "btnOrders":
                    case "btnSeafood":
                    case "btnCustomer":
                    case "btnTable":
                    case "btnMenu":
                    case "btnInventory":
                        return true;
                    default:
                        return false;
                }
            }
            // Unknown -> only dashboard
            return buttonName == "btnDashboard";
        }
    }
}

