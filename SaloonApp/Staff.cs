using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

public class Staff
{
    private string Firstname;
    private string Lastname;
    private string Email;
    private string TeamGroup;
    private int StaffId;

    private static Dictionary<int, string> TeamGroupNames = Config.TeamNames;
    public Staff(int inStaffId, string inFirstname, string inLastname, string inTeamGroup, string inEmail)
    {
        StaffId = inStaffId;
        Firstname = inFirstname;
        Lastname = inLastname;
        Email = inEmail;
        TeamGroup = inTeamGroup;
    }
    public void Save()
    {
        // Serialize this data
        StreamWriter writer = new StreamWriter(Config.StaffDatabase, append: true);
        writer.WriteLine($"{StaffId}{Config.SemiColon}" +
            $"Firstname{Config.Colon}{Firstname}{Config.Comma}" +
            $"Lastname{Config.Colon}{Lastname}{Config.Comma}" +
            $"TeamGroup{Config.Colon}{TeamGroup}{Config.Comma}" +
            $"Email{Config.Colon}{Email}");
        writer.Close();
    }
    public void Update()
    {
        List<string> tempStaffDatabase = AppUtility.DatabaseExcludingSpecifiedRecord(StaffId, Config.StaffDatabase);
        string updatedStaffData = $"{StaffId}{Config.SemiColon}" +
            $"Firstname{Config.Colon}{Firstname}{Config.Comma}" +
            $"Lastname{Config.Colon}{Lastname}{Config.Comma}" +
            $"TeamGroup{Config.Colon}{TeamGroup}{Config.Comma}" +
            $"Email{Config.Colon}{Email}";
        tempStaffDatabase.Add(updatedStaffData);

        AppUtility.ReplaceFileContent(tempStaffDatabase, Config.StaffDatabase);

    }
    public static bool AddStaff(string action)
    {
        List<string> StaffEmailExist = new List<string>();
        string firstnameString = string.Empty, lastnameString = string.Empty, teamGroupString = string.Empty, emailString = string.Empty;
        do
        {
            AppUtility.DisplayTitleWithReturn(action, "To abort and return, press x\n");

            firstnameString = AppUtility.GetValidInput("Firstname", typeof(string));
            if (AppUtility.ExitProcessString(firstnameString.ToUpper())) { return true; }

            lastnameString = AppUtility.GetValidInput("Lastname", typeof(string));
            if (AppUtility.ExitProcessString(lastnameString.ToUpper())) { return true; }

            // Display Team Names
            AppUtility.DisplayTitleWithBorder("\nTeam Groups");

            AppUtility.DisplayNumberedMenu(TeamGroupNames);

            teamGroupString = AppUtility.GetValidMenuSelection("Select Group No", TeamGroupNames);
            if (AppUtility.ExitProcessString(teamGroupString.ToUpper())) { return true; }

            Console.WriteLine($"\nTeam {TeamGroupNames[Convert.ToInt32(teamGroupString)]} selected.\n");

            emailString =  AppUtility.GetValidInput("Email");
            if (AppUtility.ExitProcessString(emailString.ToUpper())) { return true; }

            try
            {
                StaffEmailExist = AppUtility.SearchDatabaseByEmail(emailString, Config.StaffDatabase);
                if (bool.Parse(StaffEmailExist.First()) == true)
                {
                    AppUtility.NotifyThenClearScreen("\nDupliate staff. Clearing buffer...");
                }
            }
            catch
            {
                Console.WriteLine($"\nUnable to proceed.\n\n\t Reason: Staff Database is missing. \n");
                Thread.Sleep(Config.TimerDuration);
                Console.Clear();
                return true;
            }
        } while (bool.Parse(StaffEmailExist.First()) == true);

        Staff newStaffRecord = new Staff(AppUtility.generateId(Config.MinimumIDRangeStaff, Config.MaximumIDRangeStaff, Config.StaffDatabase), firstnameString, lastnameString, TeamGroupNames[Convert.ToInt32(teamGroupString)], emailString.ToLower());
        newStaffRecord.Save();
        AppUtility.NotifyThenClearScreen($"\nNew Staff created and Added to Team {TeamGroupNames[Convert.ToInt32(teamGroupString)]}.\nReturning to Staff Menu...");

        return true;
    }
    public static bool ModifyStaff(string action)
    {
        string rawStaffData = string.Empty;
        string staffIdString = string.Empty;
        string firstnameString = string.Empty, lastnameString = string.Empty, teamGroupString = string.Empty, emailString = string.Empty;
        bool progress = true;
        do
        {
            AppUtility.DisplayTitleWithReturn(action, "To abort and return, press x\n");

            staffIdString = AppUtility.GetValidInput("Staff ID", typeof(int));
            if (AppUtility.ExitProcessString(staffIdString.ToUpper())) { return true; }

            try
            {
                rawStaffData = AppUtility.SearchDatabaseById(Convert.ToInt32(staffIdString), Config.StaffDatabase);
                if (rawStaffData == string.Empty)
                {
                    AppUtility.NotifyThenClearScreen("\nRecord not found. Clearing buffer...");
                }
            }
            catch
            {
                Console.WriteLine($"\nUnable to save.\n\n\t Reason: Staff Database is missing. \n");
                Thread.Sleep(Config.TimerDuration);
                Console.Clear();
                return true;
            }

        } while (rawStaffData == string.Empty);

        AppUtility.DisplayFormattedRecord(rawStaffData, "\nRecord Found");

        if (action.Contains("UPDATE"))
        {
            AppUtility.DisplayTitleWithBorder("\nModify");

            do
            {
                firstnameString = AppUtility.GetValidInput("Firstname", typeof(string));
                if (AppUtility.ExitProcessString(firstnameString.ToUpper())) { return true; }

                lastnameString = AppUtility.GetValidInput("Lastname", typeof(string));
                if (AppUtility.ExitProcessString(lastnameString.ToUpper())) { return true; }

                // LOAD TEAM NAMES in do while, expecting an input
                AppUtility.DisplayTitleWithBorder("\nTeam Groups");

                AppUtility.DisplayNumberedMenu(TeamGroupNames);

                teamGroupString = AppUtility.GetValidMenuSelection("Select Group No", TeamGroupNames);
                if (AppUtility.ExitProcessString(teamGroupString.ToUpper())) { return true; }

                Console.WriteLine($"\nTeam {TeamGroupNames[Convert.ToInt32(teamGroupString)]} selected.\n");

                emailString = AppUtility.GetValidInput("Email");
                if (AppUtility.ExitProcessString(emailString.ToUpper())) { return true; }
                progress = false;
            } while (progress);

            Staff existingStaffRecord = new Staff(Convert.ToInt32(staffIdString), firstnameString, lastnameString, TeamGroupNames[Convert.ToInt32(teamGroupString)], emailString);
            existingStaffRecord.Update();
            AppUtility.NotifyThenClearScreen("\nRecord Updated.\nReturning to Staff Menu...");
        }
        else
        {
            AppUtility.ProcessDeletion(staffIdString, Config.StaffDatabase, "Staff");
        }



        return true;
    }
    public static bool TrackAvailability(String action) {

        //int staffId, DateTime referenceDate
        string rawStaffData = string.Empty;
        string staffIdString = string.Empty;
        string appointmentDateString = string.Empty; 

        int daysToAppointment = 0;

        do
        {
            AppUtility.DisplayTitleWithReturn(action, "To abort and return, press x\n");

            staffIdString = AppUtility.GetValidInput("Enter Staff ID", typeof(int));           
            if (AppUtility.ExitProcessString(staffIdString.ToUpper())) { return true; }

            try
            {
                rawStaffData = AppUtility.SearchDatabaseById(Convert.ToInt32(staffIdString), Config.StaffDatabase);
                if (rawStaffData == string.Empty)
                {
                    AppUtility.NotifyThenClearScreen("\nRecord not found. Clearing buffer...");
                }
            }
            catch
            {
                Console.WriteLine($"\nUnable to proceed.\n\n\t Reason: Staff Database is missing. \n");
                Thread.Sleep(Config.TimerDuration);
                Console.Clear();
                return true;
            }

        } while (rawStaffData == string.Empty);

        Dictionary<string, string> staffData = AppUtility.FormatRawRecordData(rawStaffData);
        string teamStaffBelongsTo = staffData["TeamGroup"];
        do
        {
            appointmentDateString = AppUtility.GetValidDate("Date of Appointment (yyyy/mm/dd)");
            if (AppUtility.ExitProcessString(appointmentDateString.ToUpper())) { return true; }

            DateTime appointmentDate = DateTime.Parse(appointmentDateString);
            daysToAppointment = AppUtility.GetTimeSpanToAppointment(Config.ShiftStarted, appointmentDate);

            AppUtility.TrackingResult newTrackingResult =  AppUtility.TrackAvailability(daysToAppointment, teamStaffBelongsTo);

            do
            {
                if (newTrackingResult.TeamOnDuty != teamStaffBelongsTo)
                {
                    AppUtility.DisplayTitleWithBorder("\nTracking Result");
                    Console.WriteLine($"Staff with ID:{staffIdString} (Team {teamStaffBelongsTo}), is not on duty today." +
                        $"\nTeam {teamStaffBelongsTo} resumes in {newTrackingResult.NextTeamResumesIn} " +
                        $"working {(newTrackingResult.NextTeamResumesIn > 1 ? "days" : "day")}" +
                        $"\n{newTrackingResult.TeamOnDuty} is on their {newTrackingResult.ShiftNumber} shift.\n\n");
                }
                else
                {
                    AppUtility.DisplayTitleWithBorder("\nTracking Result");
                    Console.WriteLine("Staff with ID: {0} belongs to (Team {1}) \n" + teamStaffBelongsTo + " is on their {2} shift.\n\n", staffIdString,teamStaffBelongsTo, newTrackingResult.ShiftNumber);
                    // Load booking details for this client for the specifed date of appointment
                }

                Console.Write("Press any key to return to Staff Menu ");
                Console.ReadKey();
                Console.WriteLine();
                AppUtility.ExitProcessString(Config.ExitWithX.ToString());

            } while (false);


        } while (daysToAppointment == Config.EmptyInteger);


        return true;
    }
}