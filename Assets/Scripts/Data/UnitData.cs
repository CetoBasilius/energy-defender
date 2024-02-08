using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class UnitData
    {
        public string name;
        public string description;
        public int health;


        public int range;
        public int attackDamage;
        public float attackRate;

        public string prefabPath;
    }
}
