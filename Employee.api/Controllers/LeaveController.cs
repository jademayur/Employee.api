using Employee.api.DTOs;
using Employee.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
                LeaveTypeId = dto.LeaveTypeId,
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
                                .FirstOrDefault(x => x.LeaveApplicationId == dto.LeaveApplicationId);

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
                    x.LeaveTypeId == leave.LeaveTypeId);

                if (balance == null || balance.AvailableLeaves < leave.TotalDays)
                    return BadRequest("Insufficient leave balance");

                balance.UsedLeaves += leave.TotalDays;
                balance.AvailableLeaves -= leave.TotalDays;
            }

            _context.SaveChanges();

            return Ok(new { success = true, message = "Leave allocation update successfully" });
        }

        [HttpGet("employee/{employeeId}")]
        public IActionResult GetEmployeeLeaves(int employeeId)
        {
            var data = _context.LeaveApplications
                      .Include(x => x.LeaveType)
                      .Where(x => x.EmployeeId == employeeId)
                      .OrderByDescending(x => x.AppliedDate)
                      .Select(x => new
                      {
                          x.LeaveApplicationId,
                          x.EmployeeId,
                          x.AppliedDate,
                          x.LeaveTypeId,
                          LeaveTypeName = x.LeaveType.LeaveTypeName,
                            x.FromDate,
                            x.ToDate,
                            x.Status
                      })
                      .ToList();

            return Ok(data);
        }

        [HttpGet("pending/{managerId}")]
        public IActionResult GetPendingLeaves(int managerId)
        {
            var leaves = (
                 from l in _context.LeaveApplications
                 join e in _context.Employees
                     on l.EmployeeId equals e.employeeId
                 join lt in _context.LeaveTypes
                     on l.LeaveTypeId equals lt.LeaveTypeId                
                 orderby l.FromDate descending
                 select new LeaveApprovalListDto
                 {
                     LeaveApplicationId = l.LeaveApplicationId,
                     EmployeeName = e.name,
                     LeaveTypeName = lt.LeaveTypeName,
                     FromDate = l.FromDate,
                     ToDate = l.ToDate,
                     TotalDays = l.TotalDays,
                     Reason = l.Reason,
                     Status = l.Status
                 }
             ).ToList();

            return Ok(leaves);
        }

        [HttpGet("history/{employeeId}")]
        public IActionResult GetEmployeeLeaveHistory(int employeeId)
        {
            var history = _context.LeaveApplications
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.AppliedDate)
                .Select(x => new
                {
                    x.LeaveApplicationId,
                    x.FromDate,
                    x.ToDate,
                    x.IsHalfDay,
                    x.TotalDays,
                    x.Status,
                    x.Reason,
                    x.AppliedDate,
                    x.ApprovedDate,
                    LeaveType = _context.LeaveTypes
                        .Where(t => t.LeaveTypeId == x.LeaveTypeId)
                        .Select(t => t.LeaveTypeName)
                        .FirstOrDefault()
                })
                .ToList();

            return Ok(history);
        }

        [HttpGet("team-history/{managerId}")]
        public IActionResult GetTeamLeaveHistory(int managerId)
        {
            var data = _context.LeaveApplications
                .Where(x => x.Status != "Pending")
                .OrderByDescending(x => x.AppliedDate)
                .ToList();

            return Ok(data);
        }

        [HttpPost("cancel/{leaveId}")]
        public IActionResult CancelLeave(int leaveId)
        {
            var leave = _context.LeaveApplications
                .FirstOrDefault(x => x.LeaveApplicationId == leaveId);

            if (leave == null)
                return NotFound("Leave not found");

            if (leave.Status != "Pending")
                return BadRequest("Only pending leave can be cancelled");

            leave.Status = "Cancelled";
            _context.SaveChanges();

            return Ok("Leave cancelled successfully");
        }
       


    }

}
