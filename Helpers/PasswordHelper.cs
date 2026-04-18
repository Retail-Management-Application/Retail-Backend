namespace RetailOrdering.API.Helpers;

public static class PasswordHelper
{
    private const int WorkFactor = 12;

    public static string HashPassword(string plainText)
        => BCrypt.Net.BCrypt.HashPassword(plainText, WorkFactor);

    public static bool VerifyPassword(string plainText, string hash)
        => BCrypt.Net.BCrypt.Verify(plainText, hash);
}
