namespace cosmeticClinic.Settings;

public class PasswordSettings
{
    private const int WorkFactor = 12; 
    private const int MinPasswordLength = 8;

    public string HashPassword(string password)
    {
        ValidatePassword(password);
        
        // BCrypt automatically:
        // 1. Generates a unique salt per password
        // 2. Combines the salt with the password
        // 3. Performs the hashing with the specified work factor
        // 4. Returns a string containing the algorithm version, work factor, salt, and hash
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            return false;

        try
        {
            // BCrypt.Verify:
            // 1. Extracts the salt from the stored hash
            // 2. Hashes the input password with the extracted salt
            // 3. Performs a time-constant comparison of the hashes
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            return false;
        }
    }

    private void ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password), "Password cannot be null or empty");

        if (password.Length < MinPasswordLength)
            throw new ArgumentException($"Password must be at least {MinPasswordLength} characters long");
    }

}