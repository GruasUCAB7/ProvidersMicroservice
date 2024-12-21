using Microsoft.OpenApi.Models;
using ProvidersMS.src.Drivers.Infrastructure.Dtos;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProvidersMS.Core.Infrastructure.Swagger
{
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParameters = context.ApiDescription.ParameterDescriptions
                .Where(p => p.ModelMetadata?.ModelType == typeof(CreateDriverWithImagesCommand));

            foreach (var fileParameter in fileParameters)
            {
                operation.Parameters.Remove(operation.Parameters.First(p => p.Name == fileParameter.Name));
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = {
                                    ["File"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}