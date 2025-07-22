using Microsoft.AspNetCore.Authorization;

namespace HRM.Authorization
{
    public class PermissionPolicyProvider: IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "Permission";
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy>(null);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                var permissions = policyName.Substring(POLICY_PREFIX.Length).Split(',');
                var policy = new AuthorizationPolicyBuilder();
                foreach (var permission in permissions)
                {
                    policy.Requirements.Add(new PermissionRequirement(permission.Trim()));
                }
                return Task.FromResult(policy.Build());
            }

            return Task.FromResult<AuthorizationPolicy>(null);
        }
    }
}
