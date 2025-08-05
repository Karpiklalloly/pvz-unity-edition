using System;
using System.Linq;
using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using Karpik.Engine.Shared.EcsRunners;
using TowerDefense.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TowerDefense.Core
{
    public class GameUIUpdateSystem : IEcsInject<EcsDefaultWorld>, IEcsInit, IEcsPausableRun, IEcsRunOnEvent<SceneLoadedEvent>, IEcsRunOnEvent<NewWaveEvent>, IEcsRunOnEvent<WinEvent>, IEcsRunOnEvent<LoseEvent>
    {
        private GameUIController _controller;
        private int _suns = 0;
        private EcsDefaultWorld _world;
        private PlantConfig[] _allPlants;

        public void PausableRun()
        {
            if (_controller == null) return;

            var suns = _world.Get<PlayerData>().SunAmount;
            if (suns != _suns)
            {
                _suns = suns;
                _controller.Sun = _suns;
            }
            
            foreach (var card in _controller.ActiveCards)
            {
                if (card.CooldownTimer > 0)
                {
                    card.CooldownTimer -= GameTime.DeltaTime;
                    float progress = card.CooldownTimer / card.Config.PlantCardConfig.Cooldown;
                    card.CooldownOverlay.style.height = Length.Percent(progress * 100);

                    if (card.CooldownTimer <= 0)
                    {
                        card.Root.RemoveFromClassList("disabled");
                    }
                }
                else if (card.Root.ClassListContains("disabled"))
                {
                    card.Root.RemoveFromClassList("disabled");
                }
            }
        }

        public void RunOnEvent(ref SceneLoadedEvent evt)
        {
            _controller = Object.FindFirstObjectByType<GameUIController>();
            if (_controller == null) return;
            _controller.Init(evt.LevelConfig, _allPlants, 
                UserData.GetStrings(PlayerPrefsConstants.unlocked_plants, Array.Empty<string>())
                .Select(name => _allPlants.FirstOrDefault(x => x.Name == name))
                .Where(static x => x is not null));
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
        }

        public void Init()
        {
            var handle = Addressables.LoadAssetsAsync<PlantConfig>("Plant", null, Addressables.MergeMode.Union);
            handle.Completed += (x) => _allPlants = x.Result.ToArray();
        }

        public void RunOnEvent(ref NewWaveEvent evt)
        {
            _controller.OnNewWave(evt);
        }

        public void RunOnEvent(ref WinEvent evt)
        {
            
        }

        public void RunOnEvent(ref LoseEvent evt)
        {
            
        }
    }
}