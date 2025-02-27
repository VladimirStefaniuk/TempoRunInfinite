using System;

namespace UI
{
    public static class UIHelper
    {
        public static string FormatDistanceWithLeadingZeroes(float distance)
        {
            return $"Distance {distance:0}";
        }
    
        public static string FormatCoinsWithLeadingZeroes(int coins)
        {
            return $"Coins {coins}";
        } 
    }
}