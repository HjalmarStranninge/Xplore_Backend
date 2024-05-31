namespace CC_Backend.Utilities
{
    public class StampUtility
    {
        public static string ConvertRarityToString(double? rarity)
        {
            if (!rarity.HasValue)
            {
                return "unknown";
            }
            else if (rarity.Value >= 0 && rarity.Value < 1)
            {
                return "Brons";
            }
            else if (rarity.Value >= 1 && rarity.Value < 2)
            {
                return "Silver";
            }
            else if (rarity.Value >= 2 && rarity.Value < 3)
            {
                return "Guld";
            }
            else if (rarity.Value >= 3 && rarity.Value <= 4)
            {
                return "Platinum";
            }
            else
            {
                return "unknown";
            }
        }
    }
}
