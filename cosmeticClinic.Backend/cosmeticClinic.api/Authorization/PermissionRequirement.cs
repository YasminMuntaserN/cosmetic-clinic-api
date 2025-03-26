using Microsoft.AspNetCore.Authorization;

namespace cosmeticClinic.Settings.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public Permission _Permission { get; }

    public PermissionRequirement(Permission permission)
    {
        _Permission = permission;
    }
}