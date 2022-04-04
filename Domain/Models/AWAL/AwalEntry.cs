using System;

namespace Domain.Models.AWAL
{
    public class AwalEntry
    {
        public DateTime FirstNCNSDate { get; set; }
        public DateTime Awal1SentDate { get; set; }
        public DateTime Awal2SentDate { get; set; }

        public bool CanAdd()
        {
            return !FirstNCNSDate.Equals(DateTime.MinValue) && FirstNCNSDate < DateTime.Now;
        }
    }
}
