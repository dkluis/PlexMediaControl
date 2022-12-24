using System;
using System.Collections.Generic;

namespace PlexMediaControl.Models.MariaDB
{
    public partial class Notinfollowed
    {
        public int ShowsTvmShowId { get; set; }
        public string ShowName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? Url { get; set; }
        public int? FollowedTvmShowId { get; set; }
    }
}
