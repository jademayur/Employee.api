using System.ComponentModel.DataAnnotations;

namespace Employee.api.Model
{
    public class LeaveBalance
    {
        [Key]
        public int LeaveBalanceId { get; set; }

        public int EmployeeId { get; set; }

        [Required, MaxLength(20)]
        public string LeaveType { get; set; } = string.Empty;
        // Casual / Sick / Earned

        public decimal TotalLeaves { get; set; }
        public decimal UsedLeaves { get; set; }
        public decimal RemainingLeaves { get; set; }
    }
}
