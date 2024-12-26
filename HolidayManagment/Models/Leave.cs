namespace HolidayManagment.Models
{
    public class Leave
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool IsApproved { get; set; }
        public int? LeaveDays { get; set; }

        public Employee? Employee { get; set; }
        public LeaveType? LeaveType { get; set; }
    }
}
