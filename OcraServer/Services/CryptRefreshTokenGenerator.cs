using System;
using System.Security.Cryptography;

namespace OcraServer.Services
{
    public class CryptRefreshTokenGenerator : IRefreshTokenGenerator
    {
        public string GenerateRefreshToken()
        {
			byte[] randomData = new byte[150];
            string result;
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomData);
				result = Convert.ToBase64String(randomData);
			}

            long ticks = DateTime.UtcNow.Ticks;
            byte[] ticksArr = BitConverter.GetBytes(ticks);
            string ticksString = Convert.ToBase64String(ticksArr);

            return result + "|" +  ticksString;
        }
    }
}
