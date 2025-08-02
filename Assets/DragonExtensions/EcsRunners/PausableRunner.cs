using DCFApixels.DragonECS;
using DCFApixels.DragonECS.RunnersCore;
using TowerDefense;
using UnityEngine;

namespace Karpik.Engine.Shared.EcsRunners
{
    public interface IEcsPausableRun : IEcsProcess
    {
        public void PausableRun();
    }

    public class EcsPausableRunner : EcsRunner<IEcsPausableRun>, IEcsPausableRun
    {
        public void PausableRun()
        {
            if (GameTime.IsPaused) return;
            
            foreach (var process in Process)
            {
                process.PausableRun();
            }
        }
    }

    public interface IEcsPausableLateRun : IEcsProcess
    {
        public void PausableLateRun();
    }

    public sealed class PausableLateRunner : EcsRunner<IEcsPausableLateRun>, IEcsPausableLateRun
    {
        public void PausableLateRun()
        {
            if (GameTime.IsPaused) return;
            
            foreach (var process in Process)
            {
                process.PausableLateRun();
            }
        }
    }
}