using CleanArch.Application.Exceptions;
using FluentValidation.Results;

namespace CleanArch.Application.UnitTests.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void NotFoundException_FormatsNameAndKey()
    {
        var ex = new NotFoundException("Event", 42);

        Assert.Equal("Event (42) is not found", ex.Message);
    }

    [Fact]
    public void BadRequestException_KeepsMessage()
    {
        Assert.Equal("bad", new BadRequestException("bad").Message);
    }

    [Fact]
    public void ApiException_FormatsArguments()
    {
        Assert.Equal("plain", new ApiException("plain").Message);
        Assert.Equal("a and 2", new ApiException("{0} and {1}", "a", 2).Message);
        Assert.NotNull(new ApiException().Message);
    }

    [Fact]
    public void ValidationException_CollectsEveryErrorMessage()
    {
        var result = new ValidationResult([new ValidationFailure("Name", "required"), new ValidationFailure("Date", "in the past")]);

        var ex = new ValidationException(result);

        Assert.Equal(["required", "in the past"], ex.ValidationErrors);
    }

    [Fact]
    public void ValidationException_WithNoFailures_HasEmptyList()
    {
        var ex = new ValidationException(new ValidationResult());

        Assert.Empty(ex.ValidationErrors);
    }
}
