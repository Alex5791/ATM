using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ATM.Models
{
    public class UserContext : DbContext
    {
        public UserContext() : base("name=UserContext") { }
        public DbSet<User> Users { get; set; }
    }
}