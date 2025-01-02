using System;
using System.Collections.Generic;

namespace RestaurantManagement.Models​;

public partial class OdItem
{
    public int OdItemId { get; set; }

    public int OdId { get; set; }

    public int MenuId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Menu Menu { get; set; } = null!;

    public virtual Od Od { get; set; } = null!;
}
