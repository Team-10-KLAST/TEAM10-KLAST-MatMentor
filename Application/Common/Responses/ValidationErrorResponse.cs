namespace Application.Common.Responses;

public sealed record ValidationErrorResponse(IReadOnlyList<string> Errors);