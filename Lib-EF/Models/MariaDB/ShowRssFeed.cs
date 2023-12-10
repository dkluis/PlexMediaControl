using System;
using System.Collections.Generic;

namespace PlexMediaControl.Models.MariaDB;

public partial class ShowRssFeed
{
    public int Id { get; set; }

    public string ShowName { get; set; } = null!;

    public bool? Processed { get; set; }

    public string Url { get; set; } = null!;

    public DateTime UpdateDate { get; set; }
}
