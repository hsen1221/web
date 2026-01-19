using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;    
// Line.cs

namespace DataAccess.Entities
{
    public class Line
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "You must name the line.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 50 characters.")]
        public string Title { get; set; } = string.Empty;

        // references to people (guids) — adapt types if you have User entities
        public Guid? DriverId { get; set; }
   
        // One-to-one: navigation to Bus
        public Bus? Bus { get; set; }

        // One-to-many: stops and passengers
        public virtual ICollection<Stop> Stops { get; set; } = new List<Stop>();
        public virtual ICollection<LinePassenger> LinePassengers { get; set; } = new List<LinePassenger>();
    }
}
