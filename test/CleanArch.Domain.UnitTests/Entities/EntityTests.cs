using System.ComponentModel.DataAnnotations;
using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Enum;
using CleanArch.Domain.Services.Authentication;

namespace CleanArch.Domain.UnitTests.Entities;

public class EntityTests
{
    [Test]
    public void Entities_AreAuditableAndKeyed()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(new Category(), Is.InstanceOf<AuditableEntity>().And.InstanceOf<IHasKey<Guid>>());
            Assert.That(new Event(), Is.InstanceOf<AuditableEntity>().And.InstanceOf<IHasKey<Guid>>());
            Assert.That(new AuditTrail(), Is.InstanceOf<IHasKey<int>>());
        }
    }

    [Test]
    public void BaseEntity_Id_IsMarkedAsKey()
    {
        var idProperty = typeof(BaseEntity<Guid>).GetProperty(nameof(BaseEntity<Guid>.Id))!;

        Assert.That(idProperty.GetCustomAttributes(typeof(KeyAttribute), false), Is.Not.Empty);
    }

    [Test]
    public void RoleConstants_AreDistinct_AndCoverEveryRole()
    {
        var roleIds = new[] { Constants.SuperAdmin, Constants.Admin, Constants.Moderator, Constants.Basic };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(roleIds, Is.Unique);
            Assert.That(System.Enum.GetNames<Roles>(), Has.Length.EqualTo(roleIds.Length));
            Assert.That(Constants.SuperAdminUser, Is.Not.EqualTo(Constants.BasicUser));
        }
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

    [Test]
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

        Assert.That(Validate(request), Is.Empty);
    }

    [Test]
    public void RegistrationRequest_Invalid_ReportsEachField()
    {
        var request = new RegistrationRequest { Email = "not-an-email", UserName = "abc", Password = "123" };

        var members = Validate(request).SelectMany(r => r.MemberNames).ToList();

        Assert.That(members, Is.SupersetOf(new[] { "FirstName", "LastName", "Email", "UserName", "Password" }));
    }

    [Test]
    public void ResetPasswordRequest_MismatchedConfirmation_IsInvalid()
    {
        var request = new ResetPasswordRequest
        {
            Email = "someone@example.com",
            Token = "t",
            Password = "Password@123",
            ConfirmPassword = "Different@123"
        };

        Assert.That(Validate(request).SelectMany(r => r.MemberNames), Does.Contain("ConfirmPassword"));
    }

    [Test]
    public void ForgotPasswordRequest_RequiresValidEmail()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Validate(new ForgotPasswordRequest()), Is.Not.Empty);
            Assert.That(Validate(new ForgotPasswordRequest { Email = "bad" }), Is.Not.Empty);
            Assert.That(Validate(new ForgotPasswordRequest { Email = "someone@example.com" }), Is.Empty);
        }
    }
}
