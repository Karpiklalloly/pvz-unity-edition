using UnityEngine;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "PlantCardConfig", menuName = "TowerDefence/Plant Card Config", order = 0)]
    public class PlantCardConfig : ScriptableObject
    {
        public PlantConfig PlantConfig;
        public Sprite Icon;
        public int Cost;
        public float Cooldown;

        private void OnValidate()
        {
            if (PlantConfig != null)
            {
                PlantConfig.PlantCardConfig = this;
            }
        }
    }
}