using System;
using System.Collections.Generic;

namespace PlexMediaControl.Models.MariaDB
{
    public partial class PlexStatus
    {
        public PlexStatus()
        {
            Episodes = new HashSet<Episode>();
        }

        public string PlexStatus1 { get; set; } = null!;

        public virtual ICollection<Episode> Episodes { get; set; }
    }
}
