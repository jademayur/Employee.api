using Employee.api.Model;
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
            //get all departments data
            var departments = _context.Departments.ToList();
            return Ok(departments);
        }

        [HttpPost]
        public IActionResult AddDepartment(Department department)
        {
            bool exists = _context.Departments.Any(d => d.departmentName.ToLower() == department.departmentName.ToLower());
            if (exists) {
                return Conflict("Department Already Exists");
            }
            _context.Add(department);
            _context.SaveChanges();
            return Ok("Department Save Successfully");
        }

        [HttpPut]
        public IActionResult UpdateDepartment(Department department)
        {
            var dept = _context.Departments.Find(department.departmentId);
            if (dept == null)
            {
                return NotFound("Department Not Found");
            }
            dept.departmentName = department.departmentName;
            dept.isActive = department.isActive;
            _context.SaveChanges();
            return Ok("Department Updated Successfully");
        }

        [HttpDelete]
        public IActionResult DeleteDepartment(int id)
        {
            var dept = _context.Departments.Find(id);
            if (dept == null)
            {
                return NotFound("Department Not Found");
            }
            _context.Departments.Remove(dept);
            _context.SaveChanges();
            return Ok("Department Deleted Successfully");
        }
    }
}
