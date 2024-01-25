using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class TowersData : Dictionary<string, TowerData>
    {
    }
    [Serializable]

    public class TowerData
    {
        public string name;
        public string description;
        public int health;
        public int range;
        public int damage;
        public float attackRate;
        public float specialRate;
        public int maxAttackUnits;
        public int energyCost;
        public int maxQuantity;
    }
}
