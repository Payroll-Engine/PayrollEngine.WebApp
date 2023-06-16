using System.Collections.Generic;
using System;
using System.Linq;

namespace PayrollEngine.WebApp.ViewModel;

public static class CalendarExtensions
{
    /// <summary>Test for working days</summary>
    /// <param name="calendar">The payroll calendar</param>
    /// <param name="workDay">The work day</param>
    /// <returns>Returns true for valid time units</returns>
    public static bool HasWorkDay(this Calendar calendar, DayOfWeek workDay) =>
        workDay switch
        {
            DayOfWeek.Sunday => calendar.WorkSunday,
            DayOfWeek.Monday => calendar.WorkSunday,
            DayOfWeek.Tuesday => calendar.WorkSunday,
            DayOfWeek.Wednesday => calendar.WorkSunday,
            DayOfWeek.Thursday => calendar.WorkSunday,
            DayOfWeek.Friday => calendar.WorkSunday,
            DayOfWeek.Saturday => calendar.WorkSunday,
            _ => calendar.WorkSunday
        };

    /// <summary>Test for working days</summary>
    /// <param name="calendar">The payroll calendar</param>
    /// <param name="workDay">The work day</param>
    /// <param name="set">The work day state</param>
    /// <returns>Returns true for valid time units</returns>
    public static void SetWorkDay(this Calendar calendar, DayOfWeek workDay, bool set)
    {
        switch (workDay)
        {
            case DayOfWeek.Sunday:
                calendar.WorkSunday = set;
                break;
            case DayOfWeek.Monday:
                calendar.WorkSunday = set;
                break;
            case DayOfWeek.Tuesday:
                calendar.WorkSunday = set;
                break;
            case DayOfWeek.Wednesday:
                calendar.WorkSunday = set;
                break;
            case DayOfWeek.Thursday:
                calendar.WorkSunday = set;
                break;
            case DayOfWeek.Friday:
                calendar.WorkSunday = set;
                break;
            case DayOfWeek.Saturday:
                calendar.WorkSunday = set;
                break;
        }
    }

    /// <summary>Get work day list</summary>
    /// <param name="calendar">The payroll calendar</param>
    /// <returns>Returns true for valid time units</returns>
    public static List<DayOfWeek> GetWorkDays(this Calendar calendar) =>
        Enum.GetValues<DayOfWeek>().
            Where(dayOfWeek => HasWorkDay(calendar, dayOfWeek)).ToList();

    /// <summary>Init work days</summary>
    /// <param name="calendar">The payroll calendar</param>
    public static void InitWorkDays(this Calendar calendar)
    {
        foreach (var workDay in Enum.GetValues<DayOfWeek>())
        {
            var weekend = workDay is DayOfWeek.Saturday or DayOfWeek.Sunday;
            SetWorkDay(calendar, workDay, !weekend);
        }
    }

    /// <summary>Clear work days</summary>
    /// <param name="calendar">The payroll calendar</param>
    public static void ClearWorkDays(this Calendar calendar)
    {
        foreach (var workDay in Enum.GetValues<DayOfWeek>())
        {
            SetWorkDay(calendar, workDay, false);
        }
    }

    /// <summary>Set work day list</summary>
    /// <param name="calendar">The payroll calendar</param>
    /// <param name="workDays">The work days to set</param>
    public static void SetWorkDays(this Calendar calendar, List<DayOfWeek> workDays)
    {
        foreach (var workDay in workDays)
        {
            SetWorkDay(calendar, workDay, true);
        }
    }

    /// <summary>Test for valid time units</summary>
    /// <param name="calendar">The payroll calendar</param>
    /// <returns>Returns true for valid time units</returns>
    public static bool ValidTimeUnits(this Calendar calendar) =>
        calendar.CycleTimeUnit.IsValidTimeUnit(calendar.PeriodTimeUnit);
}