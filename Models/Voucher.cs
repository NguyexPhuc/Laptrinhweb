using System;
using System.Collections.Generic;

namespace Web_BanHang.Models;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string Code { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public string? DiscountType { get; set; }

    public decimal? MinOrderValue { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? UsageLimit { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
