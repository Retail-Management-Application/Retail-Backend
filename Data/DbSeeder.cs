namespace RetailOrdering.API.Data
{
    using global::RetailOrdering.API.Common.Enums;
    using global::RetailOrdering.API.Helpers;
    using global::RetailOrdering.API.Models;
    using Microsoft.EntityFrameworkCore;

    namespace RetailOrdering.API.Data
    {
        public static class DbSeeder
        {
            public static async Task SeedAsync(IServiceProvider services, ILogger logger)
            {
                using var scope = services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                try
                {
                    // Apply any pending migrations automatically
                    await context.Database.MigrateAsync();

                    // Seed default admin if none exists
                    bool adminExists = await context.Users
                        .AnyAsync(u => u.Role == UserRole.Admin);

                    if (!adminExists)
                    {
                        var admin = new User
                        {
                            FullName = "Super Admin",
                            Email = "admin@retailhub.com",
                            PasswordHash = PasswordHelper.HashPassword("Admin@123"),
                            PhoneNumber = "+91 9000000000",
                            Address = "RetailHub HQ, Chennai, Tamil Nadu",
                            Role = UserRole.Admin,
                            CreatedAt = DateTime.UtcNow
                        };

                        context.Users.Add(admin);
                        await context.SaveChangesAsync();

                        logger.LogInformation("✅ Default admin seeded → Email: admin@retailhub.com | Password: Admin@123");
                    }
                    else
                    {
                        logger.LogInformation("ℹ️  Admin already exists — skipping seed.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "❌ Seeding failed: {Message}", ex.Message);
                    throw;
                }
            }
        }
    }
}
