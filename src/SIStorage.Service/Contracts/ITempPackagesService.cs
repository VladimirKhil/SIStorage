namespace SIStorage.Service.Contracts;

/// <summary>
/// Provides API for working with temporary packages.
/// </summary>
public interface ITempPackagesService
{
    /// <summary>
    /// Generates path for temporary package.
    /// </summary>
    /// <param name="packageId">Package identifier.</param>
    /// <returns>Generated path.</returns>
    string GenerateFilePath(Guid packageId);

    /// <summary>
    /// Cleans temporary packages. 
    /// </summary>
    void Clean();
}
