using System;

public static class DamageFormat
{
    public static string Format(int value, DamageDisplaySettings settings)
    {
        return Format((double)value, settings);
    }

    public static string Format(double value, DamageDisplaySettings settings)
    {
        if (value < settings.formatThreshold)
        {
            return value.ToString(settings.numberFormat);
        }

        if (value >= 1000 && value < 1000000)
        {
            return $"{((float)value / 1000).ToString(settings.numberFormat)}{settings.thousandSuffix}";
        }

        return $"{((float)value / 1000000).ToString(settings.numberFormat)}{settings.millionSuffix}";
    }

    [Serializable]
    public struct DamageDisplaySettings
    {
        public double formatThreshold;
        public string thousandSuffix;
        public string millionSuffix;
        public string numberFormat;
        public string layoutTemplate;

        public DamageDisplaySettings(double formatThreshold, string thousandSuffix, string millionSuffix,
                                     string numberFormat, string layoutTemplate)
        {
            this.formatThreshold = formatThreshold;
            this.thousandSuffix = thousandSuffix;
            this.millionSuffix = millionSuffix;
            this.numberFormat = numberFormat;
            this.layoutTemplate = layoutTemplate;
        }
    }

    public static string FormatDamageMetrics(TotalDamageStatistics damageStats, float percentage, DamageDisplaySettings settings)
    {
        // Format and return damage metrics string
        return settings.layoutTemplate
            .Replace("DPS", Format(damageStats.DamagePerSecond, settings))
            .Replace("TOTAL", Format(damageStats.AccumulatedDamage, settings))
            .Replace("PERCENT", $"{(percentage * 100).ToString(settings.numberFormat)}%");
    }
}