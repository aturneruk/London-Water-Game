using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Month {
    Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
}

public static class GameTime {

    static private int year;
    static private Month month;
    static private int day;
    static private float dayFraction;

    static private int gameSpeed = 1;

    static public void Set(int y, Month m, int d) {
        year = y;
        month = m;
        day = d;
        dayFraction = 0f;
    }

    static public void SetSpeed(int speed) {
        gameSpeed = speed;
    }

    static public void UpdateTime() {
        dayFraction += gameSpeed * Time.unscaledDeltaTime;

        if (dayFraction >= 1) {
            NextDay();
        }

        switch (month) {
            case Month.Feb:
                // Check for leap year
                if (year % 400 == 0 | (year % 4 == 0 && year % 100 != 0)) {
                    // leap year
                    if (day > 29) {
                        NextMonth();
                    }
                }
                else {
                    // not leap year
                    if (day > 28) {
                        NextMonth();
                    }
                }
                break;

            case Month.Apr:
            case Month.Jun:
            case Month.Sep:
            case Month.Nov:
                if (day > 30) {
                    NextMonth();
                }
                break;

            case Month.Jan:
            case Month.Mar:
            case Month.May:
            case Month.Jul:
            case Month.Aug:
            case Month.Oct:
                if (day > 31) {
                    NextMonth();
                }
                break;

            case Month.Dec:
                if (day > 31) {
                    NextYear();
                }
                break;

            default:
                Debug.LogError("GameTime.month is outside of expected values");
                break;
        }
    }

    static private void NextDay() {
        day++;
        dayFraction--;
    }

    static private void NextMonth() {
        month++;
        day = 1;
    }

    static private void NextYear() {
        year++;
        month = Month.Jan;
        day = 1;
    }

    static public string GetShortForm() {
        int monthNumber = (int)month + 1;
        return day.ToString() + "/" + monthNumber + "/" + year.ToString();
    }
}
