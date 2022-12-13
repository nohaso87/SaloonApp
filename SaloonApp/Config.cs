using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

class Config
{
    public const string configDatabase = "Config.txt";

    public static string ClientDatabase;
    public static string BookingDatabase;
    public static string StaffDatabase;
    public static int LoopFirstIndex; 
    public static int EmptyInteger;
    public static char SemiColon;
    public static char Comma;
    public static char Colon;
    public static string UKBankHolidaysAPILink;
    public static char LeftBoxBracket;
    public static char RightBoxBracket;
    public static char LeftCurlyBracket;
    public static char RightCurlyBracket;
    public static string EscapeCharacterDoubleQuotes;
    public static char TitleUnderlined;
    public static int GroupLength;
    public static int TimerDuration;
    public static char ExitWithX;    
    public static int MinimumIDRange;
    public static int MaximumIDRange;
    public static char ProceedWithYes;
    public static char ExitWithNo;
    public static int MinimumIDRangeStaff;
    public static int MaximumIDRangeStaff;
    public static Dictionary<int,string> TeamNames;
    public static int LoginAttemptLimit;
    public static int DaysOnDuty;
    public static int DaysWorkedByTeam;
    public static int ShiftStartNumber;
    public static DateTime ShiftStarted;
    public static DateTime StartOfBusiness;
    public static DateTime CloseOfBusiness;
    public static bool True;
    public static bool False;

    public Config()
    {
        StreamReader configurationFile = new StreamReader(configDatabase);

        ClientDatabase = configurationFile.ReadLine();
        BookingDatabase = configurationFile.ReadLine();
        StaffDatabase = configurationFile.ReadLine();
        LoopFirstIndex = int.Parse(configurationFile.ReadLine());
        EmptyInteger = int.Parse(configurationFile.ReadLine());
        SemiColon = char.Parse(configurationFile.ReadLine());
        Comma = char.Parse(configurationFile.ReadLine());
        Colon = char.Parse(configurationFile.ReadLine());
        UKBankHolidaysAPILink = configurationFile.ReadLine();
        LeftBoxBracket = char.Parse(configurationFile.ReadLine());
        RightBoxBracket = char.Parse(configurationFile.ReadLine());
        LeftCurlyBracket = char.Parse(configurationFile.ReadLine());
        RightCurlyBracket = char.Parse(configurationFile.ReadLine());
        EscapeCharacterDoubleQuotes = configurationFile.ReadLine();
        TitleUnderlined = char.Parse(configurationFile.ReadLine());
        GroupLength = int.Parse(configurationFile.ReadLine());
        TimerDuration = int.Parse(configurationFile.ReadLine());
        ExitWithX = char.Parse(configurationFile.ReadLine());
        MinimumIDRange = int.Parse(configurationFile.ReadLine());
        MaximumIDRange = int.Parse(configurationFile.ReadLine());
        ProceedWithYes = char.Parse(configurationFile.ReadLine());
        ExitWithNo = char.Parse(configurationFile.ReadLine());
        MinimumIDRangeStaff = int.Parse(configurationFile.ReadLine());
        MaximumIDRangeStaff = int.Parse(configurationFile.ReadLine());
        TeamNames = configurationFile.ReadLine().Split(Comma).Select((dataValue, dataIndex) => new { Index = dataIndex, Value = dataValue }).GroupBy(dataGroup => dataGroup.Index / GroupLength).ToDictionary(parameter => int.Parse(parameter.First().Value), parameter => parameter.Last().Value); ;
        LoginAttemptLimit = int.Parse(configurationFile.ReadLine());
        DaysOnDuty = int.Parse(configurationFile.ReadLine());
        DaysWorkedByTeam = int.Parse(configurationFile.ReadLine());
        ShiftStartNumber = int.Parse(configurationFile.ReadLine());
        ShiftStarted = DateTime.Parse(configurationFile.ReadLine());
        StartOfBusiness = DateTime.Parse(configurationFile.ReadLine());
        CloseOfBusiness = DateTime.Parse(configurationFile.ReadLine());
        True = bool.Parse(configurationFile.ReadLine());
        False = bool.Parse(configurationFile.ReadLine());

        configurationFile.Close();
    }

}