using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATM.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public int Money { get; set; }
        public string Operation { get; set; }
        public string Receiver { get; set; }
        public string Sender { get; set; }
        public DateTime Date { get; set; }
    }
}