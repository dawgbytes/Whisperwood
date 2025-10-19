using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Whisperwood
{
    /// <summary>
    /// Cross-Platform Manager for Whisperwood
    /// Supports PC (Windows, Mac, Linux) and PlayStation 1-5 with unified modding and creative systems
    /// </summary>
    public class CrossPlatformManager : MonoBehaviour
    {
        [Header("Platform Detection")]
        public bool isPC = false;
        public bool isPlayStation = false;
        public PlayStationGeneration playStationGeneration = PlayStationGeneration.PS5;
        public bool isModdingEnabled = false;
        public bool isCreativeModeEnabled = false;
        
        [Header("PC Specific Settings")]
        public bool enableSteamWorkshop = true;
        public bool enableNexusMods = true;
        public bool enableCustomModLoader = true;
        public string modDirectory = "Mods";
        
        [Header("PlayStation Specific Settings")]
        public bool enablePlayStationMods = true;
        public bool enablePlayStationSharing = true;
        public bool enablePlayStationCloudSave = true;
        public string playStationModCache = "PlayStationMods";
        
        [Header("Creative Building Settings")]
        public bool enableCreativeMode = true;
        public bool enableWorldSharing = true;
        public bool enableBlueprintSystem = true;
        public bool enableTutorialSystem = true;
        
        [Header("World Generation")]
        public bool enableProceduralGeneration = true;
        public bool enableCustomBiomes = true;
        public bool enableFashionWeekEvents = true;
        public bool enableLuxuryShoppingDistricts = true;
        
        // Platform-specific components
        private ModdingSystem moddingSystem;
        private CreativeBuildingSystem creativeBuildingSystem;
        private WorldSharingSystem worldSharingSystem;
        private PlaystationCompatibilityLayer playstationCompatibility;
        private LearningToolsSystem learningToolsSystem;
        
        // Cross-platform state
        private Dictionary<string, object> platformSpecificData;
        private bool isInitialized = false;
        
        // Events
        public System.Action<GamePlatform> OnPlatformDetected;
        public System.Action<bool> OnModdingEnabled;
        public System.Action<bool> OnCreativeModeEnabled;
        public System.Action<string> OnWorldShared;
        public System.Action<string> OnModLoaded;
        
        // Singleton
        public static CrossPlatformManager Instance { get; private set; }
        
        // DLL imports for platform-specific functionality
        [DllImport("kernel32.dll")]
        private static extern uint GetVersion();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeCrossPlatform();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            DetectPlatform();
            InitializePlatformSpecificSystems();
            SetupCrossPlatformFeatures();
        }
        
        /// <summary>
        /// Initialize cross-platform manager
        /// </summary>
        private void InitializeCrossPlatform()
        {
            platformSpecificData = new Dictionary<string, object>();
            isInitialized = false;
            
            Debug.Log("Whisperwood Cross-Platform Manager initialized!");
        }
        
        /// <summary>
        /// Detect current platform
        /// </summary>
        private void DetectPlatform()
        {
            // Detect platform based on runtime
            #if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX
                isPC = true;
                isPlayStation = false;
                playStationGeneration = PlayStationGeneration.None;
                OnPlatformDetected?.Invoke(GamePlatform.PC);
            #elif UNITY_PS5
                isPC = false;
                isPlayStation = true;
                playStationGeneration = PlayStationGeneration.PS5;
                OnPlatformDetected?.Invoke(GamePlatform.PlayStation5);
            #elif UNITY_PS4
                isPC = false;
                isPlayStation = true;
                playStationGeneration = PlayStationGeneration.PS4;
                OnPlatformDetected?.Invoke(GamePlatform.PlayStation4);
            #elif UNITY_PS3
                isPC = false;
                isPlayStation = true;
                playStationGeneration = PlayStationGeneration.PS3;
                OnPlatformDetected?.Invoke(GamePlatform.PlayStation3);
            #elif UNITY_PS2
                isPC = false;
                isPlayStation = true;
                playStationGeneration = PlayStationGeneration.PS2;
                OnPlatformDetected?.Invoke(GamePlatform.PlayStation2);
            #elif UNITY_PS1
                isPC = false;
                isPlayStation = true;
                playStationGeneration = PlayStationGeneration.PS1;
                OnPlatformDetected?.Invoke(GamePlatform.PlayStation1);
            #else
                // Fallback detection
                isPC = !isPlayStation;
                OnPlatformDetected?.Invoke(isPC ? GamePlatform.PC : GamePlatform.Unknown);
            #endif
            
            Debug.Log($"Platform detected: {(isPC ? "PC" : "PlayStation")} - Generation: {playStationGeneration}");
        }
        
        /// <summary>
        /// Initialize platform-specific systems
        /// </summary>
        private void InitializePlatformSpecificSystems()
        {
            if (isPC)
            {
                InitializePCSystems();
            }
            else if (isPlayStation)
            {
                InitializePlayStationSystems();
            }
            
            // Initialize cross-platform systems
            InitializeCrossPlatformSystems();
        }
        
        /// <summary>
        /// Initialize PC-specific systems
        /// </summary>
        private void InitializePCSystems()
        {
            // PC gets full modding capabilities
            isModdingEnabled = true;
            isCreativeModeEnabled = true;
            
            // Initialize modding system
            moddingSystem = gameObject.AddComponent<ModdingSystem>();
            moddingSystem.Initialize(modDirectory, enableSteamWorkshop, enableNexusMods);
            
            // Initialize creative building system
            creativeBuildingSystem = gameObject.AddComponent<CreativeBuildingSystem>();
            creativeBuildingSystem.Initialize(enableCreativeMode, enableBlueprintSystem);
            
            // Initialize world sharing system
            worldSharingSystem = gameObject.AddComponent<WorldSharingSystem>();
            worldSharingSystem.Initialize(enableWorldSharing, "PC");
            
            Debug.Log("PC systems initialized with full modding support");
        }
        
        /// <summary>
        /// Initialize PlayStation-specific systems
        /// </summary>
        private void InitializePlayStationSystems()
        {
            // PlayStation gets limited modding based on generation
            isModdingEnabled = playStationGeneration >= PlayStationGeneration.PS4;
            isCreativeModeEnabled = playStationGeneration >= PlayStationGeneration.PS3;
            
            // Initialize PlayStation compatibility layer
            playstationCompatibility = gameObject.AddComponent<PlaystationCompatibilityLayer>();
            playstationCompatibility.Initialize(playStationGeneration);
            
            if (isModdingEnabled)
            {
                // Limited modding for PS4/PS5
                moddingSystem = gameObject.AddComponent<ModdingSystem>();
                moddingSystem.Initialize(playStationModCache, false, false); // No Steam/Nexus on console
            }
            
            if (isCreativeModeEnabled)
            {
                // Creative building for PS3+
                creativeBuildingSystem = gameObject.AddComponent<CreativeBuildingSystem>();
                creativeBuildingSystem.Initialize(enableCreativeMode, enableBlueprintSystem);
            }
            
            // World sharing for all PlayStation generations
            worldSharingSystem = gameObject.AddComponent<WorldSharingSystem>();
            worldSharingSystem.Initialize(enablePlayStationSharing, playStationGeneration.ToString());
            
            Debug.Log($"PlayStation {playStationGeneration} systems initialized");
        }
        
        /// <summary>
        /// Initialize cross-platform systems
        /// </summary>
        private void InitializeCrossPlatformSystems()
        {
            // Initialize learning tools system
            learningToolsSystem = gameObject.AddComponent<LearningToolsSystem>();
            learningToolsSystem.Initialize(enableTutorialSystem, playStationGeneration);
            
            Debug.Log("Cross-platform systems initialized");
        }
        
        /// <summary>
        /// Setup cross-platform features
        /// </summary>
        private void SetupCrossPlatformFeatures()
        {
            // Setup world generation
            if (enableProceduralGeneration)
            {
                SetupProceduralGeneration();
            }
            
            // Setup creative building
            if (isCreativeModeEnabled)
            {
                SetupCreativeBuilding();
            }
            
            // Setup modding
            if (isModdingEnabled)
            {
                SetupModding();
            }
            
            // Setup world sharing
            if (enableWorldSharing)
            {
                SetupWorldSharing();
            }
            
            isInitialized = true;
            Debug.Log("Cross-platform features setup complete!");
        }
        
        /// <summary>
        /// Setup procedural generation
        /// </summary>
        private void SetupProceduralGeneration()
        {
            // Initialize procedural world generation
            var worldGenerator = gameObject.AddComponent<ProceduralWorldGenerator>();
            worldGenerator.Initialize(enableCustomBiomes, enableFashionWeekEvents, enableLuxuryShoppingDistricts);
            
            Debug.Log("Procedural world generation enabled");
        }
        
        /// <summary>
        /// Setup creative building
        /// </summary>
        private void SetupCreativeBuilding()
        {
            if (creativeBuildingSystem != null)
            {
                creativeBuildingSystem.OnBuildingCreated += OnBuildingCreated;
                creativeBuildingSystem.OnBlueprintSaved += OnBlueprintSaved;
            }
            
            Debug.Log("Creative building system enabled");
        }
        
        /// <summary>
        /// Setup modding
        /// </summary>
        private void SetupModding()
        {
            if (moddingSystem != null)
            {
                moddingSystem.OnModLoaded += OnModLoaded;
                moddingSystem.OnModUnloaded += OnModUnloaded;
                moddingSystem.OnModError += OnModError;
            }
            
            Debug.Log("Modding system enabled");
        }
        
        /// <summary>
        /// Setup world sharing
        /// </summary>
        private void SetupWorldSharing()
        {
            if (worldSharingSystem != null)
            {
                worldSharingSystem.OnWorldUploaded += OnWorldUploaded;
                worldSharingSystem.OnWorldDownloaded += OnWorldDownloaded;
            }
            
            Debug.Log("World sharing system enabled");
        }
        
        /// <summary>
        /// Load mod from file
        /// </summary>
        public bool LoadMod(string modPath)
        {
            if (!isModdingEnabled)
            {
                Debug.LogWarning("Modding not enabled on this platform");
                return false;
            }
            
            if (moddingSystem != null)
            {
                return moddingSystem.LoadMod(modPath);
            }
            
            return false;
        }
        
        /// <summary>
        /// Create new world
        /// </summary>
        public void CreateNewWorld(string worldName, WorldType worldType)
        {
            if (creativeBuildingSystem != null)
            {
                creativeBuildingSystem.CreateNewWorld(worldName, worldType);
            }
        }
        
        /// <summary>
        /// Share world
        /// </summary>
        public void ShareWorld(string worldName)
        {
            if (worldSharingSystem != null)
            {
                worldSharingSystem.UploadWorld(worldName);
            }
        }
        
        /// <summary>
        /// Download world
        /// </summary>
        public void DownloadWorld(string worldId)
        {
            if (worldSharingSystem != null)
            {
                worldSharingSystem.DownloadWorld(worldId);
            }
        }
        
        /// <summary>
        /// Start creative tutorial
        /// </summary>
        public void StartCreativeTutorial()
        {
            if (learningToolsSystem != null)
            {
                learningToolsSystem.StartCreativeTutorial();
            }
        }
        
        /// <summary>
        /// Get platform-specific data
        /// </summary>
        public T GetPlatformData<T>(string key)
        {
            if (platformSpecificData.ContainsKey(key))
            {
                return (T)platformSpecificData[key];
            }
            return default(T);
        }
        
        /// <summary>
        /// Set platform-specific data
        /// </summary>
        public void SetPlatformData(string key, object value)
        {
            platformSpecificData[key] = value;
        }
        
        /// <summary>
        /// Check if feature is supported
        /// </summary>
        public bool IsFeatureSupported(CrossPlatformFeature feature)
        {
            switch (feature)
            {
                case CrossPlatformFeature.Modding:
                    return isModdingEnabled;
                case CrossPlatformFeature.CreativeBuilding:
                    return isCreativeModeEnabled;
                case CrossPlatformFeature.WorldSharing:
                    return enableWorldSharing;
                case CrossPlatformFeature.SteamWorkshop:
                    return isPC && enableSteamWorkshop;
                case CrossPlatformFeature.PlayStationSharing:
                    return isPlayStation && enablePlayStationSharing;
                default:
                    return false;
            }
        }
        
        // Event handlers
        private void OnBuildingCreated(string buildingName)
        {
            Debug.Log($"Building created: {buildingName}");
        }
        
        private void OnBlueprintSaved(string blueprintName)
        {
            Debug.Log($"Blueprint saved: {blueprintName}");
        }
        
        private void OnModLoaded(string modName)
        {
            Debug.Log($"Mod loaded: {modName}");
            OnModLoaded?.Invoke(modName);
        }
        
        private void OnModUnloaded(string modName)
        {
            Debug.Log($"Mod unloaded: {modName}");
        }
        
        private void OnModError(string error)
        {
            Debug.LogError($"Mod error: {error}");
        }
        
        private void OnWorldUploaded(string worldName)
        {
            Debug.Log($"World uploaded: {worldName}");
            OnWorldShared?.Invoke(worldName);
        }
        
        private void OnWorldDownloaded(string worldName)
        {
            Debug.Log($"World downloaded: {worldName}");
        }
        
        /// <summary>
        /// Get current platform info
        /// </summary>
        public PlatformInfo GetPlatformInfo()
        {
            return new PlatformInfo
            {
                isPC = isPC,
                isPlayStation = isPlayStation,
                playStationGeneration = playStationGeneration,
                isModdingEnabled = isModdingEnabled,
                isCreativeModeEnabled = isCreativeModeEnabled,
                isInitialized = isInitialized
            };
        }
    }
    
    /// <summary>
    /// Game platforms
    /// </summary>
    public enum GamePlatform
    {
        PC,
        PlayStation1,
        PlayStation2,
        PlayStation3,
        PlayStation4,
        PlayStation5,
        Unknown
    }
    
    /// <summary>
    /// PlayStation generations
    /// </summary>
    public enum PlayStationGeneration
    {
        None,
        PS1,
        PS2,
        PS3,
        PS4,
        PS5
    }
    
    /// <summary>
    /// Cross-platform features
    /// </summary>
    public enum CrossPlatformFeature
    {
        Modding,
        CreativeBuilding,
        WorldSharing,
        SteamWorkshop,
        PlayStationSharing,
        CloudSave,
        Multiplayer
    }
    
    /// <summary>
    /// World types
    /// </summary>
    public enum WorldType
    {
        Creative,
        Survival,
        BattleRoyale,
        Campaign,
        Custom
    }
    
    /// <summary>
    /// Platform information structure
    /// </summary>
    [System.Serializable]
    public struct PlatformInfo
    {
        public bool isPC;
        public bool isPlayStation;
        public PlayStationGeneration playStationGeneration;
        public bool isModdingEnabled;
        public bool isCreativeModeEnabled;
        public bool isInitialized;
    }
}
