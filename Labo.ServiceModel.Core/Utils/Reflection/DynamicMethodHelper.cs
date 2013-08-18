using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;

namespace Labo.ServiceModel.Core.Utils.Reflection
{
    internal static class DynamicMethodHelper
    {
        public static DynamicMethod CreateDynamicMethod(string name, System.Type ownerType, System.Type returnType, System.Type[] argumentTypes)
        {
            System.Type owner = ownerType.IsInterface ? typeof(object) : ownerType;

            //Check reflection member access permission
            PermissionSet permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
            bool canSkipChecks = true;
            //uncomment me in 4.0
            //permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);

            return new DynamicMethod(name, returnType, argumentTypes, owner, canSkipChecks);
        }
    }
}
