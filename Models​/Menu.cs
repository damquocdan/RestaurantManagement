using System;
using System.Collections.Generic;

namespace RestaurantManagement.Models​;

public partial class Menu
{
    public int MenuId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<OrdersItem> OrdersItems { get; set; } = new List<OrdersItem>();
}
