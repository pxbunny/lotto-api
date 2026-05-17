using System.Net;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace Lotto.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class OperationAttribute(string operationId, string tag)
    : OpenApiOperationAttribute(operationId, tag);

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
internal sealed class JsonResponseAttribute(HttpStatusCode statusCode, Type bodyType)
    : OpenApiResponseWithBodyAttribute(statusCode, "application/json", bodyType);

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
internal sealed class NoBodyResponseAttribute(HttpStatusCode statusCode)
    : OpenApiResponseWithoutBodyAttribute(statusCode);

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
internal sealed class QueryParamAttribute : OpenApiParameterAttribute
{
    public QueryParamAttribute(string name) : base(name)
    {
        In = ParameterLocation.Query;
    }

    public QueryParamAttribute(string name, Type type) : this(name)
    {
        Type = type;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
internal sealed class PathParamAttribute : OpenApiParameterAttribute
{
    public PathParamAttribute(string name) : base(name)
    {
        In = ParameterLocation.Path;
    }
}
