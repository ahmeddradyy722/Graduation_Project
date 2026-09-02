using GraduationProject.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<DiabetesPrediction> DiabetesPredictions { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // One To Many Relationship...(One User => MAny Predictions)
        builder.Entity<DiabetesPrediction>()
            .HasOne(p => p.User)
            .WithMany() 
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
