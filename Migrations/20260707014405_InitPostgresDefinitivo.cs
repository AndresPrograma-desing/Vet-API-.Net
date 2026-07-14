using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class InitPostgresDefinitivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias_productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: true),
                    creado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias_productos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    apellido = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    direccion = table.Column<string>(type: "text", nullable: true),
                    identificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    nota = table.Column<string>(type: "text", nullable: true),
                    creado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HtmlCode = table.Column<string>(type: "text", nullable: false),
                    TypeEmail = table.Column<string>(type: "text", nullable: true),
                    Update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacturaConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Days = table.Column<int>(type: "integer", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    GenerateEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "metodos_pago",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    creado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metodos_pago", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MoneyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MoneyName = table.Column<string>(type: "text", nullable: false),
                    BcvDollar = table.Column<decimal>(type: "numeric", nullable: false),
                    DollarPersistence = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReporConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Days = table.Column<int>(type: "integer", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    GenerateEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reportes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Categoria = table.Column<string>(type: "text", nullable: true),
                    Filtro = table.Column<string>(type: "text", nullable: true),
                    Datos = table.Column<string>(type: "text", nullable: true),
                    GeneradoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "system_configs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    frontend_url = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    backend_external_url = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_configs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    apellido = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    contraseña = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordRecoveryCode = table.Column<string>(type: "text", nullable: true),
                    CodeRecoveryExpireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rol = table.Column<string>(type: "text", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: true),
                    ultimo_acceso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "WSMessageAPIData",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    ApiKey = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    Update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WSMessageAPIData", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    categoria_id = table.Column<int>(type: "integer", nullable: true),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    precio = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    precio_venta = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    stock = table.Column<int>(type: "integer", nullable: true),
                    stock_minimo = table.Column<int>(type: "integer", nullable: true),
                    unidad_medida = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    proveedor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    requiere_receta = table.Column<bool>(type: "boolean", nullable: true),
                    creado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos", x => x.id);
                    table.ForeignKey(
                        name: "productos_ibfk_1",
                        column: x => x.categoria_id,
                        principalTable: "categorias_productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "mascotas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cliente_id = table.Column<int>(type: "integer", nullable: false),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    especie = table.Column<string>(type: "text", nullable: false),
                    raza = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    color = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    sexo = table.Column<string>(type: "text", nullable: false),
                    fecha_nacimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    peso = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    identeficacion_mascota = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    alergias = table.Column<string>(type: "text", nullable: true),
                    condiciones_medicas = table.Column<string>(type: "text", nullable: true),
                    esterilizado = table.Column<bool>(type: "boolean", nullable: true),
                    creado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mascotas", x => x.id);
                    table.ForeignKey(
                        name: "mascotas_ibfk_1",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alertas_internas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    mensaje = table.Column<string>(type: "text", nullable: true),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    prioridad = table.Column<string>(type: "text", nullable: true),
                    destinatario_rol = table.Column<string>(type: "text", nullable: true),
                    destinatario_id = table.Column<int>(type: "integer", nullable: true),
                    referencia_tabla = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    referencia_id = table.Column<int>(type: "integer", nullable: true),
                    accion_requerida = table.Column<bool>(type: "boolean", nullable: true),
                    fecha_limite = table.Column<DateOnly>(type: "date", nullable: true),
                    completada = table.Column<bool>(type: "boolean", nullable: true),
                    fecha_completado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    leida = table.Column<bool>(type: "boolean", nullable: true),
                    fecha_lectura = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    leida_por = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alertas_internas", x => x.id);
                    table.ForeignKey(
                        name: "alertas_internas_ibfk_1",
                        column: x => x.destinatario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "alertas_internas_ibfk_2",
                        column: x => x.leida_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "logs_sistema",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: true),
                    accion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tabla_afectada = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    registro_id = table.Column<int>(type: "integer", nullable: true),
                    datos_previos = table.Column<string>(type: "text", nullable: true),
                    datos_nuevos = table.Column<string>(type: "text", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logs_sistema", x => x.id);
                    table.ForeignKey(
                        name: "logs_sistema_ibfk_1",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "mensajes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    emisor_id = table.Column<int>(type: "integer", nullable: false),
                    receptor_id = table.Column<int>(type: "integer", nullable: false),
                    contenido = table.Column<string>(type: "text", nullable: false),
                    leido = table.Column<bool>(type: "boolean", nullable: true),
                    fecha_envio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mensajes", x => x.id);
                    table.ForeignKey(
                        name: "mensajes_ibfk_1",
                        column: x => x.emisor_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "mensajes_ibfk_2",
                        column: x => x.receptor_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historial_precios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    producto_id = table.Column<int>(type: "integer", nullable: false),
                    precio_anterior = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    precio_nuevo = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    motivo = table.Column<string>(type: "text", nullable: true),
                    fecha_cambio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historial_precios", x => x.id);
                    table.ForeignKey(
                        name: "historial_precios_ibfk_1",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "historial_precios_ibfk_2",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movimientos_inventario",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    producto_id = table.Column<int>(type: "integer", nullable: false),
                    tipo_movimiento = table.Column<string>(type: "text", nullable: false),
                    cantidad = table.Column<int>(type: "integer", nullable: false),
                    stock_anterior = table.Column<int>(type: "integer", nullable: false),
                    stock_nuevo = table.Column<int>(type: "integer", nullable: false),
                    motivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    referencia_id = table.Column<int>(type: "integer", nullable: true),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_movimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movimientos_inventario", x => x.id);
                    table.ForeignKey(
                        name: "movimientos_inventario_ibfk_1",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "movimientos_inventario_ibfk_2",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "citas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mascota_id = table.Column<int>(type: "integer", nullable: false),
                    doctor_id = table.Column<int>(type: "integer", nullable: false),
                    secretaria_id = table.Column<int>(type: "integer", nullable: true),
                    fecha_cita = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hora_cita = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    motivo = table.Column<string>(type: "text", nullable: false),
                    tipo_cita = table.Column<string>(type: "text", nullable: true),
                    estado = table.Column<string>(type: "text", nullable: true),
                    notas = table.Column<string>(type: "text", nullable: true),
                    metodo_pago_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citas", x => x.id);
                    table.ForeignKey(
                        name: "citas_ibfk_1",
                        column: x => x.mascota_id,
                        principalTable: "mascotas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "citas_ibfk_2",
                        column: x => x.doctor_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "citas_ibfk_3",
                        column: x => x.secretaria_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "citas_ibfk_4",
                        column: x => x.metodo_pago_id,
                        principalTable: "metodos_pago",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "consultas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cita_id = table.Column<int>(type: "integer", nullable: false),
                    mascota_id = table.Column<int>(type: "integer", nullable: false),
                    doctor_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_consulta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    peso_actual = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    temperatura = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: true),
                    sintomas = table.Column<string>(type: "text", nullable: false),
                    diagnostico = table.Column<string>(type: "text", nullable: true),
                    tratamiento = table.Column<string>(type: "text", nullable: true),
                    receta = table.Column<string>(type: "text", nullable: true),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    ConsultaPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    creado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consultas", x => x.id);
                    table.ForeignKey(
                        name: "consultas_ibfk_1",
                        column: x => x.cita_id,
                        principalTable: "citas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "consultas_ibfk_2",
                        column: x => x.mascota_id,
                        principalTable: "mascotas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "consultas_ibfk_3",
                        column: x => x.doctor_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "consultas_productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    consulta_id = table.Column<int>(type: "integer", nullable: false),
                    producto_id = table.Column<int>(type: "integer", nullable: false),
                    cantidad = table.Column<int>(type: "integer", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    dosis = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    via_administracion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    frecuencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    duracion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    instrucciones = table.Column<string>(type: "text", nullable: true),
                    aplicado_por = table.Column<int>(type: "integer", nullable: true),
                    creado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consultas_productos", x => x.id);
                    table.ForeignKey(
                        name: "consultas_productos_ibfk_1",
                        column: x => x.consulta_id,
                        principalTable: "consultas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "consultas_productos_ibfk_2",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "consultas_productos_ibfk_3",
                        column: x => x.aplicado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "facturas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numero_factura = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cliente_id = table.Column<int>(type: "integer", nullable: false),
                    mascota_id = table.Column<int>(type: "integer", nullable: true),
                    consulta_id = table.Column<int>(type: "integer", nullable: true),
                    secretaria_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_emision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    descuento = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    metodo_pago = table.Column<string>(type: "text", nullable: false),
                    estado_pago = table.Column<string>(type: "text", nullable: true),
                    notas = table.Column<string>(type: "text", nullable: true),
                    url_docx = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    creado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actualizado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facturas", x => x.id);
                    table.ForeignKey(
                        name: "facturas_ibfk_1",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "facturas_ibfk_2",
                        column: x => x.mascota_id,
                        principalTable: "mascotas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "facturas_ibfk_3",
                        column: x => x.consulta_id,
                        principalTable: "consultas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "facturas_ibfk_4",
                        column: x => x.secretaria_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vacunas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mascota_id = table.Column<int>(type: "integer", nullable: false),
                    producto_id = table.Column<int>(type: "integer", nullable: false),
                    consulta_id = table.Column<int>(type: "integer", nullable: true),
                    fecha_vacunacion = table.Column<DateOnly>(type: "date", nullable: false),
                    proxima_dosis = table.Column<DateOnly>(type: "date", nullable: true),
                    lote = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    doctor_id = table.Column<int>(type: "integer", nullable: true),
                    nota = table.Column<string>(type: "text", nullable: true),
                    creado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacunas", x => x.id);
                    table.ForeignKey(
                        name: "vacunas_ibfk_1",
                        column: x => x.mascota_id,
                        principalTable: "mascotas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "vacunas_ibfk_2",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "vacunas_ibfk_3",
                        column: x => x.consulta_id,
                        principalTable: "consultas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "vacunas_ibfk_4",
                        column: x => x.doctor_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "detalles_factura",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    factura_id = table.Column<int>(type: "integer", nullable: false),
                    producto_id = table.Column<int>(type: "integer", nullable: false),
                    productos_consultas_id = table.Column<int>(type: "integer", nullable: true),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    cantidad = table.Column<int>(type: "integer", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalles_factura", x => x.id);
                    table.ForeignKey(
                        name: "detalles_factura_ibfk_1",
                        column: x => x.factura_id,
                        principalTable: "facturas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "detalles_factura_ibfk_2",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "detalles_factura_ibfk_3",
                        column: x => x.productos_consultas_id,
                        principalTable: "consultas_productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "destinatario_id",
                table: "alertas_internas",
                column: "destinatario_id");

            migrationBuilder.CreateIndex(
                name: "idx_fecha_limite",
                table: "alertas_internas",
                column: "fecha_limite");

            migrationBuilder.CreateIndex(
                name: "idx_pendientes",
                table: "alertas_internas",
                columns: new[] { "leida", "completada", "prioridad" });

            migrationBuilder.CreateIndex(
                name: "idx_rol",
                table: "alertas_internas",
                column: "destinatario_rol");

            migrationBuilder.CreateIndex(
                name: "leida_por",
                table: "alertas_internas",
                column: "leida_por");

            migrationBuilder.CreateIndex(
                name: "nombre",
                table: "categorias_productos",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_doctor_fecha",
                table: "citas",
                columns: new[] { "doctor_id", "fecha_cita" });

            migrationBuilder.CreateIndex(
                name: "idx_estado",
                table: "citas",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "idx_fecha",
                table: "citas",
                column: "fecha_cita");

            migrationBuilder.CreateIndex(
                name: "mascota_id",
                table: "citas",
                column: "mascota_id");

            migrationBuilder.CreateIndex(
                name: "metodo_pago_id",
                table: "citas",
                column: "metodo_pago_id");

            migrationBuilder.CreateIndex(
                name: "secretaria_id",
                table: "citas",
                column: "secretaria_id");

            migrationBuilder.CreateIndex(
                name: "unique_cita_doctor",
                table: "citas",
                columns: new[] { "doctor_id", "fecha_cita", "hora_cita" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "email",
                table: "clientes",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "identificacion",
                table: "clientes",
                column: "identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_Busqueda_clientes",
                table: "clientes",
                columns: new[] { "nombre", "apellido", "email" });

            migrationBuilder.CreateIndex(
                name: "idx_nombre",
                table: "clientes",
                columns: new[] { "nombre", "apellido" });

            migrationBuilder.CreateIndex(
                name: "idx_telefono",
                table: "clientes",
                column: "telefono");

            migrationBuilder.CreateIndex(
                name: "cita_id",
                table: "consultas",
                column: "cita_id");

            migrationBuilder.CreateIndex(
                name: "doctor_id",
                table: "consultas",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "idx_fecha_consulta",
                table: "consultas",
                column: "fecha_consulta");

            migrationBuilder.CreateIndex(
                name: "idx_mascota",
                table: "consultas",
                column: "mascota_id");

            migrationBuilder.CreateIndex(
                name: "aplicado_por",
                table: "consultas_productos",
                column: "aplicado_por");

            migrationBuilder.CreateIndex(
                name: "idx_consulta_producto",
                table: "consultas_productos",
                column: "consulta_id");

            migrationBuilder.CreateIndex(
                name: "producto_id",
                table: "consultas_productos",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "idx_factura",
                table: "detalles_factura",
                column: "factura_id");

            migrationBuilder.CreateIndex(
                name: "producto_id1",
                table: "detalles_factura",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "productos_consultas_id",
                table: "detalles_factura",
                column: "productos_consultas_id");

            migrationBuilder.CreateIndex(
                name: "consulta_id",
                table: "facturas",
                column: "consulta_id");

            migrationBuilder.CreateIndex(
                name: "idx_cliente_factura",
                table: "facturas",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "idx_estado_pago",
                table: "facturas",
                column: "estado_pago");

            migrationBuilder.CreateIndex(
                name: "idx_factura_fecha",
                table: "facturas",
                columns: new[] { "fecha_emision", "estado_pago" });

            migrationBuilder.CreateIndex(
                name: "idx_fecha_emision",
                table: "facturas",
                column: "fecha_emision");

            migrationBuilder.CreateIndex(
                name: "mascota_id1",
                table: "facturas",
                column: "mascota_id");

            migrationBuilder.CreateIndex(
                name: "numero_factura",
                table: "facturas",
                column: "numero_factura",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "secretaria_id1",
                table: "facturas",
                column: "secretaria_id");

            migrationBuilder.CreateIndex(
                name: "idx_producto_fecha",
                table: "historial_precios",
                columns: new[] { "producto_id", "fecha_cambio" });

            migrationBuilder.CreateIndex(
                name: "usuario_id",
                table: "historial_precios",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "idx_fecha_log",
                table: "logs_sistema",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_usuario_log",
                table: "logs_sistema",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "identeficacion_mascota",
                table: "mascotas",
                column: "identeficacion_mascota",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_Busqueda_mascotas",
                table: "mascotas",
                columns: new[] { "nombre", "especie", "raza" });

            migrationBuilder.CreateIndex(
                name: "idx_cliente_id",
                table: "mascotas",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "idx_nombre_mascota",
                table: "mascotas",
                column: "nombre");

            migrationBuilder.CreateIndex(
                name: "IX_mensajes_emisor_id",
                table: "mensajes",
                column: "emisor_id");

            migrationBuilder.CreateIndex(
                name: "IX_mensajes_receptor_id",
                table: "mensajes",
                column: "receptor_id");

            migrationBuilder.CreateIndex(
                name: "nombre1",
                table: "metodos_pago",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_fecha_movimiento",
                table: "movimientos_inventario",
                column: "fecha_movimiento");

            migrationBuilder.CreateIndex(
                name: "idx_producto_movimiento",
                table: "movimientos_inventario",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "usuario_id1",
                table: "movimientos_inventario",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "categoria_id",
                table: "productos",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "codigo",
                table: "productos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_Busqueda_productos",
                table: "productos",
                columns: new[] { "nombre", "codigo", "categoria_id" });

            migrationBuilder.CreateIndex(
                name: "idx_nombre_producto",
                table: "productos",
                column: "nombre");

            migrationBuilder.CreateIndex(
                name: "idx_stock",
                table: "productos",
                column: "stock");

            migrationBuilder.CreateIndex(
                name: "email1",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_rol1",
                table: "usuarios",
                column: "rol");

            migrationBuilder.CreateIndex(
                name: "consulta_id1",
                table: "vacunas",
                column: "consulta_id");

            migrationBuilder.CreateIndex(
                name: "doctor_id1",
                table: "vacunas",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "idx_fecha_vacuna",
                table: "vacunas",
                column: "fecha_vacunacion");

            migrationBuilder.CreateIndex(
                name: "idx_mascota_vacuna",
                table: "vacunas",
                column: "mascota_id");

            migrationBuilder.CreateIndex(
                name: "producto_id2",
                table: "vacunas",
                column: "producto_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alertas_internas");

            migrationBuilder.DropTable(
                name: "detalles_factura");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropTable(
                name: "FacturaConfigs");

            migrationBuilder.DropTable(
                name: "historial_precios");

            migrationBuilder.DropTable(
                name: "logs_sistema");

            migrationBuilder.DropTable(
                name: "mensajes");

            migrationBuilder.DropTable(
                name: "MoneyTypes");

            migrationBuilder.DropTable(
                name: "movimientos_inventario");

            migrationBuilder.DropTable(
                name: "ReporConfigs");

            migrationBuilder.DropTable(
                name: "Reportes");

            migrationBuilder.DropTable(
                name: "system_configs");

            migrationBuilder.DropTable(
                name: "vacunas");

            migrationBuilder.DropTable(
                name: "WSMessageAPIData");

            migrationBuilder.DropTable(
                name: "facturas");

            migrationBuilder.DropTable(
                name: "consultas_productos");

            migrationBuilder.DropTable(
                name: "consultas");

            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropTable(
                name: "citas");

            migrationBuilder.DropTable(
                name: "categorias_productos");

            migrationBuilder.DropTable(
                name: "mascotas");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "metodos_pago");

            migrationBuilder.DropTable(
                name: "clientes");
        }
    }
}
