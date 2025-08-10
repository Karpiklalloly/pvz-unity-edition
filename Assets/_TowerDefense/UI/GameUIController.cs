using System;
using System.Collections.Generic;
using System.Linq;
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

        public class SelectablePlantCard
        {
            public VisualElement Root;
            public PlantConfig Config;
            public bool IsSelected;
        }

        public class SelectedPlantSlot
        {
            public VisualElement Root;
            public PlantConfig Config;
            public VisualElement Icon;
            public bool IsFilled;
        }
        
        [SerializeField] [Required]
        private UIDocument _uiDocument;
        [SerializeField] [Required]
        private VisualTreeAsset _plantCardTemplate;
        [SerializeField] [Required]
        private VisualTreeAsset _selectablePlantCardTemplate;
        [SerializeField] [Required]
        private VisualTreeAsset _selectedPlantSlotTemplate;

        public int Sun
        {
            set => _sunLabel.text = value.ToString();
        }
        
        // TODO: запускать уровень только после выбора растений
        
        public int WaveIndex
        {
            set
            {
                _waveLabel.text = $"Волна {value} / {_maxWaves}";
                _progressBar.style.width = Length.Percent((float)value / _maxWaves * 100);
            }
        }
        
        public IEnumerable<PlantCard> ActiveCards => _activeCards;
        
        // UI Elements
        private VisualElement _plantCardsContainer;
        private Label _sunLabel;
        private Label _waveLabel;
        private VisualElement _progressBar;
        
        // Plant Selection Menu
        private VisualElement _plantSelectionMenu;
        private ScrollView _availablePlantsScroll;
        private VisualElement _selectedPlantsContainer;
        private Label _selectionCounter;
        private Button _startLevelButton;
        
        // Game End Windows
        private VisualElement _victoryWindow;
        private VisualElement _defeatWindow;
        private Label _wavesCompleted;
        private Label _zombiesKilled;
        private Label _sunCollected;
        private Label _wavesCompletedDefeat;
        private Label _zombiesKilledDefeat;
        private Label _sunCollectedDefeat;
        
        // Data
        private List<PlantCard> _activeCards = new();
        private List<SelectablePlantCard> _selectablePlantCards = new();
        private List<SelectedPlantSlot> _selectedPlantSlots = new();
        private List<PlantConfig> _selectedPlants = new();
        private EcsDefaultWorld _world;
        private EcsEventWorld _eventWorld;
        
        private int _maxWaves = 0;
        private const int MAX_SELECTED_PLANTS = 6;

        private void OnEnable()
        {
            var root = _uiDocument.rootVisualElement;
            
            // Game HUD elements
            _sunLabel = root.Q<Label>("sun-label");
            _plantCardsContainer = root.Q<VisualElement>("plant-cards-container");
            _waveLabel = root.DeepQ<Label>("wave-label");
            _progressBar = root.DeepQ<VisualElement>("progress-bar-fill");
            
            // Plant Selection Menu elements
            _plantSelectionMenu = root.Q<VisualElement>("plant-selection-menu");
            _availablePlantsScroll = root.Q<ScrollView>("available-plants-scroll");
            _selectedPlantsContainer = root.Q<VisualElement>("selected-plants-container");
            _selectionCounter = root.Q<Label>("selection-counter");
            _startLevelButton = root.Q<Button>("start-level-button");
            
            // Game End Windows
            _victoryWindow = root.Q<VisualElement>("victory-window");
            _defeatWindow = root.Q<VisualElement>("defeat-window");
            _wavesCompleted = root.Q<Label>("waves-completed");
            _zombiesKilled = root.Q<Label>("zombies-killed");
            _sunCollected = root.Q<Label>("sun-collected");
            _wavesCompletedDefeat = root.Q<Label>("waves-completed-defeat");
            _zombiesKilledDefeat = root.Q<Label>("zombies-killed-defeat");
            _sunCollectedDefeat = root.Q<Label>("sun-collected-defeat");
            
            // Button events
            _startLevelButton.clicked += OnStartLevelClicked;
            root.Q<Button>("next-level-button").RegisterCallback<ClickEvent>(_ => OnNextLevelClicked());
            root.Q<Button>("restart-level-button").RegisterCallback<ClickEvent>(_ => OnRestartLevelClicked());
            root.Q<Button>("retry-level-button").RegisterCallback<ClickEvent>(_ => OnRestartLevelClicked());
            root.Q<Button>("main-menu-button").RegisterCallback<ClickEvent>(_ => OnMainMenuClicked());
            root.Q<Button>("main-menu-button-defeat").RegisterCallback<ClickEvent>(_ => OnMainMenuClicked());
            
            _world = EcsDefaultWorldSingletonProvider.Instance.Get();
            _eventWorld = EcsEventWorldSingletonProvider.Instance.Get();
        }

        public void ShowPlantSelectionMenu(IEnumerable<PlantConfig> availablePlants)
        {
            _plantSelectionMenu.style.display = DisplayStyle.Flex;
            InitializePlantSelection(availablePlants);
        }

        public void HidePlantSelectionMenu()
        {
            _plantSelectionMenu.style.display = DisplayStyle.None;
        }

        private void InitializePlantSelection(IEnumerable<PlantConfig> availablePlants)
        {
            _availablePlantsScroll.Clear();
            _selectedPlantsContainer.Clear();
            _selectablePlantCards.Clear();
            _selectedPlantSlots.Clear();
            _selectedPlants.Clear();

            // Create available plant cards
            foreach (var plant in availablePlants)
            {
                var cardElement = _selectablePlantCardTemplate.CloneTree();
                var card = new SelectablePlantCard
                {
                    Root = cardElement,
                    Config = plant,
                    IsSelected = false
                };

                cardElement.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(plant.PlantCardConfig.Icon);
                cardElement.Q<Label>("plant-name").text = plant.Name;
                cardElement.RegisterCallback<ClickEvent>(_ => OnSelectablePlantClicked(card));

                _selectablePlantCards.Add(card);
                _availablePlantsScroll.Add(cardElement);
            }

            // Create selected plant slots
            for (int i = 0; i < MAX_SELECTED_PLANTS; i++)
            {
                var slotElement = _selectedPlantSlotTemplate.CloneTree();
                var slot = new SelectedPlantSlot
                {
                    Root = slotElement,
                    Config = null,
                    Icon = slotElement.Q<VisualElement>("slot-icon"),
                    IsFilled = false
                };

                slotElement.RegisterCallback<ClickEvent>(_ => OnSelectedPlantSlotClicked(slot));
                _selectedPlantSlots.Add(slot);
                _selectedPlantsContainer.Add(slotElement);
            }

            UpdateSelectionUI();
        }

        public void Init(LevelConfig config, IEnumerable<PlantConfig> selectedPlants)
        {
            _plantCardsContainer.Clear();
            _activeCards.Clear();

            WaveIndex = 0;
            
            // Initialize plant cards based on selected plants
            foreach (var plant in selectedPlants)
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
                cardElement.AddManipulator(new TooltipManipulator(
                    () => _plantCardsContainer.parent.parent,
                    () => card.Config.Name,
                    () => string.Empty,
                    TooltipManipulator.Mode.FollowCursor));
                cardElement.DeepQ<Label>("cost").text = plant.PlantCardConfig.Cost.ToString();
                cardElement.DeepQ<VisualElement>("icon").style.backgroundImage = new StyleBackground(plant.PlantCardConfig.Icon);
                
                cardElement.RegisterCallback<ClickEvent>((evt) => OnPlantCardClicked(card));
                
                _activeCards.Add(card);
                _plantCardsContainer.Add(cardElement);
            }
        }

        public void OnNewWave(NewWaveEvent evt)
        {
            _maxWaves = evt.LastIndex + 1;
            WaveIndex = evt.WaveIndex + 1;
        }

        private void OnSelectablePlantClicked(SelectablePlantCard card)
        {
            if (card.IsSelected)
            {
                // Remove from selection
                RemovePlantFromSelection(card);
            }
            else
            {
                // Add to selection if there's space
                if (_selectedPlants.Count < MAX_SELECTED_PLANTS)
                {
                    AddPlantToSelection(card);
                }
            }
            UpdateSelectionUI();
        }

        private void OnSelectedPlantSlotClicked(SelectedPlantSlot slot)
        {
            if (slot.IsFilled)
            {
                // Remove plant from this slot
                var plantToRemove = slot.Config;
                var cardToDeselect = _selectablePlantCards.FirstOrDefault(c => c.Config == plantToRemove);
                if (cardToDeselect != null)
                {
                    RemovePlantFromSelection(cardToDeselect);
                    UpdateSelectionUI();
                }
            }
        }

        private void AddPlantToSelection(SelectablePlantCard card)
        {
            card.IsSelected = true;
            _selectedPlants.Add(card.Config);
            
            var emptySlot = _selectedPlantSlots.FirstOrDefault(s => !s.IsFilled);
            if (emptySlot != null)
            {
                emptySlot.Config = card.Config;
                emptySlot.IsFilled = true;
                emptySlot.Icon.style.backgroundImage = new StyleBackground(card.Config.PlantCardConfig.Icon);
                emptySlot.Icon.style.display = DisplayStyle.Flex;
                emptySlot.Root.Q<Label>().style.display = DisplayStyle.None;
                emptySlot.Root.AddToClassList("filled");
            }
        }

        private void RemovePlantFromSelection(SelectablePlantCard card)
        {
            card.IsSelected = false;
            _selectedPlants.Remove(card.Config);
            
            var filledSlot = _selectedPlantSlots.FirstOrDefault(s => s.Config == card.Config);
            if (filledSlot != null)
            {
                filledSlot.Config = null;
                filledSlot.IsFilled = false;
                filledSlot.Icon.style.display = DisplayStyle.None;
                filledSlot.Root.Q<Label>().style.display = DisplayStyle.Flex;
                filledSlot.Root.RemoveFromClassList("filled");
            }
        }

        private void UpdateSelectionUI()
        {
            // Update selection counter
            _selectionCounter.text = $"{_selectedPlants.Count}/{MAX_SELECTED_PLANTS}";
            
            // Update start button state
            _startLevelButton.SetEnabled(_selectedPlants.Count > 0);
            
            // Update card visual states
            foreach (var card in _selectablePlantCards)
            {
                if (card.IsSelected)
                {
                    card.Root.AddToClassList("selected");
                }
                else
                {
                    card.Root.RemoveFromClassList("selected");
                }
            }
        }

        private void OnStartLevelClicked()
        {
            if (_selectedPlants.Count > 0)
            {
                HidePlantSelectionMenu();
                _eventWorld.SendEvent(new StartLevelEvent { SelectedPlants = _selectedPlants.ToList() });
            }
        }

        public void ShowVictoryWindow(int wavesCompleted, int zombiesKilled, int sunCollected)
        {
            _victoryWindow.style.display = DisplayStyle.Flex;
            _wavesCompleted.text = $"Волн пройдено: {wavesCompleted}/{_maxWaves}";
            _zombiesKilled.text = $"Зомби уничтожено: {zombiesKilled}";
            _sunCollected.text = $"Солнца собрано: {sunCollected}";
        }

        public void ShowDefeatWindow(int wavesCompleted, int zombiesKilled, int sunCollected)
        {
            _defeatWindow.style.display = DisplayStyle.Flex;
            _wavesCompletedDefeat.text = $"Волн пройдено: {wavesCompleted}/{_maxWaves}";
            _zombiesKilledDefeat.text = $"Зомби уничтожено: {zombiesKilled}";
            _sunCollectedDefeat.text = $"Солнца собрано: {sunCollected}";
        }

        public void HideGameEndWindows()
        {
            _victoryWindow.style.display = DisplayStyle.None;
            _defeatWindow.style.display = DisplayStyle.None;
        }

        private void OnNextLevelClicked()
        {
            _eventWorld.SendEvent(new NextLevelEvent());
        }

        private void OnRestartLevelClicked()
        {
            _eventWorld.SendEvent(new RestartLevelEvent());
        }

        private void OnMainMenuClicked()
        {
            _eventWorld.SendEvent(new GoToMainMenuEvent());
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