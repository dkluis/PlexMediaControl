using System;
using System.Collections.Generic;

namespace PlexMediaControl.Models.MariaDB
{
    public partial class Nobroadcastdate
    {
        public int Id { get; set; }
        public int TvmShowId { get; set; }
        public int TvmEpisodeId { get; set; }
        public string TvmUrl { get; set; } = null!;
        public string SeasonEpisode { get; set; } = null!;
        public int Season { get; set; }
        public int Episode { get; set; }
        public DateTime? BroadcastDate { get; set; }
        public string PlexStatus { get; set; } = null!;
        public DateTime? PlexDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
