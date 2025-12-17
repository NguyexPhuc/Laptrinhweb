using System;
using System.Collections.Generic;

namespace Web_BanHang.Models;

public partial class News
{
    public int NewsId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int? AuthorId { get; set; }

    public DateTime? PublishedAt { get; set; }

    public virtual User? Author { get; set; }
}
