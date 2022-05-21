using Domain.DataManager;
using Domain.Interfaces;
using Domain.Models;
using Domain.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Data
{
    public class RosterDataManager : DataManager
    {
        
        public Task<IList<Roster>> GetWebRosterAsync(IWebStream dataStream, string rosterUrl)
        {

            return Task.Run(() =>
            {
                return dataStream.Get(rosterUrl, RequiredRosterHeaders(), CreateRosterObject);
            });
        }

        public Task WriteRosterAsync(IDataStream dataStream, IEnumerable<Roster> rosterList) => WriteToCsvAsync(dataStream, rosterList);

        public Roster CreateRosterObject(string[] fields, Dictionary<string, int> map)
        {
            
            var employee = new Roster();
            try
            {
                if (map["EmployeeID"] >= 0) employee.EmployeeID = fields[map["EmployeeID"]];
                if (map["UserID"] >= 0) employee.UserID = fields[map["UserID"]];
                if (map["EmployeeName"] >= 0) employee.EmployeeName = fields[map["EmployeeName"]];
                if (map["DepartmentID"] >= 0) employee.DepartmentID = fields[map["DepartmentID"]];

                if (map["EmploymentStartDate"] >= 0)
                {
                    if (fields[map["EmploymentStartDate"]].Contains("UTC"))
                    {
                        DateTime.TryParseExact(fields[map["EmploymentStartDate"]], DataStorage.AppSettings.RosterWebDateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime date);
                        employee.EmploymentStartDate = date;
                    }
                    else
                    {
                         employee.EmploymentStartDate = DateTime.Parse(fields[map["EmploymentStartDate"]]);
                    }
                }

                if (map["EmploymentType"] >= 0) employee.EmploymentType = fields[map["EmploymentType"]];
                if (map["ManagerName"] >= 0) employee.ManagerName = fields[map["ManagerName"]];
                if (map["TempAgencyCode"] >= 0) employee.AgencyCode = fields[map["TempAgencyCode"]];
                if (map["JobTitle"] >= 0) employee.JobTitle = fields[map["JobTitle"]];
                if (map["ManagementAreaID"] >= 0) employee.FCLM = fields[map["ManagementAreaID"]];
                if (map["ShiftPattern"] >= 0) employee.ShiftPattern = fields[map["ShiftPattern"]];
            }
            catch (Exception EX)
            {
                LoggerManager.Log("CreateRosterObject", EX.Message);
            }

            return employee;
        }

        public string[] RequiredRosterHeaders()
        {
            return new string[] { "EmployeeID", "UserID", "EmployeeName", "DepartmentID", "EmploymentStartDate", "EmploymentType", "ManagerName", "TempAgencyCode", "JobTitle", "ManagementAreaID", "ShiftPattern" };
        }
    }
}
