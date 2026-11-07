using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<Response<Guid>> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto, string createdBy);
    }
}
