namespace InstaMenu.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid merchantId, string name);
    }

}
