using System;

namespace Domain.Models.Sanctions
{
    public struct SanctionPreview
    {
        public string HealthSanction { get; set; }
        public DateTime HealthExpireDate { get; set; }
        public string DisciplinarySanction { get; set; }
        public DateTime DisciplinaryExpireDate { get; set; }
    }
}
