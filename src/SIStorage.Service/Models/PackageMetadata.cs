using SIStorage.Database.Models;

namespace SIStorage.Service.Models;

internal sealed record PackageMetadata(Guid Id, RoundModel[] Rounds);
