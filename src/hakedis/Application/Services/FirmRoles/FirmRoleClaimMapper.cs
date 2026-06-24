using Domain.Enums;

namespace Application.Services.FirmRoles;

public static class FirmRoleClaimMapper
{
    public static IReadOnlyList<string> GetClaimNames(FirmRole role) =>
        role switch
        {
            FirmRole.FirmaYoneticisi =>
            [
                "Users.Admin",
                "Projects.Admin",
                "Sites.Admin",
                "PuantajRecords.Admin",
                "Workers.Admin",
                "MetrajResults.Admin",
                "MetrajRuleTemplates.Admin",
                "Drawings.Admin",
                "HakedisPeriods.Admin",
                "ContractItems.Admin",
                "ProgressEntries.Admin",
            ],
            FirmRole.SantiyeSefi =>
            [
                "Projects.Read",
                "Sites.Read",
                "Sites.Write",
                "Sites.Create",
                "Sites.Update",
                "PuantajRecords.Admin",
                "Workers.Admin",
            ],
            FirmRole.Puantor =>
            [
                "Projects.Read",
                "Sites.Read",
                "PuantajRecords.Read",
                "PuantajRecords.Write",
                "PuantajRecords.Create",
                "PuantajRecords.Update",
                "Workers.Read",
            ],
            FirmRole.MetrajMuhendisi =>
            [
                "Projects.Read",
                "Sites.Read",
                "MetrajResults.Admin",
                "MetrajRuleTemplates.Admin",
                "Drawings.Read",
                "Drawings.Write",
                "Drawings.Create",
                "Drawings.Update",
            ],
            FirmRole.HakedisMuhasebe =>
            [
                "Projects.Read",
                "HakedisPeriods.Admin",
                "ContractItems.Admin",
                "ProgressEntries.Admin",
            ],
            FirmRole.SaltOkunur =>
            [
                "Projects.Read",
                "Sites.Read",
                "PuantajRecords.Read",
                "MetrajResults.Read",
                "MetrajRuleTemplates.Read",
                "Drawings.Read",
                "Workers.Read",
                "HakedisPeriods.Read",
                "ContractItems.Read",
                "ProgressEntries.Read",
            ],
            _ => [],
        };

    public static IReadOnlyList<string> GetClaimNames(FirmRole primary, FirmRole? secondary)
    {
        HashSet<string> claims = new(GetClaimNames(primary), StringComparer.Ordinal);
        if (secondary.HasValue)
        {
            foreach (string claim in GetClaimNames(secondary.Value))
            {
                claims.Add(claim);
            }
        }

        return claims.ToList();
    }

    public static string GetDisplayName(FirmRole role) =>
        role switch
        {
            FirmRole.FirmaYoneticisi => "Firma Yöneticisi",
            FirmRole.SantiyeSefi => "Şantiye Şefi",
            FirmRole.Puantor => "Puantör",
            FirmRole.MetrajMuhendisi => "Metraj Mühendisi",
            FirmRole.HakedisMuhasebe => "Hakediş / Muhasebe",
            FirmRole.SaltOkunur => "Sadece Görüntüleme",
            _ => role.ToString(),
        };

    public static string GetDescription(FirmRole role) =>
        role switch
        {
            FirmRole.FirmaYoneticisi => "Kurumdaki tüm modüller ve kullanıcı yönetimi",
            FirmRole.SantiyeSefi => "Puantaj girişi, işçi ve şantiye yönetimi",
            FirmRole.Puantor => "Yalnızca puantaj kaydı girişi",
            FirmRole.MetrajMuhendisi => "Metraj ve çizim işlemleri",
            FirmRole.HakedisMuhasebe => "Hakediş dönemleri ve sözleşme kalemleri",
            FirmRole.SaltOkunur => "Tüm modüllerde salt okunur erişim",
            _ => string.Empty,
        };
}
