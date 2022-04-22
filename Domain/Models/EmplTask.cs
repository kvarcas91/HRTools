using Domain.Extensions;
using Domain.Interfaces;
using Domain.Storage;
using System;

namespace Domain.Models
{
    public class EmplTask : IQueryable
    {
        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string TaskOrigin { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime TaskDueDate { get; set; }
        public string CompletedBy { get; set; }
        public DateTime CompletedAt { get; set; }

        public string GetHeader() =>
            "(id, employeeID, taskOrigin, createdBy, createdAt, content, isCompleted, taskDueDate, completedBy, completedAt)";

        public string GetValues()
        {
            return $@"('{ID}', '{EmployeeID}', '{TaskOrigin}', '{CreatedBy}', {CreatedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)}, '{Content.DbSanityCheck()}', '{Convert.ToInt16(IsCompleted)}',
                    {TaskDueDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)}, '{CompletedBy}', {CompletedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)})";
        }
    }
}
