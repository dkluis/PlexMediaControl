using System;
using System.Collections.Generic;

namespace PlexMediaControl.Models.MariaDB;

public partial class Followed
{
    public int Id { get; set; }

    public int TvmShowId { get; set; }

    public DateTime UpdateDate { get; set; }
}
