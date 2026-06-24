using Domain.Enums;

namespace Application.Services.MetrajCalculation;

public static class MetrajCalculationRuleBuilder
{
    public static MetrajKalemRule BuildFromProjectLayers(
        MetrajKalemType kalemType,
        IEnumerable<string> layerNames
    )
    {
        MetrajKalemRule template = MetrajCalculationDefaults
            .GetDefaultRules()
            .First(rule => rule.KalemType == kalemType);

        string[] patterns = layerNames
            .Select(name => name.Trim())
            .Where(name => name.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new MetrajKalemRule
        {
            KalemType = kalemType,
            Unit = template.Unit,
            LayerPatterns = patterns,
            EntityTypes = template.EntityTypes,
            Method = template.Method,
            DefaultHeight = template.DefaultHeight,
            DefaultThickness = template.DefaultThickness,
            DeductOpenings = template.DeductOpenings,
            SideCount = template.SideCount,
            Description = $"Proje katman eşlemesi ({patterns.Length} katman)",
        };
    }

    public static IReadOnlyList<MetrajKalemRule> MergeProjectLayerMappings(
        IEnumerable<Domain.Entities.ProjectMetrajLayerMapping> projectMappings,
        IReadOnlyList<MetrajKalemRule> fallbackRules
    )
    {
        Dictionary<MetrajKalemType, MetrajKalemRule> fallbackByType = fallbackRules.ToDictionary(
            rule => rule.KalemType
        );

        List<MetrajKalemRule> rules = [];

        foreach (MetrajKalemType kalemType in Enum.GetValues<MetrajKalemType>())
        {
            Domain.Entities.ProjectMetrajLayerMapping? mapping = projectMappings.FirstOrDefault(
                item => item.KalemType == kalemType
            );

            if (mapping != null && !string.IsNullOrWhiteSpace(mapping.LayerNames))
            {
                string[] layerNames = mapping
                    .LayerNames.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (layerNames.Length > 0)
                {
                    rules.Add(BuildFromProjectLayers(kalemType, layerNames));
                    continue;
                }
            }

            if (fallbackByType.TryGetValue(kalemType, out MetrajKalemRule? fallbackRule))
            {
                rules.Add(fallbackRule);
            }
        }

        return rules;
    }

    public static bool HasProjectLayerMappings(
        IEnumerable<Domain.Entities.ProjectMetrajLayerMapping> projectMappings
    ) =>
        projectMappings.Any(mapping => !string.IsNullOrWhiteSpace(mapping.LayerNames));
}
