using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee> CreateAsync(Employee employee);
        Task<Employee> GetByUserIdAsync(Guid userId);
        Task<List<Employee>> GetAllAsync();
        Task<Employee> UpdateAsync(Employee employee);
    }
}
