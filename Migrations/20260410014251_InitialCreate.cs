using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "categorias_productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descripcion = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'"),
                    creado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()"),
                    actualizado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apellido = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefono = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    direccion = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    identificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nota = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    creado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()"),
                    actualizado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "metodos_pago",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    creado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()"),
                    actualizado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apellido = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contraseña = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rol = table.Column<string>(type: "enum('admin','doctor','secretaria')", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'"),
                    ultimo_acceso = table.Column<DateTime>(type: "datetime", nullable: true),
                    creado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()"),
                    actualizado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nombre = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descripcion = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    categoria_id = table.Column<int>(type: "int(11)", nullable: true),
                    tipo = table.Column<string>(type: "enum('medicamento','alimento','accesorio')", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    precio = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    precio_venta = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    stock = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    stock_minimo = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'5'"),
                    unidad_medida = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, defaultValueSql: "'unidad'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    proveedor = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    requiere_receta = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'0'"),
                    creado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()"),
                    actualizado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "productos_ibfk_1",
                        column: x => x.categoria_id,
                        principalTable: "categorias_productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "mascotas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cliente_id = table.Column<int>(type: "int(11)", nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    especie = table.Column<string>(type: "enum('perro','gato')", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    raza = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    color = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sexo = table.Column<string>(type: "enum('macho','hembra')", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_nacimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    peso = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    identeficacion_mascota = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alergias = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    condiciones_medicas = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    esterilizado = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'0'"),
                    creado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()"),
                    actualizado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "mascotas_ibfk_1",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "alertas_internas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    titulo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mensaje = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo = table.Column<string>(type: "enum('stock_bajo','producto_vencido','cita_pendiente','tarea_admin','factura_pendiente','seguimiento_medico','sistema')", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    prioridad = table.Column<string>(type: "enum('baja','media','alta','urgente')", nullable: true, defaultValueSql: "'media'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    destinatario_rol = table.Column<string>(type: "enum('admin','doctor','secretaria','todos')", nullable: true, defaultValueSql: "'todos'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    destinatario_id = table.Column<int>(type: "int(11)", nullable: true),
                    referencia_tabla = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    referencia_id = table.Column<int>(type: "int(11)", nullable: true),
                    accion_requerida = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'0'"),
                    fecha_limite = table.Column<DateOnly>(type: "date", nullable: true),
                    completada = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'0'"),
                    fecha_completado = table.Column<DateTime>(type: "timestamp", nullable: true),
                    leida = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'0'"),
                    fecha_lectura = table.Column<DateTime>(type: "timestamp", nullable: true),
                    leida_por = table.Column<int>(type: "int(11)", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "logs_sistema",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    usuario_id = table.Column<int>(type: "int(11)", nullable: true),
                    accion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tabla_afectada = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    registro_id = table.Column<int>(type: "int(11)", nullable: true),
                    datos_previos = table.Column<string>(type: "json", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    datos_nuevos = table.Column<string>(type: "json", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ip_address = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_agent = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "logs_sistema_ibfk_1",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "mensajes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    emisor_id = table.Column<int>(type: "int(11)", nullable: false),
                    receptor_id = table.Column<int>(type: "int(11)", nullable: false),
                    contenido = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    leido = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'0'"),
                    fecha_envio = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "historial_precios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    producto_id = table.Column<int>(type: "int(11)", nullable: false),
                    precio_anterior = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    precio_nuevo = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    usuario_id = table.Column<int>(type: "int(11)", nullable: false),
                    motivo = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_cambio = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "movimientos_inventario",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    producto_id = table.Column<int>(type: "int(11)", nullable: false),
                    tipo_movimiento = table.Column<string>(type: "enum('entrada','salida','ajuste')", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cantidad = table.Column<int>(type: "int(11)", nullable: false),
                    stock_anterior = table.Column<int>(type: "int(11)", nullable: false),
                    stock_nuevo = table.Column<int>(type: "int(11)", nullable: false),
                    motivo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    referencia_id = table.Column<int>(type: "int(11)", nullable: true),
                    usuario_id = table.Column<int>(type: "int(11)", nullable: false),
                    fecha_movimiento = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "citas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mascota_id = table.Column<int>(type: "int(11)", nullable: false),
                    doctor_id = table.Column<int>(type: "int(11)", nullable: false),
                    secretaria_id = table.Column<int>(type: "int(11)", nullable: true),
                    fecha_cita = table.Column<DateTime>(type: "datetime", nullable: false),
                    hora_cita = table.Column<TimeOnly>(type: "time", nullable: false),
                    motivo = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_cita = table.Column<string>(type: "enum('consulta','vacunacion','cirugia','emergencia','seguimiento')", nullable: true, defaultValueSql: "'consulta'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado = table.Column<string>(type: "enum('programada','completada','cancelada','no_asistida','en_curso')", nullable: true, defaultValueSql: "'programada'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notas = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    metodo_pago_id = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "consultas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cita_id = table.Column<int>(type: "int(11)", nullable: false),
                    mascota_id = table.Column<int>(type: "int(11)", nullable: false),
                    doctor_id = table.Column<int>(type: "int(11)", nullable: false),
                    fecha_consulta = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()"),
                    peso_actual = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    temperatura = table.Column<decimal>(type: "decimal(4,1)", precision: 4, scale: 1, nullable: true),
                    sintomas = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    diagnostico = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tratamiento = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    receta = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observaciones = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    creado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "consultas_productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    consulta_id = table.Column<int>(type: "int(11)", nullable: false),
                    producto_id = table.Column<int>(type: "int(11)", nullable: false),
                    cantidad = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    precio_unitario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    dosis = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    via_administracion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    frecuencia = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    duracion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    instrucciones = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    aplicado_por = table.Column<int>(type: "int(11)", nullable: true),
                    creado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "facturas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    numero_factura = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cliente_id = table.Column<int>(type: "int(11)", nullable: false),
                    mascota_id = table.Column<int>(type: "int(11)", nullable: true),
                    consulta_id = table.Column<int>(type: "int(11)", nullable: true),
                    secretaria_id = table.Column<int>(type: "int(11)", nullable: false),
                    fecha_emision = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()"),
                    subtotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    descuento = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    total = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    metodo_pago = table.Column<string>(type: "enum('efectivo','tarjeta','transferencia','pago movil','otro')", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado_pago = table.Column<string>(type: "enum('pendiente','pagada','cancelada')", nullable: true, defaultValueSql: "'pendiente'", collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notas = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    url_docx = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    creado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()"),
                    actualizado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "vacunas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mascota_id = table.Column<int>(type: "int(11)", nullable: false),
                    producto_id = table.Column<int>(type: "int(11)", nullable: false),
                    consulta_id = table.Column<int>(type: "int(11)", nullable: true),
                    fecha_vacunacion = table.Column<DateOnly>(type: "date", nullable: false),
                    proxima_dosis = table.Column<DateOnly>(type: "date", nullable: true),
                    lote = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    doctor_id = table.Column<int>(type: "int(11)", nullable: true),
                    nota = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    creado = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "detalles_factura",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    factura_id = table.Column<int>(type: "int(11)", nullable: false),
                    producto_id = table.Column<int>(type: "int(11)", nullable: false),
                    productos_consultas_id = table.Column<int>(type: "int(11)", nullable: true),
                    descripcion = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cantidad = table.Column<int>(type: "int(11)", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    total = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
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
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

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
                name: "historial_precios");

            migrationBuilder.DropTable(
                name: "logs_sistema");

            migrationBuilder.DropTable(
                name: "mensajes");

            migrationBuilder.DropTable(
                name: "movimientos_inventario");

            migrationBuilder.DropTable(
                name: "vacunas");

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
