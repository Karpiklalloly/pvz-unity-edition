using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.Core
{
    public class SceneLoaderSystem : IEcsInject<EcsEventWorld>, IEcsRunOnEvent<LoadSceneEvent>, IEcsRunOnEvent<SceneLoadedEvent>
    {
        private EcsEventWorld _eventWorld;

        public void RunOnEvent(ref LoadSceneEvent evt)
        {
            SceneManager.LoadScene(evt.SceneName);
            var config = evt.LevelConfig;
            var sceneName = evt.SceneName;
            Bootstrap.Instance.RunOnNextFrame(() =>
            {
                _eventWorld.SendEvent(new SceneLoadedEvent()
                {
                    LevelConfig = config,
                    SceneName = sceneName,
                });
            });
        }

        public void RunOnEvent(ref SceneLoadedEvent evt)
        {
            Debug.Log($"Loaded scene: {evt.SceneName}");
        }

        public void Inject(EcsEventWorld obj)
        {
            _eventWorld = obj;
        }
    }
}