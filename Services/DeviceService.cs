using AuthService.UserContext;
using AuthService.Entities;
using AuthService.Exceptions;
using Microsoft.EntityFrameworkCore;
using AuthService.Models;
using AutoMapper;

namespace AuthService.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly UserDbContext _dbContext;
        private readonly ILogger<UserService> _logger;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public DeviceService(UserDbContext dbContext, ILogger<UserService> logger, IUserContext userContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userContext = userContext;
            _mapper = mapper;
        }

        public async Task AssignDeviceIdToUserAsync(string deviceId)
        {
            var currentUser = _userContext.GetCurrentUser();
            var dbUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == currentUser.Id);
            if (dbUser is null) throw new NotFoundException("user: " + currentUser.Id + "not found");
            dbUser.DeviceId = deviceId;
            await _dbContext.SaveChangesAsync();
        }
    }

    public interface IDeviceService
    {
        Task AssignDeviceIdToUserAsync(string deviceId);
    }
}