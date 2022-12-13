using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading;

public class Booking
{
    private string Firstname;
    private string Lastname;
    private string Email;
    private int ClientId;

    public static bool Book(string action)
    {

        List<string> rawClientData = new List<string>();
        string clientEmailString = String.Empty;
        string appointmentDateString = string.Empty;
        string appointmentTimeString = string.Empty;
        int daysToAppointment = 0;
        bool progress = false;

        do
        {
            AppUtility.DisplayTitleWithReturn(action, "To abort and return, press x\n");

            clientEmailString = AppUtility.GetValidInput("Client Email");
            if (AppUtility.ExitProcessString(clientEmailString.ToUpper())) { return true; }

            try
            {
                rawClientData = AppUtility.SearchDatabaseByEmail(clientEmailString, Config.ClientDatabase);
                if (bool.Parse(rawClientData.First()) == false)
                {
                    AppUtility.NotifyThenClearScreen("\nRecord not found. Please register");
                    return false;
                }
            }
            catch
            {
                Console.WriteLine($"\nUnable to proceed.\n\n\t Reason: Client Database is missing. \n");
                Thread.Sleep(Config.TimerDuration);
                Console.Clear();
                return true;
            }

        } while (bool.Parse(rawClientData.First()) == false);

        AppUtility.DisplayFormattedRecord(rawClientData.Last(), "\nRecord Found");

        Console.WriteLine(); 

        do
        {
            appointmentDateString = AppUtility.GetValidDate("Date of Appointment (yyyy/mm/dd)");
            if (AppUtility.ExitProcessString(appointmentDateString.ToUpper())) { return true; }

            appointmentTimeString = AppUtility.GetValidTime("Time of Appointment (hh:mm)");
            if (AppUtility.ExitProcessString(appointmentDateString.ToUpper())) { return true; }

            Dictionary<string, string> staffAvailable =
                AppUtility.GetBookingAvailability(DateTime.Parse(appointmentDateString), DateTime.Parse(appointmentTimeString));

            foreach (KeyValuePair<string,string> staffInfo in staffAvailable)
            {
                Console.WriteLine($"{staffInfo.Key} {staffInfo.Value}");
            }

            Console.ReadKey();


            // Load available staff
               // load time members available for that time

            // if none are available, load the other times they're available

            // or 
            // if no team member is available for that time, just show fully booked
            // then request new time from client




            /*DateTime appointmentDate = DateTime.Parse(appointmentDateString);
            daysToAppointment = AppUtility.GetTimeSpanToAppointment(Config.ShiftStarted, appointmentDate);

            AppUtility.TrackingResult newTrackingResult = AppUtility.TrackAvailability(daysToAppointment);*/       


        } while (daysToAppointment == Config.EmptyInteger);

        return true;
    }
    public static void Modify(string action) { }
    public static void MakePayment(string action) { }


}