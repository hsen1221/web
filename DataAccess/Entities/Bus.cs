using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

// Bus.cs

namespace DataAccess.Entities
{
    public class Bus
    {
        public Guid BusId { get; set; } = Guid.NewGuid();

        [Range(10, 100, ErrorMessage = "Capacity must be between 10 and 100.")] // <--- The Logic
        public int Capacity { get; set; }

        // Foreign key for one-to-one with Line
        public Guid? LineId { get; set; }

        // Navigation
        public Line? Line { get; set; }
        
    }
}
