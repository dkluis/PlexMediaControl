using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

namespace PlexMediaControl.Models.MariaDB;

public partial class TvMaze : DbContext
{
    public TvMaze()
    {
    }

    public TvMaze(DbContextOptions<TvMaze> options) : base(options)
    {
    }

    public virtual DbSet<ActionItem> ActionItems { get; set; }

    public virtual DbSet<Episode> Episodes { get; set; }

    public virtual DbSet<Episodesfromtodayback> Episodesfromtodaybacks { get; set; }

    public virtual DbSet<Episodesfullinfo> Episodesfullinfos { get; set; }

    public virtual DbSet<Episodestoacquire> Episodestoacquires { get; set; }

    public virtual DbSet<Followed> Followeds { get; set; }

    public virtual DbSet<LastShowEvaluated> LastShowEvaluateds { get; set; }

    public virtual DbSet<MediaType> MediaTypes { get; set; }

    public virtual DbSet<Nobroadcastdate> Nobroadcastdates { get; set; }

    public virtual DbSet<Notinfollowed> Notinfolloweds { get; set; }

    public virtual DbSet<Notinshow> Notinshows { get; set; }

    public virtual DbSet<Orphanedepisode> Orphanedepisodes { get; set; }

    public virtual DbSet<PlexStatus> PlexStatuses { get; set; }

    public virtual DbSet<PlexWatchedEpisode> PlexWatchedEpisodes { get; set; }

    public virtual DbSet<Show> Shows { get; set; }

    public virtual DbSet<ShowRssFeed> ShowRssFeeds { get; set; }

    public virtual DbSet<ShowStatus> ShowStatuses { get; set; }

    public virtual DbSet<Showepisodecount> Showepisodecounts { get; set; }

    public virtual DbSet<Showsnotinfollowed> Showsnotinfolloweds { get; set; }

    public virtual DbSet<Showstorefresh> Showstorefreshes { get; set; }

    public virtual DbSet<TvmShowUpdate> TvmShowUpdates { get; set; }

    public virtual DbSet<TvmStatus> TvmStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    const string connectionString = "server=ubuntumediahandler.local;port=3306;database=MediaHandlerDb;uid=dick;pwd=Sandy3942";
    optionsBuilder.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.6.16-mariadb"));
}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("utf8mb4_general_ci").HasCharSet("utf8mb4");

        modelBuilder.Entity<ActionItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => new {e.Program, e.Message, e.UpdateDateTime}, "ActionItems_UN").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.Program).HasMaxLength(25);
            entity.Property(e => e.UpdateDateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Episode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.TvmShowId, "Episodes_FK");

            entity.HasIndex(e => e.PlexStatus, "Episodes_FK_1");

            entity.HasIndex(e => e.TvmEpisodeId, "Episodes_UN").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.BroadcastDate).HasColumnType("datetime");
            entity.Property(e => e.Episode1).HasColumnType("int(11)").HasColumnName("Episode");
            entity.Property(e => e.PlexDate).HasColumnType("datetime");
            entity.Property(e => e.PlexStatus).HasMaxLength(10).HasDefaultValueSql("' '");
            entity.Property(e => e.Season).HasColumnType("int(11)");
            entity.Property(e => e.SeasonEpisode).HasMaxLength(10);
            entity.Property(e => e.TvmEpisodeId).HasColumnType("int(11)");
            entity.Property(e => e.TvmShowId).HasColumnType("int(11)");
            entity.Property(e => e.TvmUrl).HasMaxLength(255).HasDefaultValueSql("' '");
            entity.Property(e => e.UpdateDate).HasDefaultValueSql("curdate()").HasColumnType("datetime");

            entity.HasOne(d => d.PlexStatusNavigation).WithMany(p => p.Episodes).HasForeignKey(d => d.PlexStatus).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("Episodes_FK_1");

            entity.HasOne(d => d.TvmShow).WithMany(p => p.Episodes).HasPrincipalKey(p => p.TvmShowId).HasForeignKey(d => d.TvmShowId).HasConstraintName("Episodes_FK");
        });

        modelBuilder.Entity<Episodesfromtodayback>(entity =>
        {
            entity.HasNoKey().ToView("episodesfromtodayback");
        });

        modelBuilder.Entity<Episodesfullinfo>(entity =>
        {
            entity.HasNoKey().ToView("episodesfullinfo");
        });

        modelBuilder.Entity<Episodestoacquire>(entity =>
        {
            entity.HasNoKey().ToView("episodestoacquire");
        });

        modelBuilder.Entity<Followed>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Followed");

            entity.HasIndex(e => e.TvmShowId, "TvmFollowedShows_UN").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.TvmShowId).HasColumnType("int(11)");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<LastShowEvaluated>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("LastShowEvaluated");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.ShowId).HasColumnType("int(11)");
        });

        modelBuilder.Entity<MediaType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.MediaType1, "MediaTypes_UN").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AutoDelete).HasMaxLength(3).IsFixedLength();
            entity.Property(e => e.MediaType1).HasMaxLength(10).HasColumnName("MediaType");
            entity.Property(e => e.PlexLocation).HasMaxLength(100);
        });

        modelBuilder.Entity<Nobroadcastdate>(entity =>
        {
            entity.HasNoKey().ToView("nobroadcastdate");

            entity.Property(e => e.BroadcastDate).HasColumnType("datetime");
            entity.Property(e => e.Episode).HasColumnType("int(11)");
            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.PlexDate).HasColumnType("datetime");
            entity.Property(e => e.PlexStatus).HasMaxLength(10).HasDefaultValueSql("' '");
            entity.Property(e => e.Season).HasColumnType("int(11)");
            entity.Property(e => e.SeasonEpisode).HasMaxLength(10);
            entity.Property(e => e.TvmEpisodeId).HasColumnType("int(11)");
            entity.Property(e => e.TvmShowId).HasColumnType("int(11)");
            entity.Property(e => e.TvmUrl).HasMaxLength(255).HasDefaultValueSql("' '");
            entity.Property(e => e.UpdateDate).HasDefaultValueSql("curdate()").HasColumnType("datetime");
        });

        modelBuilder.Entity<Notinfollowed>(entity =>
        {
            entity.HasNoKey().ToView("notinfollowed");

            entity.Property(e => e.FollowedTvmShowId).HasColumnType("int(11)");
            entity.Property(e => e.ShowName).HasMaxLength(100);
            entity.Property(e => e.ShowsTvmShowId).HasColumnType("int(11)");
            entity.Property(e => e.Status).HasMaxLength(10);
            entity.Property(e => e.Url).HasMaxLength(175).HasDefaultValueSql("' '").HasColumnName("URL");
        });

        modelBuilder.Entity<Notinshow>(entity =>
        {
            entity.HasNoKey().ToView("notinshows");

            entity.Property(e => e.FollowedTvmShowId).HasColumnType("int(11)");
        });

        modelBuilder.Entity<Orphanedepisode>(entity =>
        {
            entity.HasNoKey().ToView("orphanedepisodes");
        });

        modelBuilder.Entity<PlexStatus>(entity =>
        {
            entity.HasKey(e => e.PlexStatus1).HasName("PRIMARY");

            entity.Property(e => e.PlexStatus1).HasMaxLength(10).HasColumnName("PlexStatus");
        });

        modelBuilder.Entity<PlexWatchedEpisode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => new {e.TvmShowId, e.TvmEpisodeId}, "PlexWatchedEpisodes_UN").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.PlexEpisodeNum).HasColumnType("int(11)");
            entity.Property(e => e.PlexSeasonEpisode).HasMaxLength(20);
            entity.Property(e => e.PlexSeasonNum).HasColumnType("int(11)");
            entity.Property(e => e.PlexShowName).HasMaxLength(100);
            entity.Property(e => e.PlexWatchedDate).HasColumnType("datetime");
            entity.Property(e => e.TvmEpisodeId).HasColumnType("int(11)");
            entity.Property(e => e.TvmShowId).HasColumnType("int(11)");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.CleanedShowName, "Shows_CleanedShowName_IDX");

            entity.HasIndex(e => e.TvmStatus, "Shows_FK");

            entity.HasIndex(e => e.ShowStatus, "Shows_FK_1");

            entity.HasIndex(e => e.MediaType, "Shows_FK_3");

            entity.HasIndex(e => e.ShowName, "Shows_ShowName_IDX");

            entity.HasIndex(e => e.TvmShowId, "Shows_TvmShowId").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AcquireShowName).HasMaxLength(100).HasDefaultValueSql("' '");
            entity.Property(e => e.CleanedShowName).HasMaxLength(100).HasDefaultValueSql("' '");
            entity.Property(e => e.Finder).HasMaxLength(10).HasDefaultValueSql("'Multi'");
            entity.Property(e => e.MediaType).HasMaxLength(10);
            entity.Property(e => e.PlexShowName).HasMaxLength(100).HasDefaultValueSql("' '");
            entity.Property(e => e.PremiereDate).HasDefaultValueSql("'1970-01-01 00:00:00'").HasColumnType("datetime");
            entity.Property(e => e.ShowName).HasMaxLength(100);
            entity.Property(e => e.ShowStatus).HasMaxLength(20);
            entity.Property(e => e.TvmShowId).HasColumnType("int(11)");
            entity.Property(e => e.TvmStatus).HasMaxLength(10);
            entity.Property(e => e.TvmUrl).HasMaxLength(175).HasDefaultValueSql("' '");
            entity.Property(e => e.UpdateDate).HasDefaultValueSql("curdate()").HasColumnType("datetime");

            entity.HasOne(d => d.MediaTypeNavigation).WithMany(p => p.Shows).HasPrincipalKey(p => p.MediaType1).HasForeignKey(d => d.MediaType).HasConstraintName("Shows_FK_3");

            entity.HasOne(d => d.ShowStatusNavigation).WithMany(p => p.Shows).HasForeignKey(d => d.ShowStatus).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("Shows_FK_1");

            entity.HasOne(d => d.TvmShow).WithOne(p => p.Show).HasPrincipalKey<TvmShowUpdate>(p => p.TvmShowId).HasForeignKey<Show>(d => d.TvmShowId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("Shows_FK_2");

            entity.HasOne(d => d.TvmStatusNavigation).WithMany(p => p.Shows).HasForeignKey(d => d.TvmStatus).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("Shows_FK");
        });

        modelBuilder.Entity<ShowRssFeed>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ShowRssFeed");

            entity.HasIndex(e => e.ShowName, "ShowRssFeed_UN").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.ShowName).HasMaxLength(150);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.Url).HasMaxLength(1500);
        });

        modelBuilder.Entity<ShowStatus>(entity =>
        {
            entity.HasKey(e => e.ShowStatus1).HasName("PRIMARY");

            entity.Property(e => e.ShowStatus1).HasMaxLength(20).HasDefaultValueSql("' '").HasColumnName("ShowStatus");
        });

        modelBuilder.Entity<Showepisodecount>(entity =>
        {
            entity.HasNoKey().ToView("showepisodecount");

            entity.Property(e => e.EpisodeCount).HasColumnType("bigint(21)");
            entity.Property(e => e.ShowName).HasMaxLength(100);
            entity.Property(e => e.ShowStatus).HasMaxLength(20);
            entity.Property(e => e.ShowsTvmShowId).HasColumnType("int(11)");
            entity.Property(e => e.Status).HasMaxLength(10);
            entity.Property(e => e.Url).HasMaxLength(175).HasDefaultValueSql("' '").HasColumnName("URL");
        });

        modelBuilder.Entity<Showsnotinfollowed>(entity =>
        {
            entity.HasNoKey().ToView("showsnotinfollowed");
        });

        modelBuilder.Entity<Showstorefresh>(entity =>
        {
            entity.HasNoKey().ToView("showstorefresh");

            entity.Property(e => e.PremiereDate).HasDefaultValueSql("'1970-01-01 00:00:00'").HasColumnType("datetime");
            entity.Property(e => e.ShowName).HasMaxLength(100);
            entity.Property(e => e.ShowStatus).HasMaxLength(20);
            entity.Property(e => e.TvmShowId).HasColumnType("int(11)").HasColumnName("TvmShowID");
            entity.Property(e => e.TvmStatus).HasMaxLength(10);
            entity.Property(e => e.TvmUrl).HasMaxLength(175).HasDefaultValueSql("' '");
            entity.Property(e => e.UpdateDate).HasDefaultValueSql("curdate()").HasColumnType("datetime");
        });

        modelBuilder.Entity<TvmShowUpdate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.TvmShowId, "TvmShowUpdates_TvmShowId").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.TvmShowId).HasColumnType("int(11)");
            entity.Property(e => e.TvmUpdateDate).HasDefaultValueSql("'1900-01-01 00:00:00'").HasColumnType("datetime");
            entity.Property(e => e.TvmUpdateEpoch).HasColumnType("int(11)");
        });

        modelBuilder.Entity<TvmStatus>(entity =>
        {
            entity.HasKey(e => e.TvmStatus1).HasName("PRIMARY");

            entity.Property(e => e.TvmStatus1).HasMaxLength(20).HasDefaultValueSql("' '").HasColumnName("TvmStatus");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
