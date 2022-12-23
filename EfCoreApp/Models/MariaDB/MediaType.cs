﻿using System;
using System.Collections.Generic;

namespace EfCoreApp.Models.MariaDB
{
    public partial class MediaType
    {
        public MediaType()
        {
            Shows = new HashSet<Show>();
        }

        public int Id { get; set; }
        public string MediaType1 { get; set; } = null!;
        public string PlexLocation { get; set; } = null!;
        public string? AutoDelete { get; set; }

        public virtual ICollection<Show> Shows { get; set; }
    }
}
