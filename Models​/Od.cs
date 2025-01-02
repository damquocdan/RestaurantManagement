using System;
using System.Collections.Generic;

namespace RestaurantManagement.Models​;

public partial class Od
{
    public int OdId { get; set; }

    public string Name { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<OdItem> OdItems { get; set; } = new List<OdItem>();
}
