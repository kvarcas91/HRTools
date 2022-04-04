using Domain.Models.DataSnips;
using Domain.Storage;
using Domain.Types;
using System;
using System.Collections.Generic;

namespace Domain.Models.AWAL
{
    public class AwalEntity
    {
        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentID { get; set; }
        public DateTime EmploymentStartDate { get; set; }
        public string EmploymentType { get; set; }
        public string ManagerName { get; set; }
        public string ShiftPattern { get; set; }
        public AwalStatus AwalStatus { get; set; }
        public DateTime FirstNCNSDate { get; set; }
        public DateTime Awal1SentDate { get; set; }
        public DateTime Awal2SentDate { get; set; }
        public DateTime DisciplinaryDate { get; set; }
        public string Outcome { get; set; }
        public DateTime UKPendingEndDate { get; set; }
        public string CloseBridge { get; set; }
        public string BridgeCreatedBy { get; set; }
        public DateTime BridgeCreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

        public AwalEntity() { }

        public AwalEntity(Roster empl)
        {
            ID = Guid.NewGuid().ToString();
            EmployeeID = empl.EmployeeID;
            UserID = empl.UserID;
            EmployeeName = empl.EmployeeName;
            DepartmentID = empl.DepartmentID;
            EmploymentStartDate = empl.EmploymentStartDate;
            EmploymentType = empl.EmploymentType;
            ManagerName = empl.ManagerName;
            ShiftPattern = empl.ShiftPattern;
            AwalStatus = AwalStatus.Active;
        }

        public AwalEntity Add(AwalEntry entry)
        {
            FirstNCNSDate = entry.FirstNCNSDate;
            Awal1SentDate = entry.Awal1SentDate;
            Awal2SentDate = entry.Awal2SentDate;
            return this;
        }

        public IEnumerable<EmployeeSummary> GetSummary()
        {
            List<EmployeeSummary> list = new List<EmployeeSummary>();

            if (!FirstNCNSDate.Equals(DateTime.MinValue))
            {
                list.Add(new EmployeeSummary
                {
                    Date = CreatedAt,
                    Event = $"AWAL process initiated. First NCNS date - {FirstNCNSDate.ToString(DataStorage.ShortPreviewDateFormat)}"
                });
            }

            if (!Awal1SentDate.Equals(DateTime.MinValue))
            {
                list.Add(new EmployeeSummary
                {
                    Date = Awal1SentDate,
                    Event = "AWAL 1 has been sent"
                });
            }
            if (!Awal2SentDate.Equals(DateTime.MinValue))
            {
                list.Add(new EmployeeSummary
                {
                    Date = Awal2SentDate,
                    Event = "AWAL 2 has been sent"
                });
            }

            if ((Awal1SentDate.Equals(DateTime.MinValue) || Awal2SentDate.Equals(DateTime.MinValue)) && !AwalStatus.Equals(AwalStatus.Active))
            {
                var closedDate = BridgeCreatedAt.Equals(DateTime.MinValue) ? UpdatedAt : BridgeCreatedAt;
                var closedBy = string.IsNullOrEmpty(BridgeCreatedBy) ? "" : $" by {BridgeCreatedBy}";
                var bridgeMessage = string.IsNullOrEmpty(CloseBridge) ? "" : $". Reason: '{CloseBridge}'";
                list.Add(new EmployeeSummary
                {
                    Date = closedDate,
                    Event = $"AWAL has been closed{closedBy}{bridgeMessage}"
                });
            }

            if (!DisciplinaryDate.Equals(DateTime.MinValue))
            {

                //string ukPending = UKPendingEndDate.Equals(DateTime.MinValue) ? "" : $". UK Pending code applied until {UKPendingEndDate.ToString(DataStorage.ShortPreviewDateFormat)}";
                //string closureBridge = string.IsNullOrEmpty(CloseBridge) && !Outcome.Equals(DisciplinaryOutcome.NFA) ? "" : $"; Closure bridge - '{CloseBridge}'";
                //string outcome = Outcome.Equals(DisciplinaryOutcome.NoOutcome) ? " - no outcome yet" : $". Outcome - {Outcome}";
                //list.Add(new EmployeeSummary
                //{
                //    Date = DisciplinaryDate,
                //    Event = $"Disciplinary meeting has been scheduled at {DisciplinaryDate.ToString("HH:mm")}{ukPending}{outcome}{closureBridge}"
                //});
            }


            return list;
        }

    }
}
