using System;
using System.Collections;

namespace WindowsScheduleService.Model;

public class Scheduler
{
    public Scheduler(ArrayList months, ArrayList days, ArrayList weekendDays, ArrayList hours, ArrayList minutes,
        string executablePath, string commandLineParameter) =>
        (Months, Days, WeekendDays, Hours, Minutes, ExecutablePath,
            CommandLineParameter) = (months, days, weekendDays, hours, minutes, executablePath, commandLineParameter);

    ArrayList Months { get; }
    ArrayList Days { get; }
    ArrayList Hours { get; }
    ArrayList Minutes { get; }
    ArrayList WeekendDays { get; }
    public string ExecutablePath { get; set; }
    public string CommandLineParameter { get; set; }

    public bool IsReady() =>
        Contains(Months, DateTime.Now.Month) &&
        Contains(Days, GetMonthDay(DateTime.Now)) &&
        Contains(WeekendDays, GetWeekDay(DateTime.Now)) &&
        Contains(Hours, DateTime.Now.Hour) &&
        Contains(Minutes, DateTime.Now.Minute);

    public int GetMonthDay(DateTime date) => date.AddMonths(1 - date.Month).DayOfYear;
    public int GetWeekDay(DateTime date) => date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int) date.DayOfWeek;
    public bool Contains(object list, int val) => ((ArrayList) list).Contains(val) || ((ArrayList) list).Contains(-1);
}