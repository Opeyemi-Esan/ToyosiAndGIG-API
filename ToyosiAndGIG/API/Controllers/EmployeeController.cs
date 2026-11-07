using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Application.Interfaces;
using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{Roles.Owner}, {Roles.Manager}")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IIdentityService _identityService;

        public EmployeeController(IEmployeeService employeeService, IIdentityService identityService)
        {
            _employeeService = employeeService;
            _identityService = identityService;
        }


        [HttpPost("employee_registration")]
        public async Task<IActionResult> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto)
        {
            var createdBy = User.FindFirstValue(ClaimTypes.Email);
            var response = await _employeeService.CreateEmployeeAsync(createEmployeeDto, createdBy);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var response = await _identityService.GetAllRolesAsync();
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
