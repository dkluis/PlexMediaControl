using System;
using System.Collections.Generic;

namespace PlexMediaControl.Models.MariaDB
{
    public partial class Movie
    {
        public string Name { get; set; } = null!;
        public string? CleanedName { get; set; }
        public string? AltName { get; set; }
        public string? SeriesName { get; set; }
        public int MovieNumber { get; set; }
        public string? FinderDate { get; set; }
        public string MediaType { get; set; } = null!;
        public bool Acquired { get; set; }

        public virtual MediaType MediaTypeNavigation { get; set; } = null!;
    }
}
