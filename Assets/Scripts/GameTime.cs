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

    static public void Set(int y, Month m, int d) {
        year = y;
        month = m;
        day = d;
    }

    static public string GetShortForm() {
        int monthNumber = (int)month + 1;
        return day.ToString() + "/" + monthNumber + "/" + year.ToString();
    }

}
