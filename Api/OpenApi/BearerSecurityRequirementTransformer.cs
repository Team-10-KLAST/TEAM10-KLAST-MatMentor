using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Api.OpenApi;

internal sealed class BearerSecurityRequirementTransformer : IOpenApiOperationTransformer
{
    // Marks an endpoint as requiring a bearer token, unless the endpoint allows anonymous access
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        bool allowsAnonymous = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<IAllowAnonymous>()
            .Any();
        if (allowsAnonymous)
            return Task.CompletedTask;

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
        });
        return Task.CompletedTask;
    }
}