using System;
using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;

namespace TowerDefense.Core
{
    public class LevelInitializationSystem : IEcsRunOnEvent<SceneLoadedEvent>, IEcsInject<EcsDefaultWorld>
    {
        private EcsDefaultWorld _world;
        public void RunOnEvent(ref SceneLoadedEvent evt)
        {
            if (!evt.SceneName.Contains("PlayScene"))
            {
                evt.LevelConfig = null;
                GameTime.IsPaused = true;
                return;
            }

            if (evt.LevelConfig == null)
            {
                throw new InvalidProgramException("No LevelConfig provided in SceneLoadedEvent.");
            }

            GameTime.IsPaused = false;
            
            _world.Get<CurrentLevel>() = new CurrentLevel()
            {
                Congig = evt.LevelConfig,
            };
            _world.Get<PlayerData>().SunAmount = evt.LevelConfig.StartSun;
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
        }
    }
}