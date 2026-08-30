using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using vet_api_Net.Models;

/*
 describe: 
 - Este archivo es para configurar el contexto de la base de datos,
   Organiza los DbSets de los modelos de la base de datos para su facil orden y escalabilidad
*/

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

    public virtual DbSet<Especie> Especies { get; set; }

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
    public virtual DbSet<HistoriaClinica> HistoriasClinicas { get; set; }
    public virtual DbSet<IaConocimiento> IaConocimientos { get; set; }

    public virtual DbSet<Vaccine> Vaccines { get; set; }

    public virtual DbSet<VaccineSchemeStage> VaccineSchemeStages { get; set; }

    public virtual DbSet<VaccineBatch> VaccineBatches { get; set; }

    public virtual DbSet<PetVaccination> PetVaccinations { get; set; }

    public virtual DbSet<Models.Reporte> Reportes { get; set; }

    public virtual DbSet<Models.WorkerConfig> WorkerConfigs { get; set; }

    public virtual DbSet<Models.MoneyType> MoneyTypes { get; set; }

    public virtual DbSet<Models.SystemConfig> SystemConfigs { get; set; }
    public virtual DbSet<Models.WSMessageAPIData> WSMessageAPIData { get; set; }
    public virtual DbSet<Models.EmailTemplate> EmailTemplates { get; set; }
    public virtual DbSet<Models.PasswordResetTicket> PasswordResetTickets { get; set; }
    public virtual DbSet<Models.UserPermission> UserPermissions { get; set; }
    public virtual DbSet<Models.PermissionModule> PermissionModules { get; set; }
    public virtual DbSet<Models.PermissionDefinition> PermissionDefinitions { get; set; }


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
        modelBuilder.Entity<PasswordResetTicket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("password_reset_tickets");

            entity.HasIndex(e => e.Token, "idx_token").IsUnique();
            entity.HasIndex(e => e.UsuarioId, "idx_usuario_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.Token).HasMaxLength(255).HasColumnName("token");
            entity.Property(e => e.Estado).HasMaxLength(50).HasColumnName("estado");
            entity.Property(e => e.Expiracion).HasColumnName("expiracion");
            entity.Property(e => e.Creado).HasColumnName("creado");

            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.PasswordResetTickets)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_password_reset_usuario");
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("user_permissions");

            entity.HasIndex(e => e.UserId, "idx_user_permissions_user_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Permissions).HasColumnType("jsonb").HasColumnName("permissions");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.User)
                .WithOne(p => p.UserPermission)
                .HasForeignKey<UserPermission>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_user_permissions_user");
        });

        modelBuilder.Entity<PermissionModule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("permission_modules");

            entity.HasIndex(e => e.ModuleKey, "idx_permission_modules_module_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ModuleKey).HasMaxLength(50).HasColumnName("module_key");
            entity.Property(e => e.Label).HasMaxLength(200).HasColumnName("label");
            entity.Property(e => e.Icon).HasMaxLength(50).HasColumnName("icon");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.Creado).HasColumnName("creado");
        });

        modelBuilder.Entity<PermissionDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("permission_definitions");

            entity.HasIndex(e => e.Key, "idx_permission_definitions_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.Key).HasMaxLength(100).HasColumnName("key");
            entity.Property(e => e.Label).HasMaxLength(200).HasColumnName("label");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.Creado).HasColumnName("creado");

            entity.HasOne(d => d.Module)
                .WithMany(m => m.Permissions)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_permission_definitions_module");
        });

        modelBuilder.Entity<AlertasInterna>(entity =>


        {
            entity.HasKey(e => e.Id);

            entity
                .ToTable("alertas_internas")
;

            entity.HasIndex(e => e.DestinatarioId, "destinatario_id");

            entity.HasIndex(e => e.FechaLimite, "idx_fecha_limite");

            entity.HasIndex(e => new { e.Leida, e.Completada, e.Prioridad }, "idx_pendientes");

            entity.HasIndex(e => e.DestinatarioRol, "idx_rol");

            entity.HasIndex(e => e.LeidaPor, "leida_por");

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.AccionRequerida)

                .HasColumnName("accion_requerida");
            entity.Property(e => e.Completada)

                .HasColumnName("completada");
            entity.Property(e => e.CreatedAt)


                .HasColumnName("created_at");
            entity.Property(e => e.DestinatarioId)

                .HasColumnName("destinatario_id");
            entity.Property(e => e.DestinatarioRol)


                .HasColumnName("destinatario_rol");
            entity.Property(e => e.FechaCompletado)

                .HasColumnName("fecha_completado");
            entity.Property(e => e.FechaLectura)

                .HasColumnName("fecha_lectura");
            entity.Property(e => e.FechaLimite).HasColumnName("fecha_limite");
            entity.Property(e => e.Leida)

                .HasColumnName("leida");
            entity.Property(e => e.LeidaPor)

                .HasColumnName("leida_por");
            entity.Property(e => e.Mensaje)

                .HasColumnName("mensaje");
            entity.Property(e => e.Prioridad)


                .HasColumnName("prioridad");
            entity.Property(e => e.ReferenciaId)

                .HasColumnName("referencia_id");
            entity.Property(e => e.ReferenciaTabla)
                .HasMaxLength(50)
                .HasColumnName("referencia_tabla");
            entity.Property(e => e.Tipo)

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
            entity.HasKey(e => e.Id);

            entity
                .ToTable("categorias_productos")
;

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.Activo)

                .HasColumnName("activo");
            entity.Property(e => e.Actualizado)



                .HasColumnName("actualizado");
            entity.Property(e => e.Creado)


                .HasColumnName("creado");
            entity.Property(e => e.Descripcion)

                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
                .ToTable("citas")
;

            entity.HasIndex(e => new { e.DoctorId, e.FechaCita }, "idx_doctor_fecha");

            entity.HasIndex(e => e.Estado, "idx_estado");

            entity.HasIndex(e => e.FechaCita, "idx_fecha");

            entity.HasIndex(e => e.MascotaId, "mascota_id");

            entity.HasIndex(e => e.SecretariaId, "secretaria_id");

            entity.HasIndex(e => e.MetodoPagoId, "metodo_pago_id");

            entity.HasIndex(e => new { e.DoctorId, e.FechaCita, e.HoraCita }, "unique_cita_doctor").IsUnique();

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.DoctorId)

                .HasColumnName("doctor_id");
            entity.Property(e => e.Estado)


                .HasColumnName("estado");
            entity.Property(e => e.FechaCita)

                .HasColumnName("fecha_cita");
            entity.Property(e => e.HoraCita)

                .HasColumnName("hora_cita");
            entity.Property(e => e.MascotaId)

                .HasColumnName("mascota_id");
            entity.Property(e => e.Motivo)

                .HasColumnName("motivo");
            entity.Property(e => e.Notas)

                .HasColumnName("notas");
            entity.Property(e => e.SecretariaId)

                .HasColumnName("secretaria_id");
            entity.Property(e => e.TipoCita)


                .HasColumnName("tipo_cita");

            entity.Property(e => e.MetodoPagoId)

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
            entity.HasKey(e => e.Id);

            entity
                .ToTable("metodos_pago")
;

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.Id)

                .HasColumnName("id");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");

            entity.Property(e => e.Creado)


                .HasColumnName("creado");

            entity.Property(e => e.Actualizado)



                .HasColumnName("actualizado");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
                .ToTable("clientes")
;

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.Identificacion, "identificacion").IsUnique();

            entity.HasIndex(e => new { e.Nombre, e.Apellido, e.Email }, "idx_Busqueda_clientes");

            entity.HasIndex(e => new { e.Nombre, e.Apellido }, "idx_nombre");

            entity.HasIndex(e => e.Telefono, "idx_telefono");

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.Actualizado)



                .HasColumnName("actualizado");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .HasColumnName("apellido");
            entity.Property(e => e.Creado)


                .HasColumnName("creado");
            entity.Property(e => e.Direccion)

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

                .HasColumnName("nota");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
                .ToTable("consultas")
;

            entity.HasIndex(e => e.CitaId, "cita_id");

            entity.HasIndex(e => e.DoctorId, "doctor_id");

            entity.HasIndex(e => e.FechaConsulta, "idx_fecha_consulta");

            entity.HasIndex(e => e.MascotaId, "idx_mascota");

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.CitaId)

                .HasColumnName("cita_id");
            entity.Property(e => e.Creado)


                .HasColumnName("creado");
            entity.Property(e => e.Diagnostico)

                .HasColumnName("diagnostico");
            entity.Property(e => e.DoctorId)

                .HasColumnName("doctor_id");
            entity.Property(e => e.FechaConsulta)


                .HasColumnName("fecha_consulta");
            entity.Property(e => e.MascotaId)

                .HasColumnName("mascota_id");
            entity.Property(e => e.Observaciones)

                .HasColumnName("observaciones");
            entity.Property(e => e.PesoActual)
                .HasPrecision(5, 2)
                .HasColumnName("peso_actual");
            entity.Property(e => e.Receta)

                .HasColumnName("receta");
            entity.Property(e => e.Sintomas)

                .HasColumnName("sintomas");
            entity.Property(e => e.Temperatura)
                .HasPrecision(4, 1)
                .HasColumnName("temperatura");
            entity.Property(e => e.Tratamiento)

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
            entity.HasKey(e => e.Id);

            entity
                .ToTable("consultas_productos")
;

            entity.HasIndex(e => e.AplicadoPor, "aplicado_por");

            entity.HasIndex(e => e.ConsultaId, "idx_consulta_producto");

            entity.HasIndex(e => e.ProductoId, "producto_id");

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.AplicadoPor)

                .HasColumnName("aplicado_por");
            entity.Property(e => e.Cantidad)


                .HasColumnName("cantidad");
            entity.Property(e => e.ConsultaId)

                .HasColumnName("consulta_id");
            entity.Property(e => e.Creado)


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

                .HasColumnName("instrucciones");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(10, 2)
                .HasColumnName("precio_unitario");
            entity.Property(e => e.ProductoId)

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
            entity.HasKey(e => e.Id);

            entity
                .ToTable("detalles_factura")
;

            entity.HasIndex(e => e.FacturaId, "idx_factura");

            entity.HasIndex(e => e.ProductoId, "producto_id");

            entity.HasIndex(e => e.VaccineId, "idx_detalles_factura_vaccine");

            entity.HasIndex(e => e.ProductosConsultasId, "productos_consultas_id");

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.Cantidad)

                .HasColumnName("cantidad");
            entity.Property(e => e.Created)


                .HasColumnName("created");
            entity.Property(e => e.Descripcion)

                .HasColumnName("descripcion");
            entity.Property(e => e.FacturaId)

                .HasColumnName("factura_id");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(10, 2)
                .HasColumnName("precio_unitario");
            entity.Property(e => e.ProductoId)

                .HasColumnName("producto_id");
            entity.Property(e => e.VaccineId)

                .HasColumnName("vaccine_id");
            entity.Property(e => e.ProductosConsultasId)

                .HasColumnName("productos_consultas_id");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");

            entity.HasOne(d => d.Factura).WithMany(p => p.DetallesFacturas)
                .HasForeignKey(d => d.FacturaId)
                .HasConstraintName("detalles_factura_ibfk_1");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetallesFacturas)
                .HasForeignKey(d => d.ProductoId)
                .IsRequired(false)
                .HasConstraintName("detalles_factura_ibfk_2");

            entity.HasOne(d => d.Vaccine).WithMany()
                .HasForeignKey(d => d.VaccineId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("detalles_factura_ibfk_4");

            entity.HasOne(d => d.ProductosConsultas).WithMany(p => p.DetallesFacturas)
                .HasForeignKey(d => d.ProductosConsultasId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("detalles_factura_ibfk_3");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
                .ToTable("facturas")
;

            entity.HasIndex(e => e.ConsultaId, "consulta_id");

            entity.HasIndex(e => e.ClienteId, "idx_cliente_factura");

            entity.HasIndex(e => e.EstadoPago, "idx_estado_pago");

            entity.HasIndex(e => new { e.FechaEmision, e.EstadoPago }, "idx_factura_fecha");

            entity.HasIndex(e => e.FechaEmision, "idx_fecha_emision");

            entity.HasIndex(e => e.MascotaId, "mascota_id");

            entity.HasIndex(e => e.NumeroFactura, "numero_factura").IsUnique();

            entity.HasIndex(e => e.SecretariaId, "secretaria_id");

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.Actualizado)



                .HasColumnName("actualizado");
            entity.Property(e => e.ClienteId)

                .HasColumnName("cliente_id");
            entity.Property(e => e.ConsultaId)

                .HasColumnName("consulta_id");
            entity.Property(e => e.Creado)


                .HasColumnName("creado");
            entity.Property(e => e.Descuento)
                .HasPrecision(10, 2)

                .HasColumnName("descuento");
            entity.Property(e => e.EstadoPago)


                .HasColumnName("estado_pago");
            entity.Property(e => e.FechaEmision)


                .HasColumnName("fecha_emision");
            entity.Property(e => e.MascotaId)

                .HasColumnName("mascota_id");
            entity.Property(e => e.MetodoPago)

                .HasColumnName("metodo_pago");
            entity.Property(e => e.Notas)

                .HasColumnName("notas");
            entity.Property(e => e.UrlDocx)
                .HasMaxLength(512)

                .HasColumnName("url_docx");
            entity.Property(e => e.NumeroFactura)
                .HasMaxLength(50)
                .HasColumnName("numero_factura");
            entity.Property(e => e.SecretariaId)

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
            entity.HasKey(e => e.Id);

            entity
                .ToTable("historial_precios")
;

            entity.HasIndex(e => new { e.ProductoId, e.FechaCambio }, "idx_producto_fecha");

            entity.HasIndex(e => e.UsuarioId, "usuario_id");

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.FechaCambio)


                .HasColumnName("fecha_cambio");
            entity.Property(e => e.Motivo)

                .HasColumnName("motivo");
            entity.Property(e => e.PrecioAnterior)
                .HasPrecision(10, 2)
                .HasColumnName("precio_anterior");
            entity.Property(e => e.PrecioNuevo)
                .HasPrecision(10, 2)
                .HasColumnName("precio_nuevo");
            entity.Property(e => e.ProductoId)

                .HasColumnName("producto_id");
            entity.Property(e => e.UsuarioId)

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
            entity.HasKey(e => e.Id);

            entity
                .ToTable("logs_sistema")
;

            entity.HasIndex(e => e.CreatedAt, "idx_fecha_log");

            entity.HasIndex(e => e.UsuarioId, "idx_usuario_log");

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.Accion)
                .HasMaxLength(100)
                .HasColumnName("accion");
            entity.Property(e => e.CreatedAt)


                .HasColumnName("created_at");
            entity.Property(e => e.DatosNuevos)

                .HasColumnName("datos_nuevos");
            entity.Property(e => e.DatosPrevios)

                .HasColumnName("datos_previos");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnName("ip_address");
            entity.Property(e => e.RegistroId)

                .HasColumnName("registro_id");
            entity.Property(e => e.TablaAfectada)
                .HasMaxLength(50)
                .HasColumnName("tabla_afectada");
            entity.Property(e => e.UserAgent)

                .HasColumnName("user_agent");
            entity.Property(e => e.UsuarioId)

                .HasColumnName("usuario_id");

            entity.HasOne(d => d.Usuario).WithMany(p => p.LogsSistemas)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("logs_sistema_ibfk_1");
        });

        modelBuilder.Entity<Mascota>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
                .ToTable("mascotas")
;

            entity.HasIndex(e => e.IdenteficacionMascota, "identeficacion_mascota").IsUnique();

            entity.HasIndex(e => e.ClienteId, "idx_cliente_id");

            entity.HasIndex(e => e.Nombre, "idx_nombre_mascota");

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.Actualizado)



                .HasColumnName("actualizado");
            entity.Property(e => e.Alergias)

                .HasColumnName("alergias");
            entity.Property(e => e.ClienteId)

                .HasColumnName("cliente_id");
            entity.Property(e => e.Color)
                .HasMaxLength(30)
                .HasColumnName("color");
            entity.Property(e => e.CondicionesMedicas)

                .HasColumnName("condiciones_medicas");
            entity.Property(e => e.Creado)


                .HasColumnName("creado");
            entity.Property(e => e.EspecieId)

                .HasColumnName("especie_id");
            entity.Property(e => e.Esterilizado)

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

                .HasColumnName("sexo");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Mascota)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("mascotas_ibfk_1");

            entity.HasOne(d => d.Especie).WithMany(p => p.Mascotas)
                .HasForeignKey(d => d.EspecieId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("mascotas_ibfk_2");
        });

        modelBuilder.Entity<HistoriaClinica>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("historias_clinicas");

            entity.HasIndex(e => e.MascotaId, "idx_hc_mascota");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MascotaId).HasColumnName("mascota_id");
            entity.Property(e => e.ResumenIa).HasColumnName("resumen_ia");
            entity.Property(e => e.AlertasRiesgoIa).HasColumnName("alertas_riesgo_ia");
            entity.Property(e => e.SugerenciasIa).HasColumnName("sugerencias_ia");
            entity.Property(e => e.NotasVeterinario).HasColumnName("notas_veterinario");
            entity.Property(e => e.UltimoAnalisisIa).HasColumnName("ultimo_analisis_ia");
            entity.Property(e => e.Creado).HasColumnName("creado");
            entity.Property(e => e.Actualizado).HasColumnName("actualizado");

            entity.HasOne(d => d.Mascota)
                .WithMany(p => p.HistoriasClinicas)
                .HasForeignKey(d => d.MascotaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_historia_clinica_mascota");
        });

        modelBuilder.Entity<MovimientosInventario>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
                .ToTable("movimientos_inventario")
;

            entity.HasIndex(e => e.FechaMovimiento, "idx_fecha_movimiento");

            entity.HasIndex(e => e.ProductoId, "idx_producto_movimiento");

            entity.HasIndex(e => e.UsuarioId, "usuario_id");

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.Cantidad)

                .HasColumnName("cantidad");
            entity.Property(e => e.FechaMovimiento)


                .HasColumnName("fecha_movimiento");
            entity.Property(e => e.Motivo)
                .HasMaxLength(255)
                .HasColumnName("motivo");
            entity.Property(e => e.ProductoId)

                .HasColumnName("producto_id");
            entity.Property(e => e.ReferenciaId)

                .HasColumnName("referencia_id");
            entity.Property(e => e.StockAnterior)

                .HasColumnName("stock_anterior");
            entity.Property(e => e.StockNuevo)

                .HasColumnName("stock_nuevo");
            entity.Property(e => e.TipoMovimiento)

                .HasColumnName("tipo_movimiento");
            entity.Property(e => e.UsuarioId)

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
            entity.HasKey(e => e.Id);

            entity
                .ToTable("productos")
;

            entity.HasIndex(e => e.CategoriaId, "categoria_id");

            entity.HasIndex(e => e.Codigo, "codigo").IsUnique();

            entity.HasIndex(e => new { e.Nombre, e.Codigo, e.CategoriaId }, "idx_Busqueda_productos");

            entity.HasIndex(e => e.Nombre, "idx_nombre_producto");

            entity.HasIndex(e => e.Stock, "idx_stock");

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.Actualizado)



                .HasColumnName("actualizado");
            entity.Property(e => e.CategoriaId)

                .HasColumnName("categoria_id");
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .HasColumnName("codigo");
            entity.Property(e => e.Creado)


                .HasColumnName("creado");
            entity.Property(e => e.Descripcion)

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

                .HasColumnName("requiere_receta");
            entity.Property(e => e.Stock)


                .HasColumnName("stock");
            entity.Property(e => e.StockMinimo)


                .HasColumnName("stock_minimo");
            entity.Property(e => e.Tipo)

                .HasColumnName("tipo");
            entity.Property(e => e.UnidadMedida)
                .HasMaxLength(20)

                .HasColumnName("unidad_medida");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("productos_ibfk_1");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
                .ToTable("usuarios")
;

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.Rol, "idx_rol");

            entity.Property(e => e.Id)

                .HasColumnName("id");
            entity.Property(e => e.Activo)

                .HasColumnName("activo");
            entity.Property(e => e.Actualizado)



                .HasColumnName("actualizado");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .HasColumnName("apellido");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("contraseña");
            entity.Property(e => e.Creado)


                .HasColumnName("creado");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Rol)

                .HasColumnName("rol");
            entity.Property(e => e.UltimoAcceso)

                .HasColumnName("ultimo_acceso");
        });

        modelBuilder.Entity<Vaccine>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("vaccines");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(150).HasColumnName("name");
            entity.Property(e => e.EspecieId).HasColumnName("especie_id");
            entity.Property(e => e.MinimumAgeWeeks).HasColumnName("minimum_age_weeks");
            entity.Property(e => e.BoosterFrequencyValue).HasColumnName("booster_frequency_value");
            entity.Property(e => e.BoosterFrequencyUnit).HasMaxLength(20).HasColumnName("booster_frequency_unit");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Price).HasPrecision(10, 2).HasColumnName("price");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(d => d.Especie).WithMany(p => p.Vaccines)
                .HasForeignKey(d => d.EspecieId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("vaccines_ibfk_1");
        });

        modelBuilder.Entity<Especie>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("especies");

            entity.HasIndex(e => e.Nombre, "idx_especies_nombre").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasMaxLength(50).HasColumnName("nombre");
        });

        modelBuilder.Entity<VaccineSchemeStage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("vaccine_scheme_stages");

            entity.HasIndex(e => e.VaccineId, "idx_scheme_stage_vaccine");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");
            entity.Property(e => e.StageType).HasMaxLength(20).HasColumnName("stage_type");
            entity.Property(e => e.DoseNumber).HasColumnName("dose_number");
            entity.Property(e => e.IntervalDaysFromPrevious).HasColumnName("interval_days_from_previous");
            entity.Property(e => e.MinimumAgeWeeksOverride).HasColumnName("minimum_age_weeks_override");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(d => d.Vaccine).WithMany(p => p.SchemeStages)
                .HasForeignKey(d => d.VaccineId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("vaccine_scheme_stages_ibfk_1");
        });

        modelBuilder.Entity<VaccineBatch>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("vaccine_batches");

            entity.HasIndex(e => new { e.VaccineId, e.ExpirationDate }, "idx_batch_vaccine_expiration");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");
            entity.Property(e => e.Laboratory).HasMaxLength(100).HasColumnName("laboratory");
            entity.Property(e => e.BatchNumber).HasMaxLength(50).HasColumnName("batch_number");
            entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
            entity.Property(e => e.QuantityInStock).HasColumnName("quantity_in_stock");
            entity.Property(e => e.ReceivedDate).HasColumnName("received_date");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(d => d.Vaccine).WithMany(p => p.Batches)
                .HasForeignKey(d => d.VaccineId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("vaccine_batches_ibfk_1");
        });

        modelBuilder.Entity<PetVaccination>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("pet_vaccinations");

            entity.HasIndex(e => e.MascotaId, "idx_pet_vaccination_mascota");

            entity.HasIndex(e => e.NextDoseDate, "idx_pet_vaccination_next_dose");

            entity.HasIndex(e => e.VaccineId, "vaccine_id");

            entity.HasIndex(e => e.VaccineBatchId, "vaccine_batch_id");

            entity.HasIndex(e => e.ConsultaId, "consulta_id");

            entity.HasIndex(e => e.DoctorId, "doctor_id");

            entity.HasIndex(e => e.DetalleFacturaId, "detalle_factura_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MascotaId).HasColumnName("mascota_id");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");
            entity.Property(e => e.VaccineBatchId).HasColumnName("vaccine_batch_id");
            entity.Property(e => e.ConsultaId).HasColumnName("consulta_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.ApplicationDate).HasColumnName("application_date");
            entity.Property(e => e.WeightAtApplication).HasPrecision(5, 2).HasColumnName("weight_at_application");
            entity.Property(e => e.Dose).HasMaxLength(100).HasColumnName("dose");
            entity.Property(e => e.NextDoseDate).HasColumnName("next_dose_date");
            entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status");
            entity.Property(e => e.PostponementReason).HasColumnName("postponement_reason");
            entity.Property(e => e.ClinicalObservations).HasColumnName("clinical_observations");
            entity.Property(e => e.DetalleFacturaId).HasColumnName("detalle_factura_id");
            entity.Property(e => e.ReminderSevenSentAt).HasColumnName("reminder_seven_sent_at");
            entity.Property(e => e.ReminderThreeSentAt).HasColumnName("reminder_three_sent_at");
            entity.Property(e => e.ReminderDayBeforeSentAt).HasColumnName("reminder_day_before_sent_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(d => d.Mascota).WithMany(p => p.PetVaccinations)
                .HasForeignKey(d => d.MascotaId)
                .HasConstraintName("pet_vaccinations_ibfk_1");

            entity.HasOne(d => d.Vaccine).WithMany(p => p.PetVaccinations)
                .HasForeignKey(d => d.VaccineId)
                .HasConstraintName("pet_vaccinations_ibfk_2");

            entity.HasOne(d => d.VaccineBatch).WithMany(p => p.PetVaccinations)
                .HasForeignKey(d => d.VaccineBatchId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("pet_vaccinations_ibfk_3");

            entity.HasOne(d => d.Consulta).WithMany(p => p.PetVaccinations)
                .HasForeignKey(d => d.ConsultaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("pet_vaccinations_ibfk_4");

            entity.HasOne(d => d.Doctor).WithMany(p => p.PetVaccinations)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("pet_vaccinations_ibfk_5");

            entity.HasOne(d => d.DetalleFactura).WithMany()
                .HasForeignKey(d => d.DetalleFacturaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("pet_vaccinations_ibfk_6");
        });

        modelBuilder.Entity<Models.WorkerConfig>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("worker_configs");

            entity.HasIndex(e => e.WorkerName, "idx_worker_configs_worker_name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.WorkerName).HasMaxLength(100).HasColumnName("worker_name");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.IntervalValue).HasColumnName("interval_value");
            entity.Property(e => e.IntervalUnit).HasMaxLength(20).HasColumnName("interval_unit");
            entity.Property(e => e.RetentionValue).HasColumnName("retention_value");
            entity.Property(e => e.RetentionUnit).HasMaxLength(20).HasColumnName("retention_unit");
            entity.Property(e => e.GenerateEnabled).HasColumnName("generate_enabled");
            entity.Property(e => e.LastUpdated).HasColumnName("last_updated");
        });

        modelBuilder.Entity<Mensaje>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
                .ToTable("mensajes")
;

            entity.Property(e => e.Id)

                .HasColumnName("id");

            entity.Property(e => e.EmisorId)

                .HasColumnName("emisor_id");

            entity.Property(e => e.ReceptorId)

                .HasColumnName("receptor_id");

            entity.Property(e => e.Contenido)

                .HasColumnName("contenido");

            entity.Property(e => e.Leido)

                .HasColumnName("leido");

            entity.Property(e => e.FechaEnvio)


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
