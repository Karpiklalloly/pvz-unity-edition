using System;
using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using TowerDefense.UI;
using UnityEngine;

namespace TowerDefense.Core
{
    [Serializable]
    public struct DamageEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public int Damage;
    }

    [Serializable]
    public struct SpawnPlantEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public entlong Prefab;
        public Vector2 GridPosition;
    }

    [Serializable]
    public struct CollectSunEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public entlong SunEntity;
    }

    [Serializable]
    public struct LoadSceneEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public string SceneName;
        public LevelConfig LevelConfig;
    }

    [Serializable]
    public struct SceneLoadedEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public string SceneName;
        public LevelConfig LevelConfig;
    }

    [Serializable]
    public struct SunSpawningEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public entlong SunEntity;
    }
    
    [Serializable]
    public struct ClickedEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
    }

    [Serializable]
    public struct PointerIsAbove : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public Vector3 WorldPosition;
        public GameObject Object;
    }

    [Serializable]
    public struct GoToMainMenuEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
    }

    [Serializable]
    public struct CardClickedEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public GameUIController.PlantCard Card;
    }

    [Serializable]
    public struct CollisionEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
    }

    [Serializable]
    public struct NewWaveEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public int WaveIndex;
        public int LastIndex;
    }

    [Serializable]
    public struct ZombieDiedEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
    }

    [Serializable]
    public struct WinEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
    }

    [Serializable]
    public struct LoseEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
    }

    [Serializable]
    public struct StartLevelEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public System.Collections.Generic.List<PlantConfig> SelectedPlants;
    }

    [Serializable]
    public struct NextLevelEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
    }

    [Serializable]
    public struct RestartLevelEvent : IEcsComponentEvent
    {
        public int Source { get; set; }
        public int Target { get; set; }
    }
}