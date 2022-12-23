﻿using System;
using System.Collections.Generic;

namespace EfCoreApp.Models.MariaDB
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
        public DateOnly? BroadcastDate { get; set; }
        public string PlexStatus { get; set; } = null!;
        public DateOnly? PlexDate { get; set; }
        public DateOnly UpdateDate { get; set; }
    }
}
