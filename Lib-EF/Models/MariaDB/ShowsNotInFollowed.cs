using System;
using System.Collections.Generic;

namespace PlexMediaControl.Models.MariaDB
{
    public partial class Showsnotinfollowed
    {
        public int Id { get; set; }
        public int TvmShowId { get; set; }
        public string TvmStatus { get; set; } = null!;
        public string? TvmUrl { get; set; }
        public string ShowName { get; set; } = null!;
        public string ShowStatus { get; set; } = null!;
        public DateTime PremiereDate { get; set; }
        public string Finder { get; set; } = null!;
        public string? MediaType { get; set; }
        public string CleanedShowName { get; set; } = null!;
        public string AltShowname { get; set; } = null!;
        public DateTime UpdateDate { get; set; }
    }
}
