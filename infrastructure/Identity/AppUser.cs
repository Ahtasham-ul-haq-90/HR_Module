using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace infrastructure.Identity
{
    public class AppUser 
        : IdentityUser<Guid>
    {
        public string FullName { get; set; }
    }
}
