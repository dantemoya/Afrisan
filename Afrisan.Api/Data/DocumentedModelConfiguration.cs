using Afrisan.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Afrisan.Api.Data;

internal static class DocumentedModelConfiguration
{
    internal static void ConfigureDocumentedModel(this ModelBuilder model)
    {
        var cliente = model.Entity<Cliente>();
        cliente.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        cliente.Property(x => x.Rut).HasMaxLength(20);
        cliente.Property(x => x.Telefono).HasMaxLength(40);
        cliente.HasIndex(x => x.Rut).IsUnique().HasFilter("\"Rut\" IS NOT NULL");
        cliente.ToTable("Clientes", t => t.HasCheckConstraint("CK_Clientes_Nombre", "length(btrim(\"Nombre\")) > 0"));

        var sucursal = model.Entity<Sucursal>();
        sucursal.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        sucursal.Property(x => x.Direccion).HasMaxLength(400).IsRequired();
        sucursal.HasIndex(x => new { x.ClienteId, x.Nombre });
        sucursal.HasOne<Cliente>().WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
        sucursal.ToTable("Sucursales", t => t.HasCheckConstraint("CK_Sucursales_Nombre", "length(btrim(\"Nombre\")) > 0"));

        var equipo = model.Entity<Equipo>();
        equipo.Property(x => x.Codigo).HasMaxLength(80).IsRequired();
        equipo.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        equipo.Property(x => x.Tipo).HasMaxLength(100).IsRequired();
        equipo.Property(x => x.CapacidadNominalKg).HasPrecision(18, 3);
        equipo.HasIndex(x => new { x.SucursalId, x.Codigo }).IsUnique();
        equipo.HasAlternateKey(x => new { x.Id, x.SucursalId });
        equipo.HasOne<Sucursal>().WithMany().HasForeignKey(x => x.SucursalId).OnDelete(DeleteBehavior.Restrict);
        equipo.HasOne<GasRefrigerante>().WithMany().HasForeignKey(x => x.GasRefrigeranteId).OnDelete(DeleteBehavior.Restrict);
        equipo.ToTable("Equipos", t =>
        {
            t.HasCheckConstraint("CK_Equipos_Capacidad", "\"CapacidadNominalKg\" > 0");
            t.HasCheckConstraint("CK_Equipos_Codigo", "length(btrim(\"Codigo\")) > 0");
        });

        var causa = model.Entity<CausaFalla>();
        causa.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
        causa.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        causa.HasIndex(x => x.Codigo).IsUnique();
        causa.ToTable("CausasFalla", t => t.HasCheckConstraint("CK_CausasFalla_Codigo", "length(btrim(\"Codigo\")) > 0"));

        var gas = model.Entity<GasRefrigerante>();
        gas.Property(x => x.FactorGwp).HasPrecision(12, 3);
        gas.Property(x => x.FuenteGwp).HasMaxLength(300);
        gas.Property(x => x.MetodologiaGwp).HasMaxLength(100);
        gas.ToTable("GasesRefrigerantes", t =>
        {
            t.HasCheckConstraint("CK_Gases_FactorGwp", "\"FactorGwp\" IS NULL OR \"FactorGwp\" >= 0");
        });

        var precio = model.Entity<PrecioRefrigerante>();
        precio.Property(x => x.PrecioKg).HasPrecision(18, 4);
        precio.Property(x => x.Moneda).HasMaxLength(3);
        precio.HasIndex(x => new { x.GasRefrigeranteId, x.Moneda, x.VigenteDesde }).IsUnique();
        precio.HasOne<GasRefrigerante>().WithMany().HasForeignKey(x => x.GasRefrigeranteId).OnDelete(DeleteBehavior.Restrict);
        precio.ToTable("PreciosRefrigerantes", t =>
        {
            t.HasCheckConstraint("CK_Precios_Importe", "\"PrecioKg\" >= 0");
            t.HasCheckConstraint("CK_Precios_Moneda", "\"Moneda\" ~ '^[A-Z]{3}$'");
        });

        var cilindro = model.Entity<Cilindro>();
        cilindro.Property(x => x.NumeroSerie).HasMaxLength(100);
        cilindro.HasIndex(x => x.NumeroSerie).IsUnique().HasFilter("\"NumeroSerie\" IS NOT NULL");
        // Se conserva PesoActualKg para compatibilidad con la Web: es el último peso bruto,
        // nunca sustituye el historial de pesajes. Los QR se generan a partir de CodigoQr.
        cilindro.Property(x => x.CapacidadKg).HasPrecision(18, 3);
        cilindro.Property(x => x.TaraKg).HasPrecision(18, 3);
        cilindro.Property(x => x.PesoActualKg).HasPrecision(18, 3);

        var orden = model.Entity<OrdenTrabajo>();
        orden.Property(x => x.Origen).HasMaxLength(50).HasDefaultValue("Local");
        orden.Property(x => x.CodigoExterno).HasMaxLength(100);
        orden.HasIndex(x => new { x.Origen, x.CodigoExterno }).IsUnique().HasFilter("\"CodigoExterno\" IS NOT NULL");
        orden.HasOne<Sucursal>().WithMany().HasForeignKey(x => x.SucursalId).OnDelete(DeleteBehavior.Restrict);
        orden.HasOne<Usuario>().WithMany().HasForeignKey(x => x.TecnicoAsignadoId).OnDelete(DeleteBehavior.Restrict);
        orden.HasOne<Usuario>().WithMany().HasForeignKey(x => x.AutorizadorId).OnDelete(DeleteBehavior.Restrict);

        var movimiento = model.Entity<MovimientoInventario>();
        movimiento.Property(x => x.IdOperacion).HasDefaultValueSql("gen_random_uuid()");
        movimiento.Property(x => x.FotoPesajeRuta).HasMaxLength(1024);
        movimiento.Property(x => x.PesoKg).HasPrecision(18, 3);
        movimiento.HasIndex(x => x.IdOperacion).IsUnique();
        movimiento.HasAlternateKey(x => new { x.Id, x.CilindroId });
        movimiento.HasIndex(x => x.CilindroId).HasDatabaseName("UX_Movimientos_CustodiaAbierta")
            .IsUnique().HasFilter("\"CustodiaAbierta\"");
        movimiento.HasIndex(x => new { x.CilindroId, x.FechaMovimiento });
        movimiento.HasIndex(x => x.DespachoId).IsUnique().HasFilter("\"DespachoId\" IS NOT NULL");
        movimiento.HasOne<Usuario>().WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        movimiento.HasOne<MovimientoInventario>().WithMany()
            .HasForeignKey(x => new { x.DespachoId, x.CilindroId })
            .HasPrincipalKey(x => new { x.Id, x.CilindroId }).OnDelete(DeleteBehavior.Restrict);
        movimiento.ToTable("MovimientosInventario", t =>
        {
            t.HasCheckConstraint("CK_Movimientos_Peso", "\"PesoKg\" >= 0");
            t.HasCheckConstraint("CK_Movimientos_Custodia", "NOT \"CustodiaAbierta\" OR \"TipoMovimiento\" = 'Despacho'");
            t.HasCheckConstraint("CK_Movimientos_Retorno", "\"DespachoId\" IS NULL OR \"TipoMovimiento\" = 'Retorno'");
        });

        var intervencion = model.Entity<Intervencion>();
        intervencion.Property(x => x.IdOperacion).HasDefaultValueSql("gen_random_uuid()");
        intervencion.HasIndex(x => x.IdOperacion).IsUnique();
        intervencion.HasIndex(x => new { x.EquipoId, x.FechaIntervencion });
        intervencion.HasIndex(x => new { x.OrdenTrabajoId, x.CilindroId });
        intervencion.Property(x => x.KgUtilizados).HasPrecision(18, 3);
        intervencion.Property(x => x.KgEmitidos).HasPrecision(18, 3);
        intervencion.Property(x => x.PesoRestanteBrutoKg).HasPrecision(18, 3);
        intervencion.Property(x => x.FactorGwpAplicado).HasPrecision(12, 3);
        intervencion.Property(x => x.PrecioKgAplicado).HasPrecision(18, 4);
        intervencion.Property(x => x.EmisionesCo2eToneladas).HasPrecision(20, 6)
            .HasComputedColumnSql("\"KgEmitidos\" * \"FactorGwpAplicado\" / 1000.0", stored: true);
        intervencion.Property(x => x.CostoTotal).HasPrecision(20, 4)
            .HasComputedColumnSql("\"KgUtilizados\" * \"PrecioKgAplicado\"", stored: true);
        intervencion.Property(x => x.Moneda).HasMaxLength(3);
        intervencion.Property(x => x.EstadoSemaforo).HasMaxLength(20);
        intervencion.Property(x => x.EstadoIntervencion).HasMaxLength(20);
        intervencion.Property(x => x.FotoBalanzaRuta).HasMaxLength(1024);
        intervencion.Property(x => x.PlanCorreccion).HasMaxLength(2000);
        intervencion.Property(x => x.FuenteGwpAplicada).HasMaxLength(300);
        intervencion.Property(x => x.MetodologiaGwpAplicada).HasMaxLength(100);
        intervencion.Property(x => x.EstadoSincronizacion).HasMaxLength(20);
        intervencion.Property(x => x.DispositivoId).HasMaxLength(100);
        intervencion.Property(x => x.Observaciones).HasMaxLength(2000);
        intervencion.HasOne<OrdenTrabajo>().WithMany(x => x.Intervenciones).HasForeignKey(x => x.OrdenTrabajoId).OnDelete(DeleteBehavior.Restrict);
        intervencion.HasOne<Cilindro>().WithMany().HasForeignKey(x => x.CilindroId).OnDelete(DeleteBehavior.Restrict);
        intervencion.HasOne<MovimientoInventario>().WithMany()
            .HasForeignKey(x => new { x.DespachoId, x.CilindroId })
            .HasPrincipalKey(x => new { x.Id, x.CilindroId }).OnDelete(DeleteBehavior.Restrict);
        intervencion.HasOne<Sucursal>().WithMany().HasForeignKey(x => x.SucursalId).OnDelete(DeleteBehavior.Restrict);
        intervencion.HasOne<Equipo>().WithMany()
            .HasForeignKey(x => new { x.EquipoId, x.SucursalId })
            .HasPrincipalKey(x => new { x.Id, x.SucursalId }).OnDelete(DeleteBehavior.Restrict);
        intervencion.HasOne<CausaFalla>().WithMany().HasForeignKey(x => x.CausaFallaId).OnDelete(DeleteBehavior.Restrict);
        intervencion.HasOne<Usuario>().WithMany().HasForeignKey(x => x.TecnicoId).OnDelete(DeleteBehavior.Restrict);
        intervencion.HasOne<Usuario>().WithMany().HasForeignKey(x => x.AutorizadorId).OnDelete(DeleteBehavior.Restrict);
        intervencion.ToTable("Intervenciones", t =>
        {
            t.HasCheckConstraint("CK_Intervenciones_Pesos", "\"KgUtilizados\" > 0 AND (\"KgEmitidos\" IS NULL OR \"KgEmitidos\" >= 0) AND (\"PesoRestanteBrutoKg\" IS NULL OR \"PesoRestanteBrutoKg\" >= 0)");
            t.HasCheckConstraint("CK_Intervenciones_Factores", "(\"FactorGwpAplicado\" IS NULL OR \"FactorGwpAplicado\" >= 0) AND (\"PrecioKgAplicado\" IS NULL OR \"PrecioKgAplicado\" >= 0)");
            t.HasCheckConstraint("CK_Intervenciones_Semaforo", "\"EstadoSemaforo\" IN ('Pendiente', 'Verde', 'Amarillo', 'Rojo')");
            t.HasCheckConstraint("CK_Intervenciones_Sync", "\"EstadoSincronizacion\" IN ('Pendiente', 'Sincronizado', 'Conflicto')");
            t.HasCheckConstraint("CK_Intervenciones_Estado", "\"EstadoIntervencion\" IN ('Registrada', 'Revisada', 'Cerrada')");
            t.HasCheckConstraint("CK_Intervenciones_FotoCierre", "\"EstadoIntervencion\" <> 'Cerrada' OR (\"FechaCierre\" IS NOT NULL AND length(btrim(coalesce(\"FotoBalanzaRuta\", ''))) > 0 AND \"PesoRestanteBrutoKg\" IS NOT NULL)");
            t.HasCheckConstraint("CK_Intervenciones_PlanRojo", "\"EstadoSemaforo\" <> 'Rojo' OR length(btrim(coalesce(\"PlanCorreccion\", ''))) > 0");
            t.HasCheckConstraint("CK_Intervenciones_Moneda", "\"PrecioKgAplicado\" IS NULL OR (\"Moneda\" IS NOT NULL AND \"Moneda\" ~ '^[A-Z]{3}$')");
        });

        var auditoria = model.Entity<RegistroAuditoria>();
        auditoria.Property(x => x.Accion).HasMaxLength(50).IsRequired();
        auditoria.Property(x => x.Entidad).HasMaxLength(100).IsRequired();
        auditoria.Property(x => x.RegistroId).HasMaxLength(100).IsRequired();
        auditoria.Property(x => x.ValoresAnteriores).HasColumnType("jsonb");
        auditoria.Property(x => x.ValoresNuevos).HasColumnType("jsonb");
        auditoria.HasIndex(x => x.Fecha);
        auditoria.HasIndex(x => new { x.Entidad, x.RegistroId });
        auditoria.HasOne<Usuario>().WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        auditoria.HasOne<Intervencion>().WithMany().HasForeignKey(x => x.IntervencionId).OnDelete(DeleteBehavior.Restrict);

        var certificado = model.Entity<CertificadoDisposicion>();
        certificado.Property(x => x.Folio).HasMaxLength(100).IsRequired();
        certificado.Property(x => x.EmpresaEmisora).HasMaxLength(200).IsRequired();
        certificado.Property(x => x.RutaDocumento).HasMaxLength(1024).IsRequired();
        certificado.HasIndex(x => new { x.EmpresaEmisora, x.Folio }).IsUnique();
        certificado.HasOne<Usuario>().WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        certificado.ToTable("CertificadosDisposicion", t => t.HasCheckConstraint("CK_Certificados_Folio", "length(btrim(\"Folio\")) > 0"));

        var detalle = model.Entity<CertificadoCilindro>();
        detalle.HasKey(x => new { x.CertificadoDisposicionId, x.CilindroId });
        detalle.HasIndex(x => x.CilindroId).IsUnique();
        detalle.Property(x => x.PesoEntregadoKg).HasPrecision(18, 3);
        detalle.HasOne<CertificadoDisposicion>().WithMany().HasForeignKey(x => x.CertificadoDisposicionId).OnDelete(DeleteBehavior.Restrict);
        detalle.HasOne<Cilindro>().WithMany().HasForeignKey(x => x.CilindroId).OnDelete(DeleteBehavior.Restrict);
        detalle.ToTable("CertificadosCilindros", t => t.HasCheckConstraint("CK_CertificadosCilindros_Peso", "\"PesoEntregadoKg\" IS NULL OR \"PesoEntregadoKg\" >= 0"));
    }
}
