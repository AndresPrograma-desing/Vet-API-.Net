using System;
using System.ComponentModel.DataAnnotations;

namespace vet_api_Net.Validation;

// "Ambos" es una especie válida para vacunas (aplica a perro y gato), pero una mascota siempre
// es de una especie concreta. Se valida sin distinguir mayúsculas ni espacios sobrantes.
public class NoAmbosEspecieAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string especie) return true;
        return !string.Equals(especie.Trim(), "Ambos", StringComparison.OrdinalIgnoreCase);
    }
}
