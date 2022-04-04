using CsvHelper;
using CsvHelper.Configuration;
using Domain.Converters;
using Domain.Interfaces;
using Domain.Models;
using Domain.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Data
{
    public class RosterDataManager
    {
        public Task<IList<Roster>> GetRosterAsync(IDataStream dataStream)
        {
            return Task.Run(() =>
            {
                return dataStream.Get(RequiredRosterHeaders(), CreateRosterObject);
            });
        }

        public Task<IList<Roster>> GetWebRosterAsync(IWebStream dataStream, string rosterUrl)
        {
            string site = DataStorage.AppSettings.SiteID;

            string siteUrl = rosterUrl.Replace("{site}", site);

            return Task.Run(() =>
            {
                return dataStream.Get<Roster>(siteUrl, RegisterRosterMap);
            });
        }

        public Task WriteRosterAsync(IDataStream dataStream, IEnumerable<Roster> rosterList)
        {
            return Task.Run(() =>
            {
                dataStream.Write(rosterList);
            });
        }

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
                    try
                    {
                        employee.EmploymentStartDate = DateTime.Parse(fields[map["EmploymentStartDate"]]);
                    }
                    catch
                    {
                        DateTime.TryParseExact(fields[map["EmploymentStartDate"]], DataStorage.AppSettings.RosterWebDateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime date);
                        employee.EmploymentStartDate = date;
                        throw;
                    }
                }

                if (map["EmploymentType"] >= 0) employee.EmploymentType = fields[map["EmploymentType"]];
                if (map["ManagerName"] >= 0) employee.ManagerName = fields[map["ManagerName"]];
                if (map["TempAgencyCode"] >= 0) employee.AgencyCode = fields[map["TempAgencyCode"]];
                if (map["JobTitle"] >= 0) employee.JobTitle = fields[map["JobTitle"]];
                if (map["ManagementAreaID"] >= 0) employee.FCLM = fields[map["ManagementAreaID"]];
                if (map["ShiftPattern"] >= 0) employee.ShiftPattern = fields[map["ShiftPattern"]];
            }
            catch
            {
                return null;
            }

            return employee;
        }

        public string[] RequiredRosterHeaders()
        {
            return new string[] { "EmployeeID", "UserID", "EmployeeName", "DepartmentID", "EmploymentStartDate", "EmploymentType", "ManagerName", "TempAgencyCode", "JobTitle", "ManagementAreaID", "ShiftPattern" };
        }

        private void RegisterRosterMap(CsvReader csv) => csv.Context.RegisterClassMap<RosterMap>();

        public sealed class RosterMap : ClassMap<Roster>
        {
            public RosterMap()
            {
                Map(m => m.EmployeeID).Name("Employee ID");
                Map(m => m.UserID).Name("User ID");
                Map(m => m.EmployeeName).Name("Employee Name");
                Map(m => m.DepartmentID).Name("Department ID");
                Map(m => m.EmploymentStartDate).Name("Employment Start Date").TypeConverter<DateConverter<Roster>>();
                Map(m => m.EmploymentType).Name("Employment Type");
                Map(m => m.ManagerName).Name("Manager Name");
                Map(m => m.AgencyCode).Name("Temp Agency Code");
                Map(m => m.JobTitle).Name("Job Title");
                Map(m => m.FCLM).Name("Management Area ID");
                Map(m => m.ShiftPattern).Name("Shift Pattern");
            }
        }
    }
}
