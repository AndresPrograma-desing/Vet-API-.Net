using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Constants;
using vet_api_Net.Data.seeds.Items;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Models;


/*
Seed con datos DUMMY minimos para pruebas de API.NO USAR EN PRODUCCION
Algunos datos se cargan desde appsettings.json
EL seed elimina los datos de todas las tablas y limpia sus id (los reinicia)

Para deshabilitar el Seed ir a:
-appsettings.json
    -SeedData
        -Initialize = false (false = deshabilitada / true = habilitada)
-DummyData
    -Contiene los datos para el seed
    -Admin
    -Doctor
    -Secretaria
*/

namespace vet_api_Net.Data.seeds
{

    public static class SeedsData
    {
        const string emails = "ignacioangel671@gmail.com";

        public static void Initialize(AppDbContext context, ApiSettingsOptions apiSettings, SeedDataOptions seedDataOptions)
        {
            //Reseteo de tablas. La tabla usuarios solo se trunca si seedDataOptions.SeedUsers está habilitado,
            //lo que permite regenerar el resto de los datos conservando los usuarios existentes.
            var truncateUsuariosNpgsql = seedDataOptions.SeedUsers ? "TRUNCATE TABLE usuarios RESTART IDENTITY CASCADE;" : "";
            var truncateUsuariosMySql = seedDataOptions.SeedUsers ? "TRUNCATE TABLE usuarios;" : "";

            if (context.Database.IsNpgsql())
            {
                var npgsqlTruncateSql = @"
                    TRUNCATE TABLE pet_vaccinations RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE vaccine_batches RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE vaccine_scheme_stages RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE vaccines RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE detalles_factura RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE consultas_productos RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE historial_precios RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE movimientos_inventario RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE facturas RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE consultas RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE citas RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE mascotas RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE clientes RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE productos RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE especies RESTART IDENTITY CASCADE;
                    {{TRUNCATE_USUARIOS}}
                    TRUNCATE TABLE categorias_productos RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE metodos_pago RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE ""MoneyTypes"" RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE worker_configs RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE ""Reportes"" RESTART IDENTITY CASCADE;
                    TRUNCATE TABLE system_configs RESTART IDENTITY CASCADE;
                ".Replace("{{TRUNCATE_USUARIOS}}", truncateUsuariosNpgsql);
                context.Database.ExecuteSqlRaw(npgsqlTruncateSql);
            }
            else
            {
                var mySqlTruncateSql = @"
                    SET FOREIGN_KEY_CHECKS = 0;
                    TRUNCATE TABLE pet_vaccinations;
                    TRUNCATE TABLE vaccine_batches;
                    TRUNCATE TABLE vaccine_scheme_stages;
                    TRUNCATE TABLE vaccines;
                    TRUNCATE TABLE detalles_factura;
                    TRUNCATE TABLE consultas_productos;
                    TRUNCATE TABLE historial_precios;
                    TRUNCATE TABLE movimientos_inventario;
                    TRUNCATE TABLE facturas;
                    TRUNCATE TABLE consultas;
                    TRUNCATE TABLE citas;
                    TRUNCATE TABLE mascotas;
                    TRUNCATE TABLE clientes;
                    TRUNCATE TABLE productos;
                    TRUNCATE TABLE especies;
                    {{TRUNCATE_USUARIOS}}
                    TRUNCATE TABLE categorias_productos;
                    TRUNCATE TABLE metodos_pago;
                    TRUNCATE TABLE MoneyTypes;
                    TRUNCATE TABLE worker_configs;
                    TRUNCATE TABLE Reportes;
                    TRUNCATE TABLE system_configs;
                    TRUNCATE TABLE consultas;
                    TRUNCATE TABLE citas;
                    SET FOREIGN_KEY_CHECKS = 1;
                ".Replace("{{TRUNCATE_USUARIOS}}", truncateUsuariosMySql);
                context.Database.ExecuteSqlRaw(mySqlTruncateSql);
            }

            if (seedDataOptions.SeedUsers && !context.Usuarios.Any() && seedDataOptions.DummyData != null)
            {
                foreach (var userEntry in seedDataOptions.DummyData.Values)
                {
                    context.Usuarios.Add(new Usuario
                    {
                        Email = userEntry.Email,
                        Password = BCrypt.Net.BCrypt.HashPassword(userEntry.Password),
                        Rol = userEntry.Rol,
                        Nombre = userEntry.Nombre,
                        Apellido = userEntry.Apellido,
                        Telefono = userEntry.Telefono ?? "0000-0000000",
                        AvatarUrl = userEntry.AvatarUrl,
                        Activo = true,
                        Creado = DateTime.Now,
                        Actualizado = DateTime.Now
                    });
                }
                context.SaveChanges();
            }

            // Asegurar que el usuario de Groq / Asistente IA existe en la BD
            var groqEmail = "groq@happy-pets.dev";
            var groqUser = context.Usuarios.FirstOrDefault(u => u.Email == groqEmail);
            if (groqUser == null)
            {
                context.Usuarios.Add(new Usuario
                {
                    Nombre = "Asistente Groq",
                    Apellido = "IA",
                    Email = groqEmail,
                    Password = BCrypt.Net.BCrypt.HashPassword("groq_secure_system_pass_123!"),
                    Rol = "assistant",
                    Telefono = "0000-0000000",
                    AvatarUrl = "https://api.dicebear.com/7.x/bottts/svg?seed=groq",
                    Activo = true,
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                });
                context.SaveChanges();
            }

            var doctor = context.Usuarios.FirstOrDefault(u => u.Rol == "doctor") ?? context.Usuarios.FirstOrDefault();
            var secretaria = context.Usuarios.FirstOrDefault(u => u.Rol == "secretaria") ?? context.Usuarios.FirstOrDefault();

            if (doctor == null || secretaria == null)
            {
                Console.WriteLine("No hay usuarios en la base de datos");
                return;
            }

            var metodosDefecto = new[] {
                "efectivo",
                "transferencia",
                "pago movil",
                "no definido",
                "pendiente",
                "otro"
            };
            foreach (var m in metodosDefecto)
            {
                if (!context.MetodoPagos.Any(x => x.Nombre == m))
                {
                    context.MetodoPagos.Add(new MetodoPago
                    {
                        Nombre = m,
                        Creado = DateTime.Now,
                        Actualizado = DateTime.Now
                    });
                }
            }
            context.SaveChanges();
            var metodoPago = context.MetodoPagos.FirstOrDefault()!;

            if (!context.MoneyTypes.Any())
            {
                context.MoneyTypes.Add(new MoneyType
                {
                    MoneyName = apiSettings.USD ?? "USD",
                    BcvDollar = 500.50m,
                    DollarPersistence = "USD",
                    Fecha = DateTime.Now
                });
                context.SaveChanges();
            }

            var emailTemplatesPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Templates", "Email");
            if (System.IO.Directory.Exists(emailTemplatesPath))
            {
                var templateFiles = System.IO.Directory.GetFiles(emailTemplatesPath, "*.html");
                foreach (var file in templateFiles)
                {
                    var templateName = System.IO.Path.GetFileNameWithoutExtension(file);
                    var existingTemplate = context.EmailTemplates.FirstOrDefault(t => t.TypeEmail == templateName);
                    if (existingTemplate == null)
                    {
                        context.EmailTemplates.Add(new EmailTemplate
                        {
                            TypeEmail = templateName,
                            HtmlCode = System.IO.File.ReadAllText(file),
                            CreatedAt = DateTime.Now,
                            Update = DateTime.Now
                        });
                    }
                    else
                    {
                        existingTemplate.HtmlCode = System.IO.File.ReadAllText(file);
                        existingTemplate.Update = DateTime.Now;
                    }
                }
                context.SaveChanges();
            }

            if (!context.WorkerConfigs.Any())
            {
                context.WorkerConfigs.AddRange(
                    new WorkerConfig
                    {
                        WorkerName = WorkerNames.DeleteFacturaWorker,
                        IsEnabled = false,
                        RetentionValue = 1,
                        RetentionUnit = "minutes",
                        LastUpdated = DateTime.Now
                    },
                    new WorkerConfig
                    {
                        WorkerName = WorkerNames.DeleteReportWorker,
                        IsEnabled = false,
                        RetentionValue = 30,
                        RetentionUnit = "minutes",
                        LastUpdated = DateTime.Now
                    },
                    new WorkerConfig
                    {
                        WorkerName = WorkerNames.AutoGenerateReportWorker,
                        GenerateEnabled = false,
                        LastUpdated = DateTime.Now
                    },
                    new WorkerConfig
                    {
                        WorkerName = WorkerNames.VaccinationReminderWorker,
                        IsEnabled = true,
                        IntervalValue = 1,
                        IntervalUnit = "minutes",
                        LastUpdated = DateTime.Now
                    }
                );
                context.SaveChanges();
            }

            var clientes = new List<Cliente>
            {
                new Cliente {
                    Nombre = "Ignacio",
                    Apellido = "Angel",
                    Identificacion = "V-12345678",
                    Email = emails,
                    Telefono = "584129361132",
                    Direccion = "Av 1, Calle 2",
                    Nota = "Cliente frecuente",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new Cliente {
                    Nombre = "Anastacia",
                    Apellido = "Martinez",
                    Identificacion = "V-87654321",
                    Email = "anastacia.martinez@gmail.com",
                    Telefono = "04121234567",
                    Direccion = "Urb Las Palmas",
                    Nota = "Requiere llamar antes",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new Cliente {
                    Nombre = "Luis",
                    Apellido = "Rodriguez",
                    Identificacion = "V-11223344",
                    Email = "luis.rodriguez@gmail.com",
                    Telefono = "04241234567",
                    Direccion = "Centro",
                    Nota = "Cliente VIP",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new Cliente {
                    Nombre = "Maria",
                    Apellido = "Perez",
                    Identificacion = "V-15678901",
                    Email = "maria.perez@gmail.com",
                    Telefono = "04161234567",
                    Direccion = "El Paraiso",
                    Nota = "Nueva clienta",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new Cliente {
                    Nombre = "Jorge",
                    Apellido = "Diaz",
                    Identificacion = "V-21345678",
                    Email = "jorge.diaz@gmail.com",
                    Telefono = "04149876543",
                    Direccion = "Chacao",
                    Nota = "Deuda pendiente",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new Cliente {
                    Nombre = "Sofia",
                    Apellido = "Gomez",
                    Identificacion = "V-25678912",
                    Email = "sofia.gomez@gmail.com",
                    Telefono = "04241112233",
                    Direccion = "La Castellana",
                    Nota = "Familiar de Carlos",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                }
            };
            context.Clientes.AddRange(clientes);
            context.SaveChanges();

            if (!context.Especies.Any())
            {
                context.Especies.AddRange(
                    new Especie { Nombre = "canino" },
                    new Especie { Nombre = "felino" });
                context.SaveChanges();
            }
            var especieCanino = context.Especies.First(e => e.Nombre == "canino");
            var especieFelino = context.Especies.First(e => e.Nombre == "felino");

            var mascotas = new List<Mascota>
            {
                new Mascota {
                    ClienteId = clientes[0].Id,
                    Nombre = "Firulais",
                    EspecieId = especieCanino.Id,
                    Sexo = "macho",
                    Color = "Marrón",
                    Peso = 15.5m,
                    IdenteficacionMascota = "M-001",
                    Esterilizado = true,
                    Raza = "Mestizo",
                    FechaNacimiento = new DateOnly(2020, 5, 10),
                    Alergias = "Ninguna",
                    CondicionesMedicas = "Sano",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new Mascota {
                    ClienteId = clientes[1].Id,
                    Nombre = "Michi",
                    EspecieId = especieFelino.Id,
                    Sexo = "macho",
                    Color = "Blanco",
                    Peso = 4.2m,
                    IdenteficacionMascota = "M-002",
                    Esterilizado = false,
                    Raza = "Siamés",
                    FechaNacimiento = new DateOnly(2021, 8, 20),
                    Alergias = "Polvo",
                    CondicionesMedicas = "Asma leve",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new Mascota {
                    ClienteId = clientes[2].Id,
                    Nombre = "Rex",
                    EspecieId = especieCanino.Id,
                    Sexo = "macho",
                    Color = "Negro",
                    Peso = 30.0m,
                    IdenteficacionMascota = "M-003",
                    Esterilizado = true,
                    Raza = "Pastor Alemán",
                    FechaNacimiento = new DateOnly(2019, 2, 15),
                    Alergias = "Ninguna",
                    CondicionesMedicas = "Displasia de cadera",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new Mascota {
                    ClienteId = clientes[3].Id,
                    Nombre = "Luna",
                    EspecieId = especieFelino.Id,
                    Sexo = "hembra",
                    Color = "Gris",
                    Peso = 3.5m,
                    IdenteficacionMascota = "M-004",
                    Esterilizado = true,
                    Raza = "Persa",
                    FechaNacimiento = new DateOnly(2022, 11, 1),
                    Alergias = "Pescado",
                    CondicionesMedicas = "Sano",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new Mascota {
                    ClienteId = clientes[4].Id,
                    Nombre = "Rocky",
                    EspecieId = especieCanino.Id,
                    Sexo = "macho",
                    Color = "Dorado",
                    Peso = 25.0m,
                    IdenteficacionMascota = "M-005",
                    Esterilizado = false,
                    Raza = "Golden Retriever",
                    FechaNacimiento = new DateOnly(2023, 1, 5),
                    Alergias = "Picaduras de pulgas",
                    CondicionesMedicas = "Sano",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new Mascota {
                    ClienteId = clientes[5].Id,
                    Nombre = "Pelusa",
                    EspecieId = especieFelino.Id,
                    Sexo = "hembra",
                    Color = "Blanco y Negro",
                    Peso = 5.1m,
                    IdenteficacionMascota = "M-006",
                    Esterilizado = true,
                    Raza = "Mestizo",
                    FechaNacimiento = new DateOnly(2018, 7, 22),
                    Alergias = "Lácteos",
                    CondicionesMedicas = "Sano",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new Mascota {
                    ClienteId = clientes[3].Id,
                    Nombre = "Max",
                    EspecieId = especieCanino.Id,
                    Sexo = "macho",
                    Color = "Blanco",
                    Peso = 8.0m, IdenteficacionMascota = "M-007",
                    Esterilizado = true,
                    Raza = "Poodle",
                    FechaNacimiento = new DateOnly(2021, 4, 18),
                    Alergias = "Ninguna",
                    CondicionesMedicas = "Sano",
                    Creado = DateTime.Now, Actualizado = DateTime.Now }
            };
            context.Mascotas.AddRange(mascotas);
            context.SaveChanges();

            var categoriasDefecto = new[]
            {
                new CategoriasProducto {
                    Nombre = "Medicinas",
                    Descripcion = "Medicamentos",
                    Activo = true,
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new CategoriasProducto {
                    Nombre = "Alimentos",
                    Descripcion = "Alimentos para mascotas",
                    Activo = true,
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new CategoriasProducto {
                    Nombre = "Accesorios",
                    Descripcion = "Collares, juguetes, etc.",
                    Activo = true,
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new CategoriasProducto {
                    Nombre = "Higiene",
                    Descripcion = "Shampoos y limpieza",
                    Activo = true,
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                }
            };
            foreach (var cat in categoriasDefecto)
            {
                if (!context.CategoriasProductos.Any(c => c.Nombre == cat.Nombre))
                {
                    context.CategoriasProductos.Add(cat);
                }
            }
            context.SaveChanges();

            // Catálogo dummy de 200 productos (50 por categoría, ver Data/seeds/Items/) para poblar
            // listados, búsquedas y paginación con volumen realista. Nombres no necesariamente reales.
            var catalogoPorCategoria = new (string CategoriaNombre, string[] Nombres, string Tipo, string UnidadMedida, bool PuedeRequerirReceta)[]
            {
                ("Medicinas", MedicineItems.Nombres, "medicamento", "unidad", true),
                ("Alimentos", FoodItems.Nombres, "alimento", "unidad", false),
                ("Accesorios", AccessoryItems.Nombres, "accesorio", "unidad", false),
                ("Higiene", HygieneItems.Nombres, "higiene", "unidad", false)
            };

            var productos = new List<Producto>();
            var rng = new Random(20260825);
            var consecutivo = 1;
            foreach (var (categoriaNombre, nombres, tipo, unidadMedida, puedeRequerirReceta) in catalogoPorCategoria)
            {
                var categoriaProducto = context.CategoriasProductos.First(c => c.Nombre == categoriaNombre);
                foreach (var nombre in nombres)
                {
                    var precio = Math.Round((decimal)(rng.NextDouble() * 45 + 2), 2);
                    var margen = 1.2m + (decimal)(rng.NextDouble() * 0.5);
                    productos.Add(new Producto
                    {
                        Codigo = $"P-{consecutivo:D4}",
                        Nombre = nombre,
                        CategoriaId = categoriaProducto.Id,
                        Tipo = tipo,
                        Precio = precio,
                        PrecioVenta = Math.Round(precio * margen, 2),
                        Stock = rng.Next(0, 120),
                        StockMinimo = rng.Next(5, 20),
                        UnidadMedida = unidadMedida,
                        RequiereReceta = puedeRequerirReceta && rng.NextDouble() < 0.5,
                        Descripcion = $"{nombre} - catálogo {categoriaNombre.ToLower()}.",
                        Proveedor = "Proveedor Genérico",
                        Creado = DateTime.Now,
                        Actualizado = DateTime.Now
                    });
                    consecutivo++;
                }
            }
            context.Productos.AddRange(productos);
            context.SaveChanges();

            var baseDate = DateTime.Now;
            var citaDate1 = baseDate.AddHours(1); // Dentro de 1 hora
            var citaDate2 = baseDate.AddHours(2); // Dentro de 2 horas
            var citaDate3 = baseDate.AddHours(3); // Dentro de 3 horas
            var citaDate4 = baseDate.AddHours(4); // Dentro de 4 horas
            var citaDate5 = baseDate.AddHours(5); // Dentro de 5 horas
            var citaDate6 = baseDate.AddHours(6); // Dentro de 6 horas

            var citas = new List<Cita>
            {
                new Cita {
                    DoctorId = doctor.Id,
                    MascotaId = mascotas[0].Id,
                    SecretariaId = secretaria.Id,
                    FechaCita = citaDate1.Date,
                    HoraCita = TimeOnly.FromDateTime(citaDate1),
                    Estado = Status.Completed,
                    TipoCita = "consulta",
                    Motivo = "Control general",
                    MetodoPagoId = metodoPago.Id,
                    Notas = "Llamar al cliente 30 min antes"
                },
                new Cita {
                    DoctorId = doctor.Id,
                    MascotaId = mascotas[1].Id,
                    SecretariaId = secretaria.Id,
                    FechaCita = citaDate2.Date,
                    HoraCita = TimeOnly.FromDateTime(citaDate2),
                    Estado = Status.Programed,
                    TipoCita = "vacunacion",
                    Motivo = "Vacuna anual",
                    MetodoPagoId = metodoPago.Id,
                    Notas = "Traer carnet de vacunación"
                },
                new Cita {
                    DoctorId = doctor.Id,
                    MascotaId = mascotas[2].Id,
                    SecretariaId = secretaria.Id,
                    FechaCita = citaDate3.Date,
                    HoraCita = TimeOnly.FromDateTime(citaDate3),
                    Estado = Status.Programed,
                    TipoCita = "consulta",
                    Motivo = "Revisión general",
                    MetodoPagoId = metodoPago.Id,
                    Notas = "Primera vez"
                },
                new Cita {
                    DoctorId = doctor.Id,
                    MascotaId = mascotas[3].Id,
                    SecretariaId = secretaria.Id,
                    FechaCita = citaDate4.Date,
                    HoraCita = TimeOnly.FromDateTime(citaDate4),
                    Estado = Status.Cancelled,
                    TipoCita = "emergencia",
                    Motivo = "Dolor abdominal",
                    MetodoPagoId = metodoPago.Id,
                    Notas = "Cliente canceló por teléfono"
                },
                new Cita {
                    DoctorId = doctor.Id,
                    MascotaId = mascotas[4].Id,
                    SecretariaId = secretaria.Id,
                    FechaCita = citaDate5.Date,
                    HoraCita = TimeOnly.FromDateTime(citaDate5),
                    Estado = Status.InCurso,
                    TipoCita = "cirugia",
                    Motivo = "Castración",
                    MetodoPagoId = metodoPago.Id,
                    Notas = "En quirófano"
                },
                new Cita {
                    DoctorId = doctor.Id,
                    MascotaId = mascotas[5].Id,
                    SecretariaId = secretaria.Id,
                    FechaCita = citaDate6.Date,
                    HoraCita = TimeOnly.FromDateTime(citaDate6),
                    Estado = Status.NotAssisted,
                    TipoCita = "consulta",
                    Motivo = "Seguimiento",
                    MetodoPagoId = metodoPago.Id,
                    Notas = "No se presentó"
                }
            };
            context.Citas.AddRange(citas);
            context.SaveChanges();

            var consultas = new List<Consulta>
            {
                new Consulta {
                    CitaId = citas[0].Id,
                    MascotaId = mascotas[0].Id,
                    DoctorId = doctor.Id,
                    FechaConsulta = DateTime.Now,
                    Sintomas = "Ninguno",
                    Diagnostico = "Sano",
                    Tratamiento = "Vitaminas",
                    PesoActual = 15.5m,
                    Temperatura = 38.5m,
                    Receta = "Dar 1 pastilla de vitamina diaria",
                    Observaciones = "Excelente estado de salud",
                    ConsultaPrice = 20.00m,
                    Creado = DateTime.Now
                }
            };
            context.Consultas.AddRange(consultas);
            context.SaveChanges();

            // Seed del catálogo de vacunas + lotes + aplicaciones, vinculadas a las mascotas/clientes creados arriba,
            // cubriendo los 3 estados de semaforización, un esquema de cachorro en curso y una postergación médica.
            if (!context.Vaccines.Any())
            {
                var today = DateOnly.FromDateTime(DateTime.Now);

                var rabiaCanina = new Vaccine
                {
                    Name = "Antirrábica",
                    EspecieId = especieCanino.Id,
                    MinimumAgeWeeks = 12,
                    BoosterFrequencyValue = 1,
                    BoosterFrequencyUnit = "years",
                    Description = "Vacuna anual contra la rabia",
                    Price = 15.00m,
                    Active = true,
                    CreatedAt = DateTime.Now
                };
                var rabiaFelina = new Vaccine
                {
                    Name = "Antirrábica",
                    EspecieId = especieFelino.Id,
                    MinimumAgeWeeks = 12,
                    BoosterFrequencyValue = 1,
                    BoosterFrequencyUnit = "years",
                    Description = "Vacuna anual contra la rabia (felinos).",
                    Price = 15.00m,
                    Active = true,
                    CreatedAt = DateTime.Now
                };
                var parvovirusCanino = new Vaccine
                {
                    Name = "Parvovirus",
                    EspecieId = especieCanino.Id,
                    MinimumAgeWeeks = 6,
                    BoosterFrequencyValue = 1,
                    BoosterFrequencyUnit = "years",
                    Description = "Esquema inicial de cachorro (series con intervalos de 21 días) y refuerzo anual en adultos.",
                    Price = 12.00m,
                    Active = true,
                    CreatedAt = DateTime.Now
                };
                var tripleFelina = new Vaccine
                {
                    Name = "Triple Felina",
                    EspecieId = especieFelino.Id,
                    MinimumAgeWeeks = 8,
                    BoosterFrequencyValue = 1,
                    BoosterFrequencyUnit = "years",
                    Description = "Panleucopenia, Rinotraqueitis y Calicivirus.",
                    Price = 18.00m,
                    Active = true,
                    CreatedAt = DateTime.Now
                };
                var moquilloCanino = new Vaccine
                {
                    Name = "Moquillo",
                    EspecieId = especieCanino.Id,
                    MinimumAgeWeeks = 8,
                    BoosterFrequencyValue = 6,
                    BoosterFrequencyUnit = "months",
                    Description = "Refuerzo semestral en adultos.",
                    Price = 14.00m,
                    Active = true,
                    CreatedAt = DateTime.Now
                };

                context.Vaccines.AddRange(rabiaCanina, rabiaFelina, parvovirusCanino, tripleFelina, moquilloCanino);
                context.SaveChanges();

                // Esquema de cachorro para Parvovirus: dosis 1 y 2 con 21 días de intervalo hacia la siguiente.
                // Al no existir etapa para la dosis 4, el sistema cae automáticamente al refuerzo anual del catálogo.
                context.VaccineSchemeStages.AddRange(
                    new VaccineSchemeStage { VaccineId = parvovirusCanino.Id, StageType = "Initial", DoseNumber = 1, IntervalDaysFromPrevious = 21, CreatedAt = DateTime.Now },
                    new VaccineSchemeStage { VaccineId = parvovirusCanino.Id, StageType = "Initial", DoseNumber = 2, IntervalDaysFromPrevious = 21, CreatedAt = DateTime.Now }
                );
                context.SaveChanges();

                var loteRabiaCanina = new VaccineBatch
                {
                    VaccineId = rabiaCanina.Id,
                    Laboratory = "Pfizer",
                    BatchNumber = "LOT-12345",
                    ExpirationDate = today.AddMonths(10),
                    QuantityInStock = 50,
                    ReceivedDate = today.AddMonths(-1),
                    Active = true,
                    CreatedAt = DateTime.Now
                };
                var loteRabiaCaninaPorVencer = new VaccineBatch
                {
                    VaccineId = rabiaCanina.Id,
                    Laboratory = "Pfizer",
                    BatchNumber = "LOT-99001",
                    ExpirationDate = today.AddDays(20),
                    QuantityInStock = 5,
                    ReceivedDate = today.AddMonths(-6),
                    Active = true,
                    CreatedAt = DateTime.Now
                };
                var loteRabiaFelina = new VaccineBatch
                {
                    VaccineId = rabiaFelina.Id,
                    Laboratory = "Zoetis",
                    BatchNumber = "LOT-55010",
                    ExpirationDate = today.AddMonths(8),
                    QuantityInStock = 30,
                    ReceivedDate = today.AddMonths(-2),
                    Active = true,
                    CreatedAt = DateTime.Now
                };
                var loteParvovirus = new VaccineBatch
                {
                    VaccineId = parvovirusCanino.Id,
                    Laboratory = "MSD",
                    BatchNumber = "LOT-77003",
                    ExpirationDate = today.AddMonths(12),
                    QuantityInStock = 40,
                    ReceivedDate = today.AddMonths(-2),
                    Active = true,
                    CreatedAt = DateTime.Now
                };
                var loteTripleFelina = new VaccineBatch
                {
                    VaccineId = tripleFelina.Id,
                    Laboratory = "Virbac",
                    BatchNumber = "LOT-88004",
                    ExpirationDate = today.AddMonths(9),
                    QuantityInStock = 25,
                    ReceivedDate = today.AddMonths(-3),
                    Active = true,
                    CreatedAt = DateTime.Now
                };
                var loteMoquillo = new VaccineBatch
                {
                    VaccineId = moquilloCanino.Id,
                    Laboratory = "Boehringer",
                    BatchNumber = "LOT-66005",
                    ExpirationDate = today.AddMonths(7),
                    QuantityInStock = 20,
                    ReceivedDate = today.AddMonths(-1),
                    Active = true,
                    CreatedAt = DateTime.Now
                };

                context.VaccineBatches.AddRange(
                    loteRabiaCanina, loteRabiaCaninaPorVencer, loteRabiaFelina, loteParvovirus, loteTripleFelina, loteMoquillo);
                context.SaveChanges();

                // Aplicaciones vinculadas a las mascotas (y por lo tanto a los clientes) sembrados arriba:
                // Firulais (cliente Ignacio) -> Verde | Michi (cliente Anastacia) -> Amarillo | Rex (cliente Luis) -> Rojo
                // Luna (cliente Maria) -> Verde | Rocky (cliente Jorge) -> esquema de cachorro en curso (Amarillo)
                // Pelusa (cliente Sofia) -> Postergada | Max (cliente Maria) -> Verde
                var petVaccinations = new List<PetVaccination>
                {
                    new PetVaccination
                    {
                        MascotaId = mascotas[0].Id, // Firulais
                        VaccineId = rabiaCanina.Id,
                        VaccineBatchId = loteRabiaCanina.Id,
                        ConsultaId = consultas[0].Id,
                        DoctorId = doctor.Id,
                        ApplicationDate = today.AddMonths(-1),
                        NextDoseDate = today.AddMonths(11),
                        WeightAtApplication = 15.5m,
                        Dose = "1ml",
                        Status = "Applied",
                        ClinicalObservations = "Refuerzo anual administrado sin complicaciones.",
                        CreatedAt = DateTime.Now
                    },
                    new PetVaccination
                    {
                        MascotaId = mascotas[1].Id, // Michi
                        VaccineId = rabiaFelina.Id,
                        VaccineBatchId = loteRabiaFelina.Id,
                        DoctorId = doctor.Id,
                        ApplicationDate = today.AddMonths(-11),
                        NextDoseDate = today.AddDays(20),
                        WeightAtApplication = 4.0m,
                        Dose = "1ml",
                        Status = "Applied",
                        ClinicalObservations = "Próxima a vencer, notificar al cliente.",
                        CreatedAt = DateTime.Now
                    },
                    new PetVaccination
                    {
                        MascotaId = mascotas[2].Id, // Rex
                        VaccineId = rabiaCanina.Id,
                        VaccineBatchId = loteRabiaCanina.Id,
                        DoctorId = doctor.Id,
                        ApplicationDate = today.AddYears(-1).AddDays(-20),
                        NextDoseDate = today.AddDays(-20),
                        WeightAtApplication = 29.0m,
                        Dose = "1ml",
                        Status = "Applied",
                        ClinicalObservations = "Refuerzo vencido, pendiente de reagendar.",
                        CreatedAt = DateTime.Now
                    },
                    new PetVaccination
                    {
                        MascotaId = mascotas[3].Id, // Luna
                        VaccineId = tripleFelina.Id,
                        VaccineBatchId = loteTripleFelina.Id,
                        DoctorId = doctor.Id,
                        ApplicationDate = today.AddMonths(-2),
                        NextDoseDate = today.AddMonths(10),
                        WeightAtApplication = 3.5m,
                        Dose = "1ml",
                        Status = "Applied",
                        ClinicalObservations = "Esquema al día.",
                        CreatedAt = DateTime.Now
                    },
                    new PetVaccination
                    {
                        MascotaId = mascotas[4].Id, // Rocky - dosis 1 de la serie de cachorro
                        VaccineId = parvovirusCanino.Id,
                        VaccineBatchId = loteParvovirus.Id,
                        DoctorId = doctor.Id,
                        ApplicationDate = today.AddDays(-42),
                        NextDoseDate = today.AddDays(-21),
                        WeightAtApplication = 6.0m,
                        Dose = "1ml",
                        Status = "Applied",
                        ClinicalObservations = "Primera dosis del esquema inicial.",
                        CreatedAt = DateTime.Now
                    },
                    new PetVaccination
                    {
                        MascotaId = mascotas[4].Id, // Rocky - dosis 2 de la serie de cachorro
                        VaccineId = parvovirusCanino.Id,
                        VaccineBatchId = loteParvovirus.Id,
                        DoctorId = doctor.Id,
                        ApplicationDate = today.AddDays(-21),
                        NextDoseDate = today.AddDays(9),
                        WeightAtApplication = 7.2m,
                        Dose = "1ml",
                        Status = "Applied",
                        ClinicalObservations = "Segunda dosis del esquema inicial, falta la tercera.",
                        CreatedAt = DateTime.Now
                    },
                    new PetVaccination
                    {
                        MascotaId = mascotas[5].Id, // Pelusa - postergada por evaluación clínica
                        VaccineId = tripleFelina.Id,
                        VaccineBatchId = null,
                        DoctorId = doctor.Id,
                        ApplicationDate = today,
                        NextDoseDate = null,
                        Status = "Postponed",
                        PostponementReason = "Mascota con fiebre, se reprograma tras evaluación clínica.",
                        ClinicalObservations = "Se pospone la aplicación hasta que la mascota esté clínicamente estable.",
                        CreatedAt = DateTime.Now
                    },
                    new PetVaccination
                    {
                        MascotaId = mascotas[6].Id, // Max
                        VaccineId = moquilloCanino.Id,
                        VaccineBatchId = loteMoquillo.Id,
                        DoctorId = doctor.Id,
                        ApplicationDate = today.AddMonths(-1),
                        NextDoseDate = today.AddMonths(5),
                        WeightAtApplication = 8.3m,
                        Dose = "1ml",
                        Status = "Applied",
                        ClinicalObservations = "Refuerzo semestral al día.",
                        CreatedAt = DateTime.Now
                    },
                    new PetVaccination
                    {
                        MascotaId = mascotas[0].Id, // Firulais - candidato al recordatorio de 7 días de VaccinationReminderWorker
                        VaccineId = moquilloCanino.Id,
                        VaccineBatchId = loteMoquillo.Id,
                        DoctorId = doctor.Id,
                        ApplicationDate = today.AddDays(-173),
                        NextDoseDate = today.AddDays(VaccinationVariables.ReminderSevenDays),
                        WeightAtApplication = 15.5m,
                        Dose = "1ml",
                        Status = "Applied",
                        ClinicalObservations = "Refuerzo semestral próximo a vencer en 7 días.",
                        CreatedAt = DateTime.Now
                    },
                    new PetVaccination
                    {
                        MascotaId = mascotas[2].Id, // Rex - candidato al recordatorio de 3 días de VaccinationReminderWorker
                        VaccineId = parvovirusCanino.Id,
                        VaccineBatchId = loteParvovirus.Id,
                        DoctorId = doctor.Id,
                        ApplicationDate = today.AddYears(-1).AddDays(-3),
                        NextDoseDate = today.AddDays(VaccinationVariables.ReminderThreeDays),
                        WeightAtApplication = 30.0m,
                        Dose = "1ml",
                        Status = "Applied",
                        ClinicalObservations = "Refuerzo anual próximo a vencer en 3 días.",
                        CreatedAt = DateTime.Now
                    }
                };
                context.PetVaccinations.AddRange(petVaccinations);

                // Reflejar el descuento de stock que habría hecho RegisterApplicationAsync por cada dosis aplicada.
                loteRabiaCanina.QuantityInStock -= 2; // Firulais + Rex
                loteRabiaFelina.QuantityInStock -= 1; // Michi
                loteTripleFelina.QuantityInStock -= 1; // Luna (Pelusa quedó postergada, no consume lote)
                loteParvovirus.QuantityInStock -= 3; // Rocky (dosis 1 y 2) + Rex
                loteMoquillo.QuantityInStock -= 2; // Max + Firulais

                context.SaveChanges();
            }

            if (!context.SystemConfigs.Any())
            {
                var systemConfig = new SystemConfig
                {
                    FrontendUrl = "https://happy-pets-web.vercel.app",
                    BackendExternalUrl = "https://g27frlv5-5168.use2.devtunnels.ms/",
                    BcvApiUrl = "https://www.bcv.org.ve",
                    ResendApiUrl = "https://api.resend.com/emails",
                    ResendApiKey = "re_8ihXsxrL_NRxgtRcoyqjou3J75MjbJdFo",
                    ResendFromEmail = "HappyPets <onboarding@resend.dev>",
                    LastUpdated = DateTime.UtcNow
                };
                context.SystemConfigs.Add(systemConfig);
                context.SaveChanges();
            }

            if (!context.IaConocimientos.Any())
            {
                var defaultIaConfig = new IaConocimiento
                {
                    Categoria = "general",
                    ReglasRespuesta = "1. Responder con tono profesional y empático.\n2. Si hay dudas médicas complejas, sugerir consultar con un veterinario.",
                    BaseConocimiento = "Happy Pets es una clínica veterinaria con servicios de consulta, vacunación y hospitalización. Contamos con médicos especialistas en caninos y felinos.",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                };
                context.IaConocimientos.Add(defaultIaConfig);
                context.SaveChanges();
            }
        }
    }
}


/*---------------------------------------------------------------------------------------

Script for permissions insert data base 

-- ADMIN: todos los usuarios con rol admin
INSERT INTO user_permissions (user_id, permissions, created_at, updated_at)
SELECT
    u.id,
    '["admin.panel","admin.dashboard","admin.users","admin.products.view","admin.products.manage","admin.messages","admin.invoices.view","admin.notifications.create","admin.clients","admin.emails","admin.settings"]'::jsonb,
    now(),
    now()
FROM usuarios u
WHERE lower(u.rol) = 'admin'
ON CONFLICT (user_id) DO UPDATE SET
    permissions = EXCLUDED.permissions,
    updated_at = now();

-- SECRETARIA: todos los usuarios con rol secretaria
INSERT INTO user_permissions (user_id, permissions, created_at, updated_at)
SELECT
    u.id,
    '["secretaria.panel","secretaria.citas.list","secretaria.citas.create","secretaria.citas.newClient","secretaria.calendar","secretaria.invoices.manage","secretaria.messages","secretaria.invoices.view","secretaria.recipes","secretaria.notifications.create","secretaria.clients"]'::jsonb,
    now(),
    now()
FROM usuarios u
WHERE lower(u.rol) = 'secretaria'
ON CONFLICT (user_id) DO UPDATE SET
    permissions = EXCLUDED.permissions,
    updated_at = now();

-- DOCTOR: todos los usuarios con rol doctor
INSERT INTO user_permissions (user_id, permissions, created_at, updated_at)
SELECT
    u.id,
    '["doctor.panel","doctor.citas","doctor.calendar","doctor.records","doctor.recipes","doctor.messages","doctor.invoices.view","doctor.notifications.create"]'::jsonb,
    now(),
    now()
FROM usuarios u
WHERE lower(u.rol) = 'doctor'
ON CONFLICT (user_id) DO UPDATE SET
    permissions = EXCLUDED.permissions,
    updated_at = now();

--------------------------------------------------------------------------------------------*/