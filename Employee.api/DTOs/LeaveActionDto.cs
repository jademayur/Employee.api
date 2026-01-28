namespace Employee.api.DTOs
{
    public class LeaveActionDto
    {
        public int LeaveApplicationId { get; set; }
        public int ManagerId { get; set; }
        public string Action { get; set; } = string.Empty; // Approve / Reject
    }
}
