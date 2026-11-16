using System.ComponentModel.DataAnnotations;
using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Enum;
using CleanArch.Domain.Services.Authentication;

namespace CleanArch.Domain.UnitTests.Entities;

public class EntityTests
{
    [Fact]
    public void Entities_AreAuditableAndKeyed()
    {
        Assert.IsAssignableFrom<AuditableEntity>(new Category());
        Assert.IsAssignableFrom<IHasKey<Guid>>(new Category());
        Assert.IsAssignableFrom<AuditableEntity>(new Event());
        Assert.IsAssignableFrom<IHasKey<Guid>>(new Event());
        Assert.IsAssignableFrom<IHasKey<int>>(new AuditTrail());
    }

    [Fact]
    public void BaseEntity_Id_IsMarkedAsKey()
    {
        var idProperty = typeof(BaseEntity<Guid>).GetProperty(nameof(BaseEntity<Guid>.Id))!;

        Assert.NotEmpty(idProperty.GetCustomAttributes(typeof(KeyAttribute), false));
    }

    [Fact]
    public void RoleConstants_AreDistinct_AndCoverEveryRole()
    {
        string[] roleIds = [Constants.SuperAdmin, Constants.Admin, Constants.Moderator, Constants.Basic];

        Assert.Equal(roleIds.Length, roleIds.Distinct().Count());
        Assert.Equal(roleIds.Length, System.Enum.GetNames<Roles>().Length);
        Assert.NotEqual(Constants.SuperAdminUser, Constants.BasicUser);
    }
}

public class AuthenticationRequestValidationTests
{
    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void RegistrationRequest_Valid_HasNoErrors()
    {
        var request = new RegistrationRequest
        {
            FirstName = "Amit",
            LastName = "Naik",
            Email = "someone@example.com",
            UserName = "someone",
            Password = "Password@123"
        };

        Assert.Empty(Validate(request));
    }

    [Fact]
    public void RegistrationRequest_Invalid_ReportsEachField()
    {
        var request = new RegistrationRequest { Email = "not-an-email", UserName = "abc", Password = "123" };

        var members = Validate(request).SelectMany(r => r.MemberNames).ToHashSet();

        Assert.Superset(new HashSet<string> { "FirstName", "LastName", "Email", "UserName", "Password" }, members);
    }

    [Fact]
    public void ResetPasswordRequest_MismatchedConfirmation_IsInvalid()
    {
        var request = new ResetPasswordRequest
        {
            Email = "someone@example.com",
            Token = "t",
            Password = "Password@123",
            ConfirmPassword = "Different@123"
        };

        Assert.Contains("ConfirmPassword", Validate(request).SelectMany(r => r.MemberNames));
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("bad", false)]
    [InlineData("someone@example.com", true)]
    public void ForgotPasswordRequest_RequiresValidEmail(string email, bool valid)
    {
        Assert.Equal(valid, Validate(new ForgotPasswordRequest { Email = email }).Count == 0);
    }
}
