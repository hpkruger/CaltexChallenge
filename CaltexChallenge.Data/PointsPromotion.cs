using System;
using System.Collections.Generic;
using System.Text;

namespace CaltexChallenge.Data
{
    public class PointsPromotion
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Category { get; set; }
        public int Points { get; set; }
    }
}
