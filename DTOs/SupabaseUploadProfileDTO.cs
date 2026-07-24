using Microsoft.AspNetCore.Http;

namespace DTOs;
public class UpdateProfileDto
{
    public string Username { get; set; } = string.Empty;
    public IFormFile? Avatar { get; set; }
    public int? UserId { get; set; }
}