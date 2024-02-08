using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class TowerData : UnitData
    {
        public float specialRate;
        public int specialRange;
        public int maxAttackUnits;
        public int energyCost;
        public int maxQuantity;
    }
}
