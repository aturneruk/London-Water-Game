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
            day++;
            dayFraction--;
        }

        switch (month) {
            case Month.Feb:
                // Check for leap year
                if (year % 400 == 0 | (year % 4 == 0 && year % 100 != 0)) {
                    // leap year
                    if (day > 29) {
                        month++;
                        day = 1;
                    }
                }
                else {
                    // not leap year
                    if (day > 28) {
                        month++;
                        day = 1;
                    }
                }
                break;

            case Month.Apr:
            case Month.Jun:
            case Month.Sep:
            case Month.Nov:
                if (day > 30) {
                    month++;
                    day = 1;
                }
                break;

            case Month.Dec:
                if (day > 31) {
                    year++;
                    month = Month.Jan;
                    day = 1;
                }
                break;

            default:
                if (day > 31) {
                    month++;
                    day = 1;
                }
                break;
        }
    }

    static public string GetShortForm() {
        int monthNumber = (int)month + 1;
        return day.ToString() + "/" + monthNumber + "/" + year.ToString();
    }

}
