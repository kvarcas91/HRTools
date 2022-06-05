namespace Domain.Models.DataSnips
{
    public class MeetingMetrics
    {
        public int TotalCaseCount { get; set; }
        public int CompletedMeetingsCount { get; set; }
        public decimal CompletionRate { get; set; }
        public int MissedMeetingsCount { get; set; }
        public int IssuedSanctionCount { get; set; }
        public int IssuedNFACount { get; set; }
        public int CancelledCount { get; set; }
        public int OverturnCount { get; set; }
        public decimal OverturnRate { get; set; }
        public decimal NFARate { get; set; }
        public decimal CancelledRate { get; set; }
        public int PaperlessCount { get; set; }
        public decimal PaperlessRate { get; set; }
        public int PendingCount { get; set; }

        public void SetRatios()
        {
            SetCompletionRate();
            SetOverturnRate();
            SetNFARate();
            SetPaperlessRate();
            SetCancelledRate();
        }

        private void SetPaperlessRate()
        {
            if (CompletedMeetingsCount == 0)
            {
                PaperlessRate = 0M;
                return;
            }

            PaperlessRate = decimal.Round(PaperlessCount / (decimal)CompletedMeetingsCount * 100, 2);
        }

        private void SetNFARate()
        {
            if (TotalCaseCount == 0)
            {
                NFARate = 0M;
                return;
            }

            NFARate = decimal.Round(IssuedNFACount / (decimal)TotalCaseCount * 100, 2);
        }

        private void SetCancelledRate()
        {
            if (TotalCaseCount == 0)
            {
                NFARate = 0M;
                return;
            }

            CancelledRate = decimal.Round(CancelledCount / (decimal)TotalCaseCount * 100, 2);
        }

        private void SetOverturnRate()
        {
            if (TotalCaseCount == 0)
            {
                OverturnRate = 0M;
                return;
            }

            OverturnRate = decimal.Round(OverturnCount / (decimal)TotalCaseCount * 100, 2);
        }

        private void SetCompletionRate()
        {
            if (TotalCaseCount == 0 || CompletedMeetingsCount == 0)
            {
                CompletionRate = 0M;
                return;
            }

            CompletionRate = decimal.Round(CompletedMeetingsCount / (decimal)(TotalCaseCount - (TotalCaseCount - MissedMeetingsCount - CompletedMeetingsCount)) * 100, 2);
        }
    }
}
