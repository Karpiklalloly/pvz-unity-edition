using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "Level", menuName = "TowerDefence/Level Config", order = -201)]
    public class LevelConfig : ScriptableObject
    {
        public GridConfig Grid;
        public SunConfig DefaultSun;
        public List<ZombieConfig> AvailableZombies;
        public List<WaveDefinition> Waves;
    }

    [Serializable]
    public class WaveDefinition
    {
        public float DelayBeforeStart = 5f;
        public int Budget;
        public bool IsKeyWave;
        public List<ZombieConfig> UnlockAfterThisWave;
    }
}