using Domain.Enums;

namespace Application.Services.FirmRoles;

public static class FirmRoleModuleMapper
{
    private static readonly string[] AllModules = ["Metraj", "Puantaj", "Hakedis"];

    public static IReadOnlyList<string> GetModuleNames(FirmRole role) =>
        role switch
        {
            FirmRole.FirmaYoneticisi => AllModules,
            FirmRole.SantiyeSefi => ["Puantaj"],
            FirmRole.Puantor => ["Puantaj"],
            FirmRole.MetrajMuhendisi => ["Metraj"],
            FirmRole.HakedisMuhasebe => ["Hakedis"],
            FirmRole.SaltOkunur => AllModules,
            _ => [],
        };

    public static IReadOnlyList<string> GetModuleNames(FirmRole primary, FirmRole? secondary)
    {
        HashSet<string> modules = new(GetModuleNames(primary), StringComparer.Ordinal);
        if (secondary.HasValue)
        {
            foreach (string module in GetModuleNames(secondary.Value))
            {
                modules.Add(module);
            }
        }

        return AllModules.Where(modules.Contains).ToList();
    }

    public static string? NormalizeModuleName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "metraj" => "Metraj",
            "puantaj" => "Puantaj",
            "hakedis" or "hakediş" => "Hakedis",
            _ => AllModules.FirstOrDefault(module =>
                module.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase)
            ),
        };
    }
}
