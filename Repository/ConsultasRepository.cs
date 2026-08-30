using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vet_api_Net.Data;
using vet_api_Net.Models;
using vet_api_Net.Interfaze.Repositories;
using DTOs;

namespace vet_api_Net.Repositories;

public class ConsultasRepository : IConsultasRepository
{
    private readonly AppDbContext _context;

    public ConsultasRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cita?> GetCitaByIdAsync(int id) => await _context.Citas.FindAsync(id);
    public async Task<Mascota?> GetMascotaByIdAsync(int id) => await _context.Mascotas.FindAsync(id);
    public async Task<Usuario?> GetDoctorByIdAsync(int id) => await _context.Usuarios.FindAsync(id);
    public async Task<Producto?> GetProductoByIdAsync(int id) => await _context.Productos.FindAsync(id);
    public async Task<MetodoPago?> GetMetodoPagoByIdAsync(int id) => await _context.MetodoPagos.FindAsync(id);
    public async Task<Consulta?> GetConsultaByIdEntityAsync(int id) => await _context.Consultas.FindAsync(id);

    public async Task<Consulta?> GetConsultaWithProductsAsync(int id)
    {
        return await _context.Consultas
            .Include(c => c.ConsultasProductos)
                .ThenInclude(cp => cp.Producto)
            .Include(c => c.PetVaccinations)
                .ThenInclude(pv => pv.Vaccine)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public void AddConsulta(Consulta medicalRecord) => _context.Consultas.Add(medicalRecord);
    public void AddConsultaProducto(ConsultasProducto detail) => _context.ConsultasProductos.Add(detail);
    public void AddFactura(Factura invoice) => _context.Facturas.Add(invoice);
    public void AddDetalleFactura(DetallesFactura invoiceDetail) => _context.DetallesFacturas.Add(invoiceDetail);

    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

    public async Task<ConsultaRequestDTO?> GetConsultaDtoByIdAsync(int id)
    {
        return await _context.Consultas
            .Where(c => c.Id == id)
            .Select(c => new ConsultaRequestDTO
            {
                Id = c.Id,
                CitaId = c.CitaId,
                MascotaId = c.MascotaId,
                DoctorId = c.DoctorId,
                FechaConsulta = c.FechaConsulta,
                PesoActual = c.PesoActual,
                Temperatura = c.Temperatura,
                Sintomas = c.Sintomas,
                Diagnostico = c.Diagnostico,
                Tratamiento = c.Tratamiento,
                Receta = c.Receta,
                Observaciones = c.Observaciones,
                ConsultaPrice = c.ConsultaPrice,
                MascotaNombre = c.Mascota != null ? c.Mascota.Nombre : null,
                ClienteTelefono = (c.Mascota != null && c.Mascota.Cliente != null) ? c.Mascota.Cliente.Telefono : null,
                TelefonoCliente = (c.Mascota != null && c.Mascota.Cliente != null) ? c.Mascota.Cliente.Telefono : null,
                CorreoCliente = (c.Mascota != null && c.Mascota.Cliente != null) ? c.Mascota.Cliente.Email : null,
                Creado = c.Creado,
                Productos = c.ConsultasProductos.Select(cp => new ConsultaProductoDetalleDTO
                {
                    Id = cp.Id,
                    ProductoId = cp.ProductoId,
                    NombreProducto = cp.Producto.Nombre,
                    Cantidad = cp.Cantidad,
                    PrecioUnitario = cp.PrecioUnitario,
                    Dosis = cp.Dosis,
                    ViaAdministracion = cp.ViaAdministracion,
                    Frecuencia = cp.Frecuencia,
                    Duracion = cp.Duracion,
                    Instrucciones = cp.Instrucciones
                }).ToList(),
                Vacunas = c.PetVaccinations.Select(pv => new ConsultaVaccineDetalleDTO
                {
                    Id = pv.Id,
                    VaccineId = pv.VaccineId,
                    NombreVacuna = pv.Vaccine.Name,
                    Dose = pv.Dose,
                    ApplicationDate = pv.ApplicationDate,
                    NextDoseDate = pv.NextDoseDate,
                    PrecioUnitario = pv.Vaccine.Price,
                    DetalleFacturaId = pv.DetalleFacturaId
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ConsultaPdfDTO?> GetConsultaPdfDataAsync(int id)
    {
        return await _context.Consultas
            .Where(c => c.Id == id)
            .Select(c => new ConsultaPdfDTO
            {
                Id = c.Id,
                CitaId = c.CitaId,
                MascotaId = c.MascotaId,
                DoctorId = c.DoctorId,
                FechaConsulta = c.FechaConsulta,
                Sintomas = c.Sintomas,
                Diagnostico = c.Diagnostico,
                Tratamiento = c.Tratamiento,
                Receta = c.Receta,
                Observaciones = c.Observaciones,
                ConsultaPrice = c.ConsultaPrice ?? 0m,
                MascotaNombre = c.Mascota != null ? c.Mascota.Nombre : null,
                ClienteId = c.Mascota != null && c.Mascota.Cliente != null ? c.Mascota.Cliente.Id : (int?)null,
                ClienteNombre = c.Mascota != null && c.Mascota.Cliente != null ? (c.Mascota.Cliente.Nombre + " " + c.Mascota.Cliente.Apellido) : null,
                ClienteTelefono = c.Mascota != null && c.Mascota.Cliente != null ? c.Mascota.Cliente.Telefono : null,
                TelefonoCliente = c.Mascota != null && c.Mascota.Cliente != null ? c.Mascota.Cliente.Telefono : null,
                CorreoCliente = c.Mascota != null && c.Mascota.Cliente != null ? c.Mascota.Cliente.Email : null,
                Productos = c.ConsultasProductos.Select(cp => new ConsultaProductoDetalleDTO
                {
                    Id = cp.Id,
                    ProductoId = cp.ProductoId,
                    NombreProducto = cp.Producto.Nombre,
                    Cantidad = cp.Cantidad,
                    PrecioUnitario = cp.PrecioUnitario,
                    Dosis = cp.Dosis,
                    ViaAdministracion = cp.ViaAdministracion,
                    Frecuencia = cp.Frecuencia,
                    Duracion = cp.Duracion,
                    Instrucciones = cp.Instrucciones
                }).ToList(),
                Vacunas = c.PetVaccinations.Select(pv => new ConsultaVaccineDetalleDTO
                {
                    Id = pv.Id,
                    VaccineId = pv.VaccineId,
                    NombreVacuna = pv.Vaccine.Name,
                    Dose = pv.Dose,
                    ApplicationDate = pv.ApplicationDate,
                    NextDoseDate = pv.NextDoseDate,
                    PrecioUnitario = pv.Vaccine.Price,
                    DetalleFacturaId = pv.DetalleFacturaId
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ConsultaRequestDTO>> GetConsultasAsync(int? doctorId = null, int? secretariaId = null)
    {
        var query = _context.Consultas.AsQueryable();

        if (doctorId.HasValue)
            query = query.Where(c => c.DoctorId == doctorId.Value);

        if (secretariaId.HasValue)
            query = query.Where(c => c.Cita.SecretariaId == secretariaId.Value);

        return await query
            .OrderByDescending(c => c.FechaConsulta)
            .Select(c => new ConsultaRequestDTO
            {
                Id = c.Id,
                CitaId = c.CitaId,
                MascotaId = c.MascotaId,
                DoctorId = c.DoctorId,
                FechaConsulta = c.FechaConsulta,
                PesoActual = c.PesoActual,
                Temperatura = c.Temperatura,
                Sintomas = c.Sintomas,
                Diagnostico = c.Diagnostico,
                Tratamiento = c.Tratamiento,
                Receta = c.Receta,
                Observaciones = c.Observaciones,
                ConsultaPrice = c.ConsultaPrice,
                MascotaNombre = c.Mascota != null ? c.Mascota.Nombre : null,
                ClienteTelefono = (c.Mascota != null && c.Mascota.Cliente != null) ? c.Mascota.Cliente.Telefono : null,
                TelefonoCliente = (c.Mascota != null && c.Mascota.Cliente != null) ? c.Mascota.Cliente.Telefono : null,
                CorreoCliente = (c.Mascota != null && c.Mascota.Cliente != null) ? c.Mascota.Cliente.Email : null,
                Creado = c.Creado,
                Productos = c.ConsultasProductos.Select(cp => new ConsultaProductoDetalleDTO
                {
                    Id = cp.Id,
                    ProductoId = cp.ProductoId,
                    NombreProducto = cp.Producto.Nombre,
                    Cantidad = cp.Cantidad,
                    PrecioUnitario = cp.PrecioUnitario,
                    Dosis = cp.Dosis,
                    ViaAdministracion = cp.ViaAdministracion,
                    Frecuencia = cp.Frecuencia,
                    Duracion = cp.Duracion,
                    Instrucciones = cp.Instrucciones
                }).ToList(),
                Vacunas = c.PetVaccinations.Select(pv => new ConsultaVaccineDetalleDTO
                {
                    Id = pv.Id,
                    VaccineId = pv.VaccineId,
                    NombreVacuna = pv.Vaccine.Name,
                    Dose = pv.Dose,
                    ApplicationDate = pv.ApplicationDate,
                    NextDoseDate = pv.NextDoseDate,
                    PrecioUnitario = pv.Vaccine.Price,
                    DetalleFacturaId = pv.DetalleFacturaId
                }).ToList()
            })
            .ToListAsync();
    }
}