using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanArch.CrossCuttingConcerns.Utility;

/// <summary>Documents actions marked with <see cref="FileResultContentTypeAttribute"/> as returning a binary file.</summary>
public class FileResultContentTypeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var requestAttribute = context.MethodInfo.GetCustomAttributes(typeof(FileResultContentTypeAttribute), false)
            .Cast<FileResultContentTypeAttribute>()
            .FirstOrDefault();

        if (requestAttribute == null) return;

        operation.Responses ??= new OpenApiResponses();
        operation.Responses.Clear();
        operation.Responses.Add("200", new OpenApiResponse
        {
            Description = "File",
            Content = new Dictionary<string, IOpenApiMediaType>
            {
                [requestAttribute.ContentType] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        Format = "binary"
                    }
                }
            }
        });
    }
}
