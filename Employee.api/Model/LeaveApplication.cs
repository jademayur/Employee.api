using System.ComponentModel.DataAnnotations;

namespace Employee.api.Model
{
    public class LeaveApplication
    {
        [Key]
        public int LeaveApplicationId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required, MaxLength(20)]
        public int LeaveTypeId { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        // 🔹 NEW
        public bool IsHalfDay { get; set; } = false;

        // 🔹 NEW (FirstHalf / SecondHalf / null)
        [MaxLength(20)]
        public string? HalfDayType { get; set; }

        [Required]
        public decimal TotalDays { get; set; }   // decimal to support 0.5

        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending";

        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public DateTime AppliedDate { get; set; } = DateTime.Now;
    }
}
