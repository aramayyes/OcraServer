using System;
namespace OcraServer.Services
{
    public interface IRefreshTokenGenerator
    {
        string GenerateRefreshToken();
    }
}
