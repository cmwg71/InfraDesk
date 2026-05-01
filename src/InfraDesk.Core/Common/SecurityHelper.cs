// Dateipfad: src/InfraDesk.Core/Common/SecurityHelper.cs
using System;
using System.Security.Cryptography;
using System.Text;

namespace InfraDesk.Core.Common;

/// <summary>
/// Hilfsklasse für kryptografische Operationen.
/// Sorgt dafür, dass Passwörter niemals im Klartext in der Datenbank landen.
/// </summary>
public static class SecurityHelper
{
    // Ein statischer Salt für unser Hashing (in einer echten Prod-Umgebung sollte dies aus den AppSettings kommen)
    private const string Salt = "InfraDesk_Enterprise_SecureSalt_V1!";

    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password)) return string.Empty;

        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + Salt));
        return Convert.ToBase64String(bytes);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}