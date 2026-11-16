namespace CleanArch.Domain.Enum;

public enum Roles
{
    SuperAdmin,
    Admin,
    Moderator,
    Basic
}

public static class Constants
{
    // Fixed ids: they are used as HasData seed keys, so they must not change between runs.
    public const string SuperAdmin = "8d1a1e3c-6f0b-4c43-9a0e-0d6c2f1b7a01";
    public const string Admin = "8d1a1e3c-6f0b-4c43-9a0e-0d6c2f1b7a02";
    public const string Moderator = "8d1a1e3c-6f0b-4c43-9a0e-0d6c2f1b7a03";
    public const string Basic = "8d1a1e3c-6f0b-4c43-9a0e-0d6c2f1b7a04";

    public const string SuperAdminUser = "3f2b7c9e-1d4a-4e8b-b5c6-7a9d0e1f2a01";
    public const string BasicUser = "3f2b7c9e-1d4a-4e8b-b5c6-7a9d0e1f2a02";
}