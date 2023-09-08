using EHI_Assignment.Models;
using Microsoft.EntityFrameworkCore;

namespace EHI_Assignment.Repository
{  
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.Include(e => e.Address).ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.Include(e => e.Address).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task CreateEmployeeAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetAllEmployeesAsync(); 
            }

            return await _context.Employees
                .Where(e => e.FirstName.ToLower().Contains(query) || e.LastName.ToLower().Contains(query))
                .Include(e => e.Address).ToListAsync();
        }
    }
}
