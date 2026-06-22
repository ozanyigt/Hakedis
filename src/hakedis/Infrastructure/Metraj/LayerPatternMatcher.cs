namespace Infrastructure.Metraj;

internal static class LayerPatternMatcher
{
  public static bool IsMatch(string layerName, IEnumerable<string> patterns)
  {
    foreach (string pattern in patterns)
    {
      if (MatchesPattern(layerName, pattern))
        return true;
    }

    return false;
  }

  private static bool MatchesPattern(string layerName, string pattern)
  {
    string normalizedLayer = layerName.Trim().ToUpperInvariant();
    string normalizedPattern = pattern.Trim().ToUpperInvariant();

    if (!normalizedPattern.Contains('*'))
      return normalizedLayer == normalizedPattern;

    string regexPattern =
      "^" + System.Text.RegularExpressions.Regex.Escape(normalizedPattern).Replace("\\*", ".*") + "$";

    return System.Text.RegularExpressions.Regex.IsMatch(
      normalizedLayer,
      regexPattern,
      System.Text.RegularExpressions.RegexOptions.CultureInvariant
    );
  }
}
