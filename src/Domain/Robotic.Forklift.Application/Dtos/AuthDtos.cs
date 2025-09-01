namespace Robotic.Forklift.Application.Dtos
{
    public record LoginRequest(string Username, string Password);
    public record LoginResponse(string Token, DateTime ExpiresAt);
}