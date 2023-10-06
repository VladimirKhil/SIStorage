namespace SIStorage.Service.Contract.Responses;

/// <summary>
/// Reprensents a successfull package creation response.
/// </summary>
/// <param name="PackageId">Created package identifier.</param>
public sealed record CreatePackageResponse(Guid PackageId);
