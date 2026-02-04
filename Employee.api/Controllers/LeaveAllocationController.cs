using Employee.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveAllocationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeaveAllocationController(AppDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = (from a in _context.LeaveAllocations
                        join e in _context.Employees on a.EmployeeId equals e.employeeId
                        join t in _context.LeaveTypes on a.LeaveTypeId equals t.LeaveTypeId
                        select new
                        {
                            a.LeaveAllocationId,
                            a.EmployeeId,
                            employeeName = e.name,
                            a.LeaveTypeId,
                            leaveTypeName = t.LeaveTypeName,
                            a.AllocationType,
                            a.TotalLeaves,
                            a.Year
                        }).ToList();

            return Ok(data);
        }

        // ================= ADD =================
        [HttpPost("Add")]
        public IActionResult Add([FromBody] LeaveAllocation model)
        {
            // ❌ Prevent duplicate allocation
            bool exists = _context.LeaveAllocations.Any(x =>
                x.EmployeeId == model.EmployeeId &&
                x.LeaveTypeId == model.LeaveTypeId &&
                x.Year == model.Year);

            if (exists)
                return BadRequest("Leave already allocated for this year");

            _context.LeaveAllocations.Add(model);

            // 🔥 Create / Update LeaveBalance
            var balance = _context.LeaveBalances.FirstOrDefault(x =>
                x.EmployeeId == model.EmployeeId &&
                x.LeaveTypeId == model.LeaveTypeId &&
                x.Year == model.Year);

            if (balance == null)
            {
                balance = new LeaveBalance
                {
                    EmployeeId = model.EmployeeId,
                    LeaveTypeId = model.LeaveTypeId,
                    TotalLeaves = model.TotalLeaves,
                    UsedLeaves = 0,
                    AvailableLeaves = model.TotalLeaves,
                    Year = model.Year
                };
                _context.LeaveBalances.Add(balance);
            }

            _context.SaveChanges();
            return Ok("Leave allocated successfully");
        }

        // ================= UPDATE =================
        [HttpPut("Update")]
        public IActionResult Update([FromBody] LeaveAllocation model)
        {
            var allocation = _context.LeaveAllocations
                .FirstOrDefault(x => x.LeaveAllocationId == model.LeaveAllocationId);

            if (allocation == null)
                return NotFound("Allocation not found");

            allocation.TotalLeaves = model.TotalLeaves;
            allocation.AllocationType = model.AllocationType;

            // 🔄 Update balance
            var balance = _context.LeaveBalances.FirstOrDefault(x =>
                x.EmployeeId == allocation.EmployeeId &&
                x.LeaveTypeId == allocation.LeaveTypeId &&
                x.Year == allocation.Year);

            if (balance != null)
            {
                balance.TotalLeaves = model.TotalLeaves;
                balance.AvailableLeaves =
                    model.TotalLeaves - balance.UsedLeaves;
            }

            _context.SaveChanges();
            return Ok("Leave allocation updated");
        }

        // ================= DELETE =================
        [HttpDelete("Delete")]
        public IActionResult Delete(int id)
        {
            var allocation = _context.LeaveAllocations
                .FirstOrDefault(x => x.LeaveAllocationId == id);

            if (allocation == null)
                return NotFound("Allocation not found");

            _context.LeaveAllocations.Remove(allocation);
            _context.SaveChanges();

            return Ok("Leave allocation deleted");
        }

    }
}
