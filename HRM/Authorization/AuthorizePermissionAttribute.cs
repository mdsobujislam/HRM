using Microsoft.AspNetCore.Authorization;

namespace HRM.Authorization
{
    public class AuthorizePermissionAttribute: AuthorizeAttribute
    {
        //[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
        public AuthorizePermissionAttribute(string permission) : base()
        {
            PermissionName = permission;
            var policy = $"Permission{permission}";
            Policy = policy;
        }
        public string PermissionName { get; }
    }
}
