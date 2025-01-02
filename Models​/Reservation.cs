using System;
using System.Collections.Generic;

namespace RestaurantManagement.Models​;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int CustomerId { get; set; }

    public int TableId { get; set; }

    public DateTime ReservationDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;
}
