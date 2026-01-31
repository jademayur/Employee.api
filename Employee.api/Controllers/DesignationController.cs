using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : ControllerBase
    {
        //designation controller with CRUD operations
        private readonly Model.AppDbContext _context;
        public DesignationController(Model.AppDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL DESIGNATIONS =================
        [HttpGet("GetAllDesignations")]
        public IActionResult GetAllDesignations()
        {
            var designations = from des in _context.Designations
                               join dep in _context.Departments
                               on des.departmentId equals dep.departmentId
                               select new
                               {
                                   des.designationId,
                                   des.designationName,
                                   des.departmentId,
                                   departmentName = dep.departmentName
                               };

            return Ok(new { success = true, data = designations });
        }

        // ================= GET DESIGNATION BY ID =================
        [HttpGet("{id}")]
        public IActionResult GetDesignationById(int id)
        {
            var designation = _context.Designations.Find(id);

            if (designation == null)
                return NotFound(new { success = false, message = "Designation Not Found" });

            return Ok(new { success = true, data = designation });
        }

        // ================= ADD DESIGNATION =================
        [HttpPost("AddDesignation")]
        public IActionResult AddDesignation(Model.Designation designation)
        {
            bool exists = _context.Designations.Any(d =>
                d.designationName.ToLower() == designation.designationName.ToLower() &&
                d.departmentId == designation.departmentId);

            if (exists)
                return Conflict(new { success = false, message = "Designation Already Exists in this Department" });

            _context.Add(designation);
            _context.SaveChanges();

            return Ok(new { success = true, message = "Designation Save successfully" });
        }

        // ================= UPDATE DESIGNATION =================
        [HttpPut("UpdateDesignation")]
        public IActionResult UpdateDesignation(Model.Designation designation)
        {
            var desig = _context.Designations.Find(designation.designationId);

            if (desig == null)
                return NotFound(new { success = false, message = "Designation Not Found" });

            desig.designationName = designation.designationName;
            desig.departmentId = designation.departmentId;
            _context.SaveChanges();

            return Ok(new { success = true, message = "Designation Update successfully" });
        }

        // ================= DELETE DESIGNATION =================
        [HttpDelete("DeleteDesignation")]
        public IActionResult DeleteDesignation(int id)
        {
            var desig = _context.Designations.Find(id);

            if (desig == null)
                return NotFound(new { success = false, message = "Designation Not Found" });

            _context.Designations.Remove(desig);
            _context.SaveChanges();

            return Ok(new { success = true, message = "Designation Delete successfully" });
        }
    }

}
