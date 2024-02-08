using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class UnitData
    {
        public string id;
        public string name;
        public string description;
        public int health;


        public int attackRange;
        public int attackDamage;
        public float attackRate;

        public string prefabPath;
    }
}
