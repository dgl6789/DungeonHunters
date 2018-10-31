using App.UI;
using UnityEngine;

public class Date {

    int year;
    int Year { get { return year; } }

    int month; 
    int Month { get {return month; } }

    int day;
    int Day { get { return day; } }

    int dayOfWeek;

    public Date(int pMonth, int pDay, int pYear) {
        month = pMonth;
        day = pDay;
        year = pYear;

        dayOfWeek = 1;
    }

	public void Advance() {
        day++;
        dayOfWeek = dayOfWeek % 7 + 1;

        if(day > NumDaysInMonth(month, year)) {
            month++;
            day = 1;

            if(month > 12) {
                year++;
                month = 1;
            }
        }
    }

    public static int NumDaysInMonth(int pMonth, int year) {
        switch (pMonth) {
            case 1:
            case 3:
            case 5:
            case 7:
            case 8:
            case 10:
            case 12:
                return 31;

            case 4:
            case 6:
            case 9:
            case 11:
                return 30;

            case 2:
                if (year % 4 == 0) return 29;
                else return 28;

            default: return -1;
        }
    }

    public static string IntToMonth(int pMonth) {
        switch (pMonth) {
            case 1: return "January";
            case 2: return "February";
            case 3: return "March";
            case 4: return "April";
            case 5: return "May";
            case 6: return "June";
            case 7: return "July";
            case 8: return "August";
            case 9: return "September";
            case 10: return "October";
            case 11: return "November";
            case 12: return "December";
            default: return "";
        }
    }

    public static string IntToWeekday(int pWeekday) {
        switch (pWeekday) {
            case 1: return "Sunday";
            case 2: return "Monday";
            case 3: return "Tuesday";
            case 4: return "Wednesday";
            case 5: return "Thursday";
            case 6: return "Friday";
            case 7: return "Saturday";
            default: return "";
        }
    }

    public string OrdinalSuffix(int pNumber) {
        int j = pNumber % 10;
        int k = pNumber % 100;

        if(j == 1 && k != 11) { return "st"; }
        if(j == 2 && k != 12) { return "nd"; }
        if(j == 3 && k != 13) { return "rd"; }

        return "th";
    }

    public override string ToString() {
        return IntToWeekday(dayOfWeek) + ", " + IntToMonth(month) + " " + day + OrdinalSuffix(day) + ", " + year;
    }
}
