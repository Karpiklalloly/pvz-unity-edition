using Karpik.Engine.Shared.DragonECS;
using TowerDefense.Core;
using UnityEngine;
using UnityEngine.UIElements;
using TriInspector;
using UnityEngine.Serialization;

namespace TowerDefense.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Document")]
        [Required]
        [SerializeField] private UIDocument uiDocument;

        [FormerlySerializedAs("levelSceneName")]
        [Header("Level Configuration")] [Required]
        [SerializeField] private LevelConfig[] availableLevels;
        [Scene]
        [SerializeField] private string _levelSceneName = "Level";

        private VisualElement _root;
        private Button _playButton;
        private Button _quitButton;
        private VisualElement _levelContainer;
        private Label _titleLabel;

        private void Start()
        {
            InitializeUI();
            CreateLevelButtons();
        }

        private void InitializeUI()
        {
            if (uiDocument == null)
            {
                uiDocument = GetComponent<UIDocument>();
            }

            if (uiDocument == null)
            {
                Debug.LogError("UIDocument not found! Please assign UIDocument component.");
                return;
            }

            _root = uiDocument.rootVisualElement;

            // Получить ссылки на UI элементы
            _titleLabel = _root.Q<Label>("title-label");
            _playButton = _root.Q<Button>("play-button");
            _quitButton = _root.Q<Button>("quit-button");
            _levelContainer = _root.Q<VisualElement>("level-container");

            if (_titleLabel != null)
            {
                _titleLabel.text = "Tower Defense";
            }

            if (_playButton != null)
            {
                _playButton.clicked += OnPlayButtonClicked;
            }

            if (_quitButton != null)
            {
                _quitButton.clicked += OnQuitButtonClicked;
            }
        }

        private void CreateLevelButtons()
        {
            if (_levelContainer == null || availableLevels == null)
            {
                Debug.LogWarning("Level container or levels not configured");
                return;
            }

            // Очистить контейнер
            _levelContainer.Clear();

            // Создать кнопки для каждого уровня
            for (int i = 0; i < availableLevels.Length; i++)
            {
                var level = availableLevels[i];
                if (level == null) continue;
            
                var levelButton = new Button();
                levelButton.text = $"Level {i + 1}";
                levelButton.AddToClassList("level-button");
                
                levelButton.clicked += () => OnLevelSelected(level);
            
                _levelContainer.Add(levelButton);
            }
        }

        private void OnPlayButtonClicked()
        {
            // Загрузить первый доступный уровень
            if (availableLevels != null && availableLevels.Length > 0)
            {
                OnLevelSelected(availableLevels[0]);
            }
            else
            {
                Debug.LogWarning("No levels available to play");
            }
        }

        private void OnLevelSelected(LevelConfig level)
        {
            Bootstrap.Instance.EventWorld.SendEvent(new LoadSceneEvent()
            {
                Source = -1,
                Target = -1,
                SceneName = _levelSceneName,
                LevelConfig = level
            });
        }

        private void OnQuitButtonClicked()
        {
            Debug.Log("Quit button clicked");
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnDestroy()
        {
            // Отписаться от событий
            if (_playButton != null)
            {
                _playButton.clicked -= OnPlayButtonClicked;
            }

            if (_quitButton != null)
            {
                _quitButton.clicked -= OnQuitButtonClicked;
            }
        }
    }
}