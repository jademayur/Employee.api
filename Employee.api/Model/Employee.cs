using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.api.Model
{
    [Table("employeeTbl")]
    public class Employee
    {
        [Key]
        public int employeeId { get; set; }

        [Required, MaxLength(50)]
        public string name { get; set; } = string.Empty;

        [Required, MaxLength(10), MinLength(10)]
        public string contactNo { get; set; } = string.Empty;

        public string? altContactNo { get; set; }

        [Required, MaxLength(50)]
        public string email { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string city { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string State { get; set; } = string.Empty;

        [Required, MaxLength(6)]
        public string pincode { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string address { get; set; } = string.Empty;

        public int designationId { get; set; }

        // 🔹 Leave Approval Hierarchy
        public int? ManagerId { get; set; }

        // 🔹 Employment Info
        public DateTime DateOfJoining { get; set; }
        public bool IsActive { get; set; } = true;

        // 🔹 System Fields
        public string role { get; set; } = string.Empty;
        public DateTime? createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
    }


    public class LoginDto
    {
        [Required]
        public string email { get; set; } = string.Empty;

        [Required]
        public string contactNo { get; set; } = string.Empty;
    }
}
