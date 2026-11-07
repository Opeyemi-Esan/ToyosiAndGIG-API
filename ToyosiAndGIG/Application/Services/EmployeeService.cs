using System.Net;
using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Application.Interfaces;
using ToyosiAndGIG.Application.Models;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAccountService _accountService;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IIdentityService identityService,
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            IAccountService accountService,
            ILogger<EmployeeService> logger)
        {
            _identityService = identityService;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _accountService = accountService;
            _logger = logger;
        }


        public async Task<Response<Guid>> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto, string createdBy)
        {
            var request = new CreateUserRequest
            {
                FirstName = createEmployeeDto.FirstName,
                LastName = createEmployeeDto.LastName,
                Email = createEmployeeDto.Email,
                UserName = createEmployeeDto.Email, // no Username field on CreateEmployeeDto, default to email
                PhoneNumber = createEmployeeDto.PhoneNumber,
                Password = createEmployeeDto.Password,
                Role = createEmployeeDto.Role,
                IsAdminAction = true // allows Member/Manager/Owner, bypasses public-only restriction
            };

            var identityResult = await _identityService.RegisterAsync(request);

            if (identityResult.StatusCode != HttpStatusCode.Created)
            {
                _logger.LogWarning("Employee account creation failed for {Email}: {Message}", createEmployeeDto.Email, identityResult.Message);
                return Response<Guid>.Fail(identityResult.Message, identityResult.StatusCode);
            }

            var user = await _userRepository.GetUserByIdAsync(identityResult.Data);

            var employee = new Employee
            {
                UserId = user.Id,
                Position = createEmployeeDto.Position,
                Role = createEmployeeDto.Role,
                Department = createEmployeeDto.Department,
                DateOfEmployment = createEmployeeDto.DateOfEmployment,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            await _employeeRepository.CreateAsync(employee);
            await _accountService.SendEmailConfirmationOtpAsync(createEmployeeDto.Email);

            _logger.LogInformation("Employee created: user {UserId}, position {Position}", user.Id, createEmployeeDto.Position);
            return Response<Guid>.Success(user.Id, "Employee created. Confirmation email sent.", HttpStatusCode.Created);
        }
    }
}
