using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Month {
    Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
}

public enum Weekday {
    Mon, Tue, Wed, Thu, Fri, Sat, Sun
}

public enum Speed {
    X1,
    X2,
    X4,
    X8
}

public static class GameTime {

    public delegate void YearAction();
    public static event YearAction NewYear;

    public delegate void MonthAction();
    public static event MonthAction NewMonth;

    public delegate void WeekAction();
    public static event WeekAction NewWeek;

    public delegate void DayAction();
    public static event DayAction NewDay;

    static private int year;
    static private Month month;
    static private int day;
    static private int week;
    static private Weekday weekday;
    static private float dayFraction;

    static private int gameSpeed;
    static private int baseGameSpeed = 60;
    static private int gameSpeedMultiplier = 1;

    static public void Set(int y, Month m, int d, Weekday wd) {
        year = y;
        month = m;
        day = d;
        week = 1;
        weekday = wd;
        dayFraction = 0f;

        NewDay += ChangeDay;
        NewWeek += ChangeWeek;
        NewMonth += ChangeMonth;
        NewYear += ChangeYear;
    }

    static public void SetSpeed(Speed speed) {

        switch (speed) {
            case Speed.X1:
                gameSpeedMultiplier = 1;
                break;
            case Speed.X2:
                gameSpeedMultiplier = 2;
                break;
            case Speed.X4:
                gameSpeedMultiplier = 4;
                break;
            case Speed.X8:
                gameSpeedMultiplier = 8;
                break;
        }
        gameSpeed = baseGameSpeed * gameSpeedMultiplier;
    }

    static public Speed GetSpeed() {
               
        switch (gameSpeedMultiplier) {
            case 1:
                return Speed.X1;
            case 2:
                return Speed.X2;
            case 4:
                return Speed.X4;
            case 8:
                return Speed.X8;
            default:
                throw new System.InvalidOperationException("Unknown value for game speed");
        }
    }


    static public void UpdateTime() {

        dayFraction += gameSpeed * Time.unscaledDeltaTime;

        while (dayFraction >= 1) {
            NewDay();
        }

        switch (month) {
            case Month.Feb:
                // Check for leap year
                if (year % 400 == 0 | (year % 4 == 0 && year % 100 != 0)) {
                    // leap year
                    if (day > 29) {
                        NewMonth();
                    }
                }
                else {
                    // not leap year
                    if (day > 28) {
                        NewMonth();
                    }
                }
                break;

            case Month.Apr:
            case Month.Jun:
            case Month.Sep:
            case Month.Nov:
                if (day > 30) {
                    NewMonth();
                }
                break;

            case Month.Jan:
            case Month.Mar:
            case Month.May:
            case Month.Jul:
            case Month.Aug:
            case Month.Oct:
                if (day > 31) {
                    NewMonth();
                }
                break;

            case Month.Dec:
                if (day > 31) {
                    NewYear();
                }
                break;

            default:
                Debug.LogError("GameTime.month is outside of expected values");
                break;
        }
    }

    static private void ChangeDay() {
        day++;
        dayFraction--;

        if (weekday == Weekday.Sun) {
            NewWeek();
        }
        else {
            weekday++;
        }
    }

    static private void ChangeWeek() {
        week++;
        weekday = Weekday.Mon;
    }

    static private void ChangeMonth() {
        month++;
        day = 1;
    }

    static private void ChangeYear() {
        year++;
        month = Month.Jan;
        day = 1;
    }

    static public string GetShortForm() {
        int monthNumber = (int)month + 1;
        return day.ToString() + "/" + monthNumber + "/" + year.ToString();
    }

    static public string GetLongForm() {

        switch (gameSpeedMultiplier) {
            case 1:
            case 2:
            case 4:
                return month.ToString() + " " + year.ToString();
            case 8:
                return year.ToString();
            default:
                return year.ToString();
        }
        



        //switch (day) {
        //    case 1:
        //    case 21:
        //    case 31:
        //        return day.ToString() + "st " + month.ToString() + " " + year.ToString();

        //    case 2:
        //    case 22:
        //        return day.ToString() + "nd " + month.ToString() + " " + year.ToString();

        //    case 3:
        //        return day.ToString() + "rd " + month.ToString() + " " + year.ToString();

        //    default:
        //        return day.ToString() + "th " + month.ToString() + " " + year.ToString();
        //}
    }
}
