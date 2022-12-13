using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

class AppUtility
{
    public struct TrackingResult
    {
        public string TeamOnDuty;
        public int ShiftNumber;
        public int NextTeamResumesIn;
    }
    public static int generateId(int minimumIDRange, int maximumIDRange, string database)
    {
        Random randomID = new Random();
        int newID = Config.EmptyInteger;
        bool isIdValid = true;
        do
        {
            newID = randomID.Next(minimumIDRange, maximumIDRange);
            isIdValid = AppUtility.IsGeneratedIDValid(newID, database);
        } while (!isIdValid);
        return newID;
    }
    public static bool ProcessDeletion(string referenceId, string referenceDatabase, string processName)
    {
        Console.Write("\nDo you want to proceed? (y/n): ");

        string userResponse = Console.ReadLine();
        if (userResponse.ToUpper() == Char.ToString(Config.ExitWithNo))
        {
            AppUtility.NotifyThenClearScreen($"\nOk. Returning to {processName} Menu...");
            return true;
        }
        else if (userResponse.ToUpper() != Char.ToString(Config.ProceedWithYes))
        {
            AppUtility.NotifyThenClearScreen($"\nInvalid input.\nReturning to {processName} Menu...");
            return true;
        }
        List<string> clientDataExcludedDatabase = AppUtility.DatabaseExcludingSpecifiedRecord(Convert.ToInt32(referenceId), referenceDatabase);
        AppUtility.ReplaceFileContent(clientDataExcludedDatabase, referenceDatabase);

        AppUtility.NotifyThenClearScreen($"\n{processName} record deleted.\nReturning to {processName} Menu...");

        return true;
    }

    public static void CheckDatabaseExists(string database)
    {
        if (!File.Exists(database))
        {
            throw new ArgumentException("File not found");
        }
    }
    public static string SearchDatabaseById(int Id, string database)
    {
        CheckDatabaseExists(database);
        string[] allDatabaseEntries = File.ReadAllLines(database);
        string rawDatabaseEntry = string.Empty;
        for (int i = Config.LoopFirstIndex; i < allDatabaseEntries.Length; i++)
        {
            string[] singleDataLine = allDatabaseEntries[i].Split(Config.SemiColon);
            if (int.Parse(singleDataLine.First()) != Id)
            {
                continue;
            }
            rawDatabaseEntry = singleDataLine.Last();
        }
        return rawDatabaseEntry;
    }
    public static List<string> SearchDatabaseByEmail(string email, string database)
    {

        CheckDatabaseExists(database);
        string[] allDatabaseEntries = File.ReadAllLines(database);
        List<string> databaseResult = new List<string>();
        for (int i = Config.LoopFirstIndex; i < allDatabaseEntries.Length; i++)
        {
            string[] singleDataLine = allDatabaseEntries[i].Split(Config.Colon);
            if (singleDataLine.Last() != email.ToLower())
            {
                continue;
            }
            string rawDatabaseEntry = allDatabaseEntries[i].Split(Config.SemiColon).Last();
            databaseResult.Add(Config.True.ToString());
            databaseResult.Add(rawDatabaseEntry);
            return databaseResult;
        }
        databaseResult.Add(Config.False.ToString());
        return databaseResult;
    }
    public static void DisplayNumberedMenu(Dictionary<int, string> menuListData)
    {
        foreach (KeyValuePair<int, string> listing in menuListData)
        {
            Console.WriteLine($"{listing.Key}. {listing.Value}");
        }
        Console.WriteLine();
    }
    public static List<string> DatabaseExcludingSpecifiedRecord(int recordId, string database)
    {
        StreamReader reader = new StreamReader(database);
        List<string> temporaryClientDatabase = new List<string>();
        while (!reader.EndOfStream)
        {
            string singleLineEntry = reader.ReadLine();
            var lineEntryArray = singleLineEntry.Split(Config.SemiColon);
            if (int.Parse(lineEntryArray.First()) == recordId)
            {
                continue;
            }
            temporaryClientDatabase.Add(singleLineEntry);
        }
        reader.Close();
        return temporaryClientDatabase;
    }
    public static Dictionary<string, string> FormatRawRecordData(string rawDatabaseRecord)
    {
        var formattedDatabaseRecord = rawDatabaseRecord.Split(Config.Comma, Config.Colon)
            .Select((dataValue, dataIndex) => new { Value = dataValue, Index = dataIndex })
            .GroupBy(dataGroup => dataGroup.Index / Config.GroupLength)
            .ToDictionary(parameter => parameter.First().Value, parameter => parameter.Last().Value);
        return formattedDatabaseRecord;
    }
    public static void DisplayFormattedRecord(string dataToDisplay, string tableHeader)
    {
        Dictionary<string,string> formattedRecord = FormatRawRecordData(dataToDisplay);

        DisplayTitleWithBorder(tableHeader);

        foreach (KeyValuePair<string, string> dataItem in formattedRecord)
        {
            Console.WriteLine($"{dataItem.Key} : {dataItem.Value}");
        }
    }
    public static void DisplayTitleWithBorder(string title)
    {
        string subTitleBottomBorder = string.Empty;
        Console.WriteLine(title);
        Console.WriteLine(subTitleBottomBorder = new string(Config.TitleUnderlined, title.Length) + "\n");
    }
    public static void DisplayTitleWithReturn(string title, string userNotification)
    {
        AppUtility.DisplayTitleWithBorder(title);
        Console.WriteLine(userNotification);
    }
    public static bool IsGeneratedIDValid(int generatedId, string database)
    {
        string[] allDatabaseData = File.ReadAllLines(database);
        for (int i = Config.LoopFirstIndex; i < allDatabaseData.Length; i++)
        {
            string[] singleDatabaseEntry = allDatabaseData[i].Split(Config.Colon);
            if (singleDatabaseEntry.First() == Convert.ToString(generatedId))
            {
                return false;
            }
            continue;
        }
        return true;
    }
    public static void NotifyThenClearScreen(string userNotification)
    {
        Console.WriteLine(userNotification);
        Thread.Sleep(Config.TimerDuration);
        Console.Clear();
    }


    // CONTINUE VETTING FROM HERE
    public static void NotifyClearScreenThenReturn(string prompt, int cursorPosition)
    {
        Console.WriteLine(prompt);
        Thread.Sleep(Config.TimerDuration);
        Console.SetCursorPosition(Console.CursorLeft, --Console.CursorTop);
        Console.Write(new string(' ', Console.BufferWidth));
        Console.SetCursorPosition(Console.CursorLeft, cursorPosition);
    }

    public static bool ExitProcessString(string enteredString)
    {
        bool isInteger = int.TryParse(enteredString, out _);
        if (isInteger)
        {
            return false;
        }
        if (enteredString == char.ToString(Config.ExitWithX) /*|| enteredString == string.Empty*/) // MAGIC STRING
        {
            NotifyThenClearScreen("\nReturning to previous menu...");
            return true;
        }
        return false;
    }
    public static void ReplaceFileContent(List<string> tempClientDatabase, string database)
    {
        StreamWriter writer = new StreamWriter(database);
        foreach (var ListItem in tempClientDatabase)
        {
            writer.WriteLine(ListItem);
        }
        writer.Close();
    }
    public static string GetValidInput(string inputTitle, Type requiredType = null)
    {
        bool stringIsInvalid = false, integerIsInvalid = false, emailIsInvalid = false;
        string validInputString = string.Empty;
        do
        {
            Console.Write($"{inputTitle}: ");
            int cursorCurrentPosition = Console.CursorTop;
            validInputString = Console.ReadLine();
            if (requiredType == typeof(string))
            {
                if (stringIsInvalid = AppUtility.InvalidStringReturn(validInputString, cursorCurrentPosition)) { continue; };
            }
            else if (requiredType == typeof(int))
            {
                if (integerIsInvalid = AppUtility.InvalidIntegerReturn(validInputString, cursorCurrentPosition)) { continue; };
            }
            else
            {
                if (emailIsInvalid = AppUtility.InvalidEmailReturn(validInputString, cursorCurrentPosition)) { continue; };
            }
        } while (stringIsInvalid == true || integerIsInvalid == true || emailIsInvalid == true);

        return validInputString;
    }
    public static string GetValidMenuSelection(string inputTitle, Dictionary<int, string> menuListData)
    {
        bool integerIsInvalid; // false by default
        string validInputString = string.Empty;
        do
        {
            Console.Write($"{inputTitle}: ");
            int cursorCurrentPosition = Console.CursorTop;
            validInputString = Console.ReadLine();
            if (validInputString.ToUpper() == char.ToString(Config.ExitWithX))
            {
                break;
            }
            else
            {
                integerIsInvalid = AppUtility.InvalidIntegerReturn(validInputString, cursorCurrentPosition);
                if (integerIsInvalid == false)
                {
                    if (int.Parse(validInputString) > 0 && int.Parse(validInputString) <= menuListData.Count)
                    {
                        break;
                    }
                    else
                    {
                        integerIsInvalid = true;
                        NotifyClearScreenThenReturn("\nInvalid selection, please try again", cursorCurrentPosition);
                    }
                }
            }


        } while (integerIsInvalid);

        return validInputString;
    }
    public static string getNextTeam(string currentTeam, List<string> TeamNames) // CONTAINS REFERENCE 
    {
        int numberOfTeams = TeamNames.Count;

        //Recursive loop to get the next item in a List.
        //Adil, Stack Overflow, 2014
        //Get previous/next item of a given item in a List<>
        //https://stackoverflow.com/questions/24799820/get-previous-next-item-of-a-given-item-in-a-list

        string nextTeam = TeamNames.SkipWhile(x => x != currentTeam).Skip(1).DefaultIfEmpty(TeamNames[0]).FirstOrDefault(); // MC
        return nextTeam;
    }
    public static void GetStaffsInTeam(string teamName)
    {
        // GET STAFF DETAILS FROM PROVIDED GROUP NAME

    }
    public static List<string> getListFromDictValues(Dictionary<int, string> dictionaryData)
    {
        List<string> listFromDictValues = new List<string>();
        foreach (KeyValuePair<int, string> dictItem in dictionaryData)
        {
            listFromDictValues.Add(dictItem.Value);
        }
        return listFromDictValues;
    }
    public static string GetValidDate(string inputTitle)
    {
        bool dateIsInvalid = false;
        string inputString = string.Empty;
        do
        {
            Console.Write($"{inputTitle}: ");
            int cursorCurrentPosition = Console.CursorTop;
            inputString = Console.ReadLine();

            if (dateIsInvalid = InvalidDateReturn(inputString, cursorCurrentPosition)) { continue; };

        } while (dateIsInvalid == true);
        return inputString;
    }
    public static string GetValidTime(string inputTitle)
    {
        bool timeIsInvalid = false;
        string inputString = string.Empty;
        do
        {
            Console.Write($"{inputTitle}: ");
            int cursorCurrentPosition = Console.CursorTop;
            inputString = Console.ReadLine();

            if (timeIsInvalid = InvalidTimeReturn(inputString, cursorCurrentPosition)) { continue; };

        } while (timeIsInvalid == true);
        return inputString;
    }

    // YOU ARE HERE!!! GO OVER THIS CODE AGAIN... SEEMS TO COMPLEX. ALSO CHECK GETSTAFFSINTEAM METHOD
    public static Dictionary<string,string> GetBookingAvailability(DateTime dateOfAppointment, DateTime timeOfAppointment)
    {
        int staffId = Config.EmptyInteger;
        Dictionary<string, string> bookingAvailabilityResult = new Dictionary<string, string>();
        string[] allStaffData = File.ReadAllLines(Config.StaffDatabase);
        string[] allBookingsData = File.ReadAllLines(Config.BookingDatabase);

        int timeToAppointment = GetTimeSpanToAppointment(Config.ShiftStarted, dateOfAppointment);
        string teamOnDuty = TrackAvailability(timeToAppointment).TeamOnDuty;

        /*for (int i = 0; i < allStaffData.Length; i++)
        {
            staffId = int.Parse(allStaffData[i].Split(Config.DataSeparatorSemiColon).First());

        }
        for (int j = 0; j < allBookingsData.Length; j++)
        {
            string[] singleLineOfData = allBookingsData[j].Split(Config.DataSeparatorComma);
            DateTime bookingDate = DateTime.Parse(singleLineOfData.First());
            if (bookingDate != dateOfAppointment)
            {
                continue;
            }
            string[] bookingDetails = singleLineOfData.Last().Split(Config.DataSeparatorSemiColon);

            string staffDetails = SearchDatabaseById(int.Parse(bookingDetails.Last()), Config.StaffDatabase);
            string staffName = staffDetails
                .Split(Config.DataSeparatorComma)[Config.EmptyIntegerZero]
                .Split(Config.DataSeparatorColon).Last();

            if (DateTime.Parse(bookingDetails.First()).TimeOfDay == timeOfAppointment.TimeOfDay)
            {
                bookingAvailabilityResult.Add($"{staffName}", "Booked");
            }
            else
            {
                bookingAvailabilityResult.Add($"{staffName}", "Available");
            }
        }*/
        return bookingAvailabilityResult;

    }
    public static int CountWeekendsAndHolidays(DateTime whenShiftStarted, DateTime dateOfAppointment)
    {
        int numberOfWeekendsAndHolidays = 0;
        for (DateTime i = whenShiftStarted; i < dateOfAppointment; i = i.AddDays(1.0))
        {
            if (i.DayOfWeek == DayOfWeek.Sunday || isBankHoliday(i) == true)
            {
                //Console.WriteLine(i + " " +i.DayOfWeek); // Uncomment for toubleshooting
                numberOfWeekendsAndHolidays++;
            }
        }
        //Console.WriteLine(numberOfWeekendsAndHolidays); // Uncomment for toubleshooting
        return numberOfWeekendsAndHolidays;
    }
    public static bool isBankHoliday(DateTime dateOfAppointment)
    {
        List<string> resultList = BankHolidayResult(getBankHolidayList(), dateOfAppointment);
        if (bool.Parse(resultList.Last()) == true)
        {
            return true;
        }
        return false;
    }

    // MAGIC STRINGS PRESENT
    public static List<string> BankHolidayResult(List<Dictionary<string, string>> bankHolidayList, DateTime dateOfAppointment)
    {
        int currentYear = DateTime.Now.Year;
        List<string> resultDictionary = new List<string>();
        for (int i = Config.LoopFirstIndex; i < bankHolidayList.Count; i++)
        {
            int HolidayYear = DateTime.Parse(bankHolidayList[i]["date"]).Year;
            if (HolidayYear == currentYear)
            {
                if (DateTime.Parse(bankHolidayList[i]["date"]) == dateOfAppointment)
                {
                    //Console.WriteLine(bankHolidayList[i]["title"]); // MAGIC STRING
                    resultDictionary.Add(bankHolidayList[i]["title"]);
                    resultDictionary.Add("true");
                    return resultDictionary;
                }
                else
                {
                    continue;
                }
            }
        }
        resultDictionary.Add("title");
        resultDictionary.Add("false");
        return resultDictionary;
    }
    public static string GetBankHolidayJson()
    {
        string url = Config.UKBankHolidaysAPILink;
        HttpClient client = new HttpClient();
        var result = client.GetStringAsync(url).Result;
        return result;
    }
    public static List<Dictionary<string, string>> getBankHolidayList()
    {
        List<Dictionary<string, string>> HolidayList = new List<Dictionary<string, string>>();
        // The json file for all Bank Holidays 
        var bankHolidaysJsonData = GetBankHolidayJson();

        // All events from England and Wales, Northen Ireland, and Scotland
        string[] bankHolidayAllRegions = bankHolidaysJsonData.Split('[', ']');

        // Splitting only events from England and Wales
        string[] bankHolidayEnglandWales = bankHolidayAllRegions[1].Split(Config.LeftCurlyBracket, Config.RightCurlyBracket);

        for (int i = Config.LoopFirstIndex; i < bankHolidayEnglandWales.Length; i++)
        {
            if (bankHolidayEnglandWales[i] == string.Empty || bankHolidayEnglandWales[i] == Convert.ToString(Config.Comma) || i == Config.LoopFirstIndex)
            {
                continue;
            }
            string cleanedHolidayData = bankHolidayEnglandWales[i].Replace(Config.EscapeCharacterDoubleQuotes.ToString(), string.Empty);
            Dictionary<string, string> singleHoliday = cleanedHolidayData.Split(Config.Colon, Config.Comma).Select((v, j) => new { Index = j, Value = v }).GroupBy(l => l.Index / Config.GroupLength)
                                .ToDictionary(p => p.First().Value, p => p.Last().Value);
            HolidayList.Add(singleHoliday);
        }
        return HolidayList;
    }
    public static int GetTimeSpanToAppointment(DateTime ShiftStartDate, DateTime dateOfAppointment)
    {
        DateTime shiftCountStart = ShiftStartDate.Subtract(TimeSpan.FromDays(1));
        TimeSpan timeSpanToAppointment = dateOfAppointment.Subtract(shiftCountStart);
        int workdaysToAppointment = timeSpanToAppointment.Days - CountWeekendsAndHolidays(shiftCountStart, dateOfAppointment);
        return workdaysToAppointment;
    }

    // Tracker should have a press any key to exit at the end
    public static TrackingResult TrackAvailability(int timeSpanToAppointment, string teamStaffBelongsTo = null)
    {
        List<string> teamNames = getListFromDictValues(Config.TeamNames);
        int shiftNumber = Config.ShiftStartNumber;
        int daysWorkedByTeam = Config.DaysWorkedByTeam;
        int daysOnDuty = Config.DaysOnDuty;
        int daysBeforeResumption = Config.EmptyInteger;

        string currentTeam = teamNames.First();

        for (int j = Config.LoopFirstIndex; j < timeSpanToAppointment; j++)
        {
            if (daysWorkedByTeam == timeSpanToAppointment)
            {
                if (j % daysOnDuty == Config.EmptyInteger)
                {
                    shiftNumber = Config.ShiftStartNumber;
                    currentTeam = getNextTeam(currentTeam, teamNames);
                    break;
                }
                break;
            }
            else
            {
                if (j == Config.EmptyInteger || j % daysOnDuty != Config.EmptyInteger)
                {
                    //Console.WriteLine($"{currentTeam} {shiftNumber}"); //Uncomment for testing
                    daysWorkedByTeam++;
                    shiftNumber++;
                    continue;
                }
                else
                {
                    shiftNumber = Config.ShiftStartNumber;
                    currentTeam = getNextTeam(currentTeam, teamNames);
                    //Console.WriteLine($"{currentTeam} {shiftNumber}"); //Uncomment for testing
                    daysWorkedByTeam++;
                    shiftNumber++;
                    continue;
                }
            }

        }
        // Shift Tracking final result
        // Console.WriteLine($"\n\nTeam on Duty: {currentTeam} on their {shiftNumber} shift"); // Uncomment for testing

        TrackingResult newTrackingResult = new TrackingResult();

        if (teamStaffBelongsTo == null)
        {
            newTrackingResult.TeamOnDuty = currentTeam;
            newTrackingResult.ShiftNumber = shiftNumber;
        }
        else
        {
            daysBeforeResumption = GetDaysBeforeResumption(currentTeam, shiftNumber, teamStaffBelongsTo);
            newTrackingResult.TeamOnDuty = currentTeam;
            newTrackingResult.ShiftNumber = shiftNumber;
            newTrackingResult.NextTeamResumesIn = daysBeforeResumption;
        }

        return newTrackingResult;
    }
    public static int GetDaysBeforeResumption(string teamOnDuty, int daysWorked, string teamToResume)
    {
        List<string> teamNames = getListFromDictValues(Config.TeamNames);
        int daysOnDuty = Config.DaysOnDuty;
        string currentTeam = teamOnDuty;
        int daysBeforeResumption = 0; // MAGIC NUMBER

        while (currentTeam != teamToResume)
        {
            if (daysWorked == daysOnDuty)
            {
                daysWorked = Config.DaysWorkedByTeam; // 1
                currentTeam = getNextTeam(currentTeam, teamNames);
                daysBeforeResumption++;
                continue;
            }
            {
                daysWorked++;
                daysBeforeResumption++;
                continue;
            }

        }

        return daysBeforeResumption;
    }

    // ADD REFERENCES TO THE REGEX CODES
    //https://www.regexlib.com/Search.aspx?k=email
    //https://www.regexlib.com/Search.aspx?k=integer
    //https://www.regexlib.com/Search.aspx?k=string

    public static bool InvalidStringReturn(string enteredString, int prevCursorPosition)
    {
        if (Regex.IsMatch(enteredString, @"^[a-zA-Z]+$"))
        {
            return false;
        }
        NotifyClearScreenThenReturn("\nInvalid input, please try again", prevCursorPosition);
        return true;
    }
    public static bool InvalidIntegerReturn(string enteredString, int prevCursorPosition)
    {
        if (enteredString.ToUpper() == char.ToString(Config.ExitWithX))
        {
            return false;
        }
        else
        {
            if (Regex.IsMatch(enteredString, @"^[0-9]+$"))
            {
                return false;
            }
            NotifyClearScreenThenReturn("\nInvalid input, please try again", prevCursorPosition);
            return true;
        }
    }
    public static bool InvalidEmailReturn(string enteredString, int prevCursorPosition)
    {
        if (enteredString == char.ToString(Config.ExitWithX).ToLower())
        {
            return false;
        }
        else
        {
            if (Regex.IsMatch(enteredString, @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"))
            {
                return false;
            }
            NotifyClearScreenThenReturn("\nInvalid email, please try again...", prevCursorPosition);
            return true;
        }
    }
    public static bool InvalidDateReturn(string enteredString, int prevCursorPosition)
    {
        if (enteredString == char.ToString(Config.ExitWithX).ToLower())
        {
            return false;
        }
        else
        {
            bool validDate = DateTime.TryParseExact(enteredString, @"yyyy/MM/dd", new CultureInfo("en-GB"), DateTimeStyles.None, out DateTime tempDate);
            if (validDate == true)
            {
                DateTime currentShortTime = DateTime.Now;
                if (currentShortTime.Subtract(tempDate) > TimeSpan.FromDays(1))
                {
                    NotifyClearScreenThenReturn("\nInvalid date, please try again", prevCursorPosition);
                    return true;
                }
                if (isBankHoliday(tempDate) || tempDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    NotifyClearScreenThenReturn("\nWe don't work on Sundays and Bank Holidays", prevCursorPosition);
                    return true;
                }
                return false;
            }
            NotifyClearScreenThenReturn("\nInvalid date, please try again", prevCursorPosition);
            return true;
        }
    }
    public static bool InvalidTimeReturn(string enteredString, int prevCursorPosition)
    {
        if (enteredString == char.ToString(Config.ExitWithX).ToLower())
        {
            return false;
        }
        else
        {
            bool validTime = DateTime.TryParse(enteredString, out DateTime tempTime);

            if (validTime == true)
            {
                if (tempTime.TimeOfDay < Config.StartOfBusiness.TimeOfDay || tempTime.TimeOfDay > Config.CloseOfBusiness.TimeOfDay.Subtract(TimeSpan.FromHours(1)))
                {
                    NotifyClearScreenThenReturn($"\nBusiness Hours are within {Config.StartOfBusiness.TimeOfDay} and {Config.CloseOfBusiness.TimeOfDay}", prevCursorPosition);
                    return true;
                }
                return false;
            }
            NotifyClearScreenThenReturn("\nInvalid time, please try again", prevCursorPosition);
            return true;
        }
    }
    public static bool InvalidString(string enteredString)
    {
        if (Regex.IsMatch(enteredString, @"^[a-zA-Z]+$"))
        {
            return false;
        }
        NotifyThenClearScreen("\nInvalid input. Clearing buffer");
        return true;
    }
    public static bool InvalidEmail(string enteredString)
    {
        if (Regex.IsMatch(enteredString, @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"))
        {
            return false;
        }
        NotifyThenClearScreen("\nInvalid email. Clearing buffer...");
        return true;
    }
    public static bool InvalidInteger(string enteredString)
    {
        if (Regex.IsMatch(enteredString, @"^[0-9]+$"))
        {
            return false;
        }
        NotifyThenClearScreen("\nInvalid integer. Clearing buffer...");
        return true;
    }
}