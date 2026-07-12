namespace WebAPI;

public class WebApiConfiguration
{
    public string ApiDomain { get; set; }
    public string[] AllowedOrigins { get; set; }

    /// <summary>
    /// Cross-subdomain cookie için örn. ".sahametrik.com". Boşsa local (Lax) kullanılır.
    /// </summary>
    public string? CookieDomain { get; set; }

    public WebApiConfiguration()
    {
        ApiDomain = string.Empty;
        AllowedOrigins = Array.Empty<string>();
    }

    public WebApiConfiguration(string apiDomain, string[] allowedOrigins)
    {
        ApiDomain = apiDomain;
        AllowedOrigins = allowedOrigins;
    }
}
