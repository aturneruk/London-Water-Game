using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Money {

    private static double balance;
    public static double Balance {
        get {
            return balance;
        }
        private set {
            if (value < 0) {
                throw new System.ArgumentOutOfRangeException("The money balance must be positive");
            }
            else {
                balance = value;
            }
        }
    }

    public static void SetMoney(double bal) {
        if (bal >= 0) {
            Balance = bal;
        }
        else {
            throw new System.ArgumentOutOfRangeException("Attempted to set a negative or non numerical balance");
        }
    }

    public static void AddMoney(double income) {
        if (income > 0) {
            Balance += income;
        }
        else {
            throw new System.ArgumentOutOfRangeException("Attempted to add a non-positive income");
        }
    }

    public static bool SubtractMoney(double cost) {
        if (balance >= cost) {
            balance -= cost;
            return true;
        }
        else {
            return false;
        }
    }

    public static string FormatMoney(double money) {
        if (money < 1000) {
            return "£" + money.ToString("###0");
        }
        else if (money >= 1000 && money < 10000) {
            return "£" + (money / 1000d).ToString("0.00") + "k";
        }
        else if (money >= 10000 && money < 100000) {
            return "£" + (money / 1000d).ToString("00.0") + "k";
        }
        else if (money >= 100000 && money < 1000000) {
            return "£" + (money / 1000d).ToString("000") + "k";
        }
        else if (money >= 1000000 && money < 10000000) {
            return "£" + (money / 1000000d).ToString("0.00") + "m";
        }
        else if (money >= 10000000 && money < 100000000) {
            return "£" + (money / 1000000d).ToString("00.0") + "m";
        }
        else if (money >= 100000000 && money < 1000000000) {
            return "£" + (money / 1000000d).ToString("000") + "m";
        }
        else if (money >= 1000000000 && money < 10000000000) {
            return "£" + (money / 1000000000d).ToString("0.00") + "b";
        }
        else if (money >= 10000000000 && money < 100000000000) {
            return "£" + (money / 1000000000d).ToString("00.0") + "b";
        }
        else if (money >= 100000000000 && money < 1000000000000) {
            return "£" + (money / 1000000000d).ToString("000") + "b";
        }
        else if (money >= 1000000000000) {
            return "£" + (money / 1000000000d).ToString("F0") + "b";
        }
        else {
            throw new System.ArgumentException("Something has gone wrong formatting the money to a string");
        }        
    }
}
