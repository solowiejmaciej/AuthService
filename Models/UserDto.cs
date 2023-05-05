using AuthService.Entities;

namespace AuthService.Models
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string? RoleId { get; set; }
        public string RoleName { get; set; }
    }
}