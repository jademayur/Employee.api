using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly Model.AppDbContext _context;
        public DepartmentController(Model.AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllDepartments()
        {
            var departments = _context.Departments.ToList();
            return Ok(departments);
        }

    }
}
