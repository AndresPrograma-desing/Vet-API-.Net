namespace vet_api_Net.HttpServices;

public interface IBcvScraper
{
    /// <summary>
    /// Obtiene el precio actual del dólar desde la web del BCV.
    /// </summary>
    /// <returns>El valor decimal del dólar o 0 si ocurre un error.</returns>
    Task<decimal?> ObtenerPrecioBcvAsync();
}