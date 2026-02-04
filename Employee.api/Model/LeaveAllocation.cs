using System.ComponentModel.DataAnnotations;

namespace Employee.api.Model
{
    public class LeaveAllocation
    {
        [Key]
        public int LeaveAllocationId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int LeaveTypeId { get; set; } // SL / CL / PL

        [Required, MaxLength(20)]
        public string AllocationType { get; set; } = "Yearly";
        // Yearly / Monthly

        [Required]
        public decimal TotalLeaves { get; set; }

        [Required]
        public int Year { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
