using CleanArch.Domain.Common;

namespace CleanArch.Domain.UnitTests.Common;

public class BaseResponseTests
{
    [Test]
    public void DefaultConstructor_IsSuccessful_WithNoMessageOrData()
    {
        var response = new BaseResponse<string>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.Null);
            Assert.That(response.Data, Is.Null);
            Assert.That(response.ValidationErrors, Is.Null);
        }
    }

    [Test]
    public void MessageConstructor_IsSuccessful_WithMessage()
    {
        var response = new BaseResponse<int>("done");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Success, Is.True);
            Assert.That(response.Message, Is.EqualTo("done"));
            Assert.That(response.Data, Is.Zero);
        }
    }

    [Test]
    public void DataConstructor_SetsDataAndOptionalMessage()
    {
        var withMessage = new BaseResponse<int>(42, "answer");
        var withoutMessage = new BaseResponse<int>(7);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(withMessage.Data, Is.EqualTo(42));
            Assert.That(withMessage.Message, Is.EqualTo("answer"));
            Assert.That(withMessage.Success, Is.True);
            Assert.That(withoutMessage.Data, Is.EqualTo(7));
            Assert.That(withoutMessage.Message, Is.Null);
        }
    }

    [Test]
    public void PrimaryConstructor_CanRepresentFailure()
    {
        var response = new BaseResponse<string>("failed", false) { ValidationErrors = ["Name is required."] };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Success, Is.False);
            Assert.That(response.Message, Is.EqualTo("failed"));
            Assert.That(response.ValidationErrors, Has.Count.EqualTo(1));
        }
    }
}
