using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace Lotto.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class FunctionKeySecurityAttribute : OpenApiSecurityAttribute
{
    public FunctionKeySecurityAttribute() : base("function_key", SecuritySchemeType.ApiKey)
    {
        Name = "x-functions-key";
        In = OpenApiSecurityLocationType.Header;
    }
}
