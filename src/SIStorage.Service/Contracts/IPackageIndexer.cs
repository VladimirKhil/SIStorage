using SIPackages;
using SIStorage.Service.Models;

namespace SIStorage.Service.Contracts;

/// <summary>
/// Extracts metadata from SIGame package.
/// </summary>
public interface IPackageIndexer
{
    /// <summary>
    /// Extracts metadata from SIGame package.
    /// </summary>
    /// <param name="document">SIGame package.</param>
    PackageInfo IndexPackage(SIDocument document);
}
