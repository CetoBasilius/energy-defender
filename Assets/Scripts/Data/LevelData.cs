using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class LevelData
    {
        public int startEnergy;
        public int maxEnergy;
        public WaveData[] waves;
        public Dictionary<string, string> tilemap;
        public string[] tiledata;
    }

    [Serializable]
    public class WaveData
    {
        public string name;
        public float time;
        public EnemyWaveData[] enemies;
    }

    [Serializable]
    public class EnemyWaveData
    {
        public string name;
        public int count;
        public float delay;
    }
}
