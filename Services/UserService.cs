using AuthService.Entities;
using AuthService.Exceptions;
using AuthService.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
    public interface IUserService
    {
        UserDto GetById(int id);

        Task<List<UserDto>> GetAll();

        Task DeleteAsync(int id);

        Task<int> AddAsync(UserBodyResponse userDto);
    }

    public class UserService : IUserService
    {
        private readonly UserDbContext _dbContext;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(UserDbContext dbContext, ILogger<UserService> logger, IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        private User GetUserFromDb(int id)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                throw new NotFoundException($"User with {id} not found");
            }
            return user;
        }

        public UserDto GetById(int id)
        {
            var user = GetUserFromDb(id);
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task<List<UserDto>> GetAll()
        {
            var allUsers = await _dbContext.Users.ToListAsync();
            var allUsersDtos = _mapper.Map<List<UserDto>>(allUsers);
            return allUsersDtos;
        }

        public async Task DeleteAsync(int id)
        {
            var userToDelete = GetUserFromDb(id);
            _dbContext.Users.Remove(userToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> AddAsync(UserBodyResponse userBodyResponse)
        {
            var newUser = new User
            {
                Login = userBodyResponse.Login
            };

            var hashedPass = _passwordHasher.HashPassword(newUser, userBodyResponse.Password);
            newUser.PasswordHashed = hashedPass;

            var userInDb = await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            return userInDb.Entity.Id;
        }
    }
}