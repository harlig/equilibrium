using System.Collections.Generic;
using System.Globalization;

public class StatisticsTracker
{
    public List<Statistic> statistics = new();

    public class Statistic
    {
        private readonly StatisticType statType;
        public float StatValue { get; set; }

        public Statistic(StatisticType statisticType)
        {
            statType = statisticType;
        }

        public string ToStatString()
        {
            // Split the string on underscores
            string[] statTypeWords = statType.ToString().Split('_');

            // Capitalize the first letter of each word and convert the rest to lower case
            for (int i = 0; i < statTypeWords.Length; i++)
            {
                statTypeWords[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                    statTypeWords[i].ToLower()
                );
            }

            return $"{string.Join(" ", statTypeWords)}: {string.Format("0:N0", StatValue)}";
        }
    }

    public enum StatisticType
    {
        DAMAGE_DEALT,
        XP_COLLECTED,
        ORBS_COLLECTED,
        OFFERS_COLLECTED,
        CHESTS_FOUND,
        ROOMS_CLEARED,
        FLOORS_CLEARED
    }
}
