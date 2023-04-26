using AuthService.Entities;

namespace AuthService.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? RoleId { get; set; }
    }
}