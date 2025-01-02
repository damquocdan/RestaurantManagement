using System;
using System.Collections.Generic;

namespace RestaurantManagement.Models​;

public partial class OrdersC
{
    public int OrderCid { get; set; }

    public string NameC { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
