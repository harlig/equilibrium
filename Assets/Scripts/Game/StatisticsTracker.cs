using System;
using System.Collections.Generic;
using System.Globalization;

public class StatisticsTracker
{
    public readonly Dictionary<StatisticType, Statistic> statistics;

    public StatisticsTracker()
    {
        Dictionary<StatisticType, Statistic> registeredStats = new();
        foreach (StatisticType statType in Enum.GetValues(typeof(StatisticType)))
        {
            registeredStats[statType] = new Statistic(statType);
        }
        statistics = registeredStats;
    }

    public void Increment(StatisticType statType, float value = 1)
    {
        statistics[statType].StatValue += value;
    }

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

            return $"{string.Join(" ", statTypeWords)}: {string.Format("{0:N0}", StatValue)}";
        }
    }

    public enum StatisticType
    {
        DAMAGE_DEALT,
        ENEMIES_DEFEATED,
        XP_COLLECTED,
        ORBS_COLLECTED,
        OFFERS_COLLECTED,
        HP_DROPS_COLLECTED,
        CHESTS_OPENED,
        ROOMS_CLEARED,
        FLOORS_VISITED,
        NUM_DEATHS
    }
}
