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
    public class GameUIUpdateSystem : IEcsInject<EcsDefaultWorld>, IEcsInject<EcsEventWorld>, IEcsInit, IEcsPausableRun, IEcsRunOnEvent<SceneLoadedEvent>, IEcsRunOnEvent<NewWaveEvent>, IEcsRunOnEvent<WinEvent>, IEcsRunOnEvent<LoseEvent>, IEcsRunOnEvent<StartLevelEvent>, IEcsRunOnEvent<ZombieDiedEvent>, IEcsRunOnEvent<CollectSunEvent>
    {
        private GameUIController _controller;
        private int _suns = 0;
        private EcsDefaultWorld _world;
        private EcsEventWorld _eventWorld;
        private PlantConfig[] _allPlants;
        private int _zombiesKilled = 0;
        private int _sunCollected = 0;

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
            
            // Reset stats
            _zombiesKilled = 0;
            _sunCollected = 0;
            
            // Show plant selection menu with unlocked plants
            var unlockedPlants = UserData.GetStrings(PlayerPrefsConstants.unlocked_plants, Array.Empty<string>())
                .Select(name => _allPlants.FirstOrDefault(x => x.Name == name))
                .Where(static x => x is not null);
            
            _controller.ShowPlantSelectionMenu(unlockedPlants);
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
        }

        public void Inject(EcsEventWorld obj)
        {
            _eventWorld = obj;
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

        public void RunOnEvent(ref StartLevelEvent evt)
        {
            if (_controller == null) return;
            
            // Initialize game UI with selected plants
            _controller.Init(_world.Get<CurrentLevel>().Congig, evt.SelectedPlants);
        }

        public void RunOnEvent(ref WinEvent evt)
        {
            if (_controller == null) return;
            
            var currentLevel = _world.Get<CurrentLevel>();
            var wavesCompleted = currentLevel.Congig.Waves.Count;
            
            _controller.ShowVictoryWindow(wavesCompleted, _zombiesKilled, _sunCollected);
        }

        public void RunOnEvent(ref LoseEvent evt)
        {
            if (_controller == null) return;
            
            var currentLevel = _world.Get<CurrentLevel>();
            var levelFlow = _world.Get<LevelFlow>();
            var wavesCompleted = currentLevel.Congig.Waves.Count - levelFlow.Waves.Count;
            
            _controller.ShowDefeatWindow(wavesCompleted, _zombiesKilled, _sunCollected);
        }

        public void RunOnEvent(ref ZombieDiedEvent evt)
        {
            _zombiesKilled++;
        }

        public void RunOnEvent(ref CollectSunEvent evt)
        {
            _sunCollected += 25; // Assuming each sun gives 25 points
        }
    }
}