using System;
using static BCrypt.Net.BCrypt;

class Program
{
    static void Main()
    {
        string password = "Admin123!";
        string hash = HashPassword(password);
        Console.WriteLine($"Password: {password}");
        Console.WriteLine($"Hash: {hash}");
        
        // Verify the hash
        bool isValid = Verify(password, hash);
        Console.WriteLine($"Verification: {isValid}");
    }
}
