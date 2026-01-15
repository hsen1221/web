using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class AppUser : IdentityUser
    {
        // We can add custom fields here later if we want, like:
        public string FullName { get; set; } = "";
    }
}