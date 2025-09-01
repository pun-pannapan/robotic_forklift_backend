using Robotic.Forklift.Application.Dtos;

namespace Robotic.Forklift.Application.Interfaces
{
    public interface IJwtTokenService
    {
        LoginResponse CreateToken(int userId, string username);
    }
}
