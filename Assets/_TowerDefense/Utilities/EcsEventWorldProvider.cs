using DCFApixels.DragonECS;
using UnityEngine;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = nameof(EcsEventWorldProvider), menuName = EcsConsts.FRAMEWORK_NAME + "/Providers/" + nameof(EcsEventWorldProvider), order = 1)]
    public class EcsEventWorldProvider : EcsWorldProvider<EcsEventWorld>
    {
        protected override EcsEventWorld BuildWorld(ConfigContainer configs) { return new EcsEventWorld(configs, WorldName, WorldID); }
    }
}