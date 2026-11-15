using Microsoft.EntityFrameworkCore;
using NominationsApi.Models;

namespace NominationsApi.Data;

public class NominationsDbContext(DbContextOptions<NominationsDbContext> options) : DbContext(options)
{
    public DbSet<Nomination> Nominations => Set<Nomination>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Nomination>(entity =>
        {
            entity.HasKey(n => n.Id);

            entity.Property(n => n.ShipperId)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(n => n.PipelineSegment)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(n => n.RequestedVolume)
                .HasPrecision(18, 4);

            entity.Property(n => n.Status)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(n => n.CreatedAt)
                .IsRequired();

            entity.HasIndex(n => n.ShipperId);
        });
    }
}
