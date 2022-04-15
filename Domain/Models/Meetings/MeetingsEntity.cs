﻿using Domain.Extensions;
using Domain.Factory;
using Domain.Interfaces;
using Domain.Storage;
using Domain.Types;
using System;

namespace Domain.Models.Meetings
{
    public class MeetingsEntity : IEmployee, IDataImportObject
    {
        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string EmployeeName { get; set; }
        public string ShiftPattern { get; set; }
        public string ManagerName { get; set; }
        public MeetingType MeetingType { get; set; }
        public DateTime FirstMeetingDate { get; set; }
        public string FirstMeetingOutcome { get; set; }
        public DateTime SecondMeetingDate { get; set; }
        public string SecondMeetingOutcome { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string MeetingStatus { get; set; }
        public bool IsERCaseStatusOpen { get; set; }
        public bool Paperless { get; set; }

        public MeetingsEntity()
        {
            IsERCaseStatusOpen = false;
        }

        public object ReadFromCSV(string[] fields, DataMap dataMap)
        {
            ID = dataMap.GetStrValue(nameof(ID), fields);
            if (string.IsNullOrEmpty(ID)) ID = Guid.NewGuid().ToString();
           
            EmployeeID = dataMap.GetStrValue(nameof(EmployeeID), fields);
            UserID = dataMap.GetStrValue(nameof(UserID), fields);
            EmployeeName = dataMap.GetStrValue(nameof(EmployeeName), fields);
            ShiftPattern = dataMap.GetStrValue(nameof(ShiftPattern), fields);
            ManagerName = dataMap.GetStrValue(nameof(ManagerName), fields);
            Enum.TryParse(dataMap.GetStrValue(nameof(MeetingType), fields), out MeetingType mType);
            MeetingType = mType;

            FirstMeetingDate = dataMap.GetDateValue(nameof(FirstMeetingDate), fields);
            FirstMeetingOutcome = dataMap.GetStrValue(nameof(FirstMeetingOutcome), fields);
            SecondMeetingDate = dataMap.GetDateValue(nameof(SecondMeetingDate), fields);
            SecondMeetingOutcome = dataMap.GetStrValue(nameof(SecondMeetingOutcome), fields);
            MeetingStatus = dataMap.GetStrValue(nameof(MeetingStatus), fields);
            CreatedAt = dataMap.GetDateValue(nameof(CreatedAt), fields);
            CreatedBy = dataMap.GetStrValue(nameof(CreatedBy), fields);
            UpdatedAt = dataMap.GetDateValue(nameof(UpdatedAt), fields);
            UpdatedBy = dataMap.GetStrValue(nameof(UpdatedBy), fields);
            IsERCaseStatusOpen = dataMap.GetBoolValue(nameof(IsERCaseStatusOpen), fields);
            Paperless = dataMap.GetBoolValue(nameof(Paperless), fields);

            return this;
        }

        public string GetHeader() => 
            "(id,employeeID,userID,employeeName,shiftPattern,managerName,meetingType,firstMeetingDate,firstMeetingOutcome,secondMeetingDate,secondMeetingOutcome,meetingStatus,createdAt,createdBy,updatedAt,updatedBy,isERCaseStatusOpen,paperless)";

        public string GetValues() =>
            $@"('{ID}','{EmployeeID}','{UserID}','{EmployeeName.DbSanityCheck()}','{ShiftPattern}','{ManagerName.DbSanityCheck()}','{(int)MeetingType}',{FirstMeetingDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},'{FirstMeetingOutcome}',
                {SecondMeetingDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},'{SecondMeetingOutcome}','{MeetingStatus}',{CreatedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)},'{CreatedBy}',
                {UpdatedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)},'{UpdatedBy}','{Convert.ToUInt16(IsERCaseStatusOpen)}','{Convert.ToUInt16(Paperless)}')";
    }
}
