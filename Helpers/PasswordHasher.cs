using BCrypt.Net;

namespace API.Helpers
{
    public class PasswordHasher
    {


        /// <summary>
        ///  Return a hashed password using BCrypt.Net.BCrypt.HashPassword
        ///  and use algorithm version 8Crypt by default.
        /// </summary>
        /// <param name="password">Plain-Text Password Provided by the user</param>
        /// <returns></returns>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        ///The function calls BCrypt.Net.BCrypt.Verify, which internally:
	    ///Extracts the salt embedded in the hashedPassword.
	    ///Re-hashes the provided password using the same salt and algorithm.
        ///Compares the resulting hash with the hashedPassword.
        /// </summary>
        /// <param name="password">Plain-Text Password provided by the user</param>
        /// <param name="hashedPassword">The hashed password stored in the system.</param>
        /// <returns></returns>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
