using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.BLL.DTOs.Employee;
using ProjectManager.BLL.Services;
using ProjectManager.BLL.Services.Employee;

namespace ProjectManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

        // GET: api/employees/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeDto>> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if(employee == null)
            {
                return NotFound(new { Message = $"Employee with ID {id} not found." });
            }
            return Ok(employee);
        }

        // GET: api/employees/search?term=Ivan
        // Requirements check: AJAX endpoint for partial text search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> Search([FromQuery] string term)
        {
            var results = await _employeeService.SearchAsync(term);
            return Ok(results);
        }

        // POST: api/employees
        [HttpPost]
        [Authorize(Roles = "Director")]
        public async Task<ActionResult<EmployeeCreatedResponseDto>> Create([FromBody] EmployeeCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var createdResponse = await _employeeService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new
                {
                    id = createdResponse.Employee.Id
                }, createdResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // PUT: api/employees/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto dto)
        {
            if(id != dto.Id)
            {
                return BadRequest(new
                {
                    Message = "ID in route does not match ID in body."
                });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _employeeService.UpdateAsync(dto);
                // 204 No Content is standard for successful updates
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if(employee == null)
            {
                return NotFound(new { Message = $"Employee with ID {id} not found." });
            }

            await _employeeService.DeleteAsync(id);
            return NoContent();
        }
    }
}
