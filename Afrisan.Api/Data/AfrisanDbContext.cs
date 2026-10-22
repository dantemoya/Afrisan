using Afrisan.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Afrisan.Api.Data
{
    public class AfrisanDbContext : IdentityUserContext<Usuario>
    {
        public AfrisanDbContext(
            DbContextOptions<AfrisanDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cilindro> Cilindros { get; set; }

        public DbSet<GasRefrigerante> GasesRefrigerantes { get; set; }

        public DbSet<MovimientoInventario> MovimientosInventario
        {
            get;
            set;
        }

        public DbSet<OrdenTrabajo> OrdenesTrabajo
        {
            get;
            set;
        }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<IdentityRole> Roles => Set<IdentityRole>();
        public DbSet<IdentityRoleClaim<string>> RoleClaims => Set<IdentityRoleClaim<string>>();
        public DbSet<Sucursal> Sucursales => Set<Sucursal>();
        public DbSet<Equipo> Equipos => Set<Equipo>();
        public DbSet<CausaFalla> CausasFalla => Set<CausaFalla>();
        public DbSet<Intervencion> Intervenciones => Set<Intervencion>();
        public DbSet<PrecioRefrigerante> PreciosRefrigerantes => Set<PrecioRefrigerante>();
        public DbSet<RegistroAuditoria> Auditorias => Set<RegistroAuditoria>();
        public DbSet<CertificadoDisposicion> CertificadosDisposicion => Set<CertificadoDisposicion>();
        public DbSet<CertificadoCilindro> CertificadosCilindros => Set<CertificadoCilindro>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            // IMPORTANTE:
            // Configura primero las tablas internas de ASP.NET Identity.
            base.OnModelCreating(modelBuilder);

            // IdentityUserContext conserva las tablas de cuentas. Se configuran los
            // roles explícitamente para usar RolId sin crear AspNetUserRoles.
            var rol = modelBuilder.Entity<IdentityRole>();
            rol.ToTable("AspNetRoles");
            rol.HasKey(x => x.Id);
            rol.HasIndex(x => x.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
            rol.Property(x => x.Name).HasMaxLength(256);
            rol.Property(x => x.NormalizedName).HasMaxLength(256);
            rol.Property(x => x.ConcurrencyStamp).IsConcurrencyToken();
            rol.HasMany<IdentityRoleClaim<string>>().WithOne().HasForeignKey(x => x.RoleId).IsRequired();
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims").HasKey(x => x.Id);
            modelBuilder.Entity<Usuario>()
                .HasOne(x => x.Rol).WithMany().HasForeignKey(x => x.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.ConfigureDocumentedModel();

            // =====================================================
            // CILINDROS
            // =====================================================

            modelBuilder.Entity<Cilindro>()
                .HasIndex(c => c.CodigoQr)
                .IsUnique();

            // =====================================================
            // ÓRDENES DE TRABAJO
            // =====================================================

            modelBuilder.Entity<OrdenTrabajo>()
                .HasIndex(o => o.Codigo)
                .IsUnique();

            // =====================================================
            // MOVIMIENTOS DE INVENTARIO
            // =====================================================

            modelBuilder.Entity<MovimientoInventario>()
                .HasOne(m => m.Cilindro)
                .WithMany()
                .HasForeignKey(m => m.CilindroId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovimientoInventario>()
                .HasOne(m => m.OrdenTrabajo)
                .WithMany(o => o.Movimientos)
                .HasForeignKey(m => m.OrdenTrabajoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
