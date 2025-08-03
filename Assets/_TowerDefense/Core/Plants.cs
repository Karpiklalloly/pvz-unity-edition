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
}