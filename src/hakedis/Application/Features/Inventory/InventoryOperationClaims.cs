namespace Application.Features.Inventory;

public static class InventoryOperationClaims
{
    private const string Section = "Inventory";
    public const string Admin = $"{Section}.Admin";
    public const string Read = $"{Section}.Read";
    public const string Write = $"{Section}.Write";
    public const string Create = $"{Section}.Create";
    public const string Update = $"{Section}.Update";
    public const string Delete = $"{Section}.Delete";
}
