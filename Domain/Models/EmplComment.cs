using Domain.Types;
using System;

namespace Domain.Models
{
    public struct EmplComment
    {
        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string CommentOrigin { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }

        public EmplComment Create(string content, string emplId, TimelineOrigin origin) => Create(content, emplId, origin, Environment.UserName, DateTime.Now);

        public EmplComment Create(string content, string emplId, TimelineOrigin origin, string createdBy, DateTime createdAt)
        {
            ID = Guid.NewGuid().ToString();
            CommentOrigin = origin.ToString();
            EmployeeID = emplId;
            Content = content;
            CreatedAt = createdAt;
            CreatedBy = createdBy;

            return this;
        }
    }
}
