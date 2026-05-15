using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace EOM.TSHotelManagement.WebApi.Filters
{
    public class CSRFTokenOperationProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var method = context.OperationDescription.Method?.ToLower();
            var path = context.OperationDescription.Path?.ToLower();

            if (method != "get" &&
                !(path?.Contains("csrf") ?? false))
            {
                context.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-CSRF-TOKEN-HEADER",
                    Kind = OpenApiParameterKind.Header,
                    Description = "CSRF Token for state-changing requests",
                    IsRequired = true,
                    Schema = new NJsonSchema.JsonSchema { Type = NJsonSchema.JsonObjectType.String }
                });
            }

            return true;
        }
    }
}