using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public struct EmplComment
    {
        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string CommentID { get; set; }
        public string MyProperty { get; set; }
    }
}
