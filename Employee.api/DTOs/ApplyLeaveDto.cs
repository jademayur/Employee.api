namespace Employee.api.DTOs
{
    public class ApplyLeaveDto
    {
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsHalfDay { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
