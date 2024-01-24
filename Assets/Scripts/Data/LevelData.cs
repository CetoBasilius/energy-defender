using System;

namespace Data
{
    [Serializable]
    public class LevelData
    {
        WaveData[] waves;
        string[] tilemap;
    }

    [Serializable]
    public class WaveData
    {
        string name;
        float time;
        EnemyWaveData[] enemies;
    }

    [Serializable]
    public class EnemyWaveData
    {
        string name;
        int count;
        float delay;
    }
}
