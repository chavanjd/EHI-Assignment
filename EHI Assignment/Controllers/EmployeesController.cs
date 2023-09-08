using EHI_Assignment.Models;
using EHI_Assignment.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHI_Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            var employeeDtos = employees.Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Age = e.Age,
                Address = new AddressDto
                {
                    Street = e.Address.Street,
                    City = e.Address.City,
                    State = e.Address.State
                }
            });
            return Ok(employeeDtos);
        }

        private EmployeeDto MapToEmployeeDto(Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Age = employee.Age,
                Address = new AddressDto
                {
                    Street = employee.Address.Street,
                    City = employee.Address.City,
                    State = employee.Address.State
                }
            };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var employeeDto = MapToEmployeeDto(employee); 
            return Ok(employeeDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest("Invalid employee data.");
            }

            var existingEmployee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            existingEmployee.FirstName = employeeDto.FirstName;
            existingEmployee.LastName = employeeDto.LastName;
            existingEmployee.Email = employeeDto.Email;
            existingEmployee.Age = employeeDto.Age;

            await _employeeRepository.UpdateEmployeeAsync(existingEmployee);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            await _employeeRepository.DeleteEmployeeAsync(id);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest("Invalid employee data.");
            }

            if (string.IsNullOrWhiteSpace(employeeDto.FirstName) )
            {
                return BadRequest("Invalid employee data. FirstName is required.");
            }
            if (string.IsNullOrWhiteSpace(employeeDto.LastName) )
            {
                return BadRequest("Invalid employee data. LastName is required.");
            }
            if (string.IsNullOrWhiteSpace(employeeDto.Email))
            {
                return BadRequest("Invalid employee data. Email is required.");
            }
            await Task.Delay(2000);
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            if (employees.Any(e =>
           e.FirstName == employeeDto.FirstName &&
           e.LastName == employeeDto.LastName &&
           e.Email == employeeDto.Email))
            {
                return Conflict("An employee with the same first name, last name, and email address already exists.");
            }

            var employee = new Employee
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                Age = employeeDto.Age
            };

            await _employeeRepository.CreateEmployeeAsync(employee);

            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> SearchEmployees([FromQuery] string query)
        {
            await Task.Delay(2000);
            var employees = await _employeeRepository.SearchEmployeesAsync(query);

            var employeeDtos = employees.Select(MapToEmployeeDto);

            return Ok(employeeDtos);
        }
    }
}
