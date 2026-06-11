using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace vet_api_Net.Models;

public partial class MoneyType
{
    public int Id { get; set; }
    public string MoneyName { get; set; } = null!;
    public decimal BcvDollar { get; set; }
    public string DollarPersistence { get; set; } = null!;
    public DateTime Fecha { get; set; }

}