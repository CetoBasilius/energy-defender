using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class EnemiesData : Dictionary<string, EnemyData>
    {
    }
    [Serializable]

    public class EnemyData
    {
        public string name;
        public string description;
        public int health;
        public float speed;
        public int attacksTowers;
        public int range;
        public int attackDamage;
        public float attackRate;
    }
}
