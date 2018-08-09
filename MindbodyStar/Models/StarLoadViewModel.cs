using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MindbodyStar.Models
{
    public class StarLoadViewModel
    {
        // Password = "Siteowner", Username = "apitest1234"
        public string Username { get; set; }
        public string Password { get; set; }
        public int StudioID { get; set; }
    }
}