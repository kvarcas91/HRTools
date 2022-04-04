namespace Domain.Repository
{
    public class RepositoryHelper : BaseRepository
    {
        public string GetDbInitQuery()
        {
            string query = @"CREATE TABLE awal (
                            id TEXT NOT NULL PRIMARY KEY,
                            employeeID TEXT NOT NULL,
                            userID TEXT,
                            employeeName TEXT,
                            departmentID TEXT,
                            employmentStartDate TEXT NOT NULL,
                            employmentType TEXT,
                            managerName TEXT,
                            shiftPattern TEXT,
                            tenureInDays INTEGER,
                            probationStatus INTEGER,
                            awalStatus INTEGER,
                            firstNCNSDate TEXT NOT NULL,
                            awal1SentDate TEXT,
                            awal2SentDate TEXT,
                            disciplinaryDate TEXT,
                            outcome INTEGER,
                            ukPendingEndDate TEXT,
                            createdBy TEXT NOT NULL,
                            createdAt TEXT NOT NULL,
                            updatedBy TEXT,
                            updatedAt TEXT,
                            acceptedBy TEXT,
                            acceptedAt TEXT,
                            closeBridge TEXT,
                            bridgeCreatedBy TEXT,
                            bridgeCreatedAt TEXT
                            );

                            CREATE table sanctions(
                            
                            );

                            CREATE table suspensions(
                            
                            );

                            CREATE table resignations(
                            
                            );

                            CREATE table meetings(
                            
                            );

                            CREATE table custom_meetings(
                            
                            );

                            CREATE table comments(
                            
                            );

                            CREATE table tasks(
                            
                            );";
            return query;
        }

        public bool CreateDatabase(string dbPath)
        {
            var canCreate = CreateDbFile(dbPath);
            if (!canCreate) return false;

            string tableQuery = GetDbInitQuery();

            return Execute(tableQuery);
        }

        public bool CheckDbHealth()
        {
            var output = GetScalar<string>("select sqlite_version();");
            return !string.IsNullOrEmpty(output);
        }
    }
}
