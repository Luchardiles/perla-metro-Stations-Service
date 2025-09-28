using Microsoft.EntityFrameworkCore;
using perla_metro_Stations_Service.src.Models;
using perla_metro_Stations_Service.src.Models.Enums;

/// <summary>   
/// Contexto de base de datos para MySQL con configuración de la entidad Station.
namespace perla_metro_Stations_Service.src.Data
{
    public class MysqlDbContext : DbContext
    {
        public MysqlDbContext(DbContextOptions<MysqlDbContext> options) : base(options)
        {
        }

        public DbSet<Station> Stations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para la entidad Station
            modelBuilder.Entity<Station>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnType("char(36)")
                    .ValueGeneratedNever(); // UUID no autoincremental

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasConversion<int>(); // Guardar enum como int

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("timestamp");

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                // Índices
                entity.HasIndex(e => e.Name).HasDatabaseName("IX_Station_Name");
                entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_Station_IsDeleted");
                entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_Station_IsActive");

                // Filtro global para soft delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<Station>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }
    }
}