using System;
using System.Collections.Generic;

namespace Web_BanHang.Models;

public partial class ProductVariant
{
    public int VariantId { get; set; }

    public int? ProductId { get; set; }

    public string SkuCode { get; set; } = null!;

    public string? Color { get; set; }

    public string? Size { get; set; }

    public int? StockQuantity { get; set; }

    public decimal? PriceAdjustment { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Product? Product { get; set; }
}
