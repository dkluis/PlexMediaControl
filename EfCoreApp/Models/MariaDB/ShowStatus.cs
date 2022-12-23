using System;
using System.Collections.Generic;

namespace EfCoreApp.Models.MariaDB
{
    public partial class ShowStatus
    {
        public ShowStatus()
        {
            Shows = new HashSet<Show>();
        }

        public string ShowStatus1 { get; set; } = null!;

        public virtual ICollection<Show> Shows { get; set; }
    }
}
