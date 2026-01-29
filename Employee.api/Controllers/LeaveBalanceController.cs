using Employee.api.DTOs;
using Employee.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveBalanceController : ControllerBase
    {
        //leave controller implementation will be here
        private readonly AppDbContext _context;

        public LeaveBalanceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("seed")]
        public IActionResult SeedLeaveBalance([FromBody] SeedLeaveBalanceDto dto)
        {
            var leaveTypes = _context.LeaveTypes.ToList();

            foreach (var type in leaveTypes)
            {
                bool exists = _context.LeaveBalances.Any(x =>
                    x.EmployeeId == dto.EmployeeId &&
                    x.LeaveTypeId == type.LeaveTypeId &&
                    x.Year == dto.Year);

                if (exists)
                    continue;

                var balance = new LeaveBalance
                {
                    EmployeeId = dto.EmployeeId,
                    LeaveTypeId = type.LeaveTypeId,
                    TotalLeaves = type.DefaultLeaves,
                    UsedLeaves = 0,
                    AvailableLeaves = type.DefaultLeaves,
                    Year = dto.Year
                };

                _context.LeaveBalances.Add(balance);
            }

            _context.SaveChanges();

            return Ok("Leave balance seeded successfully");
        }

        [HttpGet("{employeeId}/{year}")]
        public IActionResult GetLeaveBalance(int employeeId, int year)
        {
            var data = _context.LeaveBalances
                .Where(x => x.EmployeeId == employeeId && x.Year == year)
                .Select(x => new
                {
                    x.LeaveTypeId,
                    LeaveType = _context.LeaveTypes
                                .Where(t => t.LeaveTypeId == x.LeaveTypeId)
                                .Select(t => t.LeaveTypeName)
                                .FirstOrDefault(),
                    x.TotalLeaves,
                    x.UsedLeaves,
                    x.AvailableLeaves
                })
                .ToList();

            return Ok(data);
        }


    }
}
