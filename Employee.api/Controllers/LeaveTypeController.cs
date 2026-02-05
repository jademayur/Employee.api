using Employee.api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveTypeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeaveTypeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            return Ok(_context.LeaveTypes.Where(x => x.IsActive).ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add(LeaveType model)
        {
            _context.LeaveTypes.Add(model);
            _context.SaveChanges();
            return Ok("Leave type added");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public IActionResult Update(LeaveType model)
        {
            _context.LeaveTypes.Update(model);
            _context.SaveChanges();
            return Ok("Leave type updated");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var leaveType = _context.LeaveTypes.Find(id);
            if (leaveType == null)
                return NotFound();

            leaveType.IsActive = false;
            _context.SaveChanges();
            return Ok("Leave type deactivated");
        }
    }
}
