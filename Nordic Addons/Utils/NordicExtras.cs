using UnityEngine;

public enum ClassColors
{
    Gladiator,
    Bard,
    Archer,
    Sorcerer,
    Paladin,
    Priest,
    Thief,
    Necromancer,
    Warrior
}

public static class ClassColorsExtension
{
    public static Color32 GetColor(this ClassColors classColor)
    {
        switch (classColor)
        {
            case ClassColors.Gladiator:
                return new Color32(196, 30, 58, 255);
            case ClassColors.Bard:
                return new Color32(51, 147, 127, 255);
            case ClassColors.Archer:
                return new Color32(170, 211, 114, 255);
            case ClassColors.Sorcerer:
                return new Color32(63, 199, 235, 255);
            case ClassColors.Paladin:
                return new Color32(244, 140, 186, 255);
            case ClassColors.Priest:
                return new Color32(255, 255, 255, 255);
            case ClassColors.Thief:
                return new Color32(255, 244, 104, 255);
            case ClassColors.Necromancer:
                return new Color32(135, 136, 238, 255);
            case ClassColors.Warrior:
                return new Color32(198, 155, 109, 255);
            default:
                return new Color32(0, 0, 0, 255);
        }
    }

    public static void SetColor(this ClassColors classColor, ref Color color)
    {
        color = GetColor(classColor);
    }

    public static void SetColor(this ClassColors classColor, ref Color32 color)
    {
        color = GetColor(classColor);
    }
}

public enum ItemQuality
{
    Poor,
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Artifact,
    Heirloom,
}

public static class ItemQualityExtensions
{
    public static Color32 GetColor(this ItemQuality quality)
    {
        switch (quality)
        {
            case ItemQuality.Poor:
                return new Color32(157, 157, 157, 255);
            case ItemQuality.Common:
                return new Color32(255, 255, 255, 255);
            case ItemQuality.Uncommon:
                return new Color32(30, 255, 0, 255);
            case ItemQuality.Rare:
                return new Color32(0, 112, 221, 255);
            case ItemQuality.Epic:
                return new Color32(163, 53, 238, 255);
            case ItemQuality.Legendary:
                return new Color32(255, 128, 0, 255);
            case ItemQuality.Artifact:
                return new Color32(230, 204, 128, 255);
            case ItemQuality.Heirloom:
                return new Color32(0, 204, 255, 255);

            default:
                return new Color32(0, 0, 0, 255);
        }
    }

    public static void SetColor(this ItemQuality quality, ref Color color)
    {
        color = quality.GetColor();
    }

    public static void SetColor(this ItemQuality quality, ref Color32 color)
    {
        color = quality.GetColor();
    }
}