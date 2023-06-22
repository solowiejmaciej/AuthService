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
        UserDto GetById(string id);

        Task<List<UserDto>> GetAll();

        Task DeleteAsync(string id);

        Task<string> AddAsync(UserBodyResponse userDto);
    }

    public class UserService : IUserService
    {
        private readonly UserDbContext _dbContext;
        private readonly ILogger<UserService> _logger;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public UserService(UserDbContext dbContext, ILogger<UserService> logger, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _dbContext = dbContext;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        private ApplicationUser GetUserFromDb(string id)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id.Equals(id));
            if (user == null)
            {
                throw new NotFoundException($"User with {id} not found");
            }
            return user;
        }

        private string GetRoleIdByUserId(string userId)
        {
            try
            {
                return _dbContext.UserRoles.FirstOrDefault(r => r.UserId == userId).RoleId;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return null;
            }
        }

        private string GetRoleNameByRoleId(string roleId)
        {
            try
            {
                return _dbContext.Roles.FirstOrDefault(r => r.Id == roleId).Name;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return null;
            }
        }

        private List<UserDto> GetUsersWithRoles(List<ApplicationUser> users)
        {
            List<UserDto> usersDtos = new List<UserDto>();
            foreach (var user in users)
            {
                string? roleId = GetRoleIdByUserId(user.Id);
                string? roleName = GetRoleNameByRoleId(roleId);
                usersDtos.Add(new UserDto
                {
                    Id = user.Id,
                    RoleId = roleId,
                    RoleName = roleName,
                    Email = user.Email,
                    DeviceId = user.DeviceId,
                });
            }
            return usersDtos;
        }

        public UserDto GetById(string id)
        {
            var user = GetUserFromDb(id);
            var roleId = GetRoleIdByUserId(user.Id);
            var roleName = GetRoleNameByRoleId(roleId);
            var userDto = new UserDto
            {
                Id = user.Id,
                RoleId = roleId,
                RoleName = roleName,
                Email = user.Email,
                DeviceId = user.DeviceId,
            };
            return userDto;
        }

        public async Task<List<UserDto>> GetAll()
        {
            var allUsers = await _dbContext.Users.ToListAsync();
            var allUsersDtos = GetUsersWithRoles(allUsers);
            return allUsersDtos;
        }

        public async Task DeleteAsync(string id)
        {
            var userToDelete = GetUserFromDb(id);
            _dbContext.Users.Remove(userToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> AddAsync(UserBodyResponse userBodyResponse)
        {
            var newUser = new ApplicationUser
            {
                Email = userBodyResponse.Email,
                UserName = userBodyResponse.Email,
                NormalizedEmail = userBodyResponse.Email.ToUpper(),
                NormalizedUserName = userBodyResponse.Email.ToUpper()
            };

            var hashedPass = _passwordHasher.HashPassword(newUser, userBodyResponse.Password);
            newUser.PasswordHash = hashedPass;

            var userInDb = await _dbContext.Users.AddAsync(newUser);
            var userRoleId = _dbContext.Roles.FirstOrDefault(r => r.Name == "User").Id;
            _dbContext.UserRoles.AddAsync(new()
            {
                RoleId = userRoleId,
                UserId = userInDb.Entity.Id
            });

            await _dbContext.SaveChangesAsync();

            return userInDb.Entity.Id;
        }
    }
}