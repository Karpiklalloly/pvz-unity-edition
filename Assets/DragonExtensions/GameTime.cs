using UnityEngine;

namespace TowerDefense
{
    public static class GameTime
    {
        public static float DeltaTime => Time.deltaTime;
        public static float FixedDeltaTime => Time.fixedDeltaTime;
        public static bool IsPaused { get; set; }
    }
}