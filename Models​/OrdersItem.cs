using System;
using System.Collections.Generic;

namespace RestaurantManagement.Models​;

public partial class OrdersItem
{
    public int OrdersItemId { get; set; }

    public int MenuId { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual MenuItem Item { get; set; } = null!;

    public virtual Menu Menu { get; set; } = null!;
}
