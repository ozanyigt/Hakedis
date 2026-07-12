namespace Domain.Enums;

public enum DrawingStatus
{
    Uploaded = 1,
    Parsing = 2,
    Parsed = 3,
    Failed = 4,
    /// <summary>Brüt metraj hesaplandı; AI hüküm / insan onayı bekleniyor.</summary>
    PendingReview = 5,
    /// <summary>Metraj kalemleri onaylanıp kilitlendi.</summary>
    Approved = 6
}

