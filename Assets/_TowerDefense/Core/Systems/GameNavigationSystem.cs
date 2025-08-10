using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.Core
{
    public class GameNavigationSystem : IEcsInject<EcsEventWorld>, IEcsInject<EcsDefaultWorld>, IEcsRunOnEvent<NextLevelEvent>, IEcsRunOnEvent<RestartLevelEvent>
    {
        private EcsEventWorld _eventWorld;
        private EcsDefaultWorld _world;

        public void Inject(EcsEventWorld obj)
        {
            _eventWorld = obj;
        }

        public void RunOnEvent(ref NextLevelEvent evt)
        {
            // TODO: Load next level
            Debug.Log("Loading next level...");
            // For now, just restart the current level
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void RunOnEvent(ref RestartLevelEvent evt)
        {
            var config = _world.Get<CurrentLevel>().Congig;
            _world.Clear(true);
            
            Debug.Log("Restarting level...");
            _eventWorld.SendEvent(new LoadSceneEvent()
            {
                LevelConfig = config,
                SceneName = SceneManager.GetActiveScene().name
            });
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
        }
    }
}