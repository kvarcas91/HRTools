namespace Domain.Models.DataSnips
{
    public struct EmployeeStatusSnip
    {
        public int SuspensionCount { get; set; }
        public bool IsRosterActive { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
