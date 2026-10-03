namespace vet_api_Net.DTOs;

public record LoginRequest(string Email, string Password);
public record UpdatePasswordRequest(string Email, string OldPassword, string NewPassword);