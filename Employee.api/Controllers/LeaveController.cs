using Employee.api.DTOs;
using Employee.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeaveController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("apply")]
        public IActionResult ApplyLeave([FromBody] ApplyLeaveDto dto)
        {
            decimal totalDays;

            if (dto.IsHalfDay)
                totalDays = 0.5M;
            else
                totalDays = (dto.ToDate - dto.FromDate).Days + 1;

            var leave = new LeaveApplication
            {
                EmployeeId = dto.EmployeeId,
                LeaveType = dto.LeaveType,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                IsHalfDay = dto.IsHalfDay,
                TotalDays = totalDays,
                Reason = dto.Reason
            };

            _context.LeaveApplications.Add(leave);
            _context.SaveChanges();

            return Ok("Leave applied successfully");
        }

        [HttpPost("action")]
        public IActionResult LeaveAction([FromBody] LeaveActionDto dto)
        {
            var leave = _context.LeaveApplications
                                .FirstOrDefault(x => x.LeaveId == dto.LeaveApplicationId);

            if (leave == null)
                return NotFound("Leave not found");

            leave.Status = dto.Action;
            leave.ApprovedBy = dto.ManagerId;
            leave.ApprovedDate = DateTime.Now;

            // 🔥 Deduct leave ONLY on approval
            if (dto.Action == "Approved")
            {
                var balance = _context.LeaveBalances.FirstOrDefault(x =>
                    x.EmployeeId == leave.EmployeeId &&
                    x.LeaveType == leave.LeaveType);

                if (balance == null || balance.RemainingLeaves < leave.TotalDays)
                    return BadRequest("Insufficient leave balance");

                balance.UsedLeaves += leave.TotalDays;
                balance.RemainingLeaves -= leave.TotalDays;
            }

            _context.SaveChanges();

            return Ok($"Leave {dto.Action}");
        }

        [HttpGet("employee/{employeeId}")]
        public IActionResult GetEmployeeLeaves(int employeeId)
        {
            var data = _context.LeaveApplications
                               .Where(x => x.EmployeeId == employeeId)
                               .OrderByDescending(x => x.AppliedDate)
                               .ToList();

            return Ok(data);
        }

        [HttpGet("pending/{managerId}")]
        public IActionResult GetPendingLeaves(int managerId)
        {
            var data = _context.LeaveApplications
                .Where(x => x.Status == "Pending")
                .ToList();

            return Ok(data);
        }

    }
}
