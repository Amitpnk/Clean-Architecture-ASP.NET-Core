using CleanArch.Domain.Entities;
using CleanArch.Domain.Enum;

namespace CleanArch.Persistence.Context.Seeds;

public static class DefaultUser
{
    public static List<ApplicationUser> IdentityBasicUserList()
    {
        return new List<ApplicationUser>()
            {
                new ApplicationUser
                {
                    Id = Constants.SuperAdminUser,
                    SecurityStamp = "B7E5C3A1D9F24E6A8C0B1D3F5E7A9C2B",
                    ConcurrencyStamp = Constants.SuperAdminUser,
                    UserName = "superadmin",
                    Email = "superadmin@gmail.com",
                    FirstName = "Amit",
                    LastName = "Naik",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    // Password@123
                    PasswordHash = "AQAAAAEAACcQAAAAEBLjouNqaeiVWbN0TbXUS3+ChW3d7aQIk/BQEkWBxlrdRRngp14b0BIH0Rp65qD6mA==",
                    NormalizedEmail= "SUPERADMIN@GMAIL.COM",
                    NormalizedUserName="SUPERADMIN"
                },
                new ApplicationUser
                {
                    Id = Constants.BasicUser,
                    SecurityStamp = "C8F6D4B2EA035F7B9D1C2E4A6F8B0D3C",
                    ConcurrencyStamp = Constants.BasicUser,
                    UserName = "basicuser",
                    Email = "basicuser@gmail.com",
                    FirstName = "Basic",
                    LastName = "User",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    // Password@123
                    PasswordHash = "AQAAAAEAACcQAAAAEBLjouNqaeiVWbN0TbXUS3+ChW3d7aQIk/BQEkWBxlrdRRngp14b0BIH0Rp65qD6mA==",
                    NormalizedEmail= "BASICUSER@GMAIL.COM",
                    NormalizedUserName="BASICUSER"
                },
            };
    }
}