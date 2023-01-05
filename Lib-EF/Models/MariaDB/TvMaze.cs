using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Common_Lib;

namespace PlexMediaControl.Models.MariaDB
{
    public partial class TvMaze : DbContext
    {
        public TvMaze()
        {
        }

        public TvMaze(DbContextOptions<TvMaze> options)
            : base(options)
        {
        }

        public virtual DbSet<ActionItem> ActionItems { get; set; } = null!;
        public virtual DbSet<Episode> Episodes { get; set; } = null!;
        public virtual DbSet<Episodesfromtodayback> Episodesfromtodaybacks { get; set; } = null!;
        public virtual DbSet<Episodesfullinfo> Episodesfullinfos { get; set; } = null!;
        public virtual DbSet<Episodestoacquire> Episodestoacquires { get; set; } = null!;
        public virtual DbSet<Followed> Followeds { get; set; } = null!;
        public virtual DbSet<LastShowEvaluated> LastShowEvaluateds { get; set; } = null!;
        public virtual DbSet<MediaType> MediaTypes { get; set; } = null!;
        public virtual DbSet<Movie> Movies { get; set; } = null!;
        public virtual DbSet<Nobroadcastdate> Nobroadcastdates { get; set; } = null!;
        public virtual DbSet<Notinfollowed> Notinfolloweds { get; set; } = null!;
        public virtual DbSet<Notinshow> Notinshows { get; set; } = null!;
        public virtual DbSet<Orphanedepisode> Orphanedepisodes { get; set; } = null!;
        public virtual DbSet<PlexStatus> PlexStatuses { get; set; } = null!;
        public virtual DbSet<PlexWatchedEpisode> PlexWatchedEpisodes { get; set; } = null!;
        public virtual DbSet<Show> Shows { get; set; } = null!;
        public virtual DbSet<ShowRssFeed> ShowRssFeeds { get; set; } = null!;
        public virtual DbSet<ShowStatus> ShowStatuses { get; set; } = null!;
        public virtual DbSet<Showepisodecount> Showepisodecounts { get; set; } = null!;
        public virtual DbSet<Showsnotinfollowed> Showsnotinfolloweds { get; set; } = null!;
        public virtual DbSet<Showstorefresh> Showstorefreshes { get; set; } = null!;
        public virtual DbSet<TvmShowUpdate> TvmShowUpdates { get; set; } = null!;
        public virtual DbSet<TvmStatus> TvmStatuses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var appInfo = new AppInfo(application: "PlexMediaControl", program: "Db Context");
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql(appInfo.ActiveDbConn, Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.5.9-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<ActionItem>(entity =>
            {
                entity.HasIndex(e => new { e.Program, e.Message, e.UpdateDateTime }, "ActionItems_UN")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Message).HasMaxLength(500);

                entity.Property(e => e.Program).HasMaxLength(25);

                entity.Property(e => e.UpdateDateTime).HasMaxLength(20);
            });

            modelBuilder.Entity<Episode>(entity =>
            {
                entity.HasIndex(e => e.TvmShowId, "Episodes_FK");

                entity.HasIndex(e => e.PlexStatus, "Episodes_FK_1");

                entity.HasIndex(e => e.TvmEpisodeId, "Episodes_UN")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Episode1)
                    .HasColumnType("int(11)")
                    .HasColumnName("Episode");

                entity.Property(e => e.PlexStatus)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.Season).HasColumnType("int(11)");

                entity.Property(e => e.SeasonEpisode).HasMaxLength(10);

                entity.Property(e => e.TvmEpisodeId).HasColumnType("int(11)");

                entity.Property(e => e.TvmShowId).HasColumnType("int(11)");

                entity.Property(e => e.TvmUrl)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("curdate()");

                entity.HasOne(d => d.PlexStatusNavigation)
                    .WithMany(p => p.Episodes)
                    .HasForeignKey(d => d.PlexStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Episodes_FK_1");

                entity.HasOne(d => d.TvmShow)
                    .WithMany(p => p.Episodes)
                    .HasPrincipalKey(p => p.TvmShowId)
                    .HasForeignKey(d => d.TvmShowId)
                    .HasConstraintName("Episodes_FK");
            });

            modelBuilder.Entity<Episodesfromtodayback>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("episodesfromtodayback");

                entity.Property(e => e.AltShowName)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.CleanedShowName)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.Episode).HasColumnType("int(11)");

                entity.Property(e => e.Finder)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("'Multi'");

                entity.Property(e => e.PlexStatus)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.Season).HasColumnType("int(11)");

                entity.Property(e => e.SeasonEpisode).HasMaxLength(10);

                entity.Property(e => e.ShowName).HasMaxLength(100);

                entity.Property(e => e.ShowStatus).HasMaxLength(10);

                entity.Property(e => e.TvmEpisodeId).HasColumnType("int(11)");

                entity.Property(e => e.TvmShowId).HasColumnType("int(11)");

                entity.Property(e => e.TvmUrl)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("curdate()");
            });

            modelBuilder.Entity<Episodesfullinfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("episodesfullinfo");

                entity.Property(e => e.AltShowName)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.AutoDelete)
                    .HasMaxLength(3)
                    .IsFixedLength();

                entity.Property(e => e.CleanedShowName)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.Episode).HasColumnType("int(11)");

                entity.Property(e => e.Finder)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("'Multi'");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.MediaType).HasMaxLength(10);

                entity.Property(e => e.PlexStatus)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.Season).HasColumnType("int(11)");

                entity.Property(e => e.SeasonEpisode).HasMaxLength(10);

                entity.Property(e => e.ShowName).HasMaxLength(100);

                entity.Property(e => e.ShowUpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("curdate()");

                entity.Property(e => e.TvmEpisodeId).HasColumnType("int(11)");

                entity.Property(e => e.TvmShowId).HasColumnType("int(11)");

                entity.Property(e => e.TvmStatus).HasMaxLength(10);

                entity.Property(e => e.TvmUrl)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("curdate()");
            });

            modelBuilder.Entity<Episodestoacquire>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("episodestoacquire");

                entity.Property(e => e.AltShowName)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.CleanedShowName)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.Episode).HasColumnType("int(11)");

                entity.Property(e => e.Finder)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("'Multi'");

                entity.Property(e => e.PlexStatus)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.Season).HasColumnType("int(11)");

                entity.Property(e => e.SeasonEpisode).HasMaxLength(10);

                entity.Property(e => e.ShowName).HasMaxLength(100);

                entity.Property(e => e.TvmEpisodeId).HasColumnType("int(11)");

                entity.Property(e => e.TvmShowId).HasColumnType("int(11)");

                entity.Property(e => e.TvmUrl)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("' '");
            });

            modelBuilder.Entity<Followed>(entity =>
            {
                entity.ToTable("Followed");

                entity.HasIndex(e => e.TvmShowId, "TvmFollowedShows_UN")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.TvmShowId).HasColumnType("int(11)");
            });

            modelBuilder.Entity<LastShowEvaluated>(entity =>
            {
                entity.ToTable("LastShowEvaluated");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ShowId).HasColumnType("int(11)");
            });

            modelBuilder.Entity<MediaType>(entity =>
            {
                entity.HasIndex(e => e.MediaType1, "MediaTypes_UN")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AutoDelete)
                    .HasMaxLength(3)
                    .IsFixedLength();

                entity.Property(e => e.MediaType1)
                    .HasMaxLength(10)
                    .HasColumnName("MediaType");

                entity.Property(e => e.PlexLocation).HasMaxLength(100);
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => e.MediaType, "Movies_FK");

                entity.HasIndex(e => e.Name, "Movies_UN")
                    .IsUnique();

                entity.Property(e => e.AltName).HasMaxLength(100);

                entity.Property(e => e.CleanedName).HasMaxLength(100);

                entity.Property(e => e.FinderDate).HasMaxLength(10);

                entity.Property(e => e.MediaType).HasMaxLength(10);

                entity.Property(e => e.MovieNumber).HasColumnType("int(11)");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.SeriesName).HasMaxLength(100);

                entity.HasOne(d => d.MediaTypeNavigation)
                    .WithMany()
                    .HasPrincipalKey(p => p.MediaType1)
                    .HasForeignKey(d => d.MediaType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Movies_FK");
            });

            modelBuilder.Entity<Nobroadcastdate>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("nobroadcastdate");

                entity.Property(e => e.Episode).HasColumnType("int(11)");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.PlexStatus)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.Season).HasColumnType("int(11)");

                entity.Property(e => e.SeasonEpisode).HasMaxLength(10);

                entity.Property(e => e.TvmEpisodeId).HasColumnType("int(11)");

                entity.Property(e => e.TvmShowId).HasColumnType("int(11)");

                entity.Property(e => e.TvmUrl)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("curdate()");
            });

            modelBuilder.Entity<Notinfollowed>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("notinfollowed");

                entity.Property(e => e.FollowedTvmShowId).HasColumnType("int(11)");

                entity.Property(e => e.ShowName).HasMaxLength(100);

                entity.Property(e => e.ShowsTvmShowId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasMaxLength(10);

                entity.Property(e => e.Url)
                    .HasMaxLength(175)
                    .HasColumnName("URL")
                    .HasDefaultValueSql("' '");
            });

            modelBuilder.Entity<Notinshow>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("notinshows");

                entity.Property(e => e.FollowedTvmShowId).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Orphanedepisode>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("orphanedepisodes");

                entity.Property(e => e.TvmShowId).HasColumnType("int(11)");
            });

            modelBuilder.Entity<PlexStatus>(entity =>
            {
                entity.HasKey(e => e.PlexStatus1)
                    .HasName("PRIMARY");

                entity.Property(e => e.PlexStatus1)
                    .HasMaxLength(10)
                    .HasColumnName("PlexStatus");
            });

            modelBuilder.Entity<PlexWatchedEpisode>(entity =>
            {
                entity.HasIndex(e => new { e.TvmShowId, e.TvmEpisodeId }, "PlexWatchedEpisodes_UN")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.PlexEpisodeNum).HasColumnType("int(11)");

                entity.Property(e => e.PlexSeasonEpisode).HasMaxLength(20);

                entity.Property(e => e.PlexSeasonNum).HasColumnType("int(11)");

                entity.Property(e => e.PlexShowName).HasMaxLength(100);

                entity.Property(e => e.PlexWatchedDate).HasMaxLength(10);

                entity.Property(e => e.TvmEpisodeId).HasColumnType("int(11)");

                entity.Property(e => e.TvmShowId).HasColumnType("int(11)");

                entity.Property(e => e.UpdateDate).HasMaxLength(10);
            });

            modelBuilder.Entity<Show>(entity =>
            {
                entity.HasIndex(e => e.CleanedShowName, "Shows_CleanedShowName_IDX");

                entity.HasIndex(e => e.TvmStatus, "Shows_FK");

                entity.HasIndex(e => e.ShowStatus, "Shows_FK_1");

                entity.HasIndex(e => e.MediaType, "Shows_FK_3");

                entity.HasIndex(e => e.ShowName, "Shows_ShowName_IDX");

                entity.HasIndex(e => e.TvmShowId, "Shows_TvmShowId")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AltShowname)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.CleanedShowName)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.Finder)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("'Multi'");

                entity.Property(e => e.MediaType).HasMaxLength(10);

                entity.Property(e => e.PremiereDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1970-01-01 00:00:00'");

                entity.Property(e => e.ShowName).HasMaxLength(100);

                entity.Property(e => e.ShowStatus).HasMaxLength(20);

                entity.Property(e => e.TvmShowId).HasColumnType("int(11)");

                entity.Property(e => e.TvmStatus).HasMaxLength(10);

                entity.Property(e => e.TvmUrl)
                    .HasMaxLength(175)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("curdate()");

                entity.HasOne(d => d.MediaTypeNavigation)
                    .WithMany(p => p.Shows)
                    .HasPrincipalKey(p => p.MediaType1)
                    .HasForeignKey(d => d.MediaType)
                    .HasConstraintName("Shows_FK_3");

                entity.HasOne(d => d.ShowStatusNavigation)
                    .WithMany(p => p.Shows)
                    .HasForeignKey(d => d.ShowStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Shows_FK_1");

                entity.HasOne(d => d.TvmShow)
                    .WithOne(p => p.Show)
                    .HasPrincipalKey<TvmShowUpdate>(p => p.TvmShowId)
                    .HasForeignKey<Show>(d => d.TvmShowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Shows_FK_2");

                entity.HasOne(d => d.TvmStatusNavigation)
                    .WithMany(p => p.Shows)
                    .HasForeignKey(d => d.TvmStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Shows_FK");
            });

            modelBuilder.Entity<ShowRssFeed>(entity =>
            {
                entity.ToTable("ShowRssFeed");

                entity.HasIndex(e => e.ShowName, "ShowRssFeed_UN")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ShowName).HasMaxLength(150);

                entity.Property(e => e.UpdateDate).HasMaxLength(10);

                entity.Property(e => e.Url).HasMaxLength(1500);
            });

            modelBuilder.Entity<ShowStatus>(entity =>
            {
                entity.HasKey(e => e.ShowStatus1)
                    .HasName("PRIMARY");

                entity.Property(e => e.ShowStatus1)
                    .HasMaxLength(20)
                    .HasColumnName("ShowStatus")
                    .HasDefaultValueSql("' '");
            });

            modelBuilder.Entity<Showepisodecount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("showepisodecount");

                entity.Property(e => e.EpisodeCount).HasColumnType("bigint(21)");

                entity.Property(e => e.ShowName).HasMaxLength(100);

                entity.Property(e => e.ShowStatus).HasMaxLength(20);

                entity.Property(e => e.ShowsTvmShowId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasMaxLength(10);

                entity.Property(e => e.Url)
                    .HasMaxLength(175)
                    .HasColumnName("URL")
                    .HasDefaultValueSql("' '");
            });

            modelBuilder.Entity<Showsnotinfollowed>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("showsnotinfollowed");

                entity.Property(e => e.AltShowname)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.CleanedShowName)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.Finder)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("'Multi'");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.MediaType).HasMaxLength(10);

                entity.Property(e => e.PremiereDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1970-01-01 00:00:00'");

                entity.Property(e => e.ShowName).HasMaxLength(100);

                entity.Property(e => e.ShowStatus).HasMaxLength(20);

                entity.Property(e => e.TvmShowId).HasColumnType("int(11)");

                entity.Property(e => e.TvmStatus).HasMaxLength(10);

                entity.Property(e => e.TvmUrl)
                    .HasMaxLength(175)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("curdate()");
            });

            modelBuilder.Entity<Showstorefresh>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("showstorefresh");

                entity.Property(e => e.PremiereDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1970-01-01 00:00:00'");

                entity.Property(e => e.ShowName).HasMaxLength(100);

                entity.Property(e => e.ShowStatus).HasMaxLength(20);

                entity.Property(e => e.TvmShowId)
                    .HasColumnType("int(11)")
                    .HasColumnName("TvmShowID");

                entity.Property(e => e.TvmStatus).HasMaxLength(10);

                entity.Property(e => e.TvmUrl)
                    .HasMaxLength(175)
                    .HasDefaultValueSql("' '");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("curdate()");
            });

            modelBuilder.Entity<TvmShowUpdate>(entity =>
            {
                entity.HasIndex(e => e.TvmShowId, "TvmShowUpdates_TvmShowId")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.TvmShowId).HasColumnType("int(11)");

                entity.Property(e => e.TvmUpdateDate).HasDefaultValueSql("'1900-01-01'");

                entity.Property(e => e.TvmUpdateEpoch).HasColumnType("int(11)");
            });

            modelBuilder.Entity<TvmStatus>(entity =>
            {
                entity.HasKey(e => e.TvmStatus1)
                    .HasName("PRIMARY");

                entity.Property(e => e.TvmStatus1)
                    .HasMaxLength(20)
                    .HasColumnName("TvmStatus")
                    .HasDefaultValueSql("' '");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
