using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using vet_api_Net.Models;

namespace vet_api_Net.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<AlertasInterna> AlertasInternas { get; set; }

    public virtual DbSet<CategoriasProducto> CategoriasProductos { get; set; }

    public virtual DbSet<Cita> Citas { get; set; }

    public virtual DbSet<MetodoPago> MetodoPagos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Consulta> Consultas { get; set; }

    public virtual DbSet<ConsultasProducto> ConsultasProductos { get; set; }

    public virtual DbSet<DetallesFactura> DetallesFacturas { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<HistorialPrecio> HistorialPrecios { get; set; }

    public virtual DbSet<LogsSistema> LogsSistemas { get; set; }

    public virtual DbSet<Mascota> Mascotas { get; set; }

    public virtual DbSet<MovimientosInventario> MovimientosInventarios { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Mensaje> Mensajes { get; set; }
        // public virtual DbSet<Models.Reporte> Reportes { get; set; }

    public virtual DbSet<Vacuna> Vacunas { get; set; }

    public virtual DbSet<Models.Reporte> Reportes { get; set; }

    public virtual DbSet<Models.ReporConfig> ReporConfigs { get; set; }

    public virtual DbSet<Models.FacturaConfig> FacturaConfigs { get; set; }

    public virtual DbSet<Models.MoneyType> MoneyTypes { get; set; }
    public virtual DbSet<Models.WSMessageAPIData> WSMessageAPIData { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Connection string removed to avoid embedding credentials in source code.
        // Configure the DbContext externally (for example in Program.cs using services.AddDbContext).
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        // Intentionally left blank: do not configure a provider here.
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_spanish2_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AlertasInterna>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("alertas_internas")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.DestinatarioId, "destinatario_id");

            entity.HasIndex(e => e.FechaLimite, "idx_fecha_limite");

            entity.HasIndex(e => new { e.Leida, e.Completada, e.Prioridad }, "idx_pendientes");

            entity.HasIndex(e => e.DestinatarioRol, "idx_rol");

            entity.HasIndex(e => e.LeidaPor, "leida_por");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AccionRequerida)
                .HasDefaultValueSql("'0'")
                .HasColumnName("accion_requerida");
            entity.Property(e => e.Completada)
                .HasDefaultValueSql("'0'")
                .HasColumnName("completada");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DestinatarioId)
                .HasColumnType("int(11)")
                .HasColumnName("destinatario_id");
            entity.Property(e => e.DestinatarioRol)
                .HasDefaultValueSql("'todos'")
                .HasColumnType("enum('admin','doctor','secretaria','todos')")
                .HasColumnName("destinatario_rol");
            entity.Property(e => e.FechaCompletado)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_completado");
            entity.Property(e => e.FechaLectura)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_lectura");
            entity.Property(e => e.FechaLimite).HasColumnName("fecha_limite");
            entity.Property(e => e.Leida)
                .HasDefaultValueSql("'0'")
                .HasColumnName("leida");
            entity.Property(e => e.LeidaPor)
                .HasColumnType("int(11)")
                .HasColumnName("leida_por");
            entity.Property(e => e.Mensaje)
                .HasColumnType("text")
                .HasColumnName("mensaje");
            entity.Property(e => e.Prioridad)
                .HasDefaultValueSql("'media'")
                .HasColumnType("enum('baja','media','alta','urgente')")
                .HasColumnName("prioridad");
            entity.Property(e => e.ReferenciaId)
                .HasColumnType("int(11)")
                .HasColumnName("referencia_id");
            entity.Property(e => e.ReferenciaTabla)
                .HasMaxLength(50)
                .HasColumnName("referencia_tabla");
            entity.Property(e => e.Tipo)
                .HasColumnType("enum('stock_bajo','producto_vencido','cita_pendiente','tarea_admin','factura_pendiente','seguimiento_medico','sistema')")
                .HasColumnName("tipo");
            entity.Property(e => e.Titulo)
                .HasMaxLength(200)
                .HasColumnName("titulo");

            entity.HasOne(d => d.Destinatario).WithMany(p => p.AlertasInternaDestinatarios)
                .HasForeignKey(d => d.DestinatarioId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("alertas_internas_ibfk_1");

            entity.HasOne(d => d.LeidaPorNavigation).WithMany(p => p.AlertasInternaLeidaPorNavigations)
                .HasForeignKey(d => d.LeidaPor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("alertas_internas_ibfk_2");
        });

        modelBuilder.Entity<CategoriasProducto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("categorias_productos")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.Actualizado)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("actualizado");
            entity.Property(e => e.Creado)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("creado");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("citas")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => new { e.DoctorId, e.FechaCita }, "idx_doctor_fecha");

            entity.HasIndex(e => e.Estado, "idx_estado");

            entity.HasIndex(e => e.FechaCita, "idx_fecha");

            entity.HasIndex(e => e.MascotaId, "mascota_id");

            entity.HasIndex(e => e.SecretariaId, "secretaria_id");

            entity.HasIndex(e => e.MetodoPagoId, "metodo_pago_id");

            entity.HasIndex(e => new { e.DoctorId, e.FechaCita, e.HoraCita }, "unique_cita_doctor").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.DoctorId)
                .HasColumnType("int(11)")
                .HasColumnName("doctor_id");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'programada'")
                .HasColumnType("enum('programada','completada','cancelada','no_asistida','en_curso')")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCita)
                .HasColumnType("datetime")
                .HasColumnName("fecha_cita");
            entity.Property(e => e.HoraCita)
                .HasColumnType("time")
                .HasColumnName("hora_cita");
            entity.Property(e => e.MascotaId)
                .HasColumnType("int(11)")
                .HasColumnName("mascota_id");
            entity.Property(e => e.Motivo)
                .HasColumnType("text")
                .HasColumnName("motivo");
            entity.Property(e => e.Notas)
                .HasColumnType("text")
                .HasColumnName("notas");
            entity.Property(e => e.SecretariaId)
                .HasColumnType("int(11)")
                .HasColumnName("secretaria_id");
            entity.Property(e => e.TipoCita)
                .HasDefaultValueSql("'consulta'")
                .HasColumnType("enum('consulta','vacunacion','cirugia','emergencia','seguimiento')")
                .HasColumnName("tipo_cita");

            entity.Property(e => e.MetodoPagoId)
                .HasColumnType("int(11)")
                .HasColumnName("metodo_pago_id");

            entity.HasOne(d => d.Doctor).WithMany(p => p.CitaDoctors)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("citas_ibfk_2");

            entity.HasOne(d => d.Mascota).WithMany(p => p.Cita)
                .HasForeignKey(d => d.MascotaId)
                .HasConstraintName("citas_ibfk_1");

            entity.HasOne(d => d.Secretaria).WithMany(p => p.CitaSecretaria)
                .HasForeignKey(d => d.SecretariaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("citas_ibfk_3");

            entity.HasOne(d => d.MetodoPago).WithMany(p => p.Citas)
                .HasForeignKey(d => d.MetodoPagoId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("citas_ibfk_4");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("metodos_pago")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");

            entity.Property(e => e.Creado)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("creado");

            entity.Property(e => e.Actualizado)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("actualizado");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("clientes")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.Identificacion, "identificacion").IsUnique();

            entity.HasIndex(e => new { e.Nombre, e.Apellido, e.Email }, "idx_Busqueda_clientes");

            entity.HasIndex(e => new { e.Nombre, e.Apellido }, "idx_nombre");

            entity.HasIndex(e => e.Telefono, "idx_telefono");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Actualizado)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("actualizado");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .HasColumnName("apellido");
            entity.Property(e => e.Creado)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("creado");
            entity.Property(e => e.Direccion)
                .HasColumnType("text")
                .HasColumnName("direccion");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Identificacion)
                .HasMaxLength(20)
                .HasColumnName("identificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Nota)
                .HasColumnType("text")
                .HasColumnName("nota");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("consultas")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.CitaId, "cita_id");

            entity.HasIndex(e => e.DoctorId, "doctor_id");

            entity.HasIndex(e => e.FechaConsulta, "idx_fecha_consulta");

            entity.HasIndex(e => e.MascotaId, "idx_mascota");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.CitaId)
                .HasColumnType("int(11)")
                .HasColumnName("cita_id");
            entity.Property(e => e.Creado)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("creado");
            entity.Property(e => e.Diagnostico)
                .HasColumnType("text")
                .HasColumnName("diagnostico");
            entity.Property(e => e.DoctorId)
                .HasColumnType("int(11)")
                .HasColumnName("doctor_id");
            entity.Property(e => e.FechaConsulta)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("fecha_consulta");
            entity.Property(e => e.MascotaId)
                .HasColumnType("int(11)")
                .HasColumnName("mascota_id");
            entity.Property(e => e.Observaciones)
                .HasColumnType("text")
                .HasColumnName("observaciones");
            entity.Property(e => e.PesoActual)
                .HasPrecision(5, 2)
                .HasColumnName("peso_actual");
            entity.Property(e => e.Receta)
                .HasColumnType("text")
                .HasColumnName("receta");
            entity.Property(e => e.Sintomas)
                .HasColumnType("text")
                .HasColumnName("sintomas");
            entity.Property(e => e.Temperatura)
                .HasPrecision(4, 1)
                .HasColumnName("temperatura");
            entity.Property(e => e.Tratamiento)
                .HasColumnType("text")
                .HasColumnName("tratamiento");

            entity.HasOne(d => d.Cita).WithMany(p => p.Consulta)
                .HasForeignKey(d => d.CitaId)
                .HasConstraintName("consultas_ibfk_1");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Consulta)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("consultas_ibfk_3");

            entity.HasOne(d => d.Mascota).WithMany(p => p.Consulta)
                .HasForeignKey(d => d.MascotaId)
                .HasConstraintName("consultas_ibfk_2");
        });

        modelBuilder.Entity<ConsultasProducto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("consultas_productos")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.AplicadoPor, "aplicado_por");

            entity.HasIndex(e => e.ConsultaId, "idx_consulta_producto");

            entity.HasIndex(e => e.ProductoId, "producto_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AplicadoPor)
                .HasColumnType("int(11)")
                .HasColumnName("aplicado_por");
            entity.Property(e => e.Cantidad)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("cantidad");
            entity.Property(e => e.ConsultaId)
                .HasColumnType("int(11)")
                .HasColumnName("consulta_id");
            entity.Property(e => e.Creado)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("creado");
            entity.Property(e => e.Dosis)
                .HasMaxLength(100)
                .HasColumnName("dosis");
            entity.Property(e => e.Duracion)
                .HasMaxLength(100)
                .HasColumnName("duracion");
            entity.Property(e => e.Frecuencia)
                .HasMaxLength(100)
                .HasColumnName("frecuencia");
            entity.Property(e => e.Instrucciones)
                .HasColumnType("text")
                .HasColumnName("instrucciones");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(10, 2)
                .HasColumnName("precio_unitario");
            entity.Property(e => e.ProductoId)
                .HasColumnType("int(11)")
                .HasColumnName("producto_id");
            entity.Property(e => e.ViaAdministracion)
                .HasMaxLength(100)
                .HasColumnName("via_administracion");

            entity.HasOne(d => d.AplicadoPorNavigation).WithMany(p => p.ConsultasProductos)
                .HasForeignKey(d => d.AplicadoPor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("consultas_productos_ibfk_3");

            entity.HasOne(d => d.Consulta).WithMany(p => p.ConsultasProductos)
                .HasForeignKey(d => d.ConsultaId)
                .HasConstraintName("consultas_productos_ibfk_1");

            entity.HasOne(d => d.Producto).WithMany(p => p.ConsultasProductos)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("consultas_productos_ibfk_2");
        });

        modelBuilder.Entity<DetallesFactura>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("detalles_factura")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.FacturaId, "idx_factura");

            entity.HasIndex(e => e.ProductoId, "producto_id");

            entity.HasIndex(e => e.ProductosConsultasId, "productos_consultas_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Cantidad)
                .HasColumnType("int(11)")
                .HasColumnName("cantidad");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("created");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.FacturaId)
                .HasColumnType("int(11)")
                .HasColumnName("factura_id");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(10, 2)
                .HasColumnName("precio_unitario");
            entity.Property(e => e.ProductoId)
                .HasColumnType("int(11)")
                .HasColumnName("producto_id");
            entity.Property(e => e.ProductosConsultasId)
                .HasColumnType("int(11)")
                .HasColumnName("productos_consultas_id");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");

            entity.HasOne(d => d.Factura).WithMany(p => p.DetallesFacturas)
                .HasForeignKey(d => d.FacturaId)
                .HasConstraintName("detalles_factura_ibfk_1");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetallesFacturas)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("detalles_factura_ibfk_2");

            entity.HasOne(d => d.ProductosConsultas).WithMany(p => p.DetallesFacturas)
                .HasForeignKey(d => d.ProductosConsultasId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("detalles_factura_ibfk_3");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("facturas")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.ConsultaId, "consulta_id");

            entity.HasIndex(e => e.ClienteId, "idx_cliente_factura");

            entity.HasIndex(e => e.EstadoPago, "idx_estado_pago");

            entity.HasIndex(e => new { e.FechaEmision, e.EstadoPago }, "idx_factura_fecha");

            entity.HasIndex(e => e.FechaEmision, "idx_fecha_emision");

            entity.HasIndex(e => e.MascotaId, "mascota_id");

            entity.HasIndex(e => e.NumeroFactura, "numero_factura").IsUnique();

            entity.HasIndex(e => e.SecretariaId, "secretaria_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Actualizado)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("actualizado");
            entity.Property(e => e.ClienteId)
                .HasColumnType("int(11)")
                .HasColumnName("cliente_id");
            entity.Property(e => e.ConsultaId)
                .HasColumnType("int(11)")
                .HasColumnName("consulta_id");
            entity.Property(e => e.Creado)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("creado");
            entity.Property(e => e.Descuento)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("descuento");
            entity.Property(e => e.EstadoPago)
                .HasDefaultValueSql("'pendiente'")
                .HasColumnType("enum('pendiente','pagada','cancelada')")
                .HasColumnName("estado_pago");
            entity.Property(e => e.FechaEmision)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("fecha_emision");
            entity.Property(e => e.MascotaId)
                .HasColumnType("int(11)")
                .HasColumnName("mascota_id");
            entity.Property(e => e.MetodoPago)
                .HasColumnType("enum('efectivo','tarjeta','transferencia','pago movil','otro')")
                .HasColumnName("metodo_pago");
            entity.Property(e => e.Notas)
                .HasColumnType("text")
                .HasColumnName("notas");
            entity.Property(e => e.UrlDocx)
                .HasMaxLength(512)
                .HasColumnType("varchar(512)")
                .HasColumnName("url_docx");
            entity.Property(e => e.NumeroFactura)
                .HasMaxLength(50)
                .HasColumnName("numero_factura");
            entity.Property(e => e.SecretariaId)
                .HasColumnType("int(11)")
                .HasColumnName("secretaria_id");
            entity.Property(e => e.Subtotal)
                .HasPrecision(10, 2)
                .HasColumnName("subtotal");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("facturas_ibfk_1");

            entity.HasOne(d => d.Consulta).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.ConsultaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("facturas_ibfk_3");

            entity.HasOne(d => d.Mascota).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.MascotaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("facturas_ibfk_2");

            entity.HasOne(d => d.Secretaria).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.SecretariaId)
                .HasConstraintName("facturas_ibfk_4");
        });

        modelBuilder.Entity<HistorialPrecio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("historial_precios")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => new { e.ProductoId, e.FechaCambio }, "idx_producto_fecha");

            entity.HasIndex(e => e.UsuarioId, "usuario_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.FechaCambio)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("fecha_cambio");
            entity.Property(e => e.Motivo)
                .HasColumnType("text")
                .HasColumnName("motivo");
            entity.Property(e => e.PrecioAnterior)
                .HasPrecision(10, 2)
                .HasColumnName("precio_anterior");
            entity.Property(e => e.PrecioNuevo)
                .HasPrecision(10, 2)
                .HasColumnName("precio_nuevo");
            entity.Property(e => e.ProductoId)
                .HasColumnType("int(11)")
                .HasColumnName("producto_id");
            entity.Property(e => e.UsuarioId)
                .HasColumnType("int(11)")
                .HasColumnName("usuario_id");

            entity.HasOne(d => d.Producto).WithMany(p => p.HistorialPrecios)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("historial_precios_ibfk_1");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistorialPrecios)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("historial_precios_ibfk_2");
        });

        modelBuilder.Entity<LogsSistema>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("logs_sistema")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.CreatedAt, "idx_fecha_log");

            entity.HasIndex(e => e.UsuarioId, "idx_usuario_log");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Accion)
                .HasMaxLength(100)
                .HasColumnName("accion");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DatosNuevos)
                .HasColumnType("json")
                .HasColumnName("datos_nuevos");
            entity.Property(e => e.DatosPrevios)
                .HasColumnType("json")
                .HasColumnName("datos_previos");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnName("ip_address");
            entity.Property(e => e.RegistroId)
                .HasColumnType("int(11)")
                .HasColumnName("registro_id");
            entity.Property(e => e.TablaAfectada)
                .HasMaxLength(50)
                .HasColumnName("tabla_afectada");
            entity.Property(e => e.UserAgent)
                .HasColumnType("text")
                .HasColumnName("user_agent");
            entity.Property(e => e.UsuarioId)
                .HasColumnType("int(11)")
                .HasColumnName("usuario_id");

            entity.HasOne(d => d.Usuario).WithMany(p => p.LogsSistemas)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("logs_sistema_ibfk_1");
        });

        modelBuilder.Entity<Mascota>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("mascotas")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.IdenteficacionMascota, "identeficacion_mascota").IsUnique();

            entity.HasIndex(e => new { e.Nombre, e.Especie, e.Raza }, "idx_Busqueda_mascotas");

            entity.HasIndex(e => e.ClienteId, "idx_cliente_id");

            entity.HasIndex(e => e.Nombre, "idx_nombre_mascota");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Actualizado)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("actualizado");
            entity.Property(e => e.Alergias)
                .HasColumnType("text")
                .HasColumnName("alergias");
            entity.Property(e => e.ClienteId)
                .HasColumnType("int(11)")
                .HasColumnName("cliente_id");
            entity.Property(e => e.Color)
                .HasMaxLength(30)
                .HasColumnName("color");
            entity.Property(e => e.CondicionesMedicas)
                .HasColumnType("text")
                .HasColumnName("condiciones_medicas");
            entity.Property(e => e.Creado)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("creado");
            entity.Property(e => e.Especie)
                .HasColumnType("enum('perro','gato')")
                .HasColumnName("especie");
            entity.Property(e => e.Esterilizado)
                .HasDefaultValueSql("'0'")
                .HasColumnName("esterilizado");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.IdenteficacionMascota)
                .HasMaxLength(20)
                .HasColumnName("identeficacion_mascota");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Peso)
                .HasPrecision(5, 2)
                .HasColumnName("peso");
            entity.Property(e => e.Raza)
                .HasMaxLength(50)
                .HasColumnName("raza");
            entity.Property(e => e.Sexo)
                .HasColumnType("enum('macho','hembra')")
                .HasColumnName("sexo");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Mascota)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("mascotas_ibfk_1");
        });

        modelBuilder.Entity<MovimientosInventario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("movimientos_inventario")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.FechaMovimiento, "idx_fecha_movimiento");

            entity.HasIndex(e => e.ProductoId, "idx_producto_movimiento");

            entity.HasIndex(e => e.UsuarioId, "usuario_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Cantidad)
                .HasColumnType("int(11)")
                .HasColumnName("cantidad");
            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("fecha_movimiento");
            entity.Property(e => e.Motivo)
                .HasMaxLength(255)
                .HasColumnName("motivo");
            entity.Property(e => e.ProductoId)
                .HasColumnType("int(11)")
                .HasColumnName("producto_id");
            entity.Property(e => e.ReferenciaId)
                .HasColumnType("int(11)")
                .HasColumnName("referencia_id");
            entity.Property(e => e.StockAnterior)
                .HasColumnType("int(11)")
                .HasColumnName("stock_anterior");
            entity.Property(e => e.StockNuevo)
                .HasColumnType("int(11)")
                .HasColumnName("stock_nuevo");
            entity.Property(e => e.TipoMovimiento)
                .HasColumnType("enum('entrada','salida','ajuste')")
                .HasColumnName("tipo_movimiento");
            entity.Property(e => e.UsuarioId)
                .HasColumnType("int(11)")
                .HasColumnName("usuario_id");

            entity.HasOne(d => d.Producto).WithMany(p => p.MovimientosInventarios)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("movimientos_inventario_ibfk_1");

            entity.HasOne(d => d.Usuario).WithMany(p => p.MovimientosInventarios)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("movimientos_inventario_ibfk_2");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("productos")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.CategoriaId, "categoria_id");

            entity.HasIndex(e => e.Codigo, "codigo").IsUnique();

            entity.HasIndex(e => new { e.Nombre, e.Codigo, e.CategoriaId }, "idx_Busqueda_productos");

            entity.HasIndex(e => e.Nombre, "idx_nombre_producto");

            entity.HasIndex(e => e.Stock, "idx_stock");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Actualizado)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("actualizado");
            entity.Property(e => e.CategoriaId)
                .HasColumnType("int(11)")
                .HasColumnName("categoria_id");
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .HasColumnName("codigo");
            entity.Property(e => e.Creado)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("creado");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");
            entity.Property(e => e.PrecioVenta)
                .HasPrecision(10, 2)
                .HasColumnName("precio_venta");
            entity.Property(e => e.Proveedor)
                .HasMaxLength(100)
                .HasColumnName("proveedor");
            entity.Property(e => e.RequiereReceta)
                .HasDefaultValueSql("'0'")
                .HasColumnName("requiere_receta");
            entity.Property(e => e.Stock)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("stock");
            entity.Property(e => e.StockMinimo)
                .HasDefaultValueSql("'5'")
                .HasColumnType("int(11)")
                .HasColumnName("stock_minimo");
            entity.Property(e => e.Tipo)
                .HasColumnType("enum('medicamento','alimento','accesorio')")
                .HasColumnName("tipo");
            entity.Property(e => e.UnidadMedida)
                .HasMaxLength(20)
                .HasDefaultValueSql("'unidad'")
                .HasColumnName("unidad_medida");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("productos_ibfk_1");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("usuarios")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.Rol, "idx_rol");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.Actualizado)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("actualizado");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .HasColumnName("apellido");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("contraseña");
            entity.Property(e => e.Creado)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("creado");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Rol)
                .HasColumnType("enum('admin','doctor','secretaria')")
                .HasColumnName("rol");
            entity.Property(e => e.UltimoAcceso)
                .HasColumnType("datetime")
                .HasColumnName("ultimo_acceso");
        });

        modelBuilder.Entity<Vacuna>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("vacunas")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.ConsultaId, "consulta_id");

            entity.HasIndex(e => e.DoctorId, "doctor_id");

            entity.HasIndex(e => e.FechaVacunacion, "idx_fecha_vacuna");

            entity.HasIndex(e => e.MascotaId, "idx_mascota_vacuna");

            entity.HasIndex(e => e.ProductoId, "producto_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.ConsultaId)
                .HasColumnType("int(11)")
                .HasColumnName("consulta_id");
            entity.Property(e => e.Creado)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("creado");
            entity.Property(e => e.DoctorId)
                .HasColumnType("int(11)")
                .HasColumnName("doctor_id");
            entity.Property(e => e.FechaVacunacion).HasColumnName("fecha_vacunacion");
            entity.Property(e => e.Lote)
                .HasMaxLength(50)
                .HasColumnName("lote");
            entity.Property(e => e.MascotaId)
                .HasColumnType("int(11)")
                .HasColumnName("mascota_id");
            entity.Property(e => e.Nota)
                .HasColumnType("text")
                .HasColumnName("nota");
            entity.Property(e => e.ProductoId)
                .HasColumnType("int(11)")
                .HasColumnName("producto_id");
            entity.Property(e => e.ProximaDosis).HasColumnName("proxima_dosis");

            entity.HasOne(d => d.Consulta).WithMany(p => p.Vacunas)
                .HasForeignKey(d => d.ConsultaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("vacunas_ibfk_3");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Vacunas)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("vacunas_ibfk_4");

            entity.HasOne(d => d.Mascota).WithMany(p => p.Vacunas)
                .HasForeignKey(d => d.MascotaId)
                .HasConstraintName("vacunas_ibfk_1");

            entity.HasOne(d => d.Producto).WithMany(p => p.Vacunas)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("vacunas_ibfk_2");
        });

        modelBuilder.Entity<Mensaje>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("mensajes")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");

            entity.Property(e => e.EmisorId)
                .HasColumnType("int(11)")
                .HasColumnName("emisor_id");

            entity.Property(e => e.ReceptorId)
                .HasColumnType("int(11)")
                .HasColumnName("receptor_id");

            entity.Property(e => e.Contenido)
                .HasColumnType("text")
                .HasColumnName("contenido");

            entity.Property(e => e.Leido)
                .HasDefaultValueSql("'0'")
                .HasColumnName("leido");

            entity.Property(e => e.FechaEnvio)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("fecha_envio");

            entity.HasOne(d => d.Emisor).WithMany(p => p.MensajesEmisor)
                .HasForeignKey(d => d.EmisorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("mensajes_ibfk_1");

            entity.HasOne(d => d.Receptor).WithMany(p => p.MensajesReceptor)
                .HasForeignKey(d => d.ReceptorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("mensajes_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
