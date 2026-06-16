using System;
using System.Collections.Generic;
using System.Linq;
using vet_api_Net.Models;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Infrastructure.Configuration;


/*
Seed con datos DUMMY minimos para pruebas de API.
NO USAR EN PRODUCCION
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

        public static void Initialize(AppDbContext context, ApiSettingsOptions apiSettings, SeedDataOptions seedDataOptions)
        {
            //Reseteo de tablas
            context.Database.ExecuteSqlRaw(@"
                SET FOREIGN_KEY_CHECKS = 0;
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
                TRUNCATE TABLE usuarios;
                TRUNCATE TABLE categorias_productos;
                TRUNCATE TABLE metodos_pago;
                TRUNCATE TABLE MoneyTypes;
                TRUNCATE TABLE FacturaConfigs;
                TRUNCATE TABLE ReporConfigs;
                SET FOREIGN_KEY_CHECKS = 1;
            ");

            if (!context.Usuarios.Any() && seedDataOptions.DummyData != null)
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
                        Activo = true,
                        Creado = DateTime.Now,
                        Actualizado = DateTime.Now
                    });
                }
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
                    context.MetodoPagos.Add(new MetodoPago { 
                        Nombre = m, 
                        Creado = DateTime.Now, 
                        Actualizado = DateTime.Now 
                    });
                }
            }
            context.SaveChanges();
            var metodoPago = context.MetodoPagos.FirstOrDefault();

            if (!context.MoneyTypes.Any())
            {
                context.MoneyTypes.Add(new MoneyType
                {
                    MoneyName = apiSettings.USD ?? "USD",
                    BcvDollar = 36.50m,
                    DollarPersistence = "36.50",
                    Fecha = DateTime.Now
                });
                context.SaveChanges();
            }

            if (!context.FacturaConfigs.Any())
            {
                context.FacturaConfigs.Add(new FacturaConfig {
                    Days = 1,
                    IsEnabled = true,
                    GenerateEnabled = true,
                    LastUpdated = DateTime.Now
                });
                context.SaveChanges();
            }

            if (!context.ReporConfigs.Any())
            {
                context.ReporConfigs.Add(new ReporConfig
                {
                    Days = 30,
                    IsEnabled = true,
                    GenerateEnabled = true,
                    LastUpdated = DateTime.Now
                });
                context.SaveChanges();
            }

            var clientes = new List<Cliente>
            {
                new Cliente { 
                    Nombre = "Carlos", 
                    Apellido = "Lopez", 
                    Identificacion = "V-12345678", 
                    Email = "carlos@gmail.com", 
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
                    Email = "ana@gmail.com", 
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
                    Email = "luis@gmail.com", 
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
                    Email = "jorge.diaz@hotmail.com", 
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
                    Email = "sofia.g@yahoo.com", 
                    Telefono = "04241112233", 
                    Direccion = "La Castellana", 
                    Nota = "Familiar de Carlos", 
                    Creado = DateTime.Now, 
                    Actualizado = DateTime.Now 
                }
            };
            context.Clientes.AddRange(clientes);
            context.SaveChanges();

            var mascotas = new List<Mascota>
            {
                new Mascota {
                    ClienteId = clientes[0].Id, 
                    Nombre = "Firulais", 
                    Especie = "perro", 
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
                    Especie = "gato", 
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
                    Especie = "perro", 
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
                    Especie = "gato", 
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
                    Especie = "perro", 
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
                    Especie = "gato", 
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
                    Especie = "perro",
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
                    Nombre = "Accesorios", Descripcion = "Collares, juguetes, etc.", Activo = true, Creado = DateTime.Now, Actualizado = DateTime.Now
                },
                new CategoriasProducto {
                    Nombre = "Higiene", Descripcion = "Shampoos y limpieza", Activo = true, Creado = DateTime.Now, Actualizado = DateTime.Now
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
            var categoria = context.CategoriasProductos.FirstOrDefault();

            var productos = new List<Producto>
            {
                new Producto {
                    Codigo = "P-001",
                    Nombre = "Vacuna Antirrábica",
                    CategoriaId = categoria.Id!,
                    Tipo = "medicamento",
                    Precio = 10.00m,
                    PrecioVenta = 15.00m,
                    Stock = 50,
                    StockMinimo = 10,
                    UnidadMedida = "dosis",
                    RequiereReceta = false,
                    Descripcion = "Vacuna anual contra la rabia",
                    Proveedor = "Pfizer",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new Producto {
                    Codigo = "P-002",
                    Nombre = "Desparasitante",
                    CategoriaId = categoria.Id,
                    Tipo = "medicamento",
                    Precio = 2.50m,
                    PrecioVenta = 5.00m,
                    Stock = 100,
                    StockMinimo = 20,
                    UnidadMedida = "pastilla",
                    RequiereReceta = false,
                    Descripcion = "Desparasitante interno de amplio espectro",
                    Proveedor = "Bayer",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                },
                new Producto {
                    Codigo = "P-003",
                    Nombre = "Antibiótico",
                    CategoriaId = categoria.Id,
                    Tipo = "medicamento",
                    Precio = 8.00m,
                    PrecioVenta = 12.00m,
                    Stock = 30,
                    StockMinimo = 5,
                    UnidadMedida = "frasco",
                    RequiereReceta = true,
                    Descripcion = "Amoxicilina para infecciones bacterianas",
                    Proveedor = "Genfarm",
                    Creado = DateTime.Now,
                    Actualizado = DateTime.Now
                }
            };
            context.Productos.AddRange(productos);
            context.SaveChanges();

            var citaDate1 = DateTime.Now.AddHours(5);
            var citaDate2 = DateTime.Now.AddHours(6);

            var citas = new List<Cita>
            {
                new Cita {
                    DoctorId = doctor.Id,
                    MascotaId = mascotas[0].Id,
                    SecretariaId = secretaria.Id,
                    FechaCita = citaDate1.Date,
                    HoraCita = TimeOnly.FromDateTime(citaDate1),
                    Estado = "completada",
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
                    Estado = "programada",
                    TipoCita = "vacunacion",
                    Motivo = "Vacuna anual",
                    MetodoPagoId = metodoPago.Id,
                    Notas = "Traer carnet de vacunación"
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



        }
    }
}
