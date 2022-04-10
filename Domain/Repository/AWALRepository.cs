using Domain.DataValidation;
using Domain.DataValidation.AWAL;
using Domain.Extensions;
using Domain.Factory;
using Domain.Models;
using Domain.Models.AWAL;
using Domain.Storage;
using Domain.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{

    public sealed class AWALRepository : BaseRepository
    {
        private enum UpdatePart { FirstNCNS, AWAL1, AWAL2, Disciplinary }

        private IDataValidation _validator;

        public AWALRepository()
        {
            _validator = new AwalValidation();
        }

        public Task<IEnumerable<AwalEntity>> GetEmployeeAwalAsync(string emplId)
        {
            string query = $"SELECT * FROM awal WHERE employeeID = '{emplId}' ORDER BY firstNCNSDate DESC;";
            return GetCachedAsync<AwalEntity>(query);
        }

        public Task<Response> InsertAllAsync(IList<IDataImportObject> awalList)
        {
            return InsertAllAsync(awalList, "awal");
        }

        public Task<Response> InsertAsync(AwalEntity awal)
        {
            var validationResponse = _validator.Validate(awal);
            if (!validationResponse.Success) return Task.Run(() => validationResponse);
            if (IsOpenAwalExists(awal.EmployeeID)) return Task.Run(() => new Response { Success = false, Message = "Open AWAL case already exists for this employee"});

            var awal1SentMessage = awal.Awal1SentDate == DateTime.MinValue ? "" : $"AWAL 1 letter sent on {awal.Awal1SentDate.ToString(DataStorage.ShortPreviewDateFormat)}";
            var timeLine = new Timeline().Create(awal.EmployeeID, TimelineOrigin.AWAL, $"AWAL case has been created by {awal.CreatedBy}. First NCNS date - {awal.FirstNCNSDate.ToString(DataStorage.ShortPreviewDateFormat)}. {awal1SentMessage}");
            var tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()};";

            var query = $"INSERT INTO awal {awal.GetHeader()} VALUES {awal.GetValues()}; {tlQuery}";
            return ExecuteAsync(query);
        }

        public Task<Response> CloseAsync(AwalEntity awal)
        {
            if (!IsOpenAwalExists(awal.EmployeeID)) return Task.Run(() => new Response { Success = false, Message = "Case already closed" });
            var timeLine = new Timeline().Create(awal.EmployeeID, TimelineOrigin.AWAL, $"AWAL case has been bridged by {awal.BridgeCreatedBy} - ''{awal.ReasonForClosure.DbSanityCheck()}''");
            var tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()};";
            var query = $@"UPDATE awal SET awalStatus = '{(int)awal.AwalStatus}', outcome = '{awal.Outcome}', updatedAt = '{awal.UpdatedAt.ToString(DataStorage.LongDBDateFormat)}', 
                        updatedBy = '{awal.UpdatedBy}', reasonForClosure = '{awal.ReasonForClosure.DbSanityCheck()}', bridgeCreatedBy = '{awal.BridgeCreatedBy}', 
                        bridgeCreatedAt = '{awal.BridgeCreatedAt.ToString(DataStorage.LongDBDateFormat)}'; {tlQuery}";
            return ExecuteAsync(query);
        }

        public Task<Response> UpdateAsync(AwalEntity awal)
        {
            var validationResponse = _validator.Validate(awal);
            if (!validationResponse.Success) return Task.Run(() => validationResponse);

            var timelineQuery = GetUpdateTimelineString(awal);
            if (string.IsNullOrEmpty(timelineQuery)) return Task.Run(() => new Response { Success = false, Message = "No changes were made" });

            awal.UpdatedAt = DateTime.Now;
            awal.UpdatedBy = Environment.UserName;
            if (awal.DisciplinaryDate != DateTime.MinValue && string.IsNullOrEmpty(awal.Outcome)) awal.AwalStatus = AwalStatus.Pending;
            if (awal.AwalStatus == AwalStatus.Cancelled && !awal.Outcome.Equals("Cancelled"))
            {
                switch(awal.Outcome)
                {
                    case "Termination":
                        awal.AwalStatus = AwalStatus.Terminated;
                        break;
                    default:
                        awal.AwalStatus = AwalStatus.Inactive;
                        break;
                }
            }
            if (awal.AwalStatus != AwalStatus.Cancelled && awal.Outcome.Equals("Cancelled"))
            {
                awal.AwalStatus = AwalStatus.Cancelled;
            }

            var query = $@"UPDATE awal SET updatedBy = '{awal.UpdatedBy}', updatedAt = '{awal.UpdatedAt.ToString(DataStorage.LongDBDateFormat)}', firstNCNSDate = '{awal.FirstNCNSDate.ToString(DataStorage.LongDBDateFormat)}', 
                        awal1SentDate = '{awal.Awal1SentDate.ToString(DataStorage.LongDBDateFormat)}', awal2SentDate = '{awal.Awal2SentDate.ToString(DataStorage.LongDBDateFormat)}', 
                        disciplinaryDate = '{awal.DisciplinaryDate.ToString(DataStorage.LongDBDateFormat)}', outcome = '{awal.Outcome}', awalStatus = '{(int)awal.AwalStatus}' WHERE id = '{awal.ID}'; {timelineQuery}";

            return ExecuteAsync(query);
        }

        private string GetUpdateTimelineString(AwalEntity awal)
        {
            var dbObj = GetCachedScalar<AwalEntity>($"SELECT * FROM awal WHERE id = '{awal.ID}';");
            if (dbObj == null) return string.Empty;
            var haveUpdate = false;
            var timelineString = new StringBuilder("INSERT INTO timeline ");
            if (awal.FirstNCNSDate != dbObj.FirstNCNSDate)
            {
                var tl = new Timeline().Create(awal.EmployeeID, TimelineOrigin.AWAL, $"First NCNS date has been updated by {Environment.UserName}. Changed ''{dbObj.FirstNCNSDate.ToString(DataStorage.ShortPreviewDateFormat)}'' into ''{awal.FirstNCNSDate.ToString(DataStorage.ShortPreviewDateFormat)}''");
                timelineString.Append($"{tl.GetHeader()} VALUES {tl.GetValues()}");
                haveUpdate = true;
            }

            if (awal.Awal1SentDate != dbObj.Awal1SentDate)
            {
                var message = dbObj.Awal1SentDate == DateTime.MinValue ?  $"AWAL 1 sent date ({awal.Awal1SentDate.ToString(DataStorage.ShortPreviewDateFormat)}) has been recorded by {Environment.UserName}" : 
                    $"AWAL 1 sent date has been updated by {Environment.UserName}. Changed ''{dbObj.Awal1SentDate.ToString(DataStorage.ShortPreviewDateFormat)}'' into ''{awal.Awal1SentDate.ToString(DataStorage.ShortPreviewDateFormat)}''";
                var tl = new Timeline().Create(awal.EmployeeID, TimelineOrigin.AWAL, message);
                if (!haveUpdate)
                {
                    timelineString.Append($"{tl.GetHeader()} VALUES {tl.GetValues()}");
                    haveUpdate = true;
                }
                else timelineString.Append($",{tl.GetValues()}");
                

            }

            if (awal.Awal2SentDate != dbObj.Awal2SentDate)
            {
                var message = dbObj.Awal2SentDate == DateTime.MinValue ? $"AWAL 2 sent date ({awal.Awal2SentDate.ToString(DataStorage.ShortPreviewDateFormat)}) has been recorded by {Environment.UserName}" :
                    $"AWAL 2 sent date has been updated by {Environment.UserName}. Changed ''{dbObj.Awal2SentDate.ToString(DataStorage.ShortPreviewDateFormat)}'' into ''{awal.Awal2SentDate.ToString(DataStorage.ShortPreviewDateFormat)}''";
                var tl = new Timeline().Create(awal.EmployeeID, TimelineOrigin.AWAL, message);
                if (!haveUpdate)
                {
                    timelineString.Append($"{tl.GetHeader()} VALUES {tl.GetValues()}");
                    haveUpdate = true;
                }
                else timelineString.Append($",{tl.GetValues()}");
            }
            
            if (awal.DisciplinaryDate != dbObj.DisciplinaryDate)
            {
                var message = dbObj.DisciplinaryDate == DateTime.MinValue ? $"Disciplinary date ({awal.DisciplinaryDate.ToString(DataStorage.ShortPreviewDateFormat)}) has been recorded by {Environment.UserName}" :
                    $"Disciplinary date has been updated by {Environment.UserName}. Changed ''{dbObj.DisciplinaryDate.ToString(DataStorage.ShortPreviewDateFormat)}'' into ''{awal.DisciplinaryDate.ToString(DataStorage.ShortPreviewDateFormat)}''";
                var tl = new Timeline().Create(awal.EmployeeID, TimelineOrigin.AWAL, message);
                if (!haveUpdate)
                {
                    timelineString.Append($"{tl.GetHeader()} VALUES {tl.GetValues()}");
                    haveUpdate = true;
                }
                else timelineString.Append($",{tl.GetValues()}");
            }

            if (awal.Outcome != null && !awal.Outcome.Equals(dbObj.Outcome))
            {
                var message = string.IsNullOrEmpty(dbObj.Outcome)? $"Outcome ({awal.Outcome}) has been recorded by {Environment.UserName}" :
                    $"Outcome has been updated by {Environment.UserName}. Changed ''{dbObj.Outcome}'' into ''{awal.Outcome}''";
                var tl = new Timeline().Create(awal.EmployeeID, TimelineOrigin.AWAL, message);
                if (!haveUpdate)
                {
                    timelineString.Append($"{tl.GetHeader()} VALUES {tl.GetValues()}");
                    haveUpdate = true;
                }
                else timelineString.Append($",{tl.GetValues()}");
                
            }

            return haveUpdate ? timelineString.ToString() : string.Empty;
        }

        private bool IsOpenAwalExists(string emplId)
        {
            return GetScalar<int>($"SELECT COUNT(*) FROM awal WHERE employeeID = '{emplId}' AND awalStatus = '{(int)AwalStatus.Active}';") > 0;
        }

    }
}
