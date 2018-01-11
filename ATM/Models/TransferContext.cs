using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ATM.Models
{
    public class TransferContext : DbContext
    {
        public TransferContext() : base("name=UserContext") { }
        public DbSet<Transfer> Transfers { get; set; }
    }
}