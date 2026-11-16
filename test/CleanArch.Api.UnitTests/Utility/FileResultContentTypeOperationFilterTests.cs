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

    [Fact]
    public void Apply_AttributedAction_ReplacesResponsesWithBinaryFile()
    {
        var operation = new OpenApiOperation { Responses = new OpenApiResponses { ["400"] = new OpenApiResponse() } };

        new FileResultContentTypeOperationFilter().Apply(operation, ContextFor(nameof(FileAction)));

        var response = Assert.Single(operation.Responses);
        var schema = response.Value.Content!["text/csv"].Schema!;
        Assert.Equal("200", response.Key);
        Assert.Equal("File", response.Value.Description);
        Assert.Equal(JsonSchemaType.String, schema.Type);
        Assert.Equal("binary", schema.Format);
    }

    [Fact]
    public void Apply_ActionWithoutAttribute_LeavesOperationUntouched()
    {
        var operation = new OpenApiOperation { Responses = new OpenApiResponses { ["400"] = new OpenApiResponse() } };

        new FileResultContentTypeOperationFilter().Apply(operation, ContextFor(nameof(PlainAction)));

        Assert.Equal(["400"], operation.Responses.Keys);
    }

    [Fact]
    public void Attribute_ExposesContentType()
    {
        Assert.Equal("application/pdf", new FileResultContentTypeAttribute("application/pdf").ContentType);
    }
}
