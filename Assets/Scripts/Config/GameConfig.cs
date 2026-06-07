using System.Collections.Generic;
using UnityEngine;

namespace CatWar
{
    public static class GameConfig
    {
        public const int TILE_SIZE = 32;
        public const int MAP_WIDTH = 80;
        public const int MAP_HEIGHT = 80;
        public const float CAMERA_SPEED = 8f;
        public const float CAMERA_EDGE_SCROLL_ZONE = 30f;
        public const float ZOOM_MIN = 0.5f;
        public const float ZOOM_MAX = 2.0f;
        public const int POPULATION_CAP = 50;

        public struct ResourceCost
        {
            public int gold;
            public int wood;
            public int stone;

            public ResourceCost(int gold = 0, int wood = 0, int stone = 0)
            {
                this.gold = gold;
                this.wood = wood;
                this.stone = stone;
            }
        }

        public struct UnitStats
        {
            public string type;
            public int hp;
            public int damage;
            public float speed;
            public int range;
            public ResourceCost cost;
            public float trainTime;
            public int popCost;
            public bool isFlyer;
            public bool isWaterOnly;
            public bool canDetectStealth;
            public float gatherRate;
            public float visionRange;
            public string description;
        }

        public struct BuildingStats
        {
            public string type;
            public int hp;
            public ResourceCost? cost; // null denotes unbuildable (like CASTLE_KEEP)
            public float buildTime;
            public Vector2Int size;
            public string[] trains;
            public int visionRange;
            public int popProvided;
            public float claimRadius;
            public float gatherBonus;
            public float attackDamage;
            public float attackRange;
            public bool isDock;
            public bool isDropOff;
            public string[] dropOffTypes;
            public bool isWall;
            public bool isGate;
            public bool isBridge;
            public bool isDrawbridge;
            public bool autoSpawn;
            public float autoSpawnTime;
            public string autoSpawnTarget;
            public string description;
        }

        public static readonly Dictionary<string, UnitStats> Units = new Dictionary<string, UnitStats>()
        {
            {
                "HEAD_MINER", new UnitStats {
                    type = "HEAD_MINER", hp = 25, damage = 4, speed = 2.2f, range = 0,
                    cost = new ResourceCost(60, 0, 0), trainTime = 15f, popCost = 1,
                    gatherRate = 1.5f, visionRange = 5f, description = "Special elite peasant - mines and builds 50% faster"
                }
            },
            {
                "PEASANT", new UnitStats {
                    type = "PEASANT", hp = 20, damage = 3, speed = 2.0f, range = 0,
                    cost = new ResourceCost(50, 0, 0), trainTime = 15f, popCost = 1,
                    gatherRate = 1.0f, visionRange = 5f, description = "Basic worker - gathers resources and constructs buildings"
                }
            },
            {
                "SWORDSCAT", new UnitStats {
                    type = "SWORDSCAT", hp = 40, damage = 6, speed = 1.8f, range = 0,
                    cost = new ResourceCost(80, 20, 0), trainTime = 20f, popCost = 1,
                    visionRange = 5f, description = "Fierce melee warrior with sword and shield"
                }
            },
            {
                "SPEARCAT", new UnitStats {
                    type = "SPEARCAT", hp = 35, damage = 8, speed = 1.6f, range = 0,
                    cost = new ResourceCost(70, 30, 0), trainTime = 22f, popCost = 1,
                    visionRange = 5f, description = "Melee defender - deals double damage vs Cavalry"
                }
            },
            {
                "ARCHER", new UnitStats {
                    type = "ARCHER", hp = 25, damage = 5, speed = 1.9f, range = 6,
                    cost = new ResourceCost(60, 40, 0), trainTime = 18f, popCost = 1,
                    visionRange = 7f, description = "Basic ranged combatant with bow"
                }
            },
            {
                "CROSSBOW", new UnitStats {
                    type = "CROSSBOW", hp = 30, damage = 9, speed = 1.5f, range = 7,
                    cost = new ResourceCost(80, 50, 0), trainTime = 25f, popCost = 1,
                    visionRange = 8f, description = "Heavy ranged unit - slow but high damage and range"
                }
            },
            {
                "BIPLANE", new UnitStats {
                    type = "BIPLANE", hp = 35, damage = 6, speed = 3.5f, range = 6,
                    cost = new ResourceCost(120, 80, 0), trainTime = 28f, popCost = 2,
                    isFlyer = true, visionRange = 8f, description = "Biplane Cat - aerial fighter that flies over all terrain and strafes targets with rapid machine guns!"
                }
            },
            {
                "KNIGHT", new UnitStats {
                    type = "KNIGHT", hp = 75, damage = 10, speed = 1.4f, range = 0,
                    cost = new ResourceCost(150, 60, 40), trainTime = 35f, popCost = 2,
                    visionRange = 5f, description = "Heavily armored melee fighter"
                }
            },
            {
                "CAVALRY", new UnitStats {
                    type = "CAVALRY", hp = 60, damage = 9, speed = 3.0f, range = 0,
                    cost = new ResourceCost(120, 40, 0), trainTime = 30f, popCost = 2,
                    visionRange = 6f, description = "Extremely fast mounted shock cavalry"
                }
            },
            {
                "HEALER", new UnitStats {
                    type = "HEALER", hp = 22, damage = 2, speed = 1.7f, range = 4,
                    cost = new ResourceCost(100, 30, 0), trainTime = 28f, popCost = 1,
                    visionRange = 6f, description = "Support unit - heals nearby wounded friendly units"
                }
            },
            {
                "CATAPULT", new UnitStats {
                    type = "CATAPULT", hp = 40, damage = 20, speed = 0.8f, range = 10,
                    cost = new ResourceCost(200, 100, 80), trainTime = 45f, popCost = 3,
                    visionRange = 11f, description = "Devastating siege engine with area damage and high range"
                }
            },
            {
                "ROYAL_COMMANDER", new UnitStats {
                    type = "ROYAL_COMMANDER", hp = 125, damage = 13, speed = 1.6f, range = 0,
                    cost = new ResourceCost(300, 100, 100), trainTime = 60f, popCost = 3,
                    visionRange = 6f, description = "Elite leader - grants nearby units 15% damage bonus aura"
                }
            },
            {
                "SCOUT", new UnitStats {
                    type = "SCOUT", hp = 15, damage = 2, speed = 3.5f, range = 0,
                    cost = new ResourceCost(30, 0, 0), trainTime = 10f, popCost = 1,
                    canDetectStealth = true, visionRange = 8f, description = "Fast scout - extended vision, detects stealth units"
                }
            },
            {
                "TRANSPORT_SHIP", new UnitStats {
                    type = "TRANSPORT_SHIP", hp = 150, damage = 0, speed = 4.5f, range = 0,
                    cost = new ResourceCost(80, 120, 0), trainTime = 20f, popCost = 2,
                    isWaterOnly = true, visionRange = 6f, description = "Coastal transport - carries up to 10 land units across water!"
                }
            },
            {
                "WARSHIP", new UnitStats {
                    type = "WARSHIP", hp = 200, damage = 8, speed = 3.5f, range = 8,
                    cost = new ResourceCost(150, 100, 0), trainTime = 30f, popCost = 3,
                    isWaterOnly = true, visionRange = 8f, description = "Reinforced warship - fires heavy arrow bolts at sea or land targets!"
                }
            }
        };

        public static readonly Dictionary<string, BuildingStats> Buildings = new Dictionary<string, BuildingStats>()
        {
            {
                "CASTLE_KEEP", new BuildingStats {
                    type = "CASTLE_KEEP", hp = 2000, cost = null, buildTime = 0f,
                    size = new Vector2Int(3, 3), trains = new string[] { "SCOUT" },
                    claimRadius = 8f, visionRange = 12, popProvided = 10,
                    description = "Main base - produces workers and provides population"
                }
            },
            {
                "BARRACKS", new BuildingStats {
                    type = "BARRACKS", hp = 800, cost = new ResourceCost(0, 100, 0), buildTime = 15f,
                    size = new Vector2Int(2, 2), trains = new string[] { "SWORDSCAT", "SPEARCAT", "KNIGHT" },
                    visionRange = 6, popProvided = 0, description = "Trains melee infantry units"
                }
            },
            {
                "ARCHERY_RANGE", new BuildingStats {
                    type = "ARCHERY_RANGE", hp = 600, cost = new ResourceCost(0, 80, 0), buildTime = 12f,
                    size = new Vector2Int(2, 2), trains = new string[] { "ARCHER", "CROSSBOW", "BIPLANE" },
                    visionRange = 6, popProvided = 0, description = "Trains ranged units"
                }
            },
            {
                "BLACKSMITH", new BuildingStats {
                    type = "BLACKSMITH", hp = 500, cost = new ResourceCost(0, 0, 150), buildTime = 18f,
                    size = new Vector2Int(2, 2), trains = new string[] { },
                    visionRange = 5, popProvided = 0, description = "Unlocks unit upgrades"
                }
            },
            {
                "STABLE", new BuildingStats {
                    type = "STABLE", hp = 700, cost = new ResourceCost(0, 100, 0), buildTime = 15f,
                    size = new Vector2Int(2, 2), trains = new string[] { "CAVALRY" },
                    visionRange = 6, popProvided = 0, description = "Trains mounted units"
                }
            },
            {
                "SIEGE_WORKSHOP", new BuildingStats {
                    type = "SIEGE_WORKSHOP", hp = 600, cost = new ResourceCost(0, 150, 100), buildTime = 20f,
                    size = new Vector2Int(3, 2), trains = new string[] { "CATAPULT" },
                    visionRange = 5, popProvided = 0, description = "Constructs siege weapons"
                }
            },
            {
                "FARM", new BuildingStats {
                    type = "FARM", hp = 300, cost = new ResourceCost(0, 50, 0), buildTime = 10f,
                    size = new Vector2Int(2, 2), trains = new string[] { },
                    visionRange = 4, popProvided = 20, description = "Produces food and increases population cap"
                }
            },
            {
                "LUMBER_MILL", new BuildingStats {
                    type = "LUMBER_MILL", hp = 400, cost = new ResourceCost(0, 30, 0), buildTime = 10f,
                    size = new Vector2Int(2, 2), trains = new string[] { },
                    gatherBonus = 0.2f, visionRange = 5, popProvided = 0,
                    description = "Increases wood gathering efficiency by 20%"
                }
            },
            {
                "STONE_QUARRY", new BuildingStats {
                    type = "STONE_QUARRY", hp = 400, cost = new ResourceCost(0, 50, 0), buildTime = 12f,
                    size = new Vector2Int(2, 2), trains = new string[] { },
                    gatherBonus = 0.2f, visionRange = 5, popProvided = 0,
                    description = "Increases stone gathering efficiency by 20%"
                }
            },
            {
                "WATCHTOWER", new BuildingStats {
                    type = "WATCHTOWER", hp = 350, cost = new ResourceCost(0, 0, 60), buildTime = 10f,
                    size = new Vector2Int(1, 1), trains = new string[] { },
                    attackDamage = 5f, attackRange = 7f, visionRange = 10, claimRadius = 10f, popProvided = 0,
                    description = "Defensive tower — attacks nearby enemies"
                }
            },
            {
                "DOCK", new BuildingStats {
                    type = "DOCK", hp = 600, cost = new ResourceCost(0, 150, 0), buildTime = 12f,
                    size = new Vector2Int(2, 2), trains = new string[] { "TRANSPORT_SHIP", "WARSHIP" },
                    isDock = true, isDropOff = true, dropOffTypes = new string[] { "GOLD", "WOOD", "STONE" },
                    visionRange = 6, popProvided = 0, description = "Coastal shipyard — drop-off point for all resources and trains naval vessels!"
                }
            },
            {
                "WALL", new BuildingStats {
                    type = "WALL", hp = 500, cost = new ResourceCost(0, 0, 10), buildTime = 5f,
                    size = new Vector2Int(1, 1), trains = new string[] { },
                    visionRange = 2, popProvided = 0, isWall = true,
                    description = "Stone wall — cheap and tough, blocks enemy movement"
                }
            },
            {
                "GATE", new BuildingStats {
                    type = "GATE", hp = 400, cost = new ResourceCost(0, 10, 20), buildTime = 8f,
                    size = new Vector2Int(1, 1), trains = new string[] { },
                    visionRange = 2, popProvided = 0, isWall = true, isGate = true,
                    description = "Gate — friendly units pass through, blocks enemies"
                }
            },
            {
                "BRIDGE", new BuildingStats {
                    type = "BRIDGE", hp = 400, cost = new ResourceCost(0, 40, 0), buildTime = 10f,
                    size = new Vector2Int(1, 1), trains = new string[] { },
                    visionRange = 3, popProvided = 0, isBridge = true,
                    description = "Wooden bridge — allows land units to cross over water!"
                }
            },
            {
                "DRAWBRIDGE", new BuildingStats {
                    type = "DRAWBRIDGE", hp = 600, cost = new ResourceCost(0, 80, 40), buildTime = 8f,
                    size = new Vector2Int(1, 1), trains = new string[] { },
                    visionRange = 4, popProvided = 0, isDrawbridge = true,
                    description = "Drawbridge — opens to let friendly ships pass, closes to let ground units cross!"
                }
            },
            {
                "HOUSE", new BuildingStats {
                    type = "HOUSE", hp = 400, cost = new ResourceCost(0, 60, 0), buildTime = 12f,
                    size = new Vector2Int(2, 2), trains = new string[] { },
                    visionRange = 4, popProvided = 5, autoSpawn = true, autoSpawnTime = 30f, autoSpawnTarget = "combat",
                    description = "Cat house — auto-spawns a cat every 30s that trains at the nearest combat building"
                }
            },
            {
                "BUILDERS_HOUSE", new BuildingStats {
                    type = "BUILDERS_HOUSE", hp = 350, cost = new ResourceCost(0, 40, 0), buildTime = 10f,
                    size = new Vector2Int(2, 2), trains = new string[] { },
                    visionRange = 4, popProvided = 5, autoSpawn = true, autoSpawnTime = 30f, autoSpawnTarget = "builder",
                    description = "Builder house — auto-spawns a peasant builder every 30s"
                }
            }
        };
    }
}
