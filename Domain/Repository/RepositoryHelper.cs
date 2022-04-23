namespace Domain.Repository
{
    public sealed class RepositoryHelper : BaseRepository
    {
        public string GetDbInitQuery()
        {
            string query = @"CREATE TABLE awal (
                            id TEXT NOT NULL PRIMARY KEY,
                            employeeID TEXT NOT NULL,
                            userID TEXT,
                            employeeName TEXT,
                            departmentID TEXT,
                            employmentType TEXT,
                            employmentStartDate TEXT,
                            managerName TEXT,
                            shiftPattern TEXT,
                            awalStatus INTEGER,
                            firstNCNSDate TEXT,
                            awal1SentDate TEXT,
                            awal2SentDate TEXT,
                            disciplinaryDate TEXT,
                            outcome TEXT,
                            createdBy TEXTL,
                            createdAt TEXT,
                            updatedBy TEXT,
                            updatedAt TEXT,
                            reasonForClosure TEXT,
                            bridgeCreatedBy TEXT,
                            bridgeCreatedAt TEXT
                            );

                            CREATE table timeline (
                            employeeID TEXT NOT NULL,
                            origin TEXT,
                            createdBy TEXT,
                            createdAt TEXT,
                            eventMessage TEXT
                            );

                            CREATE table sanctions (
                            id TEXT NOT NULL PRIMARY KEY,
                            employeeID TEXT NOT NULL,
                            userID text,
                            employeeName TEXT,
                            shift TEXT,
                            manager TEXT,
                            sanction TEXT,
                            sanctionStartDate TEXT,
                            sanctionEndDate TEXT,
                            createdBy TEXT,
                            createdAt TEXT,
                            meetingType TEXT,
                            overriden BOOLEAN NOT NULL Check (overriden IN (0, 1)),
                            overridenBy TEXT,
                            overridenAt TEXT
                            );

                            CREATE table suspensions (
                            id TEXT NOT NULL PRIMARY KEY,
                            employeeID TEXT NOT NULL,
                            createdAt TEXT,
                            createdBy TEXT,
                            suspensionRemovedAt TEXT,
                            suspensionRemovedBy TEXT
                            );

                            CREATE table resignations (
                            id TEXT NOT NULL PRIMARY KEY,
                            employeeID TEXT NOT NULL,
                            userID TEXT,
                            name TEXT,
                            manager TEXT,
                            shift TEXT,
                            lastWorkingDay TEXT,
                            ttLink TEXT,
                            createdBy TEXT,
                            createdAt TEXT,
                            reasonForResignation TEXT
                            );

                            CREATE table meetings (
                            id TEXT NOT NULL PRIMARY KEY,
                            employeeID TEXT NOT NULL,
                            userID TEXT,
                            employeeName TEXT,
                            managerName TEXT,
                            shiftPattern TEXT,
                            departmentID TEXT,
                            meetingType TEXT TEXT,
                            firstMeetingDate TEXT,
                            firstMeetingOutcome TEXT,
                            secondMeetingDate TEXT,
                            secondMeetingOutcome TEXT,
                            createdAt TEXT,
                            createdBy TEXT,
                            updatedAt TEXT,
                            updatedBy TEXT,
                            meetingStatus TEXT,
                            isERCaseStatusOpen BOOLEAN NOT NULL Check (isERCaseStatusOpen IN (0, 1)),
                            paperless BOOLEAN NOT NULL Check (paperless IN (0, 1))
                            );

                            CREATE table custom_meetings (
                            id TEXT NOT NULL PRIMARY KEY,
                            createdAt TEXT,
                            createdBy TEXT,
                            updatedAt TEXT,
                            updatedBy TEXT,
                            closedAt TEXT,
                            closedBy TEXT,
                            meetingStatus TEXT,
                            exactCaseID TEXT,
                            meetingType TEXT,
                            claimantID TEXT,
                            claimantName TEXT,
                            claimantUserID TEXT,
                            claimantManager TEXT,
                            claimantDepartment TEXT,
                            claimantShift TEXT,
                            claimantEmploymentStartDate TEXT,
                            respondentID TEXT,
                            respondentName TEXT,
                            respondentUserID TEXT,
                            respondentManager TEXT,
                            respondentDepartment TEXT,
                            respondentShift TEXT,
                            respondentEmploymentStartDate TEXT,
                            firstMeetingDate TEXT,
                            firstMeetingOwner TEXT,
                            firstMeetingHRSupport TEXT,
                            firstMeetingOutcome TEXT,
                            secondMeetingDate TEXT,
                            secondMeetingOwner TEXT,
                            secondMeetingHRSupport TEXT,
                            secondMeetingOutcome TEXT,
                            paperless BOOLEAN NOT NULL Check (paperless IN (0, 1)),
                            isUnionPresent BOOLEAN NOT NULL Check (isUnionPresent IN (0, 1)),
                            isWIMRaised BOOLEAN NOT NULL Check (isWIMRaised IN (0, 1))
                            );

                            CREATE table comments (
                            id TEXT NOT NULL PRIMARY KEY,
                            employeeID TEXT NOT NULL,
                            commentOrigin TEXT,
                            createdAt TEXT,
                            createdBy TEXT,
                            content TEXT
                            );

                            CREATE table tasks (
                            id TEXT NOT NULL PRIMARY KEY,
                            employeeID TEXT NOT NULL,
                            taskOrigin TEXT,
                            createdAt TEXT,
                            createdBy TEXT,
                            content TEXT,
                            isCompleted BOOLEAN NOT NULL Check (isCompleted IN (0, 1)),
                            taskDueDate TEXT,
                            completedBy TEXT,
                            completedAt TEXT
                            );";
            return query;
        }

        public bool CreateDatabase(string dbPath)
        {
            var canCreate = CreateDbFile(dbPath);
            if (!canCreate) return false;

            string tableQuery = GetDbInitQuery();

            return Execute(tableQuery).Success;
        }

        public bool CheckDbHealth()
        {
            var output = GetScalar<string>("select sqlite_version();");
            return !string.IsNullOrEmpty(output);
        }
    }
}
