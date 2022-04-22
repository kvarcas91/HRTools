using Domain.DataManager;
using Domain.Factory;
using Domain.Models;
using Domain.Models.Sanctions;
using Domain.Storage;
using Domain.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public sealed class SanctionsRepository : BaseRepository
    {
        public bool CanModify (SanctionEntity sanction)
        {
            string query = string.Empty;
            DateTime sanctDate;
            if (sanction.Overriden)
            {
                sanctDate = sanction.OverridenAt;
                query = $"SELECT overridenAt FROM sanctions WHERE id = '{sanction.ID}'";
            }
            else
            {
                sanctDate = sanction.CreatedAt;
                query = $"SELECT createdAt FROM sanctions WHERE id = '{sanction.ID}'";
            }

            var lastEditDate = GetScalar<DateTime>(query);

            return lastEditDate.ToString().Equals(sanctDate.ToString());
        }

        public Task<Response> InsertAllAsync(IList<IDataImportObject> sanctionList) => base.InsertAllAsync(sanctionList, "sanctions");

        public Task<SanctionEntity> OverrideSanctionAsync(SanctionEntity sanction)
        {
            var sanct = sanction;
            return Task.Run(() =>
            {
                if (!CanModify(sanction)) return sanct;

                var overrideDate = DateTime.Now;

                var timelineEntry = new Timeline().Create(sanct.EmployeeID, TimelineOrigin.Sanctions);
                timelineEntry.EventMessage = $"{sanct.Sanction} has been overriden by {Environment.UserName}";
                var timelineQuery = $"INSERT INTO timeline {timelineEntry.GetHeader()} VALUES {timelineEntry.GetValues()};";

                string query = $@"UPDATE sanctions SET sanctionEndDate = '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}', overriden = '{1}', overridenBy = '{Environment.UserName}', 
                                overridenAt = '{overrideDate.ToString(DataStorage.LongDBDateFormat)}' WHERE id ='{sanction.ID}'; {timelineQuery}";
                var response = Execute(query);

                if (response.Success)
                {
                    sanct.OverridenAt = overrideDate;
                    sanct.SanctionEndDate = DateTime.Now;
                    sanct.Overriden = true;
                    sanct.OverridenBy = Environment.UserName;
                }
                return sanct;
            });
            
        }

        public Task<SanctionEntity> ReissueSanctionAsync(SanctionEntity sanction)
        {
            var sanct = sanction;
            return Task.Run(() =>
            {
                if (!CanModify(sanction)) return sanct;

                var sanctionEndDate = SanctionManager.GetSanctionEndDate(sanction.Sanction, sanction.SanctionStartDate);
                if (sanctionEndDate.Equals(sanct.SanctionEndDate)) return sanct;

                var timelineEntry = new Timeline().Create(sanct.EmployeeID, TimelineOrigin.Sanctions);
                timelineEntry.EventMessage = $"{sanct.Sanction} has been re-issued by {Environment.UserName}";
                var timelineQuery = $"INSERT INTO timeline {timelineEntry.GetHeader()} VALUES {timelineEntry.GetValues()};";

                string query = $@"UPDATE sanctions SET sanctionEndDate = '{sanctionEndDate.ToString(DataStorage.ShortDBDateFormat)}', overriden = '{0}', overridenBy = '', 
                                overridenAt = NULL WHERE id ='{sanction.ID}'; {timelineQuery}";
                var response = Execute(query);
                if (response.Success)
                {
                    sanct.OverridenAt = DateTime.MinValue;
                    sanct.SanctionEndDate = sanctionEndDate;
                    sanct.Overriden = false;
                    sanct.OverridenBy = string.Empty;
                }
                return sanct;
            });
        }

        public Task<Response> InsertAsync(SanctionEntity sanction)
        {
            var timelineEntry = new Timeline().Create(sanction.EmployeeID, TimelineOrigin.Sanctions);
            timelineEntry.EventMessage = $"{sanction.Sanction} has been recorded by {Environment.UserName} and is active until {sanction.SanctionEndDate.ToString(DataStorage.ShortPreviewDateFormat)}";
            var timelineQuery = $"INSERT INTO timeline {timelineEntry.GetHeader()} VALUES {timelineEntry.GetValues()};";

            string query = $@"INSERT INTO sanctions {sanction.GetHeader()} VALUES {sanction.GetValues()}; {timelineQuery}";
            return ExecuteAsync(query);
        }

        public Task<IEnumerable<SanctionEntity>> GetEmployeeSanctionsAsync(string emplId)
        {
            string query = $@"SELECT * FROM sanctions WHERE employeeID = '{emplId}' ORDER BY createdAt DESC;";
            return GetCachedAsync<SanctionEntity>(query);
        }
    }
}
