namespace CalDAV.Models;

/// <summary>
/// Represents the credentials needed to connect to a CalDAV server
/// </summary>
public class CalDAVCredentials
{
    public string ServerUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public CalDAVCredentials(string serverUrl, string username, string password)
    {
        ServerUrl = serverUrl;
        Username = username;
        Password = password;
    }
}