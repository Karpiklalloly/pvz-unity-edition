using System;
using System.Collections.Generic;
using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using Karpik.UIExtension;
using TowerDefense.Core;
using TriInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace TowerDefense.UI
{
    public class GameUIController : MonoBehaviour
    {
        public class PlantCard
        {
            public VisualElement Root;
            public PlantConfig Config;
            public VisualElement CooldownOverlay;
            public float CooldownTimer;
        }
        
        [SerializeField] [Required]
        private UIDocument _uiDocument;
        [SerializeField] [Required]
        private VisualTreeAsset _plantCardTemplate;

        public int Sun
        {
            set => _sunLabel.text = value.ToString();
        }
        
        public IEnumerable<PlantCard> ActiveCards => _activeCards;
        
        private VisualElement _plantCardsContainer;
        private Label _sunLabel;
        private List<PlantCard> _activeCards = new();
        private EcsDefaultWorld _world;
        private EcsEventWorld _eventWorld;

        private void OnEnable()
        {
            var root = _uiDocument.rootVisualElement;
            _sunLabel = root.Q<Label>("sun-label");
            _plantCardsContainer = root.Q<VisualElement>("plant-cards-container");
            _world = EcsDefaultWorldSingletonProvider.Instance.Get();
            _eventWorld = EcsEventWorldSingletonProvider.Instance.Get();
        }

        public void Init(LevelConfig config, IEnumerable<PlantConfig> allPlants, IEnumerable<PlantConfig> unlockedPlants)
        {
            _plantCardsContainer.Clear();
            _activeCards.Clear();
            
            // TODO: Initialize plant cards based on unlocked plants
            foreach (var plant in unlockedPlants)
            {
                var cardElement = _plantCardTemplate.CloneTree();
                cardElement.style.display = DisplayStyle.Flex;

                var card = new PlantCard()
                {
                    Root = cardElement,
                    Config = plant,
                    CooldownOverlay = cardElement.Q<VisualElement>("cooldown-overlay"),
                    CooldownTimer = 0f
                };
                cardElement.DeepQ<Label>("cost").text = plant.PlantCardConfig.Cost.ToString();
                cardElement.DeepQ<VisualElement>("icon").style.backgroundImage = new StyleBackground(plant.PlantCardConfig.Icon);
                
                cardElement.RegisterCallback<ClickEvent>((evt) => OnPlantCardClicked(card));
                
                _activeCards.Add(card);
                _plantCardsContainer.Add(cardElement);
            }
        }

        private void OnPlantCardClicked(PlantCard card)
        {
            ref var data = ref _world.Get<PlayerData>();
            if (data.SunAmount < card.Config.PlantCardConfig.Cost)
            {
                return;
            }
            if (card.CooldownTimer > 0) return;
            Debug.Log($"Выбрано растение: {card.Config.Name}");
            _eventWorld.SendEvent(new CardClickedEvent()
            {
                Card = card
            });

        }
    }
}