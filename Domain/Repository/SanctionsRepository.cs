using Domain.DataManager;
using Domain.Models.Sanctions;
using Domain.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public class SanctionsRepository : BaseRepository
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

        public Task<SanctionEntity> OverrideSanctionAsync(SanctionEntity sanction)
        {
            var sanct = sanction;
            return Task.Run(() =>
            {
                if (!CanModify(sanction)) return sanct;

                var overrideDate = DateTime.Now;

                string query = $@"UPDATE sanctions SET sanctionEndDate = '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}', overriden = '{1}', overridenBy = '{Environment.UserName}', 
                                overridenAt = '{overrideDate.ToString(DataStorage.LongDBDateFormat)}' WHERE id ='{sanction.ID}';";
                var response = Execute(query);
                if (response)
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

                string query = $@"UPDATE sanctions SET sanctionEndDate = '{sanctionEndDate.ToString(DataStorage.ShortDBDateFormat)}', overriden = '{0}', overridenBy = '', 
                                overridenAt = '{DateTime.MinValue.ToString(DataStorage.LongDBDateFormat)}' WHERE id ='{sanction.ID}';";
                var response = Execute(query);
                if (response)
                {
                    sanct.OverridenAt = DateTime.MinValue;
                    sanct.SanctionEndDate = sanctionEndDate;
                    sanct.Overriden = false;
                    sanct.OverridenBy = string.Empty;
                }
                return sanct;
            });
        }

        public Task<bool> InsertAsync(SanctionEntity sanction)
        {
            string query = $@"INSERT INTO sanctions {sanction.GetDbInsertHeader()} VALUES {sanction.GetDbInsertValues()};";
            return ExecuteAsync(query);
        }

        public Task<IEnumerable<SanctionEntity>> GetEmployeeSanctionsAsync(string emplId)
        {
            string query = $@"SELECT * FROM sanctions WHERE employeeID = '{emplId}' ORDER BY createdAt DESC;";
            return GetCachedAsync<SanctionEntity>(query);
        }
    }
}
