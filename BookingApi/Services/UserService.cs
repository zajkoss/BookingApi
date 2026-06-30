using AutoMapper;
using BookingApi.Commands;
using BookingApi.DTOs;
using BookingApi.Exceptions;
using BookingApi.Models;
using BookingApi.Repository;

namespace BookingApi.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;
    
    public UserService(ILogger<UserService> logger, IUserRepository userRepository, IMapper mapper, IJwtService jwtService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    public async Task<UserDto> RegisterAsync(RegisterUserCommand command)
    {
        _logger.LogInformation("Registering user {Email}",command.Email);
        var byEmailAsync = await _userRepository.GetByEmailAsync(command.Email);

        if (byEmailAsync != null)
        {
            _logger.LogInformation("User with email {Email} already exists",command.Email);
            throw new ConflictException("Email already exists");
        }
        
        var newUser = new User
        {
            Email = command.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
            FirstName = command.FirstName,
            LastName = command.LastName,
            IsActive = true
        };
        
        var createdUser = await _userRepository.CreateAsync(newUser);
        _logger.LogInformation("User {Email} registered",newUser.Email);
        var user = _mapper.Map<User, UserDto>(createdUser);
        return user;
    }

    public async Task<string> LoginAsync(LoginUserCommand command)
    {
        _logger.LogInformation("Logining user {Email}", command.Email);
        var user = await _userRepository.GetByEmailAsync(command.Email);
        if (user == null) throw new UnauthorizedException("Invalid credentials");
        var verify = BCrypt.Net.BCrypt.Verify(command.Password,user.PasswordHash);
        if (!verify) throw new UnauthorizedException("Invalid credentials");
        return _jwtService.GenerateToken(user);
    }
}