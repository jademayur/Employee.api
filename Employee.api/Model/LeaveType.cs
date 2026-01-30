using System.ComponentModel.DataAnnotations;

namespace Employee.api.Model
{
    public class LeaveType
    {
        [Key]
        public int LeaveTypeId { get; set; }

        [Required, MaxLength(50)]
        public string LeaveTypeName { get; set; } = string.Empty;

        public decimal DefaultLeaves { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
