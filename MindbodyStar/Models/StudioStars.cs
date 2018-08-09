using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MindbodyStar.Models
{
    public class StudioStars
    {
        public string StudioID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double getTotalDays()
        {
            return EndDate.Subtract(StartDate).TotalDays + 1;
        }
        public List<ClientVisits> ClientList { get; set; }
    }
}