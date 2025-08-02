using DCFApixels.DragonECS;
using UnityEngine;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = NAME, menuName = EcsConsts.FRAMEWORK_NAME + "/Providers/" + NAME, order = 1)]
    public class EcsEventWorldSingletonProvider : EcsWorldProvider<EcsEventWorld>
    {
        private const string NAME = "SingletonEventWorld";
        private static EcsEventWorldSingletonProvider _instance;
        public static EcsEventWorldSingletonProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindOrCreateSingleton<EcsEventWorldSingletonProvider>(NAME);
                }
                return _instance;
            }
        }
        protected override EcsEventWorld BuildWorld(ConfigContainer configs) { return new EcsEventWorld(configs, WorldName, WorldID); }
    }
}