using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Whisperwood
{
    /// <summary>
    /// Creative Building System for Whisperwood
    /// Minecraft-style building with easy-to-learn tools and educational features
    /// </summary>
    public class CreativeBuildingSystem : MonoBehaviour
    {
        [Header("Creative Building Settings")]
        public bool enableCreativeMode = true;
        public bool enableBlueprintSystem = true;
        public bool enableTutorialSystem = true;
        public bool enableCollaborativeBuilding = true;
        public bool enableRealTimeSharing = true;
        
        [Header("Building Tools")]
        public bool enableBlockPlacing = true;
        public bool enableBlockDestroying = true;
        public bool enableBlockPainting = true;
        public bool enableBlockCopying = true;
        public bool enableBlockPasting = true;
        public bool enableUndoRedo = true;
        
        [Header("Advanced Tools")]
        public bool enableShapeTools = true;
        public bool enableTextureTools = true;
        public bool enableLightingTools = true;
        public bool enableAnimationTools = true;
        public bool enableScriptingTools = true;
        
        [Header("Educational Features")]
        public bool enableStepByStepTutorials = true;
        public bool enableInteractiveLessons = true;
        public bool enableCodingTutorials = true;
        public bool enableArtTutorials = true;
        public bool enableGameDesignTutorials = true;
        
        [Header("World Settings")]
        public int maxWorldSize = 1000;
        public int maxBuildHeight = 256;
        public int maxBuildDepth = 64;
        public bool enableInfiniteWorlds = false;
        
        // Building components
        private BuildingToolManager toolManager;
        private BlockSystem blockSystem;
        private BlueprintSystem blueprintSystem;
        private TutorialSystem tutorialSystem;
        private CollaborationSystem collaborationSystem;
        private WorldSavingSystem worldSavingSystem;
        
        // Creative tools
        private ShapeTool shapeTool;
        private TextureTool textureTool;
        private LightingTool lightingTool;
        private AnimationTool animationTool;
        private ScriptingTool scriptingTool;
        
        // Educational tools
        private StepByStepTutorial stepByStepTutorial;
        private InteractiveLesson interactiveLesson;
        private CodingTutorial codingTutorial;
        private ArtTutorial artTutorial;
        private GameDesignTutorial gameDesignTutorial;
        
        // Building state
        private bool isBuildingMode = false;
        private bool isTutorialMode = false;
        private string currentWorldName = "";
        private List<BuildingAction> undoStack;
        private List<BuildingAction> redoStack;
        
        // Events
        public System.Action<string> OnBuildingCreated;
        public System.Action<string> OnBlueprintSaved;
        public System.Action<string> OnTutorialStarted;
        public System.Action<string> OnTutorialCompleted;
        public System.Action<string> OnWorldCreated;
        public System.Action<string> OnWorldSaved;
        
        private void Awake()
        {
            InitializeCreativeBuildingSystem();
        }
        
        private void Start()
        {
            SetupBuildingEnvironment();
            InitializeCreativeTools();
            InitializeEducationalTools();
        }
        
        /// <summary>
        /// Initialize creative building system
        /// </summary>
        public void Initialize(bool creativeMode, bool blueprintSystem)
        {
            enableCreativeMode = creativeMode;
            enableBlueprintSystem = blueprintSystem;
            
            InitializeCreativeBuildingSystem();
        }
        
        /// <summary>
        /// Initialize creative building system
        /// </summary>
        private void InitializeCreativeBuildingSystem()
        {
            undoStack = new List<BuildingAction>();
            redoStack = new List<BuildingAction>();
            
            // Initialize core systems
            toolManager = gameObject.AddComponent<BuildingToolManager>();
            blockSystem = gameObject.AddComponent<BlockSystem>();
            
            if (enableBlueprintSystem)
            {
                blueprintSystem = gameObject.AddComponent<BlueprintSystem>();
            }
            
            if (enableTutorialSystem)
            {
                tutorialSystem = gameObject.AddComponent<TutorialSystem>();
            }
            
            if (enableCollaborativeBuilding)
            {
                collaborationSystem = gameObject.AddComponent<CollaborationSystem>();
            }
            
            worldSavingSystem = gameObject.AddComponent<WorldSavingSystem>();
            
            Debug.Log("Creative Building System initialized!");
        }
        
        /// <summary>
        /// Setup building environment
        /// </summary>
        private void SetupBuildingEnvironment()
        {
            // Initialize block system
            blockSystem.Initialize(maxWorldSize, maxBuildHeight, maxBuildDepth);
            
            // Initialize tool manager
            toolManager.Initialize(enableBlockPlacing, enableBlockDestroying, enableBlockPainting);
            
            // Initialize blueprint system
            if (blueprintSystem != null)
            {
                blueprintSystem.Initialize();
            }
            
            // Initialize world saving system
            worldSavingSystem.Initialize();
            
            Debug.Log("Building environment setup complete");
        }
        
        /// <summary>
        /// Initialize creative tools
        /// </summary>
        private void InitializeCreativeTools()
        {
            if (enableShapeTools)
            {
                shapeTool = gameObject.AddComponent<ShapeTool>();
                shapeTool.Initialize();
            }
            
            if (enableTextureTools)
            {
                textureTool = gameObject.AddComponent<TextureTool>();
                textureTool.Initialize();
            }
            
            if (enableLightingTools)
            {
                lightingTool = gameObject.AddComponent<LightingTool>();
                lightingTool.Initialize();
            }
            
            if (enableAnimationTools)
            {
                animationTool = gameObject.AddComponent<AnimationTool>();
                animationTool.Initialize();
            }
            
            if (enableScriptingTools)
            {
                scriptingTool = gameObject.AddComponent<ScriptingTool>();
                scriptingTool.Initialize();
            }
            
            Debug.Log("Creative tools initialized");
        }
        
        /// <summary>
        /// Initialize educational tools
        /// </summary>
        private void InitializeEducationalTools()
        {
            if (enableStepByStepTutorials)
            {
                stepByStepTutorial = gameObject.AddComponent<StepByStepTutorial>();
                stepByStepTutorial.Initialize();
            }
            
            if (enableInteractiveLessons)
            {
                interactiveLesson = gameObject.AddComponent<InteractiveLesson>();
                interactiveLesson.Initialize();
            }
            
            if (enableCodingTutorials)
            {
                codingTutorial = gameObject.AddComponent<CodingTutorial>();
                codingTutorial.Initialize();
            }
            
            if (enableArtTutorials)
            {
                artTutorial = gameObject.AddComponent<ArtTutorial>();
                artTutorial.Initialize();
            }
            
            if (enableGameDesignTutorials)
            {
                gameDesignTutorial = gameObject.AddComponent<GameDesignTutorial>();
                gameDesignTutorial.Initialize();
            }
            
            Debug.Log("Educational tools initialized");
        }
        
        /// <summary>
        /// Start building mode
        /// </summary>
        public void StartBuildingMode()
        {
            isBuildingMode = true;
            toolManager.EnableBuildingMode();
            
            Debug.Log("Building mode started");
        }
        
        /// <summary>
        /// Stop building mode
        /// </summary>
        public void StopBuildingMode()
        {
            isBuildingMode = false;
            toolManager.DisableBuildingMode();
            
            Debug.Log("Building mode stopped");
        }
        
        /// <summary>
        /// Create new world
        /// </summary>
        public void CreateNewWorld(string worldName, WorldType worldType)
        {
            try
            {
                currentWorldName = worldName;
                
                // Create world data
                WorldData worldData = new WorldData
                {
                    name = worldName,
                    type = worldType,
                    createdDate = System.DateTime.Now,
                    modifiedDate = System.DateTime.Now,
                    size = maxWorldSize,
                    height = maxBuildHeight,
                    depth = maxBuildDepth,
                    blocks = new Dictionary<Vector3Int, BlockData>(),
                    entities = new List<EntityData>(),
                    settings = new WorldSettings()
                };
                
                // Initialize world
                blockSystem.CreateWorld(worldData);
                
                // Save world
                worldSavingSystem.SaveWorld(worldData);
                
                OnWorldCreated?.Invoke(worldName);
                Debug.Log($"New world created: {worldName}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error creating world {worldName}: {e.Message}");
            }
        }
        
        /// <summary>
        /// Load world
        /// </summary>
        public void LoadWorld(string worldName)
        {
            try
            {
                WorldData worldData = worldSavingSystem.LoadWorld(worldName);
                
                if (worldData != null)
                {
                    currentWorldName = worldName;
                    blockSystem.LoadWorld(worldData);
                    
                    Debug.Log($"World loaded: {worldName}");
                }
                else
                {
                    Debug.LogError($"World not found: {worldName}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading world {worldName}: {e.Message}");
            }
        }
        
        /// <summary>
        /// Save current world
        /// </summary>
        public void SaveCurrentWorld()
        {
            if (string.IsNullOrEmpty(currentWorldName))
            {
                Debug.LogWarning("No world loaded to save");
                return;
            }
            
            try
            {
                WorldData worldData = blockSystem.GetCurrentWorldData();
                worldData.modifiedDate = System.DateTime.Now;
                
                worldSavingSystem.SaveWorld(worldData);
                
                OnWorldSaved?.Invoke(currentWorldName);
                Debug.Log($"World saved: {currentWorldName}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error saving world {currentWorldName}: {e.Message}");
            }
        }
        
        /// <summary>
        /// Start tutorial
        /// </summary>
        public void StartTutorial(TutorialType tutorialType)
        {
            if (!enableTutorialSystem)
            {
                Debug.LogWarning("Tutorial system not enabled");
                return;
            }
            
            isTutorialMode = true;
            
            switch (tutorialType)
            {
                case TutorialType.BasicBuilding:
                    stepByStepTutorial?.StartTutorial("BasicBuilding");
                    break;
                case TutorialType.AdvancedBuilding:
                    stepByStepTutorial?.StartTutorial("AdvancedBuilding");
                    break;
                case TutorialType.Coding:
                    codingTutorial?.StartTutorial("BasicCoding");
                    break;
                case TutorialType.Art:
                    artTutorial?.StartTutorial("BasicArt");
                    break;
                case TutorialType.GameDesign:
                    gameDesignTutorial?.StartTutorial("BasicGameDesign");
                    break;
            }
            
            OnTutorialStarted?.Invoke(tutorialType.ToString());
            Debug.Log($"Tutorial started: {tutorialType}");
        }
        
        /// <summary>
        /// Complete tutorial
        /// </summary>
        public void CompleteTutorial(TutorialType tutorialType)
        {
            isTutorialMode = false;
            
            OnTutorialCompleted?.Invoke(tutorialType.ToString());
            Debug.Log($"Tutorial completed: {tutorialType}");
        }
        
        /// <summary>
        /// Create blueprint
        /// </summary>
        public BlueprintData CreateBlueprint(string blueprintName, Bounds selection)
        {
            if (blueprintSystem == null)
            {
                Debug.LogWarning("Blueprint system not enabled");
                return null;
            }
            
            try
            {
                BlueprintData blueprint = blueprintSystem.CreateBlueprint(blueprintName, selection);
                
                OnBlueprintSaved?.Invoke(blueprintName);
                Debug.Log($"Blueprint created: {blueprintName}");
                
                return blueprint;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error creating blueprint {blueprintName}: {e.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Place blueprint
        /// </summary>
        public void PlaceBlueprint(BlueprintData blueprint, Vector3 position)
        {
            if (blueprintSystem == null)
            {
                Debug.LogWarning("Blueprint system not enabled");
                return;
            }
            
            try
            {
                blueprintSystem.PlaceBlueprint(blueprint, position);
                
                // Add to undo stack
                if (enableUndoRedo)
                {
                    AddToUndoStack(new BuildingAction
                    {
                        type = BuildingActionType.PlaceBlueprint,
                        blueprint = blueprint,
                        position = position,
                        timestamp = System.DateTime.Now
                    });
                }
                
                Debug.Log($"Blueprint placed: {blueprint.name} at {position}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error placing blueprint {blueprint.name}: {e.Message}");
            }
        }
        
        /// <summary>
        /// Undo last action
        /// </summary>
        public void Undo()
        {
            if (!enableUndoRedo || undoStack.Count == 0)
            {
                return;
            }
            
            BuildingAction action = undoStack[undoStack.Count - 1];
            undoStack.RemoveAt(undoStack.Count - 1);
            
            // Undo action
            UndoAction(action);
            
            // Add to redo stack
            redoStack.Add(action);
            
            Debug.Log($"Action undone: {action.type}");
        }
        
        /// <summary>
        /// Redo last undone action
        /// </summary>
        public void Redo()
        {
            if (!enableUndoRedo || redoStack.Count == 0)
            {
                return;
            }
            
            BuildingAction action = redoStack[redoStack.Count - 1];
            redoStack.RemoveAt(redoStack.Count - 1);
            
            // Redo action
            RedoAction(action);
            
            // Add to undo stack
            undoStack.Add(action);
            
            Debug.Log($"Action redone: {action.type}");
        }
        
        /// <summary>
        /// Add action to undo stack
        /// </summary>
        private void AddToUndoStack(BuildingAction action)
        {
            if (enableUndoRedo)
            {
                undoStack.Add(action);
                
                // Limit undo stack size
                if (undoStack.Count > 100)
                {
                    undoStack.RemoveAt(0);
                }
                
                // Clear redo stack
                redoStack.Clear();
            }
        }
        
        /// <summary>
        /// Undo action
        /// </summary>
        private void UndoAction(BuildingAction action)
        {
            switch (action.type)
            {
                case BuildingActionType.PlaceBlock:
                    blockSystem.RemoveBlock(action.position);
                    break;
                case BuildingActionType.RemoveBlock:
                    blockSystem.PlaceBlock(action.position, action.blockData);
                    break;
                case BuildingActionType.PlaceBlueprint:
                    // Remove blueprint blocks
                    break;
                case BuildingActionType.PaintBlock:
                    blockSystem.SetBlockTexture(action.position, action.oldTexture);
                    break;
            }
        }
        
        /// <summary>
        /// Redo action
        /// </summary>
        private void RedoAction(BuildingAction action)
        {
            switch (action.type)
            {
                case BuildingActionType.PlaceBlock:
                    blockSystem.PlaceBlock(action.position, action.blockData);
                    break;
                case BuildingActionType.RemoveBlock:
                    blockSystem.RemoveBlock(action.position);
                    break;
                case BuildingActionType.PlaceBlueprint:
                    // Place blueprint blocks
                    break;
                case BuildingActionType.PaintBlock:
                    blockSystem.SetBlockTexture(action.position, action.newTexture);
                    break;
            }
        }
        
        /// <summary>
        /// Get available tutorials
        /// </summary>
        public List<TutorialInfo> GetAvailableTutorials()
        {
            List<TutorialInfo> tutorials = new List<TutorialInfo>();
            
            if (stepByStepTutorial != null)
            {
                tutorials.AddRange(stepByStepTutorial.GetAvailableTutorials());
            }
            
            if (codingTutorial != null)
            {
                tutorials.AddRange(codingTutorial.GetAvailableTutorials());
            }
            
            if (artTutorial != null)
            {
                tutorials.AddRange(artTutorial.GetAvailableTutorials());
            }
            
            if (gameDesignTutorial != null)
            {
                tutorials.AddRange(gameDesignTutorial.GetAvailableTutorials());
            }
            
            return tutorials;
        }
        
        /// <summary>
        /// Get available blueprints
        /// </summary>
        public List<BlueprintData> GetAvailableBlueprints()
        {
            if (blueprintSystem != null)
            {
                return blueprintSystem.GetAvailableBlueprints();
            }
            
            return new List<BlueprintData>();
        }
        
        /// <summary>
        /// Get building statistics
        /// </summary>
        public BuildingStatistics GetBuildingStatistics()
        {
            return new BuildingStatistics
            {
                blocksPlaced = blockSystem.GetBlocksPlaced(),
                blocksRemoved = blockSystem.GetBlocksRemoved(),
                blueprintsCreated = blueprintSystem?.GetBlueprintsCreated() ?? 0,
                tutorialsCompleted = GetTutorialsCompleted(),
                timeSpentBuilding = GetTimeSpentBuilding(),
                worldsCreated = GetWorldsCreated()
            };
        }
        
        /// <summary>
        /// Check if in building mode
        /// </summary>
        public bool IsBuildingMode()
        {
            return isBuildingMode;
        }
        
        /// <summary>
        /// Check if in tutorial mode
        /// </summary>
        public bool IsTutorialMode()
        {
            return isTutorialMode;
        }
        
        /// <summary>
        /// Get current world name
        /// </summary>
        public string GetCurrentWorldName()
        {
            return currentWorldName;
        }
        
        // Helper methods
        private int GetTutorialsCompleted()
        {
            // This would track completed tutorials
            return 0;
        }
        
        private float GetTimeSpentBuilding()
        {
            // This would track time spent building
            return 0f;
        }
        
        private int GetWorldsCreated()
        {
            // This would track worlds created
            return 0;
        }
    }
    
    /// <summary>
    /// Building action structure
    /// </summary>
    [System.Serializable]
    public class BuildingAction
    {
        public BuildingActionType type;
        public Vector3Int position;
        public BlockData blockData;
        public BlueprintData blueprint;
        public string oldTexture;
        public string newTexture;
        public System.DateTime timestamp;
    }
    
    /// <summary>
    /// Building action types
    /// </summary>
    public enum BuildingActionType
    {
        PlaceBlock,
        RemoveBlock,
        PaintBlock,
        PlaceBlueprint,
        RemoveBlueprint
    }
    
    /// <summary>
    /// Tutorial types
    /// </summary>
    public enum TutorialType
    {
        BasicBuilding,
        AdvancedBuilding,
        Coding,
        Art,
        GameDesign
    }
    
    /// <summary>
    /// Tutorial information structure
    /// </summary>
    [System.Serializable]
    public class TutorialInfo
    {
        public string id;
        public string name;
        public string description;
        public TutorialType type;
        public int difficulty;
        public float estimatedTime;
        public List<string> prerequisites;
        public bool isCompleted;
    }
    
    /// <summary>
    /// Building statistics structure
    /// </summary>
    [System.Serializable]
    public class BuildingStatistics
    {
        public int blocksPlaced;
        public int blocksRemoved;
        public int blueprintsCreated;
        public int tutorialsCompleted;
        public float timeSpentBuilding;
        public int worldsCreated;
    }
}
