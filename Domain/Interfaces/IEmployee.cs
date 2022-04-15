namespace Domain.Interfaces
{
    public interface IEmployee
    {
        string EmployeeID { get; set; }
        string UserID { get; set; }
        string EmployeeName { get; set; }
        string ShiftPattern { get; set; }
        string ManagerName { get; set; }
    }
}
