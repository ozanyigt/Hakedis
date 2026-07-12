namespace Domain.Enums;

/// <summary>
/// AI / politika hükmü. Sayı üretmez; brüt miktarın sayılıp sayılmayacağını önerir.
/// </summary>
public enum MetrajJudgmentDecision
{
    Count = 1,
    Ignore = 2,
    NeedsReview = 3
}
