using System;
using BCrypt.Net;

class Program
{
    static void Main()
    {
        string password = "Admin123!";
        string hash = BCrypt.HashPassword(password);
        Console.WriteLine($"Password: {password}");
        Console.WriteLine($"Hash: {hash}");
    }
}
