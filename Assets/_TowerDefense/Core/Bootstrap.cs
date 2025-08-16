using System;
using System.Collections;
using System.Linq;
using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using Karpik.Engine.Shared.EcsRunners;
using TowerDefense.Core.PlantAttackSystems;
using TriInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static TowerDefense.PlayerPrefsConstants;

namespace TowerDefense.Core
{
    public class Bootstrap : MonoBehaviour
    {
        public static Bootstrap Instance { get; private set; }

        public EcsDefaultWorld World => _world;
        public EcsEventWorld EventWorld => _eventWorld;

        private EcsDefaultWorld _world;
        private EcsEventWorld _eventWorld;
        private EcsPipeline _pipeline;
        private EcsPausableRunner _pausableRunner;
        private PausableLateRunner _pausableLateRunner;

        [Scene] [SerializeField] [Required]
        private string _menuSceneName;
        [SerializeField]
        private ProjectileDataCenter _projectileDataCenter;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeECS();
            InitializePlayerPrefs();
            _eventWorld.SendEvent(new LoadSceneEvent()
            {
                SceneName = _menuSceneName
            });
        }

        private void Update()
        {
            _pipeline.Run();
            _pausableRunner.PausableRun();
        }

        private void FixedUpdate()
        {
            _pipeline.FixedRun();
        }

        private void LateUpdate()
        {
            _pipeline.LateRun();
            _pausableLateRunner.PausableLateRun();
        }

        private void OnDestroy()
        {
            _pipeline?.Destroy();
            _world?.Destroy();

            if (Instance == this)
            {
                Instance = null;
            }
        }
        
        public void RunOnNextFrame(Action action)
        {
            StartCoroutine(NextFrameCoroutine(action));
        }
        
        public void RunOn(Action action, YieldInstruction delay)
        {
            StartCoroutine(RunOnCoroutine(action, delay));
        }

        // ReSharper disable once InconsistentNaming
        private void InitializeECS()
        {
            _world = EcsDefaultWorldSingletonProvider.Instance.Get();
            _eventWorld = EcsEventWorldSingletonProvider.Instance.Get();

            _pipeline = EcsPipeline.New()
                .Inject(_world)
                .Inject(_eventWorld)
                .Inject(_projectileDataCenter)
                
                .AddUnityDebug(_world, _eventWorld)
                
                .AddRunner<EcsPausableRunner>()
                .AddRunner<PausableLateRunner>()
                
                //.Add(new PlayerInputSystem())
                .Add(new SceneLoaderSystem())
                .Add(new LevelInitializationSystem())
                .Add(new RaycastMousePositionSystem())
                .Add(new StopAfterSystem())
                .Add(new MovementSystem())
                .Add(new GridInitializationSystem())
                .Add(new DrawHoveredCellSystem())
                .Add(new GameUIUpdateSystem())
                .Add(new TargetingSystem())
                .Add(new ProjectileDamageSystem())
                
                // Plants
                .Add(new PeaShooterSystem())
                .Add(new SunflowerSystem())
                
                
                .Add(new ZombieEatingSystem())
                
                // .Add(new GameFlowSystem())
                .Add(new SunSpawnSystem())
                //
                .Add(new DamageSystem())
                .Add(new PlantSpawningSystem())
                .Add(new ZombieSpawnSystem())
                .Add(new SunCollectionSystem())
                .Add(new DeathSystem())
                .Add(new WinLoseSystem())
                .Add(new GameNavigationSystem())
                //
                // .Add(new CleanupSystem())
                .Add(new LookAtCameraSystem())
                .Add(new PauseSystem())
                
                .AddCaller<CollisionEvent>()
                .AddCaller<DamageEvent>()
                .AddCaller<SpawnPlantEvent>()
                .AddCaller<CollectSunEvent>()
                .AddCaller<LoadSceneEvent>()
                .AddCaller<SceneLoadedEvent>()
                .AddCaller<NewWaveEvent>()
                .AddCaller<SunSpawningEvent>()
                .AddCaller<PointerIsAbove>()
                .AddCaller<ClickedEvent>()
                .AddCaller<CardClickedEvent>()
                .AddCaller<GoToMainMenuEvent>()
                .AddCaller<ZombieDiedEvent>()
                .AddCaller<WinEvent>()
                .AddCaller<LoseEvent>()
                .AddCaller<StartLevelEvent>()
                .AddCaller<NextLevelEvent>()
                .AddCaller<RestartLevelEvent>()

                .BuildAndInit();
            _pausableRunner = _pipeline.GetRunner<EcsPausableRunner>();
            _pausableLateRunner = _pipeline.GetRunner<PausableLateRunner>();
        }
        
        private void InitializePlayerPrefs()
        {
            if (!UserData.HasKey(unlocked_plants))
            {
                UserData.SetString(unlocked_plants, string.Empty);
            }
            var unlocked = UserData.GetStrings(unlocked_plants);
            if (unlocked[0] == string.Empty)
            {
                UserData.SetStrings(unlocked_plants, "Pea Shooter");
                Debug.LogError("No unlocked plants found.");
                // UserData.SetStrings(unlocked_plants, _allPlants[0].Name);
                UserData.Save();
            }
        }

        private IEnumerator RunOnCoroutine(Action action, YieldInstruction instruction)
        {
            yield return instruction;
            action?.Invoke();
        }

        private IEnumerator NextFrameCoroutine(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }
    }
}
