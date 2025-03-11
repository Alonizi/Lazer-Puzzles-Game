using System.Collections.Generic;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;


public enum CustomColors{
red , blue , green , purple , yellow , white
}

public static class CustomColorsUtility
{
    private static Dictionary<CustomColors, string> colortohex = new Dictionary<CustomColors, string>()
    {
        {CustomColors.white,"#FFFFFF"},
        {CustomColors.blue,"#87C3E1"},
        {CustomColors.red,"#D95952"},
        {CustomColors.green,"#A8DAA7"},
        {CustomColors.purple,"#99639A"},
        {CustomColors.yellow,"#F4E964"}
    };
    
    public static string ColorHex(CustomColors color)
    {
        return colortohex[color];
    }

    public static Color CustomColorToUnityColor(CustomColors color)
    {
        ColorHex(color);
        Color outputColor;
        ColorUtility.TryParseHtmlString(ColorHex(color), out outputColor);
        return outputColor;
    }
}