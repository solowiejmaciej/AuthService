﻿using AuthService.Entities;
using AuthService.Models;
using AuthService.Services;
using Microsoft.EntityFrameworkCore;

namespace AuthService
{
    public class Seeder
    {
        private readonly UserDbContext _dbContext;
        private readonly IUserService _userService;

        public Seeder(UserDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                var pendingMigrations = _dbContext.Database.GetPendingMigrations();

                if (pendingMigrations != null && pendingMigrations.Any())
                {
                    _dbContext.Database.Migrate();
                }

                if (!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }
                //To refactor while adding UserService
                if (!_dbContext.Users.Any())
                {
                    var User = new UserBodyResponse()
                    {
                        Login = "cwsuser",
                        Password = "string"
                    };
                    _userService.AddAsync(User);
                    var createdUser = _dbContext.Users.FirstOrDefault(u => u.Login == User.Login);
                    createdUser.RoleId = 2;
                    _dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new ()
                {
                    Name = "User"
                },
                new ()
                {
                    Name = "Admin"
                }
            };

            return roles;
        }
    }
}