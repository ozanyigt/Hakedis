using System.Reflection;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Contexts;

public class BaseDbContext : DbContext
{
    protected IConfiguration Configuration { get; set; }
    public DbSet<EmailAuthenticator> EmailAuthenticators { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }
    public DbSet<OtpAuthenticator> OtpAuthenticators { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<Drawing> Drawings { get; set; }
    public DbSet<MetrajRuleTemplate> MetrajRuleTemplates { get; set; }
    public DbSet<MetrajResult> MetrajResults { get; set; }
    public DbSet<Worker> Workers { get; set; }
    public DbSet<PuantajRecord> PuantajRecords { get; set; }
    public DbSet<ContractItem> ContractItems { get; set; }
    public DbSet<HakedisPeriod> HakedisPeriods { get; set; }
    public DbSet<ProgressEntry> ProgressEntries { get; set; }

    public BaseDbContext(DbContextOptions dbContextOptions, IConfiguration configuration)
        : base(dbContextOptions)
    {
        Configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
