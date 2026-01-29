using System.ComponentModel.DataAnnotations;

namespace Employee.api.Model
{
    public class LeaveBalance
    {
        [Key]
        public int LeaveBalanceId { get; set; }

        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }

        public decimal TotalLeaves { get; set; }
        public decimal UsedLeaves { get; set; }
        public decimal AvailableLeaves { get; set; }

        public int Year { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
