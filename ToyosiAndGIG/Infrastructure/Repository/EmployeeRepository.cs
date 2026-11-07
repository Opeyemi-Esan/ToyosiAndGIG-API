using Microsoft.EntityFrameworkCore;
using ToyosiAndGIG.Application.Interfaces;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Infrastructure.Data;

namespace ToyosiAndGIG.Infrastructure.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Employee> CreateAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> GetByUserIdAsync(Guid userId)
        {
             var employee = await _context.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.UserId == userId);
            return employee;
        }

        public Task<List<Employee>> GetAllAsync()
        {
            var employees = _context.Employees.Include(e => e.User).ToListAsync();
            return employees;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            var existing = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id);
            if (existing == null) return null;

            existing.Position = employee.Position;
            existing.Department = employee.Department;

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
