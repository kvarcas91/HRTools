using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Resignations
{
    public class ResignationEntity
    {
        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Namager { get; set; }
        public string Shift { get; set; }
        public DateTime LastWorkingDay { get; set; }
        public string TTLink { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ReasonForResignation { get; set; }

        public ResignationEntity()
        {

        }

        public ResignationEntity(ResignationEntry entry)
        {

        }
    }
}
