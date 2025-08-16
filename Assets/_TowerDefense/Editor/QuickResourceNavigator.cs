using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class AssetNavigationWindow : EditorWindow
{
    [MenuItem("Tools/Asset Navigation Window")]
    public static void ShowWindow()
    {
        GetWindow<AssetNavigationWindow>("Asset Navigation");
    }

    private TreeViewState treeViewState;
    private AssetTreeView treeView;
    private SearchField searchField;
    private string searchFilter = "";
    private Vector2 scrollPosition;
    private AssetTreeViewItem selectedItem;
    private string newLabel = "";
    private AddressableAssetSettings settings;
    
    // Фильтры по типу ассета
    private bool filterScripts = true;
    private bool filterScenes = true;
    private bool filterPrefabs = true;
    private bool filterTextures = true;
    private bool filterMaterials = true;
    private bool filterAudio = true;
    private bool filterModels = true;
    private bool filterScriptableObjects = true;
    private bool filterOther = true;

    private void OnEnable()
    {
        treeViewState = new TreeViewState();
        searchField = new SearchField();
        RefreshTreeView();
        
        // Подписываемся на события изменения ассетов
        EditorApplication.projectChanged += RefreshTreeView;
    }

    private void OnDisable()
    {
        EditorApplication.projectChanged -= RefreshTreeView;
    }

    private void RefreshTreeView()
    {
        treeView = new AssetTreeView(treeViewState);
        treeView.SetSearchFilter(searchFilter);
        treeView.SetTypeFilters(
            filterScripts, filterScenes, filterPrefabs, filterTextures,
            filterMaterials, filterAudio, filterModels, filterScriptableObjects, filterOther
        );
        treeView.Reload();
        Repaint();
    }

    private void OnGUI()
    {
        DrawToolbar();
        DrawFilterBar();
        DrawMainArea();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        
        // Поле поиска
        string newSearchFilter = searchField.OnToolbarGUI(searchFilter);
        if (newSearchFilter != searchFilter)
        {
            searchFilter = newSearchFilter;
            RefreshTreeView();
        }
        
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawFilterBar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        
        GUILayout.Label("Type Filters:", EditorStyles.miniLabel);
        
        bool oldScripts = filterScripts;
        bool oldScenes = filterScenes;
        bool oldPrefabs = filterPrefabs;
        bool oldTextures = filterTextures;
        bool oldMaterials = filterMaterials;
        bool oldAudio = filterAudio;
        bool oldModels = filterModels;
        bool oldScriptableObjects = filterScriptableObjects;
        bool oldOther = filterOther;
        
        filterScripts = GUILayout.Toggle(filterScripts, "Scripts", EditorStyles.toolbarButton);
        filterScenes = GUILayout.Toggle(filterScenes, "Scenes", EditorStyles.toolbarButton);
        filterPrefabs = GUILayout.Toggle(filterPrefabs, "Prefabs", EditorStyles.toolbarButton);
        filterTextures = GUILayout.Toggle(filterTextures, "Textures", EditorStyles.toolbarButton);
        filterMaterials = GUILayout.Toggle(filterMaterials, "Materials", EditorStyles.toolbarButton);
        filterAudio = GUILayout.Toggle(filterAudio, "Audio", EditorStyles.toolbarButton);
        filterModels = GUILayout.Toggle(filterModels, "Models", EditorStyles.toolbarButton);
        filterScriptableObjects = GUILayout.Toggle(filterScriptableObjects, "ScriptableObjects", EditorStyles.toolbarButton);
        filterOther = GUILayout.Toggle(filterOther, "Other", EditorStyles.toolbarButton);
        
        // Проверяем, изменились ли фильтры
        if (oldScripts != filterScripts || oldScenes != filterScenes || oldPrefabs != filterPrefabs ||
            oldTextures != filterTextures || oldMaterials != filterMaterials || oldAudio != filterAudio ||
            oldModels != filterModels || oldScriptableObjects != filterScriptableObjects || oldOther != filterOther)
        {
            RefreshTreeView();
        }
        
        if (GUILayout.Button("All", EditorStyles.toolbarButton, GUILayout.Width(40)))
        {
            filterScripts = filterScenes = filterPrefabs = filterTextures = 
            filterMaterials = filterAudio = filterModels = filterScriptableObjects = filterOther = true;
            RefreshTreeView();
        }
        
        if (GUILayout.Button("None", EditorStyles.toolbarButton, GUILayout.Width(40)))
        {
            filterScripts = filterScenes = filterPrefabs = filterTextures = 
            filterMaterials = filterAudio = filterModels = filterScriptableObjects = filterOther = false;
            RefreshTreeView();
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawMainArea()
    {
        EditorGUILayout.BeginHorizontal();
        
        // Левая панель - дерево ассетов
        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true));
        
        if (treeView != null)
        {
            treeView.OnGUI(new Rect(0, 0, position.width * 0.7f - 10, position.height - 90));
        }
        
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        
        // Правая панель - редактирование Addressable Label
        DrawLabelEditor();
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawLabelEditor()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.3f));
        
        GUILayout.Label("Addressable Label", EditorStyles.boldLabel);
        
        if (selectedItem != null && selectedItem.assetPath != null)
        {
            GUILayout.Label($"Selected: {selectedItem.displayName}", EditorStyles.helpBox);
            
            // Проверяем, является ли ассет Addressable
            bool isAddressable = IsAssetAddressable(selectedItem.assetPath);
            if (isAddressable)
            {
                EditorGUILayout.HelpBox("This asset is Addressable", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("This asset is NOT Addressable", MessageType.Warning);
            }
            
            // Получаем текущие лейблы
            var labels = GetAssetLabels(selectedItem.assetPath);
            if (labels.Count > 0)
            {
                GUILayout.Label("Current Labels:", EditorStyles.miniLabel);
                foreach (var label in labels)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(label, EditorStyles.miniLabel);
                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        RemoveAssetLabel(selectedItem.assetPath, label);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else if (isAddressable)
            {
                GUILayout.Label("No labels assigned", EditorStyles.miniLabel);
            }
            
            GUILayout.Space(10);
            
            // Кнопки действий
            GUILayout.Label("Actions:", EditorStyles.miniLabel);
            if (!isAddressable)
            {
                if (GUILayout.Button("Make Addressable"))
                {
                    MakeAssetAddressable(selectedItem.assetPath);
                }
            }
            else
            {
                if (GUILayout.Button("Remove from Addressables"))
                {
                    RemoveAssetFromAddressables(selectedItem.assetPath);
                }
            }
            
            if (GUILayout.Button("Show in Project"))
            {
                ShowInProject(selectedItem.assetPath);
            }
            
            GUILayout.Space(10);
            
            // Добавление нового лейбла (только для Addressable ассетов)
            if (isAddressable)
            {
                GUILayout.Label("Add Label:", EditorStyles.miniLabel);
                newLabel = EditorGUILayout.TextField("Label Name", newLabel);
                
                if (GUILayout.Button("Add Label") && !string.IsNullOrEmpty(newLabel))
                {
                    AddAssetLabel(selectedItem.assetPath, newLabel);
                    newLabel = "";
                }
                
                // Предложенные лейблы
                GUILayout.Space(10);
                GUILayout.Label("Suggested Labels:", EditorStyles.miniLabel);
                DrawSuggestedLabels();
            }
        }
        else
        {
            GUILayout.Label("Select an asset to edit labels", EditorStyles.helpBox);
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawSuggestedLabels()
    {
        if (settings == null)
        {
            settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return;
        }

        // Получаем все доступные лейблы через публичный API
        var allLabels = settings.GetLabels().ToList();
        
        int buttonsPerRow = 3;
        int count = 0;

        EditorGUILayout.BeginHorizontal();
        foreach (var label in allLabels.Take(12)) // Ограничиваем для UI
        {
            if (count > 0 && count % buttonsPerRow == 0)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            if (GUILayout.Button(label, GUILayout.Width(80), GUILayout.Height(25)))
            {
                if (selectedItem != null && selectedItem.assetPath != null)
                {
                    AddAssetLabel(selectedItem.assetPath, label);
                }
            }
            count++;
        }
        EditorGUILayout.EndHorizontal();
    }

    private bool IsAssetAddressable(string assetPath)
    {
        if (settings == null)
        {
            settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return false;
        }

        var guid = AssetDatabase.AssetPathToGUID(assetPath);
        var entry = settings.FindAssetEntry(guid);
        return entry != null;
    }

    private List<string> GetAssetLabels(string assetPath)
    {
        if (settings == null)
        {
            settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return new List<string>();
        }

        var guid = AssetDatabase.AssetPathToGUID(assetPath);
        var entry = settings.FindAssetEntry(guid);
        if (entry != null && entry.labels != null)
        {
            return entry.labels.ToList();
        }
        return new List<string>();
    }

    private void AddAssetLabel(string assetPath, string label)
    {
        if (settings == null)
        {
            settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return;
        }

        // Добавляем лейбл в настройки, если его еще нет
        if (!settings.GetLabels().Contains(label))
        {
            settings.AddLabel(label);
        }
        
        var guid = AssetDatabase.AssetPathToGUID(assetPath);
        var entry = settings.FindAssetEntry(guid);
        
        // Если entry не существует, создаем его
        if (entry == null)
        {
            entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
        }
        
        if (entry != null)
        {
            entry.SetLabel(label, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);
            AssetDatabase.SaveAssets();
        }
    }

    private void RemoveAssetLabel(string assetPath, string label)
    {
        if (settings == null) return;

        var guid = AssetDatabase.AssetPathToGUID(assetPath);
        var entry = settings.FindAssetEntry(guid);
        if (entry != null)
        {
            entry.SetLabel(label, false);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);
            AssetDatabase.SaveAssets();
        }
    }

    private void MakeAssetAddressable(string assetPath)
    {
        if (settings == null)
        {
            settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return;
        }

        var guid = AssetDatabase.AssetPathToGUID(assetPath);
        var entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
        
        if (entry != null)
        {
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, entry, true);
            AssetDatabase.SaveAssets();
        }
    }

    private void RemoveAssetFromAddressables(string assetPath)
    {
        if (settings == null) return;

        var guid = AssetDatabase.AssetPathToGUID(assetPath);
        var entry = settings.FindAssetEntry(guid);
        if (entry != null)
        {
            settings.RemoveAssetEntry(entry.guid);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, entry, true);
            AssetDatabase.SaveAssets();
        }
    }

    private void ShowInProject(string assetPath)
    {
        var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
        if (asset != null)
        {
            EditorGUIUtility.PingObject(asset);
            Selection.activeObject = asset;
        }
    }

    // Класс дерева
    public class AssetTreeView : TreeView
    {
        private string searchFilter = "";
        private Dictionary<string, AssetTreeViewItem> itemCache = new Dictionary<string, AssetTreeViewItem>();
        private Dictionary<string, bool> folderHasMissingLabelsCache = new Dictionary<string, bool>();
        private AssetTreeViewItem contextMenuItem;
        
        // Фильтры по типу
        private bool filterScripts = true;
        private bool filterScenes = true;
        private bool filterPrefabs = true;
        private bool filterTextures = true;
        private bool filterMaterials = true;
        private bool filterAudio = true;
        private bool filterModels = true;
        private bool filterScriptableObjects = true;
        private bool filterOther = true;

        // Маппинг типов SO к лейблам
        private Dictionary<string, string> soTypeToLabelMap = new Dictionary<string, string>
        {
            { "TowerDefense.PlantConfig", "Plant" },
            { "TowerDefense.LevelConfig", "Level" },
            { "TowerDefense.ZombieConfig", "Zombie" }
        };

        public AssetTreeView(TreeViewState state) : base(state)
        {
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            Reload();
        }

        public void SetSearchFilter(string filter)
        {
            searchFilter = filter?.ToLower() ?? "";
        }
        
        public void SetTypeFilters(
            bool scripts, bool scenes, bool prefabs, bool textures,
            bool materials, bool audio, bool models, bool scriptableObjects, bool other)
        {
            filterScripts = scripts;
            filterScenes = scenes;
            filterPrefabs = prefabs;
            filterTextures = textures;
            filterMaterials = materials;
            filterAudio = audio;
            filterModels = models;
            filterScriptableObjects = scriptableObjects;
            filterOther = other;
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            folderHasMissingLabelsCache.Clear();
            
            // Получаем все ассеты в папке Assets
            string[] allAssets = AssetDatabase.GetAllAssetPaths()
                .Where(path => path.StartsWith("Assets/") && 
                              !path.EndsWith(".meta") && 
                              File.Exists(path))
                .Where(path => IsAssetTypeAllowed(path)) // Применяем фильтр по типу
                .ToArray();

            if (string.IsNullOrEmpty(searchFilter))
            {
                // Строим полную иерархию
                BuildFullHierarchy(root, allAssets);
            }
            else
            {
                // Строим фильтрованную иерархию
                BuildFilteredHierarchy(root, allAssets);
            }

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        private bool IsAssetTypeAllowed(string assetPath)
        {
            string extension = Path.GetExtension(assetPath).ToLower();
            string fileName = Path.GetFileName(assetPath);

            // Определяем тип ассета
            if (extension == ".cs" || extension == ".js" || extension == ".boo")
            {
                return filterScripts;
            }
            else if (extension == ".unity")
            {
                return filterScenes;
            }
            else if (extension == ".prefab")
            {
                return filterPrefabs;
            }
            else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || 
                     extension == ".tga" || extension == ".psd" || extension == ".gif" ||
                     extension == ".bmp" || extension == ".tif" || extension == ".tiff")
            {
                return filterTextures;
            }
            else if (extension == ".mat")
            {
                return filterMaterials;
            }
            else if (extension == ".wav" || extension == ".mp3" || extension == ".ogg" ||
                     extension == ".aif" || extension == ".aiff")
            {
                return filterAudio;
            }
            else if (extension == ".fbx" || extension == ".obj" || extension == ".dae" ||
                     extension == ".3ds" || extension == ".dxf")
            {
                return filterModels;
            }
            else if (AssetDatabase.GetMainAssetTypeAtPath(assetPath) == typeof(ScriptableObject))
            {
                return filterScriptableObjects;
            }
            else
            {
                return filterOther;
            }
        }

        private void BuildFullHierarchy(TreeViewItem root, string[] allAssets)
        {
            itemCache.Clear();
            
            // Создаем все папки
            var folderPaths = new HashSet<string>();
            foreach (var assetPath in allAssets)
            {
                var dir = Path.GetDirectoryName(assetPath);
                while (dir != null && dir != "Assets")
                {
                    folderPaths.Add(dir);
                    dir = Path.GetDirectoryName(dir);
                }
            }
            folderPaths.Add("Assets");

            // Создаем элементы для папок
            foreach (var folderPath in folderPaths.OrderBy(p => p))
            {
                var item = new AssetTreeViewItem
                {
                    id = folderPath.GetHashCode(),
                    depth = folderPath.Split('/').Length - 1,
                    displayName = Path.GetFileName(folderPath),
                    assetPath = folderPath,
                    isFolder = true
                };
                
                itemCache[folderPath] = item;
                
                if (folderPath == "Assets")
                {
                    root.AddChild(item);
                }
                else
                {
                    var parentPath = Path.GetDirectoryName(folderPath);
                    if (itemCache.ContainsKey(parentPath))
                    {
                        itemCache[parentPath].AddChild(item);
                    }
                }
            }

            // Добавляем ассеты и обновляем информацию о папках
            foreach (var assetPath in allAssets)
            {
                var parentPath = Path.GetDirectoryName(assetPath);
                if (itemCache.ContainsKey(parentPath))
                {
                    var item = new AssetTreeViewItem
                    {
                        id = assetPath.GetHashCode(),
                        depth = assetPath.Split('/').Length,
                        displayName = Path.GetFileName(assetPath),
                        assetPath = assetPath,
                        isFolder = false,
                        assetType = GetAssetType(assetPath),
                        requiredLabel = GetRequiredLabelForSO(assetPath)
                    };
                    
                    itemCache[parentPath].AddChild(item);
                    
                    // Обновляем информацию о папке
                    if (item.requiredLabel != null)
                    {
                        var labels = GetAssetLabels(assetPath);
                        if (!labels.Contains(item.requiredLabel))
                        {
                            UpdateFolderMissingLabels(parentPath);
                        }
                    }
                }
            }
        }

        private void BuildFilteredHierarchy(TreeViewItem root, string[] allAssets)
        {
            itemCache.Clear();
            folderHasMissingLabelsCache.Clear();
            
            // Находим ассеты, соответствующие фильтру
            var matchingAssets = allAssets
                .Where(path => Path.GetFileName(path).ToLower().Contains(searchFilter))
                .ToList();

            // Находим все папки, содержащие подходящие ассеты
            var requiredFolders = new HashSet<string>();
            foreach (var assetPath in matchingAssets)
            {
                var dir = Path.GetDirectoryName(assetPath);
                while (dir != null && dir.StartsWith("Assets"))
                {
                    requiredFolders.Add(dir);
                    dir = Path.GetDirectoryName(dir);
                }
            }

            // Создаем элементы для папок
            var allFolders = requiredFolders.OrderBy(p => p).ToList();
            foreach (var folderPath in allFolders)
            {
                var item = new AssetTreeViewItem
                {
                    id = folderPath.GetHashCode(),
                    depth = folderPath.Split('/').Length - 1,
                    displayName = Path.GetFileName(folderPath),
                    assetPath = folderPath,
                    isFolder = true
                };
                
                itemCache[folderPath] = item;
                
                if (folderPath == "Assets")
                {
                    root.AddChild(item);
                }
                else
                {
                    var parentPath = Path.GetDirectoryName(folderPath);
                    if (itemCache.ContainsKey(parentPath))
                    {
                        itemCache[parentPath].AddChild(item);
                    }
                }
            }

            // Добавляем только подходящие ассеты и обновляем информацию о папках
            foreach (var assetPath in matchingAssets)
            {
                var parentPath = Path.GetDirectoryName(assetPath);
                if (itemCache.ContainsKey(parentPath))
                {
                    var item = new AssetTreeViewItem
                    {
                        id = assetPath.GetHashCode(),
                        depth = assetPath.Split('/').Length,
                        displayName = Path.GetFileName(assetPath),
                        assetPath = assetPath,
                        isFolder = false,
                        assetType = GetAssetType(assetPath),
                        requiredLabel = GetRequiredLabelForSO(assetPath)
                    };
                    
                    itemCache[parentPath].AddChild(item);
                    
                    // Обновляем информацию о папке
                    if (item.requiredLabel != null)
                    {
                        var labels = GetAssetLabels(assetPath);
                        if (!labels.Contains(item.requiredLabel))
                        {
                            UpdateFolderMissingLabels(parentPath);
                        }
                    }
                }
            }
        }

        private void UpdateFolderMissingLabels(string folderPath)
        {
            folderHasMissingLabelsCache[folderPath] = true;
            var parentPath = Path.GetDirectoryName(folderPath);
            while (parentPath != null && parentPath.StartsWith("Assets"))
            {
                folderHasMissingLabelsCache[parentPath] = true;
                parentPath = Path.GetDirectoryName(parentPath);
            }
        }

        private Type GetAssetType(string assetPath)
        {
            return AssetDatabase.GetMainAssetTypeAtPath(assetPath);
        }

        private string GetRequiredLabelForSO(string assetPath)
        {
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (assetType != null && assetType.IsSubclassOf(typeof(ScriptableObject)))
            {
                string fullTypeName = assetType.FullName;
                if (soTypeToLabelMap.ContainsKey(fullTypeName))
                {
                    return soTypeToLabelMap[fullTypeName];
                }
            }
            return null;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = args.item as AssetTreeViewItem;
            if (item == null) return;

            Rect rowRect = args.rowRect;
            rowRect.x += GetContentIndent(item);
            rowRect.width -= GetContentIndent(item);

            // Рисуем иконку
            Rect iconRect = new Rect(rowRect.x, rowRect.y, 18, 18);
            Texture icon = item.isFolder ? 
                EditorGUIUtility.FindTexture("Folder Icon") : 
                AssetDatabase.GetCachedIcon(item.assetPath);
            
            if (icon != null)
            {
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
            }

            // Рисуем дополнительную иконку для SO с отсутствующим лейблом
            float warningIconOffset = 0;
            if (!item.isFolder && item.requiredLabel != null)
            {
                var labels = GetAssetLabels(item.assetPath);
                if (!labels.Contains(item.requiredLabel))
                {
                    // Рисуем предупреждающую иконку
                    Rect warningIconRect = new Rect(iconRect.x + iconRect.width + 2, rowRect.y, 16, 16);
                    Texture warningIcon = EditorGUIUtility.FindTexture("console.warnicon");
                    if (warningIcon != null)
                    {
                        GUI.DrawTexture(warningIconRect, warningIcon, ScaleMode.ScaleToFit);
                        warningIconOffset = 18; // 16px иконка + 2px отступ
                    }
                }
            }
            // Рисуем иконку для папок с отсутствующими лейблами
            else if (item.isFolder && folderHasMissingLabelsCache.ContainsKey(item.assetPath) && 
                     folderHasMissingLabelsCache[item.assetPath])
            {
                Rect warningIconRect = new Rect(iconRect.x + iconRect.width + 2, rowRect.y, 16, 16);
                Texture warningIcon = EditorGUIUtility.FindTexture("console.warnicon");
                if (warningIcon != null)
                {
                    GUI.DrawTexture(warningIconRect, warningIcon, ScaleMode.ScaleToFit);
                    warningIconOffset = 18; // 16px иконка + 2px отступ
                }
            }

            // Рисуем имя
            float offset = 22 + warningIconOffset;
            
            Rect labelRect = new Rect(rowRect.x + offset, rowRect.y, rowRect.width - offset, rowRect.height);
            EditorGUI.LabelField(labelRect, item.displayName);

            // Выделяем всю строку при выборе
            if (args.selected)
            {
                EditorGUI.DrawRect(rowRect, new Color(0.24f, 0.49f, 0.91f, 0.3f));
            }
        }

        private List<string> GetAssetLabels(string assetPath)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return new List<string>();

            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            var entry = settings.FindAssetEntry(guid);
            if (entry != null && entry.labels != null)
            {
                return entry.labels.ToList();
            }
            return new List<string>();
        }

        protected override void DoubleClickedItem(int id)
        {
            var item = FindItem(id, rootItem) as AssetTreeViewItem;
            if (item != null && !item.isFolder)
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(item.assetPath));
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            
            if (selectedIds.Count > 0)
            {
                var item = FindItem(selectedIds[0], rootItem) as AssetTreeViewItem;
                if (item != null)
                {
                    var window = EditorWindow.focusedWindow as AssetNavigationWindow;
                    if (window != null)
                    {
                        window.selectedItem = item;
                    }
                }
            }
        }

        protected override void ContextClickedItem(int id)
        {
            var item = FindItem(id, rootItem) as AssetTreeViewItem;
            if (item != null)
            {
                contextMenuItem = item;
                
                // Создаем контекстное меню как в Project окне
                var menu = new GenericMenu();
                
                // Пункты для всех элементов
                menu.AddItem(new GUIContent("Show in Explorer"), false, () => {
                    EditorUtility.RevealInFinder(item.assetPath);
                });
                
                menu.AddSeparator("");
                
                menu.AddItem(new GUIContent("Rename"), false, () => {
                    BeginRename(item);
                });
                
                menu.AddItem(new GUIContent("Delete"), false, () => {
                    string message = item.isFolder ? 
                        $"Are you sure you want to delete '{item.displayName}' and its contents?\n\nYou cannot undo this action." :
                        $"Are you sure you want to delete '{item.displayName}'?\n\nYou cannot undo this action.";
                    
                    string title = item.isFolder ? "Delete Selected Folder" : "Delete Selected Asset";
                    
                    if (EditorUtility.DisplayDialog(title, message, "Delete", "Cancel"))
                    {
                        AssetDatabase.DeleteAsset(item.assetPath);
                        var window = EditorWindow.focusedWindow as AssetNavigationWindow;
                        if (window != null)
                        {
                            window.RefreshTreeView();
                        }
                    }
                });
                
                menu.AddSeparator("");
                
                menu.AddItem(new GUIContent("Copy Path"), false, () => {
                    EditorGUIUtility.systemCopyBuffer = item.assetPath;
                });
                
                if (!item.isFolder)
                {
                    menu.AddItem(new GUIContent("Copy GUID"), false, () => {
                        var guid = AssetDatabase.AssetPathToGUID(item.assetPath);
                        EditorGUIUtility.systemCopyBuffer = guid;
                    });
                }
                
                menu.AddSeparator("");
                
                // Пункты специфичные для файлов
                if (!item.isFolder)
                {
                    menu.AddItem(new GUIContent("Open"), false, () => {
                        AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(item.assetPath));
                    });
                    
                    menu.AddItem(new GUIContent("Open in New Window"), false, () => {
                        AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(item.assetPath));
                    });
                }
                
                menu.ShowAsContext();
            }
        }

        protected override void ContextClicked()
        {
            // Контекстное меню для пустого пространства
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Refresh"), false, () => {
                var window = EditorWindow.focusedWindow as AssetNavigationWindow;
                if (window != null)
                {
                    window.RefreshTreeView();
                }
            });
            menu.ShowAsContext();
        }

        protected override bool CanRename(TreeViewItem item)
        {
            return true;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            var item = FindItem(args.itemID, rootItem) as AssetTreeViewItem;
            if (item != null && args.acceptedRename)
            {
                string oldPath = item.assetPath;
                string newPath = Path.GetDirectoryName(oldPath) + "/" + args.newName;
                
                // Добавляем расширение если его нет и это файл
                if (!item.isFolder && !Path.HasExtension(newPath) && Path.HasExtension(oldPath))
                {
                    newPath += Path.GetExtension(oldPath);
                }
                
                string error = AssetDatabase.MoveAsset(oldPath, newPath);
                if (string.IsNullOrEmpty(error))
                {
                    item.assetPath = newPath;
                    item.displayName = Path.GetFileName(newPath);
                    var window = EditorWindow.focusedWindow as AssetNavigationWindow;
                    if (window != null)
                    {
                        window.RefreshTreeView();
                    }
                }
                else
                {
                    Debug.LogError("Failed to rename asset: " + error);
                }
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }
    }

    // Класс элемента дерева
    public class AssetTreeViewItem : TreeViewItem
    {
        public string assetPath;
        public bool isFolder;
        public Type assetType;
        public string requiredLabel;
    }
}