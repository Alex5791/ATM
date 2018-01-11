using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATM.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Telephone { get; set; }
        public string Cardnumber { get; set; }
        public int Balance { get; set; }
        public string Role { get; set; }
    }
}