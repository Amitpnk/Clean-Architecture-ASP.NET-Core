using System.Reflection;
using CleanArch.CrossCuttingConcerns.Utility;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanArch.Api.UnitTests.Utility;

public class FileResultContentTypeOperationFilterTests
{
    [FileResultContentType("text/csv")]
    private static void FileAction() { }

    private static void PlainAction() { }

    private static OperationFilterContext ContextFor(string methodName) =>
        new(new ApiDescription(), null, new SchemaRepository(), new OpenApiDocument(),
            typeof(FileResultContentTypeOperationFilterTests).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static));

    [Test]
    public void Apply_AttributedAction_ReplacesResponsesWithBinaryFile()
    {
        var operation = new OpenApiOperation { Responses = new OpenApiResponses { ["400"] = new OpenApiResponse() } };

        new FileResultContentTypeOperationFilter().Apply(operation, ContextFor(nameof(FileAction)));

        var response = operation.Responses.Single();
        var schema = response.Value.Content!["text/csv"].Schema!;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Key, Is.EqualTo("200"));
            Assert.That(response.Value.Description, Is.EqualTo("File"));
            Assert.That(schema.Type, Is.EqualTo(JsonSchemaType.String));
            Assert.That(schema.Format, Is.EqualTo("binary"));
        }
    }

    [Test]
    public void Apply_ActionWithoutAttribute_LeavesOperationUntouched()
    {
        var operation = new OpenApiOperation { Responses = new OpenApiResponses { ["400"] = new OpenApiResponse() } };

        new FileResultContentTypeOperationFilter().Apply(operation, ContextFor(nameof(PlainAction)));

        Assert.That(operation.Responses.Keys, Is.EqualTo(new[] { "400" }));
    }

    [Test]
    public void Attribute_ExposesContentType()
    {
        Assert.That(new FileResultContentTypeAttribute("application/pdf").ContentType, Is.EqualTo("application/pdf"));
    }
}
