
namespace Application.Common.Models;

public record LoginResult(bool Succeeded, bool IsLockedOut, int? StudentId);
