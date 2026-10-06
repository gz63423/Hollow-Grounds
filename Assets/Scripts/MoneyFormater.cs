using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public static class MoneyFormatter
{
    public static string FormatMoney(float money)
    {
        if (money < 1000f)
        {
            return "$" + money.ToString("N2");
        }

        if (money < 1_000_000f)
        {
            return "$" + (money / 1_000f).ToString("0.00") + "K";
        }

        if (money < 1_000_000_000f)
        {
            return "$" + (money / 1_000_000f).ToString("0.00") + "M";
        }

        if (money < 1_000_000_000_000f)
        {
            return "$" + (money / 1_000_000_000f).ToString("0.00") + "B";
        }

        return "$" + (money / 1_000_000_000_000f).ToString("0.00") + "T";
    }
}
