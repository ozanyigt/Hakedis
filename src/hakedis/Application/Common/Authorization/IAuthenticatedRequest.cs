namespace Application.Common.Authorization;

/// <summary>
/// Marker for requests that require only a valid JWT (no operation-claim check).
/// Use with [Authorize] on the controller action — do not combine with ISecuredRequest.
/// </summary>
public interface IAuthenticatedRequest;
