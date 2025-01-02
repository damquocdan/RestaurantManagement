using System;
using System.Collections.Generic;

namespace RestaurantManagement.Models​;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderCid { get; set; }

    public int MenuId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Menu Menu { get; set; } = null!;

    public virtual OrdersC OrderC { get; set; } = null!;
}
