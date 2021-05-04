using CleanArch.Domain.Enum;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CleanArch.Persistence.Context.Seeds
{
    public static class DefaultRoles
    {
        public static List<IdentityRole> IdentityRoleList()
        {
            return new List<IdentityRole>()
            {
                new IdentityRole
                {
                    Id = Constants.SuperAdmin,
                    Name = Roles.SuperAdmin.ToString(),
                    NormalizedName = Roles.SuperAdmin.ToString()
                },
                new IdentityRole
                {
                    Id = Constants.Admin,
                    Name = Roles.Admin.ToString(),
                    NormalizedName = Roles.Admin.ToString()
                },
                new IdentityRole
                {
                    Id = Constants.Moderator,
                    Name = Roles.Moderator.ToString(),
                    NormalizedName = Roles.Moderator.ToString()
                },
                new IdentityRole
                {
                    Id = Constants.Basic,
                    Name = Roles.Basic.ToString(),
                    NormalizedName = Roles.Basic.ToString()
                }
            };
        }
    }
}
