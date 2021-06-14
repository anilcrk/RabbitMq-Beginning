using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace FileCreateWorkerService.Models
{
    public partial class TurkeyCityContext : DbContext
    {
        public TurkeyCityContext()
        {
        }

        public TurkeyCityContext(DbContextOptions<TurkeyCityContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ulkeler> Ulkelers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Ulkeler>(entity =>
            {
                entity.HasKey(e => e.UlkeId);

                entity.ToTable("Ulkeler");

                entity.Property(e => e.UlkeId).ValueGeneratedNever();

                entity.Property(e => e.IkiliKod)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.Property(e => e.TelKodu)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.Property(e => e.UcluKod)
                    .IsRequired()
                    .HasMaxLength(3);

                entity.Property(e => e.UlkeAdi)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
