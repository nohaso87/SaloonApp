
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace SaloonApp
{
    internal class Program
    {
        const string SaloonName = "SALOON APP";
        const string SelectionToBeginApp = "1";
        const string SelectionToExitApp = "2";

        const string FailedAttemptString = "Invalid credentials, please try again";
        const string LastFailedAttemptString = "Invalid credentials, this is your last attempt";
        const string ClosingApplication = "\nApplication is closing...";
        const string LoggingOut = "\nLogging out...";
        const string InvalidRetryString = "Invalid input, retry in 3 seconds...";
        const string AdminCredentialRequestString = "Enter admin credentials.";
        const string SelectionStringForApp = "Press 1 to Login and 2 to exit application: ";

        static void Main(string[] args)
        {
            
            bool startApplication = true;

            //try
            //{
                Config configurations = new Config();
            //}
            //catch
            //{
            //    startApplication = false;
            //}

            //if (startApplication == true)
            //{

                bool progressToMainMenu = true;
                do
                {
                    DisplayCompanyName();

                    progressToMainMenu = AccessNavigation();
                    if (!progressToMainMenu)
                    {
                        Console.WriteLine(ClosingApplication);
                        Thread.Sleep(Config.TimerDuration);
                        break;
                    }
                    else
                    {
                        bool loggedIn = true;
                        do
                        {
                            loggedIn = HomePage();
                        } while (loggedIn);
                        Console.WriteLine(LoggingOut);
                        Thread.Sleep(Config.TimerDuration);
                        Console.Clear();
                    }
                } while (true);
            //}
            //else
            //{
            //    Console.WriteLine("Application failed to start. \n\n\t Reason: Configuration file not found.\n\n\n");
            //}
        }

        static void DisplayCompanyName()
        {
            Console.Title = SaloonName;
        }
        static bool AccessNavigation()
        {
            do
            {
                Console.Write(SelectionStringForApp);
                switch (Console.ReadLine())
                {
                    case SelectionToBeginApp:
                        Console.Clear();
                        bool isUserValid = AuthenticationLogic();
                        return isUserValid;
                    case SelectionToExitApp:
                        Console.Clear();
                        return false;
                    default:
                        Console.WriteLine(InvalidRetryString);
                        Thread.Sleep(Config.TimerDuration);
                        Console.Clear();
                        continue;
                }
            } while (true);

        }
        static List<string> InputString(string loginStatus)
        {
            Console.Write(loginStatus == string.Empty ? string.Empty : loginStatus);
            Console.Write("\nUsername: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            var credentials = new List<string>();
            credentials.Add(username);
            credentials.Add(password);

            return credentials;
        }
        static bool AuthenticationLogic()
        {
            bool validUser = true;
            int loginAttempts = Config.LoginAttemptLimit - Config.LoginAttemptLimit;
            string loginStatus = AdminCredentialRequestString;

            do
            {
                if (loginAttempts > Config.LoginAttemptLimit)
                {
                    validUser = false;
                    break;
                }
                else if (loginAttempts == Config.LoginAttemptLimit)
                {
                    loginStatus = LastFailedAttemptString;
                }
                var userCredentials = InputString(loginStatus);
                validUser = Login.AuthenticateUser(userCredentials.First(), userCredentials.Last());
                loginStatus = FailedAttemptString;
                loginAttempts++;
                Console.Clear();
            } while (!validUser);
            return validUser;
        }
        static bool HomePage()
        {
            Console.Title = "SALOON APP | Home";

            Console.WriteLine("MENU");
            Console.WriteLine("----");
            Console.WriteLine("");
            Console.WriteLine("1. Client Section");
            Console.WriteLine("2. Booking Section");
            Console.WriteLine("3. Staff Section");
            Console.WriteLine("4. View Dashboard");
            Console.WriteLine("5. Logout");
            Console.Write("\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Clear();
                    ClientNavPage();
                    return true;
                case "2":
                    Console.Clear();
                    BookingNavPage();
                    return true;
                case "3":
                    Console.Clear();
                    StaffNavPage();
                    return true;
                case "4":
                    Console.Clear();
                    ViewDashboard();
                    return true;
                case "5":
                    return false;
                default:
                    Console.Clear();
                    return true;

            }
        }

        // CLIENT LANDING
        static bool ClientNavPage()
        {
            bool remainInClient = true;
            do
            {
                remainInClient = ClientSection();
            } while (remainInClient);
            return true;
        }
        static bool ClientSection()
        {
            Console.Title = "SALOON APP | Client";

            Console.WriteLine("MENU");
            Console.WriteLine("----");
            Console.WriteLine("");
            Console.WriteLine("1. Register Client");
            Console.WriteLine("2. Modify Client");
            Console.WriteLine("3. Delete Client Record");
            Console.WriteLine("4. Return Home");
            Console.Write("\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Clear();
                    Client.RegisterClient();
                    return true;
                case "2":
                    Console.Clear();
                    Client.ModifyClient("UPDATE CLIENT");
                    return true;
                case "3":
                    Console.Clear();
                    Client.ModifyClient("DELETE CLIENT");
                    return true;
                case "4":
                    Console.Clear();
                    return false;
                default:
                    Console.Clear();
                    return true;

            }
        }

        // STAFF LANDING
        static bool StaffNavPage()
        {
            bool remainInStaff = true;
            do
            {
                remainInStaff = StaffSection();
            } while (remainInStaff);
            return true;
        }
        static bool StaffSection()
        {
            Console.Title = "SALOON APP | Staff";

            Console.WriteLine("MENU");
            Console.WriteLine("----");
            Console.WriteLine("");
            Console.WriteLine("1. Add Staff");
            Console.WriteLine("2. Modify Staff");
            Console.WriteLine("3. Track Availability");
            Console.WriteLine("4. Delete Staff");
            Console.WriteLine("5. Return Home");
            Console.Write("\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Clear();
                    Staff.AddStaff("ADD STAFF");
                    return true;
                case "2":
                    Console.Clear();
                    Staff.ModifyStaff("UPDATE STAFF");
                    return true;
                case "3":
                    Console.Clear();
                    Staff.TrackAvailability("TRACK");
                    return true;
                case "4":
                    Console.Clear();
                    Staff.ModifyStaff("DELETE STAFF");
                    return true;
                case "5":
                    Console.Clear();
                    return false;
                default:
                    Console.Clear();
                    return true;

            }
        }

        // BOOKING LANDING
        static bool BookingNavPage()
        {
            bool remainInBooking = true;
            do
            {
                remainInBooking = BookingSection();
            } while (remainInBooking);
            return true;
        }
        static bool BookingSection()
        {
            Console.Title = "SALOON APP | Booking";

            Console.WriteLine("MENU");
            Console.WriteLine("----");
            Console.WriteLine("");
            Console.WriteLine("1. Book Client");
            Console.WriteLine("2. Modify Booking");
            Console.WriteLine("3. Pay");
            Console.WriteLine("4. Cancel Booking");
            Console.WriteLine("5. Return Home");
            Console.Write("\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Clear();
                    Booking.Book("BOOK CLIENT");
                    return true;
                case "2":
                    Console.Clear();
                    Booking.Modify("MODIFY BOOKING");
                    return true;
                case "3":
                    Console.Clear();
                    Booking.MakePayment("PAY");
                    return true;
                case "4":
                    Console.Clear();
                    Booking.Modify("CANCEL BOOKING");
                    return true;
                case "5":
                    Console.Clear();
                    return false;
                default:
                    Console.Clear();
                    return true;

            }
        }


        static void ViewDashboard() { }


    }
}
