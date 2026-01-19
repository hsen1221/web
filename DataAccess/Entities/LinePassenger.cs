using System;
// LinePassenger.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity; // Add this namespace
namespace DataAccess.Entities
{
    public class LinePassenger
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string FullName { get; set; } = string.Empty; // <--- NEW FIELD

        // which Line this passenger is registered for
        [Required(ErrorMessage = "Please select a bus line.")]
        public Guid LineId { get; set; }
        
        public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Link to the User Account
        public string AppUserId { get; set; } // Foreign Key to AspNetUsers table
        public AppUser? AppUser { get; set; } // Navigation property
        public Line? Line { get; set; }


        // add passenger info here or reference to a Person/Passenger table (e.g. public Guid PassengerId { get; set; })
    }
}
