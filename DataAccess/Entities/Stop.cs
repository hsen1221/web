using System;
// Stop.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Stop
    {
        public Guid StopId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "You must name the line.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 50 characters.")]
        public string Title { get; set; } = string.Empty;

        // foreign key -> Line
        public Guid LineId { get; set; }
        public Line? Line { get; set; }

        // You can add order index if needed to keep stop sequence, e.g. public int Sequence { get; set; }
    }
}
