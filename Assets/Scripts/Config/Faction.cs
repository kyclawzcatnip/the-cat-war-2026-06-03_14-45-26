using System.Collections.Generic;
using UnityEngine;

namespace CatWar
{
    public enum FactionId
    {
        None,
        LION,
        SIAMESE,
        MAINE_COON,
        BLACK_CAT,
        PERSIAN
    }

    public struct FactionColors
    {
        public Color primary;
        public Color secondary;

        public FactionColors(string primHex, string secHex)
        {
            ColorUtility.TryParseHtmlString(primHex, out primary);
            ColorUtility.TryParseHtmlString(secHex, out secondary);
        }
    }

    public struct FactionBonuses
    {
        public float meleeDamage;
        public float unitHP;
        public float rangedDamage;
        public float unitSpeed;
        public float siegeDamage;
        public float buildingHP;
        public float gatherRate;
        public float unitCost;
        public int rangeBonus;
        public float stealthRange;
        public float firstStrikeDamage;
        public float scoutSpeedBonus;
        public int freeScouts;
    }

    public struct AIPersonality
    {
        public string type;
        public string description;
        public string rushTiming;
        public int armyThreshold;
        public float retreatThreshold;
        public string[] preferredUnits;
        public int economyTarget;
        public float expansionPriority;
        public float aggressionLevel;
        public float scoutFrequency; // in seconds
    }

    public struct FactionInfo
    {
        public FactionId id;
        public string name;
        public string shortName;
        public FactionColors colors;
        public string icon;
        public string lore;
        public string[] strengths;
        public FactionBonuses bonuses;
        public AIPersonality personality;
    }

    public static class Factions
    {
        private static readonly Dictionary<FactionId, FactionInfo> FactionsData = new Dictionary<FactionId, FactionInfo>()
        {
            {
                FactionId.LION, new FactionInfo
                {
                    id = FactionId.LION,
                    name = "Lion Cats",
                    shortName = "Lion",
                    colors = new FactionColors("#DAA520", "#8B0000"),
                    icon = "🦁",
                    lore = "The proud Lion Cats rule from their golden fortress. Their warriors are the fiercest in all the kingdoms.",
                    strengths = new string[] { "Melee combat", "Early aggression" },
                    bonuses = new FactionBonuses
                    {
                        meleeDamage = 1.20f,
                        unitHP = 1.15f,
                        rangedDamage = 1.0f,
                        unitSpeed = 1.0f,
                        siegeDamage = 1.0f,
                        buildingHP = 1.0f,
                        gatherRate = 1.0f,
                        unitCost = 1.0f,
                        rangeBonus = 0,
                        stealthRange = 0f,
                        firstStrikeDamage = 1.0f,
                        scoutSpeedBonus = 1.0f,
                        freeScouts = 0
                    },
                    personality = new AIPersonality
                    {
                        type = "aggressive",
                        description = "Aggressive, early rushes",
                        rushTiming = "early",
                        armyThreshold = 6,
                        retreatThreshold = 0.25f,
                        preferredUnits = new string[] { "KNIGHT", "SWORDSCAT" },
                        economyTarget = 4,
                        expansionPriority = 0.3f,
                        aggressionLevel = 0.9f,
                        scoutFrequency = 15f
                    }
                }
            },
            {
                FactionId.SIAMESE, new FactionInfo
                {
                    id = FactionId.SIAMESE,
                    name = "Siamese Cats",
                    shortName = "Siamese",
                    colors = new FactionColors("#4682B4", "#C0C0C0"),
                    icon = "🐱",
                    lore = "Swift and cunning, the Siamese Cats strike like lightning and vanish before the enemy can react.",
                    strengths = new string[] { "Speed", "Ranged combat" },
                    bonuses = new FactionBonuses
                    {
                        meleeDamage = 1.0f,
                        unitHP = 1.0f,
                        rangedDamage = 1.0f,
                        unitSpeed = 1.30f,
                        siegeDamage = 1.0f,
                        buildingHP = 1.0f,
                        gatherRate = 1.0f,
                        unitCost = 1.0f,
                        rangeBonus = 2,
                        stealthRange = 0f,
                        firstStrikeDamage = 1.0f,
                        scoutSpeedBonus = 1.20f,
                        freeScouts = 1
                    },
                    personality = new AIPersonality
                    {
                        type = "harasser",
                        description = "Harassing, hit-and-run",
                        rushTiming = "mid",
                        armyThreshold = 8,
                        retreatThreshold = 0.45f,
                        preferredUnits = new string[] { "ARCHER", "CROSSBOW" },
                        economyTarget = 5,
                        expansionPriority = 0.5f,
                        aggressionLevel = 0.6f,
                        scoutFrequency = 10f
                    }
                }
            },
            {
                FactionId.MAINE_COON, new FactionInfo
                {
                    id = FactionId.MAINE_COON,
                    name = "Maine Coon Cats",
                    shortName = "Maine Coon",
                    colors = new FactionColors("#2E8B57", "#8B4513"),
                    icon = "🐈",
                    lore = "The mighty Maine Coons build impregnable fortresses and crush their foes with devastating siege weapons.",
                    strengths = new string[] { "Defense", "Siege" },
                    bonuses = new FactionBonuses
                    {
                        meleeDamage = 1.0f,
                        unitHP = 1.0f,
                        rangedDamage = 1.0f,
                        unitSpeed = 1.0f,
                        siegeDamage = 1.25f,
                        buildingHP = 1.20f,
                        gatherRate = 1.0f,
                        unitCost = 1.0f,
                        rangeBonus = 0,
                        stealthRange = 0f,
                        firstStrikeDamage = 1.0f,
                        scoutSpeedBonus = 1.0f,
                        freeScouts = 0
                    },
                    personality = new AIPersonality
                    {
                        type = "defensive",
                        description = "Defensive, late-game powerhouse",
                        rushTiming = "late",
                        armyThreshold = 15,
                        retreatThreshold = 0.35f,
                        preferredUnits = new string[] { "CATAPULT", "SWORDSCAT" },
                        economyTarget = 7,
                        expansionPriority = 0.7f,
                        aggressionLevel = 0.3f,
                        scoutFrequency = 20f
                    }
                }
            },
            {
                FactionId.BLACK_CAT, new FactionInfo
                {
                    id = FactionId.BLACK_CAT,
                    name = "Black Cat Kingdom",
                    shortName = "Black Cat",
                    colors = new FactionColors("#6A0DAD", "#1C1C1C"),
                    icon = "🐈‍⬛",
                    lore = "The shadowy Black Cat Kingdom moves unseen through the night. Their enemies never hear the final meow.",
                    strengths = new string[] { "Stealth", "Ambush" },
                    bonuses = new FactionBonuses
                    {
                        meleeDamage = 1.0f,
                        unitHP = 1.0f,
                        rangedDamage = 1.0f,
                        unitSpeed = 1.0f,
                        siegeDamage = 1.0f,
                        buildingHP = 1.0f,
                        gatherRate = 1.0f,
                        unitCost = 1.0f,
                        rangeBonus = 0,
                        stealthRange = 4.0f,
                        firstStrikeDamage = 1.20f,
                        scoutSpeedBonus = 1.0f,
                        freeScouts = 0
                    },
                    personality = new AIPersonality
                    {
                        type = "sneaky",
                        description = "Sneaky, guerrilla warfare, ambush tactics",
                        rushTiming = "mid",
                        armyThreshold = 5,
                        retreatThreshold = 0.40f,
                        preferredUnits = new string[] { "SWORDSCAT", "ARCHER" },
                        economyTarget = 5,
                        expansionPriority = 0.4f,
                        aggressionLevel = 0.7f,
                        scoutFrequency = 8f
                    }
                }
            },
            {
                FactionId.PERSIAN, new FactionInfo
                {
                    id = FactionId.PERSIAN,
                    name = "Persian Cat Empire",
                    shortName = "Persian",
                    colors = new FactionColors("#FFFFF0", "#FFD700"),
                    icon = "👑",
                    lore = "The wealthy Persian Empire crushes opposition through sheer economic might and endless armies.",
                    strengths = new string[] { "Economy", "Numbers" },
                    bonuses = new FactionBonuses
                    {
                        meleeDamage = 1.0f,
                        unitHP = 1.0f,
                        rangedDamage = 1.0f,
                        unitSpeed = 1.0f,
                        siegeDamage = 1.0f,
                        buildingHP = 1.0f,
                        gatherRate = 1.30f,
                        unitCost = 0.85f,
                        rangeBonus = 0,
                        stealthRange = 0f,
                        firstStrikeDamage = 1.0f,
                        scoutSpeedBonus = 1.0f,
                        freeScouts = 0
                    },
                    personality = new AIPersonality
                    {
                        type = "economic",
                        description = "Economic boom, overwhelming numbers",
                        rushTiming = "late",
                        armyThreshold = 20,
                        retreatThreshold = 0.30f,
                        preferredUnits = new string[] { "SWORDSCAT", "ARCHER", "SPEARCAT" },
                        economyTarget = 10,
                        expansionPriority = 0.9f,
                        aggressionLevel = 0.4f,
                        scoutFrequency = 18f
                    }
                }
            }
        };

        public static FactionInfo GetInfo(FactionId id)
        {
            if (FactionsData.TryGetValue(id, out var info))
                return info;
            return default;
        }

        public static FactionColors GetColor(FactionId id)
        {
            return GetInfo(id).colors;
        }

        public static float GetBonus(FactionId id, string statType)
        {
            if (id == FactionId.None) return 1.0f;
            var b = GetInfo(id).bonuses;
            switch (statType)
            {
                case "meleeDamage": return b.meleeDamage;
                case "unitHP": return b.unitHP;
                case "rangedDamage": return b.rangedDamage;
                case "unitSpeed": return b.unitSpeed;
                case "siegeDamage": return b.siegeDamage;
                case "buildingHP": return b.buildingHP;
                case "gatherRate": return b.gatherRate;
                case "unitCost": return b.unitCost;
                case "firstStrikeDamage": return b.firstStrikeDamage;
                case "scoutSpeedBonus": return b.scoutSpeedBonus;
                default: return 1.0f;
            }
        }
    }
}
