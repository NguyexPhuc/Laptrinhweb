using System;
using System.Collections.Generic;

namespace Web_BanHang.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public int? UserId { get; set; }

    public string? RecipientName { get; set; }

    public string? Phone { get; set; }

    public string AddressLine { get; set; } = null!;

    public string? City { get; set; }

    public bool? IsDefault { get; set; }

    public virtual User? User { get; set; }
}
