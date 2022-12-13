using System;

public class Login
{
    private string username;
    private string password;
    private const string adminUsername = "admin";
    private const string adminPassword = "enter";

    public static bool AuthenticateUser(string enteredUsername, string enteredPassword)
    {
        if (enteredUsername == adminUsername && enteredPassword == adminPassword)
        {
            return true;
        }
        return false;
    }
}
