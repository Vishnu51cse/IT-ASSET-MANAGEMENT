using Backend.Models;

namespace Backend.Helpers
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, Employee? employee);
    }
}
