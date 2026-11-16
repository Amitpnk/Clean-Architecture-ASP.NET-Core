using CleanArch.Application.Exceptions;
using FluentValidation.Results;

namespace CleanArch.Application.UnitTests.Exceptions;

public class ExceptionTests
{
    [Test]
    public void NotFoundException_FormatsNameAndKey()
    {
        var ex = new NotFoundException("Event", 42);

        Assert.That(ex.Message, Is.EqualTo("Event (42) is not found"));
    }

    [Test]
    public void BadRequestException_KeepsMessage()
    {
        Assert.That(new BadRequestException("bad").Message, Is.EqualTo("bad"));
    }

    [Test]
    public void ApiException_FormatsArguments()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(new ApiException("plain").Message, Is.EqualTo("plain"));
            Assert.That(new ApiException("{0} and {1}", "a", 2).Message, Is.EqualTo("a and 2"));
            Assert.That(new ApiException().Message, Is.Not.Null);
        }
    }

    [Test]
    public void ValidationException_CollectsEveryErrorMessage()
    {
        var result = new ValidationResult([new ValidationFailure("Name", "required"), new ValidationFailure("Date", "in the past")]);

        var ex = new ValidationException(result);

        Assert.That(ex.ValdationErrors, Is.EqualTo(new[] { "required", "in the past" }));
    }

    [Test]
    public void ValidationException_WithNoFailures_HasEmptyList()
    {
        var ex = new ValidationException(new ValidationResult());

        Assert.That(ex.ValdationErrors, Is.Empty);
    }
}
