using Microsoft.AspNetCore.Authorization;

namespace cosmeticClinic.Settings.Authorization;

[AttributeUsage(AttributeTargets.Method )]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public Permission Permission { get; }

    public RequirePermissionAttribute(Permission permission) 
        : base(permission.ToString())  
    {
        Permission = permission;
    }
}