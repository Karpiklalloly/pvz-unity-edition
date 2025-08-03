using System;
using DCFApixels.DragonECS;

namespace TowerDefense.Core
{
    [Serializable]
    public struct PeaShooter : IEcsTagComponent
    {
    }

    public class PeaShooterTemplate : TagComponentTemplate<PeaShooter>
    {
    }

    [Serializable]
    public struct Sunflower : IEcsTagComponent
    {
    }

    public class SunflowerTemplate : TagComponentTemplate<Sunflower>
    {
    }
}