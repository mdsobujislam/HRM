using Microsoft.AspNetCore.Authorization;

namespace HRM.Authorization
{
    public static class PermissionAuthorizationExtensions
    {
        public static void AddPermissionAuthorization(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }
    }
}
