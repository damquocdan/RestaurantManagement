using System;
using System.Collections.Generic;

namespace RestaurantManagement.Models​;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual MenuItem Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
