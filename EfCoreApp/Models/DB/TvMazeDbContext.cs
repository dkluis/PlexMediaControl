using Microsoft.EntityFrameworkCore;

namespace EfCoreApp.Models.DB;

public partial class TvMazeDbContext : DbContext
{
    public TvMazeDbContext()
    {
    }

    public TvMazeDbContext(DbContextOptions<TvMazeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActionItem> ActionItems { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer(
                "Server=tcp:192.168.142.152,1433;Database=TvMazeDb;User ID=sa;Password=Sandy3942");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActionItem>(entity =>
        {
            entity.HasIndex(e => new {e.Program, e.Message, e.UpdateDateTime}, "ActionItems_UN")
                .IsUnique();

            entity.Property(e => e.Message)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.Program)
                .HasMaxLength(25)
                .IsUnicode(false);

            entity.Property(e => e.UpdateDateTime)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}