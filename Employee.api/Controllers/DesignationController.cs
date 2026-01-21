using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : ControllerBase
    {
        private readonly Model.AppDbContext _context;
        public DesignationController(Model.AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllDesignations()
        {
            //get all designations data
            var designations = _context.Designations.ToList();
            return Ok(designations);
        }
        [HttpGet]
        public IActionResult GetDesignationById(int id)
        {
            var designation = _context.Designations.Find(id);
            return Ok(designation);
        }
        [HttpPost]
        public IActionResult AddDesignation(Model.Designation designation)
        {

            bool exists = _context.Designations.Any(d => d.designationName.ToLower() == designation.designationName.ToLower() && d.departmentId == designation.departmentId);

            if(exists) {
                return Conflict("Designation Already Exists in this Department");
            }

            _context.Add(designation);
            _context.SaveChanges();
            return Ok("Designation Save Successfully");
        }
        [HttpPut]
        public IActionResult UpdateDesignation(Model.Designation designation)
        {
            var desig = _context.Designations.Find(designation.designationId);
            if (desig == null)
            {
                return NotFound("Designation Not Found");
            }
            desig.designationName = designation.designationName;
            desig.departmentId = designation.departmentId;
            _context.SaveChanges();
            return Ok("Designation Updated Successfully");

        }
        [HttpDelete]
        public IActionResult DeleteDesignation(int id)
        {
            var desig = _context.Designations.Find(id);
            if (desig == null)
            {
                return NotFound("Designation Not Found");
            }
            _context.Designations.Remove(desig);
            _context.SaveChanges();
            return Ok("Designation Deleted Successfully");
        }
    }
}
