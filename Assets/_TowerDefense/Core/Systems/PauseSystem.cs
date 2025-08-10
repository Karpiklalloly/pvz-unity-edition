using Karpik.Engine.Shared.DragonECS;

namespace TowerDefense.Core
{
    public class PauseSystem : IEcsRunOnEvent<WinEvent>, IEcsRunOnEvent<LoseEvent>
    {
        public void RunOnEvent(ref WinEvent evt)
        {
            GameTime.IsPaused = true;
        }

        public void RunOnEvent(ref LoseEvent evt)
        {
            GameTime.IsPaused = true;
        }
    }
}