namespace API.Helpers
{
    public class TokenService
    {
        private readonly IRandomNumberGeneratorService _randomNumberGeneratorService;

        // Constructor Injection
        public TokenService(IRandomNumberGeneratorService randomNumberGeneratorService)
        {
            _randomNumberGeneratorService = randomNumberGeneratorService;
        }

        // Instance method to generate the refresh token
        public string GenerateRefreshToken()
        {
            var randomBytes = _randomNumberGeneratorService.GenerateBytes(64);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
