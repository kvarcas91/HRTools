using Domain.Automation;
using Domain.DataValidation;
using Domain.DataValidation.AWAL;
using Domain.Extensions;
using Domain.Factory;
using Domain.Models;
using Domain.Models.AWAL;
using Domain.Networking;
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

        public Task<Response> InsertAllAsync(IList<IDataImportObject> awalList) => base.InsertAllAsync(awalList, "awal");

        public Task<Response> InsertAsync(AwalEntity awal)
        {
            var validationResponse = _validator.Validate(awal);
            if (!validationResponse.Success) return Task.Run(() => validationResponse);
            if (IsOpenAwalExists(awal.EmployeeID)) return Task.Run(() => new Response { Success = false, Message = "Open AWAL case already exists for this employee"});

            var awal1SentMessage = awal.Awal1SentDate == DateTime.MinValue ? "" : $"AWAL 1 letter sent on {awal.Awal1SentDate.ToString(DataStorage.ShortPreviewDateFormat)}";
            var timeLine = new Timeline().Create(awal.EmployeeID, TimelineOrigin.AWAL, $"AWAL case has been created by {awal.CreatedBy}. First NCNS date - {awal.FirstNCNSDate.ToString(DataStorage.ShortPreviewDateFormat)}. {awal1SentMessage}");
            var tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()};";

            var query = $"INSERT INTO awal {awal.GetHeader()} VALUES {awal.GetValues()}; {tlQuery}";

            var dbAwal = GetScalar<AwalEntity>($"SELECT * FROM awal WHERE id = '{awal.ID}';");
            Automate(awal, dbAwal, AutomationAction.OnIntake);
            return ExecuteAsync(query);
        }

        public Task<Response> CloseAsync(AwalEntity awal)
        {
            if (!IsOpenAwalExists(awal.EmployeeID)) return Task.Run(() => new Response { Success = false, Message = "Case already closed" });
            var timeLine = new Timeline().Create(awal.EmployeeID, TimelineOrigin.AWAL, $"AWAL case has been bridged by {awal.BridgeCreatedBy} - '{awal.ReasonForClosure.DbSanityCheck()}'");
            var tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()};";
            var query = $@"UPDATE awal SET awalStatus = '{(int)awal.AwalStatus}', outcome = '{awal.Outcome}', updatedAt = '{awal.UpdatedAt.ToString(DataStorage.LongDBDateFormat)}', 
                        updatedBy = '{awal.UpdatedBy}', reasonForClosure = '{awal.ReasonForClosure.DbSanityCheck()}', bridgeCreatedBy = '{awal.BridgeCreatedBy}', 
                        bridgeCreatedAt = '{awal.BridgeCreatedAt.ToString(DataStorage.LongDBDateFormat)}' WHERE id= '{awal.ID}'; {tlQuery}";

            if (awal.Awal1SentDate != DateTime.MinValue || awal.Awal2SentDate != DateTime.MinValue)
            {
               
                WebHook.PostAsync(DataStorage.AppSettings.AwalChanelWebHook, $"Hello, please close AWAL case for {awal.EmployeeID} if exists");
            }

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
            if (!string.IsNullOrEmpty(awal.Outcome) && !awal.Outcome.Equals("Cancelled"))
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
            if (awal.AwalStatus != AwalStatus.Cancelled && !string.IsNullOrEmpty(awal.Outcome) && awal.Outcome.Equals("Cancelled"))
            {
                awal.AwalStatus = AwalStatus.Cancelled;
            }

            var query = $@"UPDATE awal SET updatedBy = '{awal.UpdatedBy}', updatedAt = '{awal.UpdatedAt.ToString(DataStorage.LongDBDateFormat)}', firstNCNSDate = {awal.FirstNCNSDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)}, 
                        awal1SentDate = {awal.Awal1SentDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)}, awal2SentDate = {awal.Awal2SentDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)}, 
                        disciplinaryDate = {awal.DisciplinaryDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)}, outcome = '{awal.Outcome}', awalStatus = '{(int)awal.AwalStatus}' WHERE id = '{awal.ID}'; {timelineQuery}";

            var dbAwal = GetScalar<AwalEntity>($"SELECT * FROM awal WHERE id = '{awal.ID}';");
            Automate(awal, dbAwal, AutomationAction.OnUpdate);

            return ExecuteAsync(query);
        }

        public Task<Response> RequestAwalLetterAsync(AwalEntity awal, string awalType)
        {

            return Task.Run(() =>
            {
                var onProbation = false;
                if (awal.EmploymentStartDate != DateTime.MinValue) onProbation = (DateTime.Now - awal.EmploymentStartDate).Days < 90;

                if ((awalType.Equals("1") && !awal.Awal1SentDate.Equals(DateTime.MinValue)) || awalType.Equals("2") && !awal.Awal2SentDate.Equals(DateTime.MinValue))
                {
                    return new Response { Success = false, Message = $"AWAL {awalType} already in process" };
                }

                switch (awalType)
                {
                    case "1":
                        if (!onProbation && awal.FirstNCNSDate.AddDays(1) > DateTime.Now)
                            return new Response { Success = false, Message = $"Cannot request AWAL 1 letter now as AA is not on probation. You will be able to do it on {DateTime.Now.AddDays(1).ToString(DataStorage.ShortPreviewDateFormat)}" };
                        break;
                    case "2":
                        if (awal.Awal1SentDate.Equals(DateTime.MinValue))
                            return new Response { Success = false, Message = "You cannot request AWAL 2 while AWAL 1 was not sent" };
                        if (awal.Awal1SentDate.AddDays(6) > DateTime.Now)
                            return new Response { Success = false, Message = $"Cannot request AWAL 2 at the moment as AA is not on probation. You will be able to request it on {awal.Awal1SentDate.AddDays(6).ToString(DataStorage.ShortPreviewDateFormat)}" };
                        break;
                }

                var tl = new Timeline().Create(awal.EmployeeID, TimelineOrigin.AWAL, $"AWAL {awalType} has been requested by {Environment.UserName}");
                Execute($"INSERT INTO timeline {tl.GetHeader()} VALUES {tl.GetValues()};");

                WebHook.PostAsync(DataStorage.AppSettings.AwalChanelWebHook, $"Hello, please initiate AWAL {awalType} for {awal.EmployeeID}");
                return new Response { Success = true };
            });

        }

        private string GetUpdateTimelineString(AwalEntity awal)
        {
            var dbObj = GetCachedScalar<AwalEntity>($"SELECT * FROM awal WHERE id = '{awal.ID}';");
            if (dbObj == null) return string.Empty;
            var haveUpdate = false;
            var timelineString = new StringBuilder("INSERT INTO timeline ");
            if (awal.FirstNCNSDate != dbObj.FirstNCNSDate)
            {
                var tl = new Timeline().Create(awal.EmployeeID, TimelineOrigin.AWAL, $"First NCNS date has been updated by {Environment.UserName}. Changed '{dbObj.FirstNCNSDate.ToString(DataStorage.ShortPreviewDateFormat)}' into '{awal.FirstNCNSDate.ToString(DataStorage.ShortPreviewDateFormat)}'");
                timelineString.Append($"{tl.GetHeader()} VALUES {tl.GetValues()}");
                haveUpdate = true;
            }

            if (awal.Awal1SentDate != dbObj.Awal1SentDate)
            {
                var message = dbObj.Awal1SentDate == DateTime.MinValue ?  $"AWAL 1 sent date ({awal.Awal1SentDate.ToString(DataStorage.ShortPreviewDateFormat)}) has been recorded by {Environment.UserName}" : 
                    awal.Awal1SentDate == DateTime.MinValue ? $"AWAL 1 sent date has been removed by {Environment.UserName}" :
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
                    awal.Awal2SentDate == DateTime.MinValue ? $"AWAL 2 sent date has been removed by {Environment.UserName}" :
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
                var message = dbObj.DisciplinaryDate == DateTime.MinValue ? $"AWAL disciplinary date ({awal.DisciplinaryDate.ToString(DataStorage.ShortPreviewDateFormat)}) has been recorded by {Environment.UserName}" :
                    awal.DisciplinaryDate == DateTime.MinValue ? $"AWAL disciplinary date has been removed by {Environment.UserName}" :
                    $"AWAL disciplinary date has been updated by {Environment.UserName}. Changed '{dbObj.DisciplinaryDate.ToString(DataStorage.ShortPreviewDateFormat)}' into '{awal.DisciplinaryDate.ToString(DataStorage.ShortPreviewDateFormat)}'";
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
                    $"Outcome has been updated by {Environment.UserName}. Changed '{dbObj.Outcome}' into '{awal.Outcome}'";
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
            return GetScalar<int>($"SELECT COUNT(*) FROM awal WHERE employeeID = '{emplId}' AND awalStatus in ({(int)AwalStatus.Active},{(int)AwalStatus.Pending});") > 0;
        }

        private void Automate(AwalEntity awal, AwalEntity dbAwal, AutomationAction action)
        {
            Task.Run(() =>
            {
                var automation = new AwalAutomation(this).SetData(dbAwal, awal);
                automation.Invoke(action);
            });
            
        }

    }
}
