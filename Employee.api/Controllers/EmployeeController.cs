using Employee.api.DTOs;
using Employee.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        //Employee controller with CRUD operations and filtering
        private readonly Model.AppDbContext _context;
        public EmployeeController(Model.AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllEmployees")]
        public IActionResult GetAllEmployees()
        {
            //get all employees data
            var employees = _context.Employees.ToList();
            return Ok(employees);
        }
        [HttpGet("{id}")]
        public IActionResult GetEmployeeById(int id) 
        {
                var employee = _context.Employees.Find(id);
                return Ok(employee);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult AddEmployee(Model.Employee employee)
        {
            bool exists = _context.Employees.Any(e =>
                e.email.ToLower() == employee.email.ToLower() || e.contactNo == employee.contactNo
            );

            if (exists)
            {
                return BadRequest(new { Message = "Employee with same Email or Contact Number already exists" });
            }

            employee.createdDate = DateTime.Now;
            _context.Add(employee);
            _context.SaveChanges();
            return Ok(new { success = true, message = "Employee saved successfully", data = employee });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, Model.Employee employee)
        {
            var emp = _context.Employees.Find(id);
            if (emp == null)
            {
                return NotFound(new { Message = "Employee Not Found" });
            }

            // Check if any other employee has same email or contactNo
            bool exists = _context.Employees.Any(e =>
                e.employeeId != id &&
                (e.email.ToLower() == employee.email.ToLower() || e.contactNo == employee.contactNo)
            );

            if (exists)
            {
                return BadRequest(new { Message = "Employee with same Email or Contact Number already exists" });
            }

            // Update fields
            emp.name = employee.name;
            emp.contactNo = employee.contactNo;
            emp.email = employee.email;
            emp.city = employee.city;
            emp.state = employee.state;
            emp.pincode = employee.pincode;
            emp.altContactNo = employee.altContactNo;
            emp.address = employee.address;
            emp.designationId = employee.designationId;
            emp.managerId = employee.managerId;
            emp.role = employee.role;
            emp.dateOfJoining = employee.dateOfJoining;
            emp.modifiedDate = DateTime.Now;
            emp.isActive = employee.isActive;

            _context.SaveChanges();

            return Ok(new { success = true, message = "Employee updated successfully" });
        }


        [HttpGet("filter")]
        public async Task<IActionResult> Filter( string? search, int? designation, string? sortBy = "name", string? sortDir = "asc", int pageNo = 1, int pageSize = 25 )
        {
            var query = _context.Employees.AsQueryable();
            //search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.name.Contains(search) ||
                                    e.email.Contains(search) || 
                                    e.contactNo.Contains(search) ||
                                    e.city.Contains(search));

               
            }
            //Filter by designation
            if (designation.HasValue)
            {
                query = query.Where(d => d.designationId == designation);
            }

            //Sorting
            switch(sortBy?.ToLower())
            {
                case "name":
                    query = sortDir == "desc" 
                        ? query.OrderByDescending(e => e.name)
                        : query.OrderBy(e => e.name);
                    break;
                case "createddate":
                    query = sortDir == "desc" 
                        ? query.OrderByDescending(e => e.createdDate) 
                        : query.OrderBy(e => e.createdDate);
                    break;
                default:
                    query = query.OrderBy(e => e.name);
                    break;
            }

            //pagination
            var totalRecords = await query.CountAsync();
            var data = await query
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            return Ok(new
            { 
                TotalRecord = totalRecords,
                PageNumber = pageNo,
                PageSize = pageSize,
                Data =  data 
            });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.email == loginDto.email
                                       && e.contactNo == loginDto.contactNo);
            if (employee == null)
            {
                return Unauthorized(new { Message = "Invalid Email or Contact Number" });
            }
            return Ok(new { Message = "Login Successful", data = new
            {
                employee.employeeId,
                employee.name,
                employee.email,
                employee.contactNo,
                employee.designationId,
                employee.city,
                employee.role
            }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound(new { Message = "Employee Not Found" });
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Employee deleted successfully" });
        }

        [HttpGet("GetManagers")]
        public IActionResult GetManagers()
        {
            var managers = _context.Employees
                .Where(e => e.role.ToLower() == "manager" && e.isActive)
                .ToList();
            return Ok(managers);

        }
    }
}   
