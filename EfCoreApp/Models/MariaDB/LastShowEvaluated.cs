using System;
using System.Collections.Generic;

namespace EfCoreApp.Models.MariaDB
{
    public partial class LastShowEvaluated
    {
        public int Id { get; set; }
        public int ShowId { get; set; }
    }
}
