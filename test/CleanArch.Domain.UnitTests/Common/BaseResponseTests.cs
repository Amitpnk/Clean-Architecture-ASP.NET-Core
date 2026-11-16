using CleanArch.Domain.Common;

namespace CleanArch.Domain.UnitTests.Common;

public class BaseResponseTests
{
    [Fact]
    public void DefaultConstructor_IsSuccessful_WithNoMessageOrData()
    {
        var response = new BaseResponse<string>();

        Assert.True(response.Success);
        Assert.Null(response.Message);
        Assert.Null(response.Data);
        Assert.Null(response.ValidationErrors);
    }

    [Fact]
    public void MessageConstructor_IsSuccessful_WithMessage()
    {
        var response = new BaseResponse<int>("done");

        Assert.True(response.Success);
        Assert.Equal("done", response.Message);
        Assert.Equal(0, response.Data);
    }

    [Fact]
    public void DataConstructor_SetsDataAndOptionalMessage()
    {
        var withMessage = new BaseResponse<int>(42, "answer");
        var withoutMessage = new BaseResponse<int>(7);

        Assert.Equal(42, withMessage.Data);
        Assert.Equal("answer", withMessage.Message);
        Assert.True(withMessage.Success);
        Assert.Equal(7, withoutMessage.Data);
        Assert.Null(withoutMessage.Message);
    }

    [Fact]
    public void PrimaryConstructor_CanRepresentFailure()
    {
        var response = new BaseResponse<string>("failed", false) { ValidationErrors = ["Name is required."] };

        Assert.False(response.Success);
        Assert.Equal("failed", response.Message);
        Assert.Single(response.ValidationErrors);
    }
}
