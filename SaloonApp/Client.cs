using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;

public class Client
{
    private string Firstname;
    private string Lastname;
    private string Email;
    private int ClientId;

    AppUtility Utility = new AppUtility();

    public Client(int inClientId, string inFirstname, string inLastname, string inEmail)
    {
        this.ClientId = inClientId;
        this.Firstname = inFirstname;
        this.Lastname = inLastname;
        this.Email = inEmail;
    }
    public void Save()
    {
        // Serialize this data
        if (!File.Exists(Config.ClientDatabase))
        {
            throw new ArgumentException("File not found");
        }
        StreamWriter writer = new StreamWriter(Config.ClientDatabase, append: true);
        writer.WriteLine($"{ClientId}{Config.SemiColon}" +
            $"Firstname{Config.Colon}{Firstname}{Config.Comma}" +
            $"Lastname{Config.Colon}{Lastname}{Config.Comma}" +
            $"Email{Config.Colon}{Email}");
        writer.Close();
    }
    public void Update()
    {
        List<string> tempClientDatabase = AppUtility.DatabaseExcludingSpecifiedRecord(ClientId, Config.ClientDatabase);
        string updatedClientData = $"{ClientId}{Config.SemiColon}" +
            $"Firstname{Config.Colon}{Firstname}{Config.Comma}" +
            $"Lastname{Config.Colon}{Lastname}{Config.Comma}" +
            $"Email{Config.Colon}{Email}";
        tempClientDatabase.Add(updatedClientData);

        AppUtility.ReplaceFileContent(tempClientDatabase, Config.ClientDatabase);

    }
    public static bool RegisterClient()
    {
        List<string> clientEmailExist = new List<string>();
        string firstnameString = string.Empty, lastnameString = string.Empty, emailString = string.Empty;
        do
        {
            AppUtility.DisplayTitleWithReturn("REGISTER CLIENT", "To abort and return, press x\n");

            firstnameString = AppUtility.GetValidInput("Firstname", typeof(string));
            if (AppUtility.ExitProcessString(firstnameString.ToUpper())) { return true; }

            lastnameString = AppUtility.GetValidInput("Lastname", typeof(string));
            if (AppUtility.ExitProcessString(lastnameString.ToUpper())) { return true; }

            emailString = AppUtility.GetValidInput("Email");
            if (AppUtility.ExitProcessString(emailString.ToUpper())) { return true; }

            try
            {
                clientEmailExist = AppUtility.SearchDatabaseByEmail(emailString, Config.ClientDatabase);
                if (bool.Parse(clientEmailExist.First()) == true)
                {
                    AppUtility.NotifyThenClearScreen("\nDupliate client. Clearing buffer...");
                }
            }
            catch
            {
                Console.WriteLine($"\nUnable to save.\n\n\t Reason: Client Database is missing. \n");
                Thread.Sleep(Config.TimerDuration);
                Console.Clear();
                return true;
            }
        } while (bool.Parse(clientEmailExist.First()) == true);

        Client newClientRecord = new Client(AppUtility.generateId(Config.MinimumIDRange,Config.MaximumIDRange,Config.ClientDatabase), firstnameString, lastnameString, emailString.ToLower());
        try
        {
            newClientRecord.Save();
            AppUtility.NotifyThenClearScreen("\nNew Record Added.\nReturning to Client Menu...");
        }
        catch
        {
            Console.WriteLine($"\nUnable to save.\n\t Reason: Client Database is missing. \n");
        }

        return true;
    }
    public static bool ModifyClient(string action)
    {
        string rawClientData = string.Empty;
        string clientIdString = String.Empty;
        string firstnameString = string.Empty, lastnameString = string.Empty, emailString = string.Empty;
        bool progress = true;
        do
        {
            AppUtility.DisplayTitleWithReturn(action, "To abort and return, press x\n");

            clientIdString = AppUtility.GetValidInput("Client ID", typeof(int));
            if (AppUtility.ExitProcessString(clientIdString.ToUpper())) { return true; }

            try
            {
                rawClientData = AppUtility.SearchDatabaseById(Convert.ToInt32(clientIdString), Config.ClientDatabase);
                if (rawClientData == string.Empty)
                {
                    AppUtility.NotifyThenClearScreen("\nRecord not found. Clearing buffer...");
                }
            }
            catch
            {
                Console.WriteLine($"\nUnable to proceed.\n\n\t Reason: Client Database is missing. \n");
                Thread.Sleep(Config.TimerDuration);
                Console.Clear();
                return true;
            }

        } while (rawClientData == string.Empty);

        AppUtility.DisplayFormattedRecord(rawClientData, "\nRecord Found");

        if (action.Contains("UPDATE"))
        {
            AppUtility.DisplayTitleWithBorder("\nModify");

            do
            {
                firstnameString = AppUtility.GetValidInput("Firstname", typeof(string));
                if (AppUtility.ExitProcessString(firstnameString.ToUpper())) { return true; }

                lastnameString = AppUtility.GetValidInput("Lastname", typeof(string));
                if (AppUtility.ExitProcessString(lastnameString.ToUpper())) { return true; }

                emailString = AppUtility.GetValidInput("Email");
                if (AppUtility.ExitProcessString(emailString.ToUpper())) { return true; }
                progress = false;
            } while (progress);

            Client existingClientRecord = new Client(Convert.ToInt32(clientIdString), firstnameString, lastnameString, emailString);
            existingClientRecord.Update();
            AppUtility.NotifyThenClearScreen("\nRecord Updated.\nReturning to Client Menu...");
        }
        else
        {
            AppUtility.ProcessDeletion(clientIdString, Config.ClientDatabase, "Client");
        }

        return true;
    }

}
