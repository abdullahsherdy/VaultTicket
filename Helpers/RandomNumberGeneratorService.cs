using System.Security.Cryptography;

namespace API.Helpers
{
    public class RandomNumberGeneratorService : IRandomNumberGeneratorService
    {
        public byte[] GenerateBytes(int length)
        {
            var randomBytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return randomBytes;
        }
    }
}
