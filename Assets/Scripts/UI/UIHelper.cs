using System;

namespace UI
{
    public static class UIHelper
    {
        public static string FormatDistance(float distance)
        {
            return $"Distance {(int)Math.Ceiling(distance):D6}";
        }

        public static string FormatCoins(int coins)
        {
            return $"Coins {coins:D6}";
        } 
    }
}