using System;
using System.Collections.Generic;

namespace RestaurantManagement.Models​;

public partial class Table
{
    public int TableId { get; set; }

    public string TableNumber { get; set; } = null!;

    public int SeatingCapacity { get; set; }

    public bool? IsAvailable { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
